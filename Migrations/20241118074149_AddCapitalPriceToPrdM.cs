using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Shopping_Online.Migrations
{
    /// <inheritdoc />
    public partial class AddCapitalPriceToPrdM : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CapitalPrice",
                table: "Products",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CapitalPrice",
                table: "Products");
        }
    }
}
