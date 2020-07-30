using Microsoft.EntityFrameworkCore.Migrations;

namespace Homanger.Migrations
{
    public partial class RemoveCartsGroupRequired : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cart_Group_OwnerGroupId",
                table: "Cart");

            migrationBuilder.AlterColumn<string>(
                name: "OwnerGroupId",
                table: "Cart",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AddForeignKey(
                name: "FK_Cart_Group_OwnerGroupId",
                table: "Cart",
                column: "OwnerGroupId",
                principalTable: "Group",
                principalColumn: "GroupId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cart_Group_OwnerGroupId",
                table: "Cart");

            migrationBuilder.AlterColumn<string>(
                name: "OwnerGroupId",
                table: "Cart",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Cart_Group_OwnerGroupId",
                table: "Cart",
                column: "OwnerGroupId",
                principalTable: "Group",
                principalColumn: "GroupId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
