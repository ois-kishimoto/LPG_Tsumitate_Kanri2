using System.ComponentModel.DataAnnotations;

namespace LPG_Tsumitate_Kanri2.Models.Entities;

public class Employee
{
    public int EmployeeId { get; set; }

    [Required(ErrorMessage = "社員番号は必須です")]
    [MaxLength(20)]
    [Display(Name = "社員番号")]
    public string EmployeeNo { get; set; } = string.Empty;

    [Required(ErrorMessage = "氏名は必須です")]
    [MaxLength(100)]
    [Display(Name = "氏名")]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "入社日は必須です")]
    [Display(Name = "入社日")]
    public DateOnly HireDate { get; set; }

    [Display(Name = "生年月日")]
    public DateOnly? BirthDate { get; set; }

    [Required(ErrorMessage = "雇用形態は必須です")]
    [Display(Name = "雇用形態")]
    public string EmploymentType { get; set; } = string.Empty;

    [Required(ErrorMessage = "役職区分は必須です")]
    [Display(Name = "役職区分")]
    public string PositionCategory { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public ICollection<CollectionRecord> CollectionRecords { get; set; } = new List<CollectionRecord>();

    public int GetYearsOfService(DateOnly asOf)
    {
        var years = asOf.Year - HireDate.Year;
        if (asOf < HireDate.AddYears(years)) years--;
        return Math.Max(0, years);
    }

    public string GetServicePeriodText(DateOnly asOf)
    {
        var years = asOf.Year - HireDate.Year;
        if (asOf < HireDate.AddYears(years)) years--;
        years = Math.Max(0, years);

        var months = asOf.Month - HireDate.AddYears(years).Month;
        if (asOf.Day < HireDate.Day) months--;
        if (months < 0) months += 12;

        return $"{years}年{months}ヶ月";
    }
}
