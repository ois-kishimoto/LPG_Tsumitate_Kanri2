using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LPG_Tsumitate_Kanri2.Data;
using LPG_Tsumitate_Kanri2.Models.Entities;
using LPG_Tsumitate_Kanri2.Models.ViewModels;
using LPG_Tsumitate_Kanri2.Services;

namespace LPG_Tsumitate_Kanri2.Controllers;

public class CollectionsController : Controller
{
    private readonly AppDbContext _db;
    private readonly ContributionCalculator _calculator;
    private readonly LedgerService _ledgerService;

    public CollectionsController(AppDbContext db, ContributionCalculator calculator, LedgerService ledgerService)
    {
        _db = db;
        _calculator = calculator;
        _ledgerService = ledgerService;
    }

    public async Task<IActionResult> Index(int? year, int? savingsTypeId)
    {
        var query = _db.CollectionSessions
            .Include(s => s.SavingsType)
            .Include(s => s.CollectionRecords)
            .AsQueryable();

        if (year.HasValue) query = query.Where(s => s.Year == year.Value);
        if (savingsTypeId.HasValue) query = query.Where(s => s.SavingsTypeId == savingsTypeId.Value);

        var sessions = await query
            .OrderByDescending(s => s.Year)
            .ThenByDescending(s => s.Month)
            .ToListAsync();

        ViewBag.SavingsTypes = await _db.SavingsTypes.OrderBy(s => s.DisplayOrder).ToListAsync();
        ViewBag.FilterYear = year;
        ViewBag.FilterSavingsTypeId = savingsTypeId;
        return View(sessions);
    }

    public async Task<IActionResult> Create()
    {
        ViewBag.SavingsTypes = await _db.SavingsTypes.OrderBy(s => s.DisplayOrder).ToListAsync();
        return View(new CollectionSession { Year = DateTime.Today.Year, Month = DateTime.Today.Month });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CollectionSession model)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.SavingsTypes = await _db.SavingsTypes.OrderBy(s => s.DisplayOrder).ToListAsync();
            return View(model);
        }

        var exists = await _db.CollectionSessions.AnyAsync(s =>
            s.SavingsTypeId == model.SavingsTypeId && s.Year == model.Year && s.Month == model.Month);
        if (exists)
        {
            ModelState.AddModelError("", "同じ種別・年月のセッションはすでに存在します。");
            ViewBag.SavingsTypes = await _db.SavingsTypes.OrderBy(s => s.DisplayOrder).ToListAsync();
            return View(model);
        }

        model.CreatedAt = DateTime.Now;
        _db.CollectionSessions.Add(model);
        await _db.SaveChangesAsync();

        var asOf = new DateOnly(model.Year, model.Month, 1);
        var employees = await _db.Employees.Where(e => e.IsActive).ToListAsync();
        var rules = await _db.ContributionAmountRules
            .Where(r => r.SavingsTypeId == model.SavingsTypeId
                && r.ValidFrom <= asOf
                && (r.ValidTo == null || r.ValidTo >= asOf))
            .OrderBy(r => r.Priority)
            .ToListAsync();

        var records = employees.Select(emp => new CollectionRecord
        {
            SessionId = model.SessionId,
            EmployeeId = emp.EmployeeId,
            ExpectedAmount = _calculator.Calculate(emp, rules, asOf)
        }).ToList();

        _db.CollectionRecords.AddRange(records);
        await _db.SaveChangesAsync();

        return RedirectToAction(nameof(Details), new { id = model.SessionId });
    }

    public async Task<IActionResult> Details(int id)
    {
        var session = await _db.CollectionSessions
            .Include(s => s.SavingsType)
            .Include(s => s.CollectionRecords).ThenInclude(r => r.Employee)
            .FirstOrDefaultAsync(s => s.SessionId == id);

        if (session == null) return NotFound();

        var vm = new CollectionCheckViewModel
        {
            Session = session,
            Records = session.CollectionRecords
                .OrderBy(r => r.Employee.EmployeeNo)
                .Select(r => new CollectionRecordRow
                {
                    RecordId = r.RecordId,
                    EmployeeNo = r.Employee.EmployeeNo,
                    FullName = r.Employee.FullName,
                    ExpectedAmount = r.ExpectedAmount,
                    IsCollected = r.IsCollected,
                    IsExcluded = r.IsExcluded,
                    Notes = r.Notes
                }).ToList(),
            TotalCollected = session.CollectionRecords.Where(r => r.IsCollected && !r.IsExcluded).Sum(r => r.ExpectedAmount),
            CollectedCount = session.CollectionRecords.Count(r => r.IsCollected && !r.IsExcluded),
            PendingCount = session.CollectionRecords.Count(r => !r.IsCollected && !r.IsExcluded),
            ExcludedCount = session.CollectionRecords.Count(r => r.IsExcluded)
        };
        return View(vm);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Toggle(int id, int recordId)
    {
        var session = await _db.CollectionSessions.FindAsync(id);
        if (session == null || session.IsCompleted)
            return Json(new { success = false, message = "操作できません。" });

        var record = await _db.CollectionRecords.FindAsync(recordId);
        if (record == null || record.SessionId != id)
            return Json(new { success = false, message = "レコードが見つかりません。" });

        record.IsCollected = !record.IsCollected;
        record.CollectedAt = record.IsCollected ? DateTime.Now : null;
        await _db.SaveChangesAsync();

        return Json(new { success = true, isCollected = record.IsCollected });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Exclude(int id, int recordId)
    {
        var session = await _db.CollectionSessions.FindAsync(id);
        if (session == null || session.IsCompleted)
            return Json(new { success = false, message = "操作できません。" });

        var record = await _db.CollectionRecords.FindAsync(recordId);
        if (record == null || record.SessionId != id)
            return Json(new { success = false, message = "レコードが見つかりません。" });

        record.IsExcluded = !record.IsExcluded;
        if (record.IsExcluded) record.IsCollected = false;
        await _db.SaveChangesAsync();

        return Json(new { success = true, isExcluded = record.IsExcluded });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Complete(int id)
    {
        var session = await _db.CollectionSessions
            .Include(s => s.SavingsType)
            .Include(s => s.CollectionRecords)
            .FirstOrDefaultAsync(s => s.SessionId == id);

        if (session == null || session.IsCompleted) return BadRequest();

        var totalAmount = session.CollectionRecords
            .Where(r => r.IsCollected && !r.IsExcluded)
            .Sum(r => r.ExpectedAmount);

        var entry = new LedgerEntry
        {
            SavingsTypeId = session.SavingsTypeId,
            TransactionDate = session.SessionDate ?? new DateOnly(session.Year, session.Month, 1),
            EntryType = "入金",
            Description = $"{session.Year}年{session.Month}月分 {session.SavingsType.Name} 徴収分",
            Amount = totalAmount,
            IsAutoGenerated = true,
            SourceSessionId = session.SessionId,
            CreatedAt = DateTime.Now
        };
        await _ledgerService.SetBalanceAsync(entry);
        _db.LedgerEntries.Add(entry);

        session.IsCompleted = true;
        await _db.SaveChangesAsync();

        return RedirectToAction(nameof(Details), new { id });
    }
}
