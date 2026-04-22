using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LPG_Tsumitate_Kanri2.Data;
using LPG_Tsumitate_Kanri2.Models.Entities;

namespace LPG_Tsumitate_Kanri2.Controllers;

public class EmployeesController : Controller
{
    private readonly AppDbContext _db;

    public EmployeesController(AppDbContext db) => _db = db;

    public async Task<IActionResult> Index()
    {
        var employees = await _db.Employees
            .Where(e => e.IsActive)
            .OrderBy(e => e.EmployeeNo)
            .ToListAsync();
        return View(employees);
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
        employee.UpdatedAt = DateTime.Now;
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
}
