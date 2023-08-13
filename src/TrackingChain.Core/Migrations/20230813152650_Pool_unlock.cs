using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrackingChain.Core.Migrations
{
    /// <inheritdoc />
    public partial class Pool_unlock : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
#pragma warning disable CA1062 // Validate arguments of public methods
            migrationBuilder.AddColumn<DateTime>(
                name: "GeneratingFrom",
                table: "TransactionPools",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
#pragma warning restore CA1062 // Validate arguments of public methods

            migrationBuilder.AddColumn<int>(
                name: "UnlockTimes",
                table: "TransactionPools",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "UnlockTimes",
                table: "TransactionPendings",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
#pragma warning disable CA1062 // Validate arguments of public methods
            migrationBuilder.DropColumn(
                name: "GeneratingFrom",
                table: "TransactionPools");
#pragma warning restore CA1062 // Validate arguments of public methods

            migrationBuilder.DropColumn(
                name: "UnlockTimes",
                table: "TransactionPools");

            migrationBuilder.DropColumn(
                name: "UnlockTimes",
                table: "TransactionPendings");
        }
    }
}
