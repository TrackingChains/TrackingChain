using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrackingChain.Core.Migrations
{
    /// <inheritdoc />
    public partial class AccountProfileGroup_Name : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
#pragma warning disable CA1062 // Validate arguments of public methods
            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "AccountProfileGroup",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
#pragma warning restore CA1062 // Validate arguments of public methods
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
#pragma warning disable CA1062 // Validate arguments of public methods
            migrationBuilder.DropColumn(
                name: "Name",
                table: "AccountProfileGroup");
#pragma warning restore CA1062 // Validate arguments of public methods
        }
    }
}
