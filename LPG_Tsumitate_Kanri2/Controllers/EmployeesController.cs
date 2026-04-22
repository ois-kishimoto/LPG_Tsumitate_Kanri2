using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LPG_Tsumitate_Kanri2.Data;
using LPG_Tsumitate_Kanri2.Models.Entities;

namespace LPG_Tsumitate_Kanri2.Controllers;

public class EmployeesController : Controller
{
    private readonly AppDbContext _db;

    public EmployeesController(AppDbContext db) => _db = db;

    public async Task<IActionResult> Index(
        string? search,
        string? employmentType,
        string? positionCategory,
        string? sortBy,
        string? sortDir)
    {
        var query = _db.Employees.AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(e => e.FullName.Contains(search) || e.EmployeeNo.Contains(search));
        if (!string.IsNullOrWhiteSpace(employmentType))
            query = query.Where(e => e.EmploymentType == employmentType);
        if (!string.IsNullOrWhiteSpace(positionCategory))
            query = query.Where(e => e.PositionCategory == positionCategory);

        query = (sortBy, sortDir) switch
        {
            ("name",     "desc") => query.OrderByDescending(e => e.FullName),
            ("name",     _)      => query.OrderBy(e => e.FullName),
            ("hiredate", "desc") => query.OrderByDescending(e => e.HireDate),
            ("hiredate", _)      => query.OrderBy(e => e.HireDate),
            // 勤続年数昇順 = 入社日降順（新しいほど勤続短い）
            ("service",  "desc") => query.OrderBy(e => e.HireDate),
            ("service",  _)      => query.OrderByDescending(e => e.HireDate),
            ("emptype",  "desc") => query.OrderByDescending(e => e.EmploymentType),
            ("emptype",  _)      => query.OrderBy(e => e.EmploymentType),
            ("position", "desc") => query.OrderByDescending(e => e.PositionCategory),
            ("position", _)      => query.OrderBy(e => e.PositionCategory),
            ("no",       "desc") => query.OrderByDescending(e => e.EmployeeNo),
            _                    => query.OrderBy(e => e.EmployeeNo),
        };

        ViewBag.Search = search;
        ViewBag.EmploymentType = employmentType;
        ViewBag.PositionCategory = positionCategory;
        ViewBag.SortBy = sortBy ?? "no";
        ViewBag.SortDir = sortDir ?? "asc";

        return View(await query.ToListAsync());
    }

    public IActionResult Create() => View(new Employee { HireDate = DateOnly.FromDateTime(DateTime.Today) });

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Employee model)
    {
        if (await _db.Employees.AnyAsync(e => e.EmployeeNo == model.EmployeeNo))
            ModelState.AddModelError(nameof(Employee.EmployeeNo), "この社員番号はすでに使用されています。");

        if (!ModelState.IsValid) return View(model);

        model.CreatedAt = DateTime.Now;
        model.UpdatedAt = DateTime.Now;
        _db.Employees.Add(model);
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var employee = await _db.Employees.FindAsync(id);
        if (employee == null) return NotFound();
        return View(employee);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Employee model)
    {
        if (id != model.EmployeeId) return BadRequest();

        if (await _db.Employees.AnyAsync(e => e.EmployeeNo == model.EmployeeNo && e.EmployeeId != id))
            ModelState.AddModelError(nameof(Employee.EmployeeNo), "この社員番号はすでに使用されています。");

        if (!ModelState.IsValid) return View(model);

        var existing = await _db.Employees.FindAsync(id);
        if (existing == null) return NotFound();

        existing.EmployeeNo = model.EmployeeNo;
        existing.FullName = model.FullName;
        existing.HireDate = model.HireDate;
        existing.BirthDate = model.BirthDate;
        existing.EmploymentType = model.EmploymentType;
        existing.PositionCategory = model.PositionCategory;
        existing.UpdatedAt = DateTime.Now;

        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Deactivate(int id)
    {
        var employee = await _db.Employees.FindAsync(id);
        if (employee == null) return NotFound();

        employee.IsActive = false;
        employee.IsOnLeave = false;
        employee.UpdatedAt = DateTime.Now;
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Reactivate(int id)
    {
        var employee = await _db.Employees.FindAsync(id);
        if (employee == null) return NotFound();

        employee.IsActive = true;
        employee.UpdatedAt = DateTime.Now;
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> SetLeave(int id)
    {
        var employee = await _db.Employees.FindAsync(id);
        if (employee == null || !employee.IsActive) return NotFound();

        employee.IsOnLeave = true;
        employee.UpdatedAt = DateTime.Now;
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> CancelLeave(int id)
    {
        var employee = await _db.Employees.FindAsync(id);
        if (employee == null) return NotFound();

        employee.IsOnLeave = false;
        employee.UpdatedAt = DateTime.Now;
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
}
