using Microsoft.EntityFrameworkCore.Migrations;

namespace CleanArchitecture.Web.Migrations
{
    public partial class Affiliate_User_Promotion : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "PromotionTypeId",
                table: "AffiliateUserMaster",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<string>(
                name: "ReferCode",
                table: "AffiliateUserMaster",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DecryptedCode",
                table: "AffiliatePromotionUserTypeMapping",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PromotionLink",
                table: "AffiliatePromotionUserTypeMapping",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PromotionTypeId",
                table: "AffiliateUserMaster");

            migrationBuilder.DropColumn(
                name: "ReferCode",
                table: "AffiliateUserMaster");

            migrationBuilder.DropColumn(
                name: "DecryptedCode",
                table: "AffiliatePromotionUserTypeMapping");

            migrationBuilder.DropColumn(
                name: "PromotionLink",
                table: "AffiliatePromotionUserTypeMapping");
        }
    }
}
