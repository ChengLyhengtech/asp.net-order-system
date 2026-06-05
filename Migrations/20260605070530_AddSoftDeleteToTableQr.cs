using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace aps.net_order_system.Migrations
{
    /// <inheritdoc />
    public partial class AddSoftDeleteToTableQr : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "TableId",
                table: "Orders",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateTable(
                name: "TableQrCodes",
                columns: table => new
                {
                    TableId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    EncryptedUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    QrCodeImageBase64 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TableQrCodes", x => x.TableId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Orders_TableId",
                table: "Orders",
                column: "TableId");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_TableQrCodes_TableId",
                table: "Orders",
                column: "TableId",
                principalTable: "TableQrCodes",
                principalColumn: "TableId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_TableQrCodes_TableId",
                table: "Orders");

            migrationBuilder.DropTable(
                name: "TableQrCodes");

            migrationBuilder.DropIndex(
                name: "IX_Orders_TableId",
                table: "Orders");

            migrationBuilder.AlterColumn<int>(
                name: "TableId",
                table: "Orders",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);
        }
    }
}
