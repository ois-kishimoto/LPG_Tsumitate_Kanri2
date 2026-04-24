using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LPG_Tsumitate_Kanri2.Data;
using LPG_Tsumitate_Kanri2.Models;
using LPG_Tsumitate_Kanri2.Models.ViewModels;
using LPG_Tsumitate_Kanri2.Services;
using System.Diagnostics;

namespace LPG_Tsumitate_Kanri2.Controllers;

public class HomeController : Controller
{
    private readonly AppDbContext _db;
    private readonly LedgerService _ledgerService;

    public HomeController(AppDbContext db, LedgerService ledgerService)
    {
        _db = db;
        _ledgerService = ledgerService;
    }

    public async Task<IActionResult> Index()
    {
        var savingsTypes = await _db.SavingsTypes.OrderBy(s => s.DisplayOrder).ToListAsync();

        var balances = new List<SavingsBalanceSummary>();
        foreach (var st in savingsTypes)
        {
            balances.Add(new SavingsBalanceSummary
            {
                SavingsTypeId = st.SavingsTypeId,
                Name = st.Name,
                CurrentBalance = await _ledgerService.GetCurrentBalanceAsync(st.SavingsTypeId)
            });
        }

        var recentEntries = await _db.LedgerEntries
            .Include(e => e.SavingsType)
            .OrderByDescending(e => e.TransactionDate)
            .ThenByDescending(e => e.EntryId)
            .Take(10)
            .ToListAsync();

        var pendingAlerts = await _db.CollectionSessions
            .Include(s => s.SavingsType)
            .Include(s => s.CollectionRecords)
            .Where(s => !s.IsCompleted)
            .OrderByDescending(s => s.Year)
            .ThenByDescending(s => s.Month)
            .ThenBy(s => s.SavingsType.DisplayOrder)
            .Select(s => new PendingCollectionAlert
            {
                SessionId = s.SessionId,
                SavingsTypeName = s.SavingsType.Name,
                Year = s.Year,
                Month = s.Month,
                PendingCount = s.CollectionRecords.Count(r => !r.IsCollected && !r.IsExcluded)
            })
            .Where(a => a.PendingCount > 0)
            .ToListAsync();

        var vm = new DashboardViewModel
        {
            Balances = balances,
            RecentEntries = recentEntries,
            PendingAlerts = pendingAlerts
        };
        return View(vm);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
