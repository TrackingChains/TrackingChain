using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrackingChain.Core.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
#pragma warning disable CA1062 // Validate arguments of public methods
            migrationBuilder.CreateTable(
                name: "Accounts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ChainWriterAddress = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ChainWatcherAddress = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PrivateKey = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Accounts", x => x.Id);
                });
#pragma warning restore CA1062 // Validate arguments of public methods

            migrationBuilder.CreateTable(
                name: "ReportData",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Sent = table.Column<bool>(type: "bit", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReportData", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ReportSettings",
                columns: table => new
                {
                    Key = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReportSettings", x => x.Key);
                });

            migrationBuilder.CreateTable(
                name: "SmartContract",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ChainNumberId = table.Column<int>(type: "int", nullable: false),
                    ChainType = table.Column<int>(type: "int", nullable: false),
                    Currency = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ExtraInfo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SmartContract", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TransactionPendings",
                columns: table => new
                {
                    TrackingId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Completed = table.Column<bool>(type: "bit", nullable: false),
                    ErrorTimes = table.Column<int>(type: "int", nullable: false),
                    IsInProgress = table.Column<bool>(type: "bit", nullable: false),
                    LastUnlockedError = table.Column<int>(type: "int", nullable: true),
                    Locked = table.Column<bool>(type: "bit", nullable: false),
                    LockedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LockedDated = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    TriageDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TxHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Priority = table.Column<byte>(type: "tinyint", nullable: false),
                    PoolDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    WatchingFrom = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DataValue = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ReceivedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ChainNumberId = table.Column<int>(type: "int", nullable: false),
                    ChainType = table.Column<int>(type: "int", nullable: false),
                    SmartContractId = table.Column<long>(type: "bigint", nullable: false),
                    SmartContractAddress = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SmartContractExtraInfo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProfileGroupId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransactionPendings", x => x.TrackingId);
                });

            migrationBuilder.CreateTable(
                name: "TransactionPools",
                columns: table => new
                {
                    TrackingId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Completed = table.Column<bool>(type: "bit", nullable: false),
                    ErrorTimes = table.Column<int>(type: "int", nullable: false),
                    GeneratingFrom = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastUnlockedError = table.Column<int>(type: "int", nullable: true),
                    Locked = table.Column<bool>(type: "bit", nullable: false),
                    LockedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LockedDated = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Priority = table.Column<byte>(type: "tinyint", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    TriageDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DataValue = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ReceivedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ChainNumberId = table.Column<int>(type: "int", nullable: false),
                    ChainType = table.Column<int>(type: "int", nullable: false),
                    SmartContractId = table.Column<long>(type: "bigint", nullable: false),
                    SmartContractAddress = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SmartContractExtraInfo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProfileGroupId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransactionPools", x => x.TrackingId);
                });

            migrationBuilder.CreateTable(
                name: "TransactionRegistries",
                columns: table => new
                {
                    TrackingId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ErrorTime = table.Column<int>(type: "int", nullable: false),
                    LastTransactionHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TransactionStep = table.Column<int>(type: "int", nullable: false),
                    TransactionErrorReason = table.Column<int>(type: "int", nullable: true),
                    TriageDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PendingDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PoolDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ReceiptBlockHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReceiptBlockNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReceiptCumulativeGasUsed = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReceiptEffectiveGasPrice = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReceiptFrom = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReceiptGasUsed = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReceiptReceived = table.Column<bool>(type: "bit", nullable: false),
                    ReceiptTransactionHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReceiptTo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RegistryDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SmartContractEndpoint = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DataValue = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ReceivedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ChainNumberId = table.Column<int>(type: "int", nullable: false),
                    ChainType = table.Column<int>(type: "int", nullable: false),
                    SmartContractId = table.Column<long>(type: "bigint", nullable: false),
                    SmartContractAddress = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SmartContractExtraInfo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProfileGroupId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransactionRegistries", x => x.TrackingId);
                });

            migrationBuilder.CreateTable(
                name: "TransactionTriages",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Completed = table.Column<bool>(type: "bit", nullable: false),
                    IsInPool = table.Column<bool>(type: "bit", nullable: false),
                    TrackingIdentify = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DataValue = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ReceivedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ChainNumberId = table.Column<int>(type: "int", nullable: false),
                    ChainType = table.Column<int>(type: "int", nullable: false),
                    SmartContractId = table.Column<long>(type: "bigint", nullable: false),
                    SmartContractAddress = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SmartContractExtraInfo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProfileGroupId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransactionTriages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Reports",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Priority = table.Column<int>(type: "int", nullable: false),
                    Reported = table.Column<bool>(type: "bit", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    TrackingId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ReportDataId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reports", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Reports_ReportData_ReportDataId",
                        column: x => x.ReportDataId,
                        principalTable: "ReportData",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ProfileGroups",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AggregationCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Authority = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Category = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SmartContractId = table.Column<long>(type: "bigint", nullable: false),
                    Priority = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProfileGroups", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProfileGroups_SmartContract_SmartContractId",
                        column: x => x.SmartContractId,
                        principalTable: "SmartContract",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AccountProfileGroup",
                columns: table => new
                {
                    AccountId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProfileGroupId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Priority = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountProfileGroup", x => new { x.AccountId, x.ProfileGroupId });
                    table.ForeignKey(
                        name: "FK_AccountProfileGroup_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AccountProfileGroup_ProfileGroups_ProfileGroupId",
                        column: x => x.ProfileGroupId,
                        principalTable: "ProfileGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AccountProfileGroup_ProfileGroupId",
                table: "AccountProfileGroup",
                column: "ProfileGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_ProfileGroups_SmartContractId",
                table: "ProfileGroups",
                column: "SmartContractId");

            migrationBuilder.CreateIndex(
                name: "IX_Reports_ReportDataId",
                table: "Reports",
                column: "ReportDataId");

            migrationBuilder.CreateIndex(
                name: "IX_TransactionPools_Locked_Priority",
                table: "TransactionPools",
                columns: new[] { "Locked", "Priority" });

            migrationBuilder.CreateIndex(
                name: "IX_TransactionTriages_IsInPool_Completed",
                table: "TransactionTriages",
                columns: new[] { "IsInPool", "Completed" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
#pragma warning disable CA1062 // Validate arguments of public methods
            migrationBuilder.DropTable(
                name: "AccountProfileGroup");
#pragma warning restore CA1062 // Validate arguments of public methods

            migrationBuilder.DropTable(
                name: "Reports");

            migrationBuilder.DropTable(
                name: "ReportSettings");

            migrationBuilder.DropTable(
                name: "TransactionPendings");

            migrationBuilder.DropTable(
                name: "TransactionPools");

            migrationBuilder.DropTable(
                name: "TransactionRegistries");

            migrationBuilder.DropTable(
                name: "TransactionTriages");

            migrationBuilder.DropTable(
                name: "Accounts");

            migrationBuilder.DropTable(
                name: "ProfileGroups");

            migrationBuilder.DropTable(
                name: "ReportData");

            migrationBuilder.DropTable(
                name: "SmartContract");
        }
    }
}
