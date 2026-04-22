namespace LPG_Tsumitate_Kanri2.Models.Entities;

public class Receipt
{
    public int ReceiptId { get; set; }
    public int EntryId { get; set; }
    public string OriginalFileName { get; set; } = string.Empty;
    public string StoredFileName { get; set; } = string.Empty;
    public int FileSize { get; set; }
    public string ContentType { get; set; } = string.Empty;
    public DateTime UploadedAt { get; set; }

    public LedgerEntry Entry { get; set; } = null!;
}
