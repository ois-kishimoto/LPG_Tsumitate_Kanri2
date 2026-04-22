namespace LPG_Tsumitate_Kanri2.Models.Entities;

public class SavingsType
{
    public int SavingsTypeId { get; set; }
    public string Name { get; set; } = string.Empty;
    public int DisplayOrder { get; set; }

    public ICollection<ContributionAmountRule> ContributionAmountRules { get; set; } = new List<ContributionAmountRule>();
    public ICollection<CollectionSession> CollectionSessions { get; set; } = new List<CollectionSession>();
    public ICollection<LedgerEntry> LedgerEntries { get; set; } = new List<LedgerEntry>();
}
