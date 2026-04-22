using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LPG_Tsumitate_Kanri2.Data;
using LPG_Tsumitate_Kanri2.Models.Entities;

namespace LPG_Tsumitate_Kanri2.Controllers;

public class ContributionRulesController : Controller
{
    private readonly AppDbContext _db;

    public ContributionRulesController(AppDbContext db) => _db = db;

    public async Task<IActionResult> Index()
    {
        var rules = await _db.ContributionAmountRules
            .Include(r => r.SavingsType)
            .OrderBy(r => r.SavingsTypeId)
            .ThenBy(r => r.Priority)
            .ToListAsync();
        return View(rules);
    }

    public async Task<IActionResult> Create()
    {
        ViewBag.SavingsTypes = await _db.SavingsTypes.OrderBy(s => s.DisplayOrder).ToListAsync();
        return View(new ContributionAmountRule { ValidFrom = DateOnly.FromDateTime(DateTime.Today) });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ContributionAmountRule model)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.SavingsTypes = await _db.SavingsTypes.OrderBy(s => s.DisplayOrder).ToListAsync();
            return View(model);
        }

        _db.ContributionAmountRules.Add(model);
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var rule = await _db.ContributionAmountRules.FindAsync(id);
        if (rule == null) return NotFound();
        ViewBag.SavingsTypes = await _db.SavingsTypes.OrderBy(s => s.DisplayOrder).ToListAsync();
        return View(rule);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, ContributionAmountRule model)
    {
        if (id != model.RuleId) return BadRequest();

        if (!ModelState.IsValid)
        {
            ViewBag.SavingsTypes = await _db.SavingsTypes.OrderBy(s => s.DisplayOrder).ToListAsync();
            return View(model);
        }

        var existing = await _db.ContributionAmountRules.FindAsync(id);
        if (existing == null) return NotFound();

        existing.SavingsTypeId = model.SavingsTypeId;
        existing.ConditionEmploymentType = model.ConditionEmploymentType;
        existing.ConditionMaxYearsOfService = model.ConditionMaxYearsOfService;
        existing.ConditionPositionCategory = model.ConditionPositionCategory;
        existing.Amount = model.Amount;
        existing.Priority = model.Priority;
        existing.ValidFrom = model.ValidFrom;
        existing.ValidTo = model.ValidTo;

        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var rule = await _db.ContributionAmountRules.FindAsync(id);
        if (rule == null) return NotFound();
        _db.ContributionAmountRules.Remove(rule);
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
}
