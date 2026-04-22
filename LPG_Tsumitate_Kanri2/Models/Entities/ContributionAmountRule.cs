using System.ComponentModel.DataAnnotations;

namespace LPG_Tsumitate_Kanri2.Models.Entities;

public class ContributionAmountRule
{
    public int RuleId { get; set; }

    [Required]
    [Display(Name = "積立種別")]
    public int SavingsTypeId { get; set; }

    [MaxLength(20)]
    [Display(Name = "雇用形態条件")]
    public string? ConditionEmploymentType { get; set; }

    [Display(Name = "勤続年数上限（未満）")]
    public int? ConditionMaxYearsOfService { get; set; }

    [MaxLength(20)]
    [Display(Name = "役職区分条件")]
    public string? ConditionPositionCategory { get; set; }

    [Required]
    [Range(0, 1000000)]
    [Display(Name = "月額（円）")]
    public int Amount { get; set; }

    [Required]
    [Display(Name = "優先順位")]
    public int Priority { get; set; }

    [Required]
    [Display(Name = "有効開始日")]
    public DateOnly ValidFrom { get; set; }

    [Display(Name = "有効終了日")]
    public DateOnly? ValidTo { get; set; }

    public SavingsType SavingsType { get; set; } = null!;

    public string ConditionSummary =>
        string.Join(" / ",
            new[]
            {
                ConditionEmploymentType is not null ? $"雇用形態={ConditionEmploymentType}" : null,
                ConditionMaxYearsOfService.HasValue ? $"勤続{ConditionMaxYearsOfService}年未満" : null,
                ConditionPositionCategory is not null ? $"役職={ConditionPositionCategory}" : null,
            }.Where(s => s is not null)) is { Length: > 0 } s ? s : "（全員）";
}
