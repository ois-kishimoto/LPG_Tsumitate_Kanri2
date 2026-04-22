using Microsoft.EntityFrameworkCore;
using LPG_Tsumitate_Kanri2.Models.Entities;
using LPG_Tsumitate_Kanri2.Services;

namespace LPG_Tsumitate_Kanri2.Data;

public static class DataSeeder
{
    public static async Task SeedAsync(AppDbContext db, ContributionCalculator calculator)
    {
        if (await db.Employees.AnyAsync()) return;

        // ── 社員 ──────────────────────────────────────────────────────────────
        var employees = new List<Employee>
        {
            new() { EmployeeNo = "21", FullName = "儀保 清美",     HireDate = new DateOnly(1990,  6,  4), BirthDate = new DateOnly(1965, 10, 15), EmploymentType = "正社員", PositionCategory = "役職者" },
            new() { EmployeeNo = "43", FullName = "辺土名 秀諭",   HireDate = new DateOnly(1990,  9, 20), BirthDate = new DateOnly(1966,  8,  4), EmploymentType = "正社員", PositionCategory = "役職者" },
            new() { EmployeeNo = "44", FullName = "佐伯 博史",     HireDate = new DateOnly(2003, 11,  1), BirthDate = new DateOnly(1975,  1, 11), EmploymentType = "正社員", PositionCategory = "役職者" },
            new() { EmployeeNo = "45", FullName = "村上 桂市",     HireDate = new DateOnly(2008, 10,  1), BirthDate = new DateOnly(1975,  9,  5), EmploymentType = "正社員", PositionCategory = "役職者" },
            new() { EmployeeNo = "24", FullName = "山川 みのり",   HireDate = new DateOnly(2011,  6,  1), BirthDate = new DateOnly(1991,  9, 30), EmploymentType = "正社員", PositionCategory = "一般職" },
            new() { EmployeeNo = "27", FullName = "浦崎 賢太",     HireDate = new DateOnly(2016, 10,  1), BirthDate = new DateOnly(1991, 12, 26), EmploymentType = "正社員", PositionCategory = "役職者" },
            new() { EmployeeNo = "25", FullName = "仲間 早妃子",   HireDate = new DateOnly(2016, 11,  1), BirthDate = new DateOnly(1993,  2,  6), EmploymentType = "正社員", PositionCategory = "一般職" },
            new() { EmployeeNo = "46", FullName = "糸数 智樹",     HireDate = new DateOnly(2017, 11,  1), BirthDate = new DateOnly(1986,  3, 27), EmploymentType = "正社員", PositionCategory = "一般職" },
            new() { EmployeeNo = "47", FullName = "根間 大輝",     HireDate = new DateOnly(2018,  5, 21), BirthDate = new DateOnly(1990,  5,  4), EmploymentType = "正社員", PositionCategory = "一般職" },
            new() { EmployeeNo = "28", FullName = "比嘉 奈津美",   HireDate = new DateOnly(2019,  2, 12), BirthDate = new DateOnly(1994,  1,  7), EmploymentType = "正社員", PositionCategory = "一般職" },
            new() { EmployeeNo = "48", FullName = "銘苅 翔太",     HireDate = new DateOnly(2020,  8, 11), BirthDate = new DateOnly(1990,  1, 20), EmploymentType = "正社員", PositionCategory = "一般職" },
            new() { EmployeeNo = "29", FullName = "小林 東夢",     HireDate = new DateOnly(2021,  2, 16), BirthDate = new DateOnly(1995,  5, 31), EmploymentType = "正社員", PositionCategory = "一般職" },
            new() { EmployeeNo = "23", FullName = "幸喜 莉沙",     HireDate = new DateOnly(2021,  4, 12), BirthDate = new DateOnly(1997,  8,  6), EmploymentType = "正社員", PositionCategory = "一般職" },
            new() { EmployeeNo = "26", FullName = "諸喜田 樹",     HireDate = new DateOnly(2021,  8,  2), BirthDate = new DateOnly(1996,  4,  2), EmploymentType = "正社員", PositionCategory = "一般職" },
            new() { EmployeeNo = "22", FullName = "石川 翔一郎",   HireDate = new DateOnly(2024,  1, 15), BirthDate = new DateOnly(2002,  1,  6), EmploymentType = "正社員", PositionCategory = "一般職" },
            new() { EmployeeNo = "32", FullName = "新城 美咲",     HireDate = new DateOnly(2024,  9,  2), BirthDate = new DateOnly(1996,  7, 12), EmploymentType = "正社員", PositionCategory = "一般職" },
            new() { EmployeeNo = "33", FullName = "田中 美帆",     HireDate = new DateOnly(2025,  1,  6), BirthDate = new DateOnly(2000,  4,  2), EmploymentType = "正社員", PositionCategory = "一般職" },
            new() { EmployeeNo = "34", FullName = "山本 暉",       HireDate = new DateOnly(2025,  3,  3), BirthDate = new DateOnly(1997,  4, 25), EmploymentType = "正社員", PositionCategory = "一般職" },
            new() { EmployeeNo = "31", FullName = "玉城 千佳子",   HireDate = new DateOnly(2025,  6,  1), BirthDate = null,                       EmploymentType = "パート",  PositionCategory = "一般職" },
            new() { EmployeeNo = "49", FullName = "岸本 幸太",     HireDate = new DateOnly(2025, 12,  1), BirthDate = new DateOnly(1994,  6,  7), EmploymentType = "正社員", PositionCategory = "一般職" },
            new() { EmployeeNo = "35", FullName = "田原 賢斗",     HireDate = new DateOnly(2026,  4,  6), BirthDate = null,                       EmploymentType = "正社員", PositionCategory = "一般職" },
        };
        db.Employees.AddRange(employees);
        await db.SaveChangesAsync();

        // ── 過去3か月（締め切り済）＋ 今月（進行中）────────────────────────────
        var pastMonths = new[] { (Year: 2026, Month: 1), (Year: 2026, Month: 2), (Year: 2026, Month: 3) };
        var savingsTypeIds = new[] { 1, 2 };
        int balance1 = 0; // 通常積立残高
        int balance2 = 0; // 還暦積立残高

        foreach (var (year, month) in pastMonths)
        {
            // そのセッション月の月末時点で在籍している社員のみ対象
            var monthEnd = new DateOnly(year, month, DateTime.DaysInMonth(year, month));
            var sessionEmployees = employees.Where(e => e.HireDate <= monthEnd).ToList();
            var asOf = new DateOnly(year, month, 1);

            foreach (var stId in savingsTypeIds)
            {
                var rules = await db.ContributionAmountRules
                    .Where(r => r.SavingsTypeId == stId
                        && r.ValidFrom <= asOf
                        && (r.ValidTo == null || r.ValidTo >= asOf))
                    .OrderBy(r => r.Priority)
                    .ToListAsync();

                var session = new CollectionSession
                {
                    SavingsTypeId = stId,
                    Year = year,
                    Month = month,
                    SessionDate = new DateOnly(year, month, 20),
                    IsCompleted = true,
                    CreatedAt = new DateTime(year, month, 1)
                };
                db.CollectionSessions.Add(session);
                await db.SaveChangesAsync();

                var records = sessionEmployees.Select(emp => new CollectionRecord
                {
                    SessionId = session.SessionId,
                    EmployeeId = emp.EmployeeId,
                    ExpectedAmount = calculator.Calculate(emp, rules, asOf),
                    IsCollected = true,
                    IsExcluded = false,
                    CollectedAt = new DateTime(year, month, 20)
                }).ToList();

                // 通常積立のみ：懇親会不参加として1人除外（ダミー）
                if (stId == 1 && records.Count > 1)
                {
                    records[^1].IsCollected = false;
                    records[^1].IsExcluded = true;
                    records[^1].CollectedAt = null;
                }

                db.CollectionRecords.AddRange(records);
                await db.SaveChangesAsync();

                var stName = stId == 1 ? "通常積立" : "還暦積立";
                var total = records.Where(r => r.IsCollected && !r.IsExcluded).Sum(r => r.ExpectedAmount);

                ref var bal = ref (stId == 1 ? ref balance1 : ref balance2);
                bal += total;

                db.LedgerEntries.Add(new LedgerEntry
                {
                    SavingsTypeId = stId,
                    TransactionDate = new DateOnly(year, month, 20),
                    EntryType = "入金",
                    Description = $"{year}年{month}月分 {stName} 徴収分",
                    Amount = total,
                    BalanceAfter = bal,
                    IsAutoGenerated = true,
                    SourceSessionId = session.SessionId,
                    CreatedAt = new DateTime(year, month, 20)
                });
                await db.SaveChangesAsync();
            }
        }

        // ── 手動支出（ダミー）────────────────────────────────────────────────
        balance1 -= 20000;
        db.LedgerEntries.Add(new LedgerEntry
        {
            SavingsTypeId = 1,
            TransactionDate = new DateOnly(2026, 2, 20),
            EntryType = "出金",
            Description = "2月懇親会 食事代",
            Amount = 20000,
            BalanceAfter = balance1,
            IsAutoGenerated = false,
            CreatedAt = new DateTime(2026, 2, 20)
        });

        balance1 -= 3000;
        db.LedgerEntries.Add(new LedgerEntry
        {
            SavingsTypeId = 1,
            TransactionDate = new DateOnly(2026, 3, 15),
            EntryType = "出金",
            Description = "比嘉さん誕生日プレゼント代",
            Amount = 3000,
            BalanceAfter = balance1,
            IsAutoGenerated = false,
            CreatedAt = new DateTime(2026, 3, 15)
        });
        await db.SaveChangesAsync();

        // ── 4月（進行中）────────────────────────────────────────────────────
        var aprEnd = new DateOnly(2026, 4, 30);
        var aprEmployees = employees.Where(e => e.HireDate <= aprEnd).ToList();
        var aprAsOf = new DateOnly(2026, 4, 1);

        var aprRulesNormal = await db.ContributionAmountRules
            .Where(r => r.SavingsTypeId == 1 && r.ValidFrom <= aprAsOf && (r.ValidTo == null || r.ValidTo >= aprAsOf))
            .OrderBy(r => r.Priority).ToListAsync();

        var aprNormalSession = new CollectionSession
        {
            SavingsTypeId = 1, Year = 2026, Month = 4,
            SessionDate = new DateOnly(2026, 4, 25),
            IsCompleted = false,
            CreatedAt = DateTime.Now
        };
        db.CollectionSessions.Add(aprNormalSession);
        await db.SaveChangesAsync();

        var aprNormalRecords = aprEmployees.Select((emp, i) => new CollectionRecord
        {
            SessionId = aprNormalSession.SessionId,
            EmployeeId = emp.EmployeeId,
            ExpectedAmount = calculator.Calculate(emp, aprRulesNormal, aprAsOf),
            IsCollected = i < 5,  // 先頭5人のみ徴収済み（ダミー）
            IsExcluded = false,
            CollectedAt = i < 5 ? DateTime.Now : null
        }).ToList();
        db.CollectionRecords.AddRange(aprNormalRecords);

        var aprRulesKanreki = await db.ContributionAmountRules
            .Where(r => r.SavingsTypeId == 2 && r.ValidFrom <= aprAsOf && (r.ValidTo == null || r.ValidTo >= aprAsOf))
            .OrderBy(r => r.Priority).ToListAsync();

        var aprKanrekiSession = new CollectionSession
        {
            SavingsTypeId = 2, Year = 2026, Month = 4,
            SessionDate = new DateOnly(2026, 4, 25),
            IsCompleted = false,
            CreatedAt = DateTime.Now
        };
        db.CollectionSessions.Add(aprKanrekiSession);
        await db.SaveChangesAsync();

        var aprKanrekiRecords = aprEmployees.Select((emp, i) => new CollectionRecord
        {
            SessionId = aprKanrekiSession.SessionId,
            EmployeeId = emp.EmployeeId,
            ExpectedAmount = calculator.Calculate(emp, aprRulesKanreki, aprAsOf),
            IsCollected = i < 3,  // 先頭3人のみ徴収済み（ダミー）
            IsExcluded = false,
            CollectedAt = i < 3 ? DateTime.Now : null
        }).ToList();
        db.CollectionRecords.AddRange(aprKanrekiRecords);

        await db.SaveChangesAsync();
    }
}
