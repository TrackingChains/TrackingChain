using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrackingChain.Core.Migrations
{
    /// <inheritdoc />
    public partial class ErrorStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
#pragma warning disable CA1062 // Validate arguments of public methods
            migrationBuilder.DropColumn(
                name: "ReceiptSuccessful",
                table: "TransactionRegistries");
#pragma warning restore CA1062 // Validate arguments of public methods

            migrationBuilder.RenameColumn(
                name: "UnlockTimes",
                table: "TransactionPools",
                newName: "Status");

            migrationBuilder.RenameColumn(
                name: "UnlockTimes",
                table: "TransactionPendings",
                newName: "Status");

            migrationBuilder.AddColumn<int>(
                name: "ErrorTime",
                table: "TransactionRegistries",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "ReceiptReceived",
                table: "TransactionRegistries",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "TransactionRegistries",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TransactionErrorReason",
                table: "TransactionRegistries",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ErrorTimes",
                table: "TransactionPools",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ErrorTimes",
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
                name: "ErrorTime",
                table: "TransactionRegistries");
#pragma warning restore CA1062 // Validate arguments of public methods

            migrationBuilder.DropColumn(
                name: "ReceiptReceived",
                table: "TransactionRegistries");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "TransactionRegistries");

            migrationBuilder.DropColumn(
                name: "TransactionErrorReason",
                table: "TransactionRegistries");

            migrationBuilder.DropColumn(
                name: "ErrorTimes",
                table: "TransactionPools");

            migrationBuilder.DropColumn(
                name: "ErrorTimes",
                table: "TransactionPendings");

            migrationBuilder.RenameColumn(
                name: "Status",
                table: "TransactionPools",
                newName: "UnlockTimes");

            migrationBuilder.RenameColumn(
                name: "Status",
                table: "TransactionPendings",
                newName: "UnlockTimes");

            migrationBuilder.AddColumn<bool>(
                name: "ReceiptSuccessful",
                table: "TransactionRegistries",
                type: "bit",
                nullable: true);
        }
    }
}
