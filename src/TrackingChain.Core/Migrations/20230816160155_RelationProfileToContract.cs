using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrackingChain.Core.Migrations
{
    /// <inheritdoc />
    public partial class RelationProfileToContract : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
#pragma warning disable CA1062 // Validate arguments of public methods
            migrationBuilder.DropIndex(
                name: "IX_ProfileGroups_SmartContractId",
                table: "ProfileGroups");
#pragma warning restore CA1062 // Validate arguments of public methods

            migrationBuilder.CreateIndex(
                name: "IX_ProfileGroups_SmartContractId",
                table: "ProfileGroups",
                column: "SmartContractId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
#pragma warning disable CA1062 // Validate arguments of public methods
            migrationBuilder.DropIndex(
                name: "IX_ProfileGroups_SmartContractId",
                table: "ProfileGroups");
#pragma warning restore CA1062 // Validate arguments of public methods

            migrationBuilder.CreateIndex(
                name: "IX_ProfileGroups_SmartContractId",
                table: "ProfileGroups",
                column: "SmartContractId",
                unique: true);
        }
    }
}
