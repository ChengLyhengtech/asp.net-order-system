using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace aps.net_order_system.Migrations
{
    /// <inheritdoc />
    public partial class UpdateProductDtoWithId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ProductImg",
                table: "Products",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProductImg",
                table: "Products");
        }
    }
}
