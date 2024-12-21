using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mika.Infastructure.Migrations
{
    /// <inheritdoc />
    public partial class init14030717 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Companies",
                columns: table => new
                {
                    CompanyId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompanyNumber = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    CompanyCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Creator = table.Column<long>(type: "bigint", nullable: false),
                    LastModificationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifier = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Companies", x => x.CompanyId);
                });

            migrationBuilder.CreateTable(
                name: "Modules",
                columns: table => new
                {
                    ModuleId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ModuleName = table.Column<string>(type: "nvarchar(45)", maxLength: 45, nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Modules", x => x.ModuleId);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    RoleId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleName = table.Column<string>(type: "nvarchar(45)", maxLength: 45, nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.RoleId);
                });

            migrationBuilder.CreateTable(
                name: "Accounts",
                columns: table => new
                {
                    AccountId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AccountNumber = table.Column<string>(type: "nvarchar(70)", maxLength: 70, nullable: false),
                    BankName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    AccountType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    InterestRatePercent = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ProjectName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    BelongsToCentralOffice = table.Column<bool>(type: "bit", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    CompanyId = table.Column<long>(type: "bigint", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Creator = table.Column<long>(type: "bigint", nullable: false),
                    LastModificationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifier = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Accounts", x => x.AccountId);
                    table.ForeignKey(
                        name: "FK_Accounts_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "CompanyId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SubModules",
                columns: table => new
                {
                    SubModuleId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SubModuleName = table.Column<string>(type: "nvarchar(45)", maxLength: 45, nullable: false),
                    SubDisplayName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ModuleId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubModules", x => x.SubModuleId);
                    table.ForeignKey(
                        name: "FK_SubModules_Modules_ModuleId",
                        column: x => x.ModuleId,
                        principalTable: "Modules",
                        principalColumn: "ModuleId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(35)", maxLength: 35, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(35)", maxLength: 35, nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(22)", maxLength: 22, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(70)", maxLength: 70, nullable: true),
                    Position = table.Column<string>(type: "nvarchar(45)", maxLength: 45, nullable: true),
                    PersonnelNumber = table.Column<string>(type: "nvarchar(24)", maxLength: 24, nullable: true),
                    Avatar = table.Column<string>(type: "nvarchar(400)", maxLength: 400, nullable: true),
                    RoleId = table.Column<int>(type: "int", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Creator = table.Column<long>(type: "bigint", nullable: false),
                    LastModificationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifier = table.Column<long>(type: "bigint", nullable: true),
                    PrimaryParentId = table.Column<long>(type: "bigint", nullable: true),
                    SecodaryParentIDs = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserId);
                    table.ForeignKey(
                        name: "FK_Users_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "RoleId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BiDataEntries",
                columns: table => new
                {
                    BiDataEntryId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BalanceOfThePreviousDay = table.Column<long>(type: "bigint", nullable: false),
                    BlockadeAmount = table.Column<long>(type: "bigint", nullable: false),
                    DepositDuringTheDay = table.Column<long>(type: "bigint", nullable: false),
                    WithdrawalDuringTheDay = table.Column<long>(type: "bigint", nullable: false),
                    BalanceWithTheBank = table.Column<long>(type: "bigint", nullable: false),
                    OnTheWayReceivablePayableDocs = table.Column<long>(type: "bigint", nullable: false),
                    DefinitiveReceivablePayableDocs = table.Column<long>(type: "bigint", nullable: false),
                    WithdrawalBalance = table.Column<long>(type: "bigint", nullable: false),
                    AccountBalance = table.Column<long>(type: "bigint", nullable: false),
                    EntryDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AccountId = table.Column<long>(type: "bigint", nullable: false),
                    CreatorId = table.Column<long>(type: "bigint", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModificationDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BiDataEntries", x => x.BiDataEntryId);
                    table.ForeignKey(
                        name: "FK_BiDataEntries_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "AccountId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BiDataEntries_Users_CreatorId",
                        column: x => x.CreatorId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "F_UserAccounts",
                columns: table => new
                {
                    F_UserAccountId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    AccountId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_F_UserAccounts", x => x.F_UserAccountId);
                    table.ForeignKey(
                        name: "FK_F_UserAccounts_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "AccountId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_F_UserAccounts_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "F_UserSubModules",
                columns: table => new
                {
                    F_UserSubModuleId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    SubModuleId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_F_UserSubModules", x => x.F_UserSubModuleId);
                    table.ForeignKey(
                        name: "FK_F_UserSubModules_SubModules_SubModuleId",
                        column: x => x.SubModuleId,
                        principalTable: "SubModules",
                        principalColumn: "SubModuleId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_F_UserSubModules_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HistoryLogs",
                columns: table => new
                {
                    HistoryLogId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EntityName = table.Column<string>(type: "nvarchar(45)", maxLength: 45, nullable: false),
                    EntityId = table.Column<long>(type: "bigint", nullable: false),
                    LogDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModuleId = table.Column<int>(type: "int", nullable: false),
                    LoggerId = table.Column<long>(type: "bigint", nullable: false),
                    Key = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HistoryLogs", x => x.HistoryLogId);
                    table.ForeignKey(
                        name: "FK_HistoryLogs_Modules_ModuleId",
                        column: x => x.ModuleId,
                        principalTable: "Modules",
                        principalColumn: "ModuleId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HistoryLogs_Users_LoggerId",
                        column: x => x.LoggerId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HistoryLogDetails",
                columns: table => new
                {
                    HistoryLogDetailId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Key = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HistoryLogId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HistoryLogDetails", x => x.HistoryLogDetailId);
                    table.ForeignKey(
                        name: "FK_HistoryLogDetails_HistoryLogs_HistoryLogId",
                        column: x => x.HistoryLogId,
                        principalTable: "HistoryLogs",
                        principalColumn: "HistoryLogId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_CompanyId",
                table: "Accounts",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_BiDataEntries_AccountId",
                table: "BiDataEntries",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_BiDataEntries_CreatorId",
                table: "BiDataEntries",
                column: "CreatorId");

            migrationBuilder.CreateIndex(
                name: "IX_F_UserAccounts_AccountId",
                table: "F_UserAccounts",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_F_UserAccounts_UserId",
                table: "F_UserAccounts",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_F_UserSubModules_SubModuleId",
                table: "F_UserSubModules",
                column: "SubModuleId");

            migrationBuilder.CreateIndex(
                name: "IX_F_UserSubModules_UserId",
                table: "F_UserSubModules",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_HistoryLogDetails_HistoryLogId",
                table: "HistoryLogDetails",
                column: "HistoryLogId");

            migrationBuilder.CreateIndex(
                name: "IX_HistoryLogs_LoggerId",
                table: "HistoryLogs",
                column: "LoggerId");

            migrationBuilder.CreateIndex(
                name: "IX_HistoryLogs_ModuleId",
                table: "HistoryLogs",
                column: "ModuleId");

            migrationBuilder.CreateIndex(
                name: "IX_SubModules_ModuleId",
                table: "SubModules",
                column: "ModuleId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_RoleId",
                table: "Users",
                column: "RoleId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BiDataEntries");

            migrationBuilder.DropTable(
                name: "F_UserAccounts");

            migrationBuilder.DropTable(
                name: "F_UserSubModules");

            migrationBuilder.DropTable(
                name: "HistoryLogDetails");

            migrationBuilder.DropTable(
                name: "Accounts");

            migrationBuilder.DropTable(
                name: "SubModules");

            migrationBuilder.DropTable(
                name: "HistoryLogs");

            migrationBuilder.DropTable(
                name: "Companies");

            migrationBuilder.DropTable(
                name: "Modules");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Roles");
        }
    }
}
