using Microsoft.EntityFrameworkCore;
using LPG_Tsumitate_Kanri2.Data;
using LPG_Tsumitate_Kanri2.Models.Entities;

namespace LPG_Tsumitate_Kanri2.Services;

public class LedgerService
{
    private readonly AppDbContext _db;

    public LedgerService(AppDbContext db) => _db = db;

    public async Task<int> GetCurrentBalanceAsync(int savingsTypeId)
    {
        var last = await _db.LedgerEntries
            .Where(e => e.SavingsTypeId == savingsTypeId)
            .OrderByDescending(e => e.TransactionDate)
            .ThenByDescending(e => e.EntryId)
            .FirstOrDefaultAsync();
        return last?.BalanceAfter ?? 0;
    }

    /// <summary>
    /// 追加する新規エントリの BalanceAfter を計算して設定する。
    /// DB 保存は呼び出し元で行う。
    /// </summary>
    public async Task SetBalanceAsync(LedgerEntry entry)
    {
        var previous = await _db.LedgerEntries
            .Where(e => e.SavingsTypeId == entry.SavingsTypeId
                && (e.TransactionDate < entry.TransactionDate
                    || (e.TransactionDate == entry.TransactionDate && e.EntryId < entry.EntryId)))
            .OrderByDescending(e => e.TransactionDate)
            .ThenByDescending(e => e.EntryId)
            .FirstOrDefaultAsync();

        var previousBalance = previous?.BalanceAfter ?? 0;
        entry.BalanceAfter = entry.EntryType == "入金"
            ? previousBalance + entry.Amount
            : previousBalance - entry.Amount;
    }

    /// <summary>
    /// entryId 以降（同日含む）のエントリの BalanceAfter を再計算する。
    /// 編集・削除後に呼び出す。
    /// </summary>
    public async Task RecalculateFromAsync(int savingsTypeId, DateOnly fromDate, int fromEntryId)
    {
        var affected = await _db.LedgerEntries
            .Where(e => e.SavingsTypeId == savingsTypeId
                && (e.TransactionDate > fromDate
                    || (e.TransactionDate == fromDate && e.EntryId >= fromEntryId)))
            .OrderBy(e => e.TransactionDate)
            .ThenBy(e => e.EntryId)
            .ToListAsync();

        var previous = await _db.LedgerEntries
            .Where(e => e.SavingsTypeId == savingsTypeId
                && (e.TransactionDate < fromDate
                    || (e.TransactionDate == fromDate && e.EntryId < fromEntryId)))
            .OrderByDescending(e => e.TransactionDate)
            .ThenByDescending(e => e.EntryId)
            .FirstOrDefaultAsync();

        var balance = previous?.BalanceAfter ?? 0;
        foreach (var e in affected)
        {
            balance = e.EntryType == "入金" ? balance + e.Amount : balance - e.Amount;
            e.BalanceAfter = balance;
        }

        await _db.SaveChangesAsync();
    }
}
