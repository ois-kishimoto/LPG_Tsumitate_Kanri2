using System.ComponentModel.DataAnnotations;
using LPG_Tsumitate_Kanri2.Models.Entities;

namespace LPG_Tsumitate_Kanri2.Models.ViewModels;

public class LedgerFormViewModel
{
    public int EntryId { get; set; }

    [Required]
    [Display(Name = "積立種別")]
    public int SavingsTypeId { get; set; }

    [Required]
    [Display(Name = "収支発生日")]
    public DateOnly TransactionDate { get; set; } = DateOnly.FromDateTime(DateTime.Today);

    [Required]
    [Display(Name = "区分")]
    public string EntryType { get; set; } = "出金";

    [Required(ErrorMessage = "内容は必須です")]
    [MaxLength(200)]
    [Display(Name = "内容")]
    public string Description { get; set; } = string.Empty;

    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "金額は1以上の整数を入力してください")]
    [Display(Name = "金額（円）")]
    public int Amount { get; set; }

    [MaxLength(500)]
    [Display(Name = "備考")]
    public string? Notes { get; set; }

    public List<SavingsType> SavingsTypes { get; set; } = new();
}
