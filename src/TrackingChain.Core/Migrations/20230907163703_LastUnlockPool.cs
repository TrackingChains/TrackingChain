using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrackingChain.Core.Migrations
{
    /// <inheritdoc />
    public partial class LastUnlockPool : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
#pragma warning disable CA1062 // Validate arguments of public methods
            migrationBuilder.AddColumn<int>(
                name: "LastUnlockedError",
                table: "TransactionPools",
                type: "int",
                nullable: true);
#pragma warning restore CA1062 // Validate arguments of public methods
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
#pragma warning disable CA1062 // Validate arguments of public methods
            migrationBuilder.DropColumn(
                name: "LastUnlockedError",
                table: "TransactionPools");
#pragma warning restore CA1062 // Validate arguments of public methods
        }
    }
}
