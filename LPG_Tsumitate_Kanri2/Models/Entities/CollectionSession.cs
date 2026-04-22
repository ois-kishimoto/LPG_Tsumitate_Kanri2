using System.ComponentModel.DataAnnotations;

namespace LPG_Tsumitate_Kanri2.Models.Entities;

public class CollectionSession
{
    public int SessionId { get; set; }

    [Required]
    [Display(Name = "積立種別")]
    public int SavingsTypeId { get; set; }

    [Required]
    [Display(Name = "年")]
    public int Year { get; set; }

    [Required]
    [Range(1, 12)]
    [Display(Name = "月")]
    public int Month { get; set; }

    [Display(Name = "懇親会開催日")]
    public DateOnly? SessionDate { get; set; }

    [MaxLength(500)]
    [Display(Name = "備考")]
    public string? Notes { get; set; }

    public bool IsCompleted { get; set; }
    public DateTime CreatedAt { get; set; }

    public SavingsType SavingsType { get; set; } = null!;
    public ICollection<CollectionRecord> CollectionRecords { get; set; } = new List<CollectionRecord>();

    public string Label => $"{Year}年{Month}月分 {SavingsType?.Name}";
}
