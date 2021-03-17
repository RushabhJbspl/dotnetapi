using Microsoft.EntityFrameworkCore.Migrations;

namespace CleanArchitecture.Web.Migrations
{
    public partial class WalletMigration24102018_2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_StckingScheme",
                table: "StckingScheme");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MemberShadowLimit",
                table: "MemberShadowLimit");

            migrationBuilder.AddColumn<long>(
                name: "WalletId",
                table: "UserStacking",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddPrimaryKey(
                name: "PK_StckingScheme",
                table: "StckingScheme",
                column: "WalletType");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MemberShadowLimit",
                table: "MemberShadowLimit",
                column: "MemberTypeId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_StckingScheme",
                table: "StckingScheme");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MemberShadowLimit",
                table: "MemberShadowLimit");

            migrationBuilder.DropColumn(
                name: "WalletId",
                table: "UserStacking");

            migrationBuilder.AddPrimaryKey(
                name: "PK_StckingScheme",
                table: "StckingScheme",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MemberShadowLimit",
                table: "MemberShadowLimit",
                column: "Id");
        }
    }
}
