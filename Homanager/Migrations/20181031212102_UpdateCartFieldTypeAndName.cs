using Microsoft.EntityFrameworkCore.Migrations;

namespace Homanger.Migrations
{
    public partial class UpdateCartFieldTypeAndName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cart_Group_OwnerGroupId",
                table: "Cart");

            migrationBuilder.DropColumn(
                name: "OwnerId",
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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cart_Group_OwnerGroupId",
                table: "Cart");

            migrationBuilder.AlterColumn<string>(
                name: "OwnerGroupId",
                table: "Cart",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AddColumn<int>(
                name: "OwnerId",
                table: "Cart",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddForeignKey(
                name: "FK_Cart_Group_OwnerGroupId",
                table: "Cart",
                column: "OwnerGroupId",
                principalTable: "Group",
                principalColumn: "GroupId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
