using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrackingChain.Core.Migrations
{
    /// <inheritdoc />
    public partial class ReportSetting : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
#pragma warning disable CA1062 // Validate arguments of public methods
            migrationBuilder.RenameColumn(
                name: "TransactionId",
                table: "Reports",
                newName: "TrackingId");
#pragma warning restore CA1062 // Validate arguments of public methods

            migrationBuilder.AddColumn<Guid>(
                name: "ReportDataId",
                table: "Reports",
                type: "uniqueidentifier",
                nullable: true);

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

            migrationBuilder.CreateIndex(
                name: "IX_Reports_ReportDataId",
                table: "Reports",
                column: "ReportDataId");

            migrationBuilder.AddForeignKey(
                name: "FK_Reports_ReportData_ReportDataId",
                table: "Reports",
                column: "ReportDataId",
                principalTable: "ReportData",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
#pragma warning disable CA1062 // Validate arguments of public methods
            migrationBuilder.DropForeignKey(
                name: "FK_Reports_ReportData_ReportDataId",
                table: "Reports");
#pragma warning restore CA1062 // Validate arguments of public methods

            migrationBuilder.DropTable(
                name: "ReportData");

            migrationBuilder.DropTable(
                name: "ReportSettings");

            migrationBuilder.DropIndex(
                name: "IX_Reports_ReportDataId",
                table: "Reports");

            migrationBuilder.DropColumn(
                name: "ReportDataId",
                table: "Reports");

            migrationBuilder.RenameColumn(
                name: "TrackingId",
                table: "Reports",
                newName: "TransactionId");
        }
    }
}
