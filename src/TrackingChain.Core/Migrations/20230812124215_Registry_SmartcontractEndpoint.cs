using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrackingChain.Core.Migrations
{
    /// <inheritdoc />
    public partial class Registry_SmartcontractEndpoint : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
#pragma warning disable CA1062 // Validate arguments of public methods
            migrationBuilder.AddColumn<string>(
                name: "SmartContractEndpoint",
                table: "TransactionRegistries",
                type: "nvarchar(max)",
                nullable: true);
#pragma warning restore CA1062 // Validate arguments of public methods
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
#pragma warning disable CA1062 // Validate arguments of public methods
            migrationBuilder.DropColumn(
                name: "SmartContractEndpoint",
                table: "TransactionRegistries");
#pragma warning restore CA1062 // Validate arguments of public methods
        }
    }
}
