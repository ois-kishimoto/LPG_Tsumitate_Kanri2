using LPG_Tsumitate_Kanri2.Models.Entities;

namespace LPG_Tsumitate_Kanri2.Services;

public class ContributionCalculator
{
    /// <summary>
    /// 社員の属性と適用ルールから徴収金額を算出する。
    /// rules は同一 SavingsType・有効期間でフィルタ済み、Priority 昇順であること。
    /// </summary>
    public int Calculate(Employee employee, IEnumerable<ContributionAmountRule> rules, DateOnly asOf)
    {
        var yearsOfService = employee.GetYearsOfService(asOf);

        foreach (var rule in rules)
        {
            if (rule.ConditionEmploymentType is not null && rule.ConditionEmploymentType != employee.EmploymentType)
                continue;

            if (rule.ConditionMaxYearsOfService.HasValue && yearsOfService >= rule.ConditionMaxYearsOfService.Value)
                continue;

            if (rule.ConditionPositionCategory is not null && rule.ConditionPositionCategory != employee.PositionCategory)
                continue;

            return rule.Amount;
        }

        return 0;
    }
}
