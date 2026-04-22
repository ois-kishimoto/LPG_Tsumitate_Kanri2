using Microsoft.EntityFrameworkCore;
using LPG_Tsumitate_Kanri2.Models.Entities;

namespace LPG_Tsumitate_Kanri2.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<SavingsType> SavingsTypes => Set<SavingsType>();
    public DbSet<Employee> Employees => Set<Employee>();
    public DbSet<ContributionAmountRule> ContributionAmountRules => Set<ContributionAmountRule>();
    public DbSet<CollectionSession> CollectionSessions => Set<CollectionSession>();
    public DbSet<CollectionRecord> CollectionRecords => Set<CollectionRecord>();
    public DbSet<LedgerEntry> LedgerEntries => Set<LedgerEntry>();
    public DbSet<Receipt> Receipts => Set<Receipt>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<SavingsType>(e =>
        {
            e.HasKey(x => x.SavingsTypeId);
            e.Property(x => x.Name).HasMaxLength(50).IsRequired();
            e.HasData(
                new SavingsType { SavingsTypeId = 1, Name = "通常積立", DisplayOrder = 1 },
                new SavingsType { SavingsTypeId = 2, Name = "還暦積立", DisplayOrder = 2 }
            );
        });

        modelBuilder.Entity<Employee>(e =>
        {
            e.HasKey(x => x.EmployeeId);
            e.Property(x => x.EmployeeNo).HasMaxLength(20).IsRequired();
            e.HasIndex(x => x.EmployeeNo).IsUnique();
            e.Property(x => x.FullName).HasMaxLength(100).IsRequired();
            e.Property(x => x.EmploymentType).HasMaxLength(20).IsRequired();
            e.Property(x => x.PositionCategory).HasMaxLength(20).IsRequired();
            e.Property(x => x.CreatedAt).HasDefaultValueSql("SYSDATETIME()");
            e.Property(x => x.UpdatedAt).HasDefaultValueSql("SYSDATETIME()");
        });

        modelBuilder.Entity<ContributionAmountRule>(e =>
        {
            e.HasKey(x => x.RuleId);
            e.Property(x => x.ConditionEmploymentType).HasMaxLength(20);
            e.Property(x => x.ConditionPositionCategory).HasMaxLength(20);
            e.HasOne(x => x.SavingsType)
                .WithMany(x => x.ContributionAmountRules)
                .HasForeignKey(x => x.SavingsTypeId)
                .OnDelete(DeleteBehavior.Restrict);
            e.HasData(
                // 通常積立: 全員一律1000円
                new ContributionAmountRule { RuleId = 1, SavingsTypeId = 1, Priority = 1, Amount = 1000, ValidFrom = new DateOnly(2026, 1, 1) },
                // 還暦積立
                new ContributionAmountRule { RuleId = 2, SavingsTypeId = 2, ConditionEmploymentType = "パート", Priority = 1, Amount = 500, ValidFrom = new DateOnly(2026, 1, 1) },
                new ContributionAmountRule { RuleId = 3, SavingsTypeId = 2, ConditionMaxYearsOfService = 3, Priority = 2, Amount = 500, ValidFrom = new DateOnly(2026, 1, 1) },
                new ContributionAmountRule { RuleId = 4, SavingsTypeId = 2, ConditionPositionCategory = "役職者", Priority = 3, Amount = 2000, ValidFrom = new DateOnly(2026, 1, 1) },
                new ContributionAmountRule { RuleId = 5, SavingsTypeId = 2, ConditionPositionCategory = "一般職", Priority = 4, Amount = 1000, ValidFrom = new DateOnly(2026, 1, 1) }
            );
        });

        modelBuilder.Entity<CollectionSession>(e =>
        {
            e.HasKey(x => x.SessionId);
            e.Property(x => x.Notes).HasMaxLength(500);
            e.HasIndex(x => new { x.SavingsTypeId, x.Year, x.Month }).IsUnique();
            e.Property(x => x.CreatedAt).HasDefaultValueSql("SYSDATETIME()");
            e.HasOne(x => x.SavingsType)
                .WithMany(x => x.CollectionSessions)
                .HasForeignKey(x => x.SavingsTypeId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<CollectionRecord>(e =>
        {
            e.HasKey(x => x.RecordId);
            e.Property(x => x.Notes).HasMaxLength(200);
            e.HasIndex(x => new { x.SessionId, x.EmployeeId }).IsUnique();
            e.HasOne(x => x.Session)
                .WithMany(x => x.CollectionRecords)
                .HasForeignKey(x => x.SessionId);
            e.HasOne(x => x.Employee)
                .WithMany(x => x.CollectionRecords)
                .HasForeignKey(x => x.EmployeeId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<LedgerEntry>(e =>
        {
            e.HasKey(x => x.EntryId);
            e.Property(x => x.EntryType).HasMaxLength(10).IsRequired();
            e.Property(x => x.Description).HasMaxLength(200).IsRequired();
            e.Property(x => x.Notes).HasMaxLength(500);
            e.Property(x => x.CreatedAt).HasDefaultValueSql("SYSDATETIME()");
            e.HasOne(x => x.SavingsType)
                .WithMany(x => x.LedgerEntries)
                .HasForeignKey(x => x.SavingsTypeId)
                .OnDelete(DeleteBehavior.Restrict);
            e.HasOne(x => x.SourceSession)
                .WithMany()
                .HasForeignKey(x => x.SourceSessionId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<Receipt>(e =>
        {
            e.HasKey(x => x.ReceiptId);
            e.Property(x => x.OriginalFileName).HasMaxLength(255).IsRequired();
            e.Property(x => x.StoredFileName).HasMaxLength(100).IsRequired();
            e.Property(x => x.ContentType).HasMaxLength(50).IsRequired();
            e.Property(x => x.UploadedAt).HasDefaultValueSql("SYSDATETIME()");
            e.HasOne(x => x.Entry)
                .WithMany(x => x.Receipts)
                .HasForeignKey(x => x.EntryId);
        });
    }
}
