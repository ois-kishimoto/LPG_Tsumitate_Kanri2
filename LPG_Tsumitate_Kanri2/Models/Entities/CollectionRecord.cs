using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace LPG_Tsumitate_Kanri2.Models.Entities;

public class CollectionRecord
{
    public int RecordId { get; set; }
    public int SessionId { get; set; }
    public int EmployeeId { get; set; }

    [Display(Name = "徴収予定金額")]
    public int ExpectedAmount { get; set; }

    [Display(Name = "不参加除外")]
    public bool IsExcluded { get; set; }

    [Display(Name = "徴収済み")]
    public bool IsCollected { get; set; }

    public DateTime? CollectedAt { get; set; }

    [MaxLength(200)]
    [Display(Name = "備考")]
    public string? Notes { get; set; }

    [ValidateNever] public CollectionSession Session { get; set; } = null!;
    [ValidateNever] public Employee Employee { get; set; } = null!;
}
