using LPG_Tsumitate_Kanri2.Models.Entities;

namespace LPG_Tsumitate_Kanri2.Models.ViewModels;

public class DashboardViewModel
{
    public List<SavingsBalanceSummary> Balances { get; set; } = new();
    public List<LedgerEntry> RecentEntries { get; set; } = new();
    public List<PendingCollectionAlert> PendingAlerts { get; set; } = new();
}

public class SavingsBalanceSummary
{
    public int SavingsTypeId { get; set; }
    public string Name { get; set; } = string.Empty;
    public int CurrentBalance { get; set; }
}

public class PendingCollectionAlert
{
    public int SessionId { get; set; }
    public string SavingsTypeName { get; set; } = string.Empty;
    public int Year { get; set; }
    public int Month { get; set; }
    public int PendingCount { get; set; }
}
