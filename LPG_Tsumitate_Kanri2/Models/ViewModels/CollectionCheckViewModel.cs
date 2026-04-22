using LPG_Tsumitate_Kanri2.Models.Entities;

namespace LPG_Tsumitate_Kanri2.Models.ViewModels;

public class CollectionCheckViewModel
{
    public CollectionSession Session { get; set; } = null!;
    public List<CollectionRecordRow> Records { get; set; } = new();
    public int TotalCollected { get; set; }
    public int CollectedCount { get; set; }
    public int PendingCount { get; set; }
    public int ExcludedCount { get; set; }
    public bool IsNormalSavings => Session.SavingsTypeId == 1;
}

public class CollectionRecordRow
{
    public int RecordId { get; set; }
    public string EmployeeNo { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public int ExpectedAmount { get; set; }
    public bool IsCollected { get; set; }
    public bool IsExcluded { get; set; }
    public string? Notes { get; set; }
}
