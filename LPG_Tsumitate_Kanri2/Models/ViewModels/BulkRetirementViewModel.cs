using System.ComponentModel.DataAnnotations;

namespace LPG_Tsumitate_Kanri2.Models.ViewModels;

public class BulkRetirementViewModel
{
    [Required(ErrorMessage = "社員を選択してください。")]
    [Display(Name = "社員")]
    public int? EmployeeId { get; set; }

    [Required(ErrorMessage = "金額を入力してください。")]
    [Range(1, int.MaxValue, ErrorMessage = "1円以上を入力してください。")]
    [Display(Name = "金額")]
    public int? Amount { get; set; }

    public List<RetirementCandidate> Candidates { get; set; } = new();
}

public class RetirementCandidate
{
    public int EmployeeId { get; set; }
    public string EmployeeNo { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public int PendingTotal { get; set; }
}
