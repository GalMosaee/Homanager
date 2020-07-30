using Microsoft.EntityFrameworkCore.Migrations;

namespace Homanger.Migrations
{
    public partial class SupermarketsMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Product_ProductCategory_CategoryProductCategoryId",
                table: "Product");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "AspNetUsers");

            migrationBuilder.AlterColumn<int>(
                name: "CategoryProductCategoryId",
                table: "Product",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.CreateTable(
                name: "Supermarket",
                columns: table => new
                {
                    SupermarketId = table.Column<string>(nullable: false),
                    Title = table.Column<string>(nullable: false),
                    Longitude = table.Column<double>(nullable: false),
                    Latitude = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Supermarket", x => x.SupermarketId);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_Product_ProductCategory_CategoryProductCategoryId",
                table: "Product",
                column: "CategoryProductCategoryId",
                principalTable: "ProductCategory",
                principalColumn: "ProductCategoryId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Product_ProductCategory_CategoryProductCategoryId",
                table: "Product");

            migrationBuilder.DropTable(
                name: "Supermarket");

            migrationBuilder.AlterColumn<int>(
                name: "CategoryProductCategoryId",
                table: "Product",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "AspNetUsers",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Product_ProductCategory_CategoryProductCategoryId",
                table: "Product",
                column: "CategoryProductCategoryId",
                principalTable: "ProductCategory",
                principalColumn: "ProductCategoryId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
