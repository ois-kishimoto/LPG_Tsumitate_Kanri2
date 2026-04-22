using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace LPG_Tsumitate_Kanri2.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Employees",
                columns: table => new
                {
                    EmployeeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeNo = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    HireDate = table.Column<DateOnly>(type: "date", nullable: false),
                    BirthDate = table.Column<DateOnly>(type: "date", nullable: true),
                    EmploymentType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    PositionCategory = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSDATETIME()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSDATETIME()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Employees", x => x.EmployeeId);
                });

            migrationBuilder.CreateTable(
                name: "SavingsTypes",
                columns: table => new
                {
                    SavingsTypeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SavingsTypes", x => x.SavingsTypeId);
                });

            migrationBuilder.CreateTable(
                name: "CollectionSessions",
                columns: table => new
                {
                    SessionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SavingsTypeId = table.Column<int>(type: "int", nullable: false),
                    Year = table.Column<int>(type: "int", nullable: false),
                    Month = table.Column<int>(type: "int", nullable: false),
                    SessionDate = table.Column<DateOnly>(type: "date", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsCompleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSDATETIME()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CollectionSessions", x => x.SessionId);
                    table.ForeignKey(
                        name: "FK_CollectionSessions_SavingsTypes_SavingsTypeId",
                        column: x => x.SavingsTypeId,
                        principalTable: "SavingsTypes",
                        principalColumn: "SavingsTypeId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ContributionAmountRules",
                columns: table => new
                {
                    RuleId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SavingsTypeId = table.Column<int>(type: "int", nullable: false),
                    ConditionEmploymentType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    ConditionMaxYearsOfService = table.Column<int>(type: "int", nullable: true),
                    ConditionPositionCategory = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Amount = table.Column<int>(type: "int", nullable: false),
                    Priority = table.Column<int>(type: "int", nullable: false),
                    ValidFrom = table.Column<DateOnly>(type: "date", nullable: false),
                    ValidTo = table.Column<DateOnly>(type: "date", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContributionAmountRules", x => x.RuleId);
                    table.ForeignKey(
                        name: "FK_ContributionAmountRules_SavingsTypes_SavingsTypeId",
                        column: x => x.SavingsTypeId,
                        principalTable: "SavingsTypes",
                        principalColumn: "SavingsTypeId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CollectionRecords",
                columns: table => new
                {
                    RecordId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SessionId = table.Column<int>(type: "int", nullable: false),
                    EmployeeId = table.Column<int>(type: "int", nullable: false),
                    ExpectedAmount = table.Column<int>(type: "int", nullable: false),
                    IsExcluded = table.Column<bool>(type: "bit", nullable: false),
                    IsCollected = table.Column<bool>(type: "bit", nullable: false),
                    CollectedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CollectionRecords", x => x.RecordId);
                    table.ForeignKey(
                        name: "FK_CollectionRecords_CollectionSessions_SessionId",
                        column: x => x.SessionId,
                        principalTable: "CollectionSessions",
                        principalColumn: "SessionId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CollectionRecords_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "EmployeeId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LedgerEntries",
                columns: table => new
                {
                    EntryId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SavingsTypeId = table.Column<int>(type: "int", nullable: false),
                    TransactionDate = table.Column<DateOnly>(type: "date", nullable: false),
                    EntryType = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Amount = table.Column<int>(type: "int", nullable: false),
                    BalanceAfter = table.Column<int>(type: "int", nullable: false),
                    IsAutoGenerated = table.Column<bool>(type: "bit", nullable: false),
                    SourceSessionId = table.Column<int>(type: "int", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSDATETIME()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LedgerEntries", x => x.EntryId);
                    table.ForeignKey(
                        name: "FK_LedgerEntries_CollectionSessions_SourceSessionId",
                        column: x => x.SourceSessionId,
                        principalTable: "CollectionSessions",
                        principalColumn: "SessionId",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_LedgerEntries_SavingsTypes_SavingsTypeId",
                        column: x => x.SavingsTypeId,
                        principalTable: "SavingsTypes",
                        principalColumn: "SavingsTypeId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Receipts",
                columns: table => new
                {
                    ReceiptId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EntryId = table.Column<int>(type: "int", nullable: false),
                    OriginalFileName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    StoredFileName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    FileSize = table.Column<int>(type: "int", nullable: false),
                    ContentType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    UploadedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSDATETIME()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Receipts", x => x.ReceiptId);
                    table.ForeignKey(
                        name: "FK_Receipts_LedgerEntries_EntryId",
                        column: x => x.EntryId,
                        principalTable: "LedgerEntries",
                        principalColumn: "EntryId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "SavingsTypes",
                columns: new[] { "SavingsTypeId", "DisplayOrder", "Name" },
                values: new object[,]
                {
                    { 1, 1, "通常積立" },
                    { 2, 2, "還暦積立" }
                });

            migrationBuilder.InsertData(
                table: "ContributionAmountRules",
                columns: new[] { "RuleId", "Amount", "ConditionEmploymentType", "ConditionMaxYearsOfService", "ConditionPositionCategory", "Priority", "SavingsTypeId", "ValidFrom", "ValidTo" },
                values: new object[,]
                {
                    { 1, 1000, null, null, null, 1, 1, new DateOnly(2026, 1, 1), null },
                    { 2, 500, "パート", null, null, 1, 2, new DateOnly(2026, 1, 1), null },
                    { 3, 500, null, 3, null, 2, 2, new DateOnly(2026, 1, 1), null },
                    { 4, 2000, null, null, "役職者", 3, 2, new DateOnly(2026, 1, 1), null },
                    { 5, 1000, null, null, "一般職", 4, 2, new DateOnly(2026, 1, 1), null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_CollectionRecords_EmployeeId",
                table: "CollectionRecords",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_CollectionRecords_SessionId_EmployeeId",
                table: "CollectionRecords",
                columns: new[] { "SessionId", "EmployeeId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CollectionSessions_SavingsTypeId_Year_Month",
                table: "CollectionSessions",
                columns: new[] { "SavingsTypeId", "Year", "Month" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ContributionAmountRules_SavingsTypeId",
                table: "ContributionAmountRules",
                column: "SavingsTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_EmployeeNo",
                table: "Employees",
                column: "EmployeeNo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LedgerEntries_SavingsTypeId",
                table: "LedgerEntries",
                column: "SavingsTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_LedgerEntries_SourceSessionId",
                table: "LedgerEntries",
                column: "SourceSessionId");

            migrationBuilder.CreateIndex(
                name: "IX_Receipts_EntryId",
                table: "Receipts",
                column: "EntryId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CollectionRecords");

            migrationBuilder.DropTable(
                name: "ContributionAmountRules");

            migrationBuilder.DropTable(
                name: "Receipts");

            migrationBuilder.DropTable(
                name: "Employees");

            migrationBuilder.DropTable(
                name: "LedgerEntries");

            migrationBuilder.DropTable(
                name: "CollectionSessions");

            migrationBuilder.DropTable(
                name: "SavingsTypes");
        }
    }
}
