using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LPG_Tsumitate_Kanri2.Data;
using LPG_Tsumitate_Kanri2.Models.Entities;

namespace LPG_Tsumitate_Kanri2.Controllers;

public class ReceiptsController : Controller
{
    private static readonly HashSet<string> AllowedContentTypes =
        ["image/jpeg", "image/png", "image/gif", "application/pdf"];

    private const long MaxFileSizeBytes = 10 * 1024 * 1024; // 10 MB

    private readonly AppDbContext _db;
    private readonly IWebHostEnvironment _env;

    public ReceiptsController(AppDbContext db, IWebHostEnvironment env)
    {
        _db = db;
        _env = env;
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Upload(int entryId, IFormFile file)
    {
        var entry = await _db.LedgerEntries.FindAsync(entryId);
        if (entry == null) return NotFound();

        if (file == null || file.Length == 0)
        {
            TempData["Error"] = "ファイルを選択してください。";
            return RedirectToAction("Details", "Ledger", new { id = entryId });
        }

        if (file.Length > MaxFileSizeBytes)
        {
            TempData["Error"] = "ファイルサイズは10MB以下にしてください。";
            return RedirectToAction("Details", "Ledger", new { id = entryId });
        }

        if (!AllowedContentTypes.Contains(file.ContentType))
        {
            TempData["Error"] = "JPEG・PNG・PDF のみアップロード可能です。";
            return RedirectToAction("Details", "Ledger", new { id = entryId });
        }

        var ext = Path.GetExtension(file.FileName);
        var storedName = $"{Guid.NewGuid()}{ext}";
        var uploadDir = Path.Combine(_env.ContentRootPath, "App_Data", "receipts");
        Directory.CreateDirectory(uploadDir);

        await using (var stream = System.IO.File.Create(Path.Combine(uploadDir, storedName)))
        {
            await file.CopyToAsync(stream);
        }

        var receipt = new Receipt
        {
            EntryId = entryId,
            OriginalFileName = file.FileName,
            StoredFileName = storedName,
            FileSize = (int)file.Length,
            ContentType = file.ContentType,
            UploadedAt = DateTime.Now
        };
        _db.Receipts.Add(receipt);
        await _db.SaveChangesAsync();

        return RedirectToAction("Details", "Ledger", new { id = entryId });
    }

    public async Task<IActionResult> View(int id)
    {
        var receipt = await _db.Receipts.FindAsync(id);
        if (receipt == null) return NotFound();

        var filePath = Path.Combine(_env.ContentRootPath, "App_Data", "receipts", receipt.StoredFileName);
        if (!System.IO.File.Exists(filePath)) return NotFound();

        var stream = System.IO.File.OpenRead(filePath);
        return File(stream, receipt.ContentType);
    }

    public async Task<IActionResult> Download(int id)
    {
        var receipt = await _db.Receipts.FindAsync(id);
        if (receipt == null) return NotFound();

        var filePath = Path.Combine(_env.ContentRootPath, "App_Data", "receipts", receipt.StoredFileName);
        if (!System.IO.File.Exists(filePath)) return NotFound();

        var stream = System.IO.File.OpenRead(filePath);
        return File(stream, receipt.ContentType, receipt.OriginalFileName);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var receipt = await _db.Receipts.FindAsync(id);
        if (receipt == null) return NotFound();

        var entryId = receipt.EntryId;
        var filePath = Path.Combine(_env.ContentRootPath, "App_Data", "receipts", receipt.StoredFileName);
        if (System.IO.File.Exists(filePath)) System.IO.File.Delete(filePath);

        _db.Receipts.Remove(receipt);
        await _db.SaveChangesAsync();

        return RedirectToAction("Details", "Ledger", new { id = entryId });
    }
}
