using Microsoft.EntityFrameworkCore.Migrations;

namespace CleanArchitecture.Web.Migrations
{
    public partial class policynameadd28112018 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_BlockWalletTrnTypeMaster",
                table: "BlockWalletTrnTypeMaster");

            migrationBuilder.AddColumn<string>(
                name: "PolicyName",
                table: "WalletUsagePolicy",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SiteURL",
                table: "AutorizedApps",
                nullable: true);

            migrationBuilder.AddUniqueConstraint(
                name: "AK_BlockWalletTrnTypeMaster_TrnTypeID_WalletTypeID",
                table: "BlockWalletTrnTypeMaster",
                columns: new[] { "TrnTypeID", "WalletTypeID" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_BlockWalletTrnTypeMaster",
                table: "BlockWalletTrnTypeMaster",
                columns: new[] { "WalletTypeID", "TrnTypeID" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropUniqueConstraint(
                name: "AK_BlockWalletTrnTypeMaster_TrnTypeID_WalletTypeID",
                table: "BlockWalletTrnTypeMaster");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BlockWalletTrnTypeMaster",
                table: "BlockWalletTrnTypeMaster");

            migrationBuilder.DropColumn(
                name: "PolicyName",
                table: "WalletUsagePolicy");

            migrationBuilder.DropColumn(
                name: "SiteURL",
                table: "AutorizedApps");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BlockWalletTrnTypeMaster",
                table: "BlockWalletTrnTypeMaster",
                column: "Id");
        }
    }
}
