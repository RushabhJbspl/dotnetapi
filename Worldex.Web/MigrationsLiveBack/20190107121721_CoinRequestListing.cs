using Microsoft.EntityFrameworkCore.Migrations;

namespace CleanArchitecture.Web.Migrations
{
    public partial class CoinRequestListing : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "TotalSupply",
                table: "CoinListRequest",
                type: "decimal(18, 8)",
                nullable: false,
                oldClrType: typeof(long));

            migrationBuilder.AlterColumn<decimal>(
                name: "MaxSupply",
                table: "CoinListRequest",
                type: "decimal(18, 8)",
                nullable: false,
                oldClrType: typeof(long));

            migrationBuilder.AlterColumn<string>(
                name: "Explorer",
                table: "CoinListRequest",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "CirculatingSupply",
                table: "CoinListRequest",
                type: "decimal(18, 8)",
                nullable: false,
                oldClrType: typeof(long));

            migrationBuilder.AddColumn<string>(
                name: "APIDocumentPath",
                table: "CoinListRequest",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "CoinListRequest",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "AddressLine2",
                table: "CoinListRequest",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "City",
                table: "CoinListRequest",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CoinTokenAddress",
                table: "CoinListRequest",
                nullable: true);

            migrationBuilder.AddColumn<short>(
                name: "CoinType",
                table: "CoinListRequest",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddColumn<string>(
                name: "CurListOnOtherExng",
                table: "CoinListRequest",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "DecimalPlace",
                table: "CoinListRequest",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "CoinListRequest",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                table: "CoinListRequest",
                maxLength: 250,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "GithubLink",
                table: "CoinListRequest",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "HowFundsWereRaised",
                table: "CoinListRequest",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastName",
                table: "CoinListRequest",
                maxLength: 250,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Phone",
                table: "CoinListRequest",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Premine",
                table: "CoinListRequest",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ProjectName",
                table: "CoinListRequest",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ProjectWebsiteLink",
                table: "CoinListRequest",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "State",
                table: "CoinListRequest",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StreetAddress",
                table: "CoinListRequest",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "TrnFee",
                table: "CoinListRequest",
                type: "decimal(18, 8)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "WebsiteFAQ",
                table: "CoinListRequest",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "WhitePaper",
                table: "CoinListRequest",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ZipCode",
                table: "CoinListRequest",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "APIDocumentPath",
                table: "CoinListRequest");

            migrationBuilder.DropColumn(
                name: "Address",
                table: "CoinListRequest");

            migrationBuilder.DropColumn(
                name: "AddressLine2",
                table: "CoinListRequest");

            migrationBuilder.DropColumn(
                name: "City",
                table: "CoinListRequest");

            migrationBuilder.DropColumn(
                name: "CoinTokenAddress",
                table: "CoinListRequest");

            migrationBuilder.DropColumn(
                name: "CoinType",
                table: "CoinListRequest");

            migrationBuilder.DropColumn(
                name: "CurListOnOtherExng",
                table: "CoinListRequest");

            migrationBuilder.DropColumn(
                name: "DecimalPlace",
                table: "CoinListRequest");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "CoinListRequest");

            migrationBuilder.DropColumn(
                name: "FirstName",
                table: "CoinListRequest");

            migrationBuilder.DropColumn(
                name: "GithubLink",
                table: "CoinListRequest");

            migrationBuilder.DropColumn(
                name: "HowFundsWereRaised",
                table: "CoinListRequest");

            migrationBuilder.DropColumn(
                name: "LastName",
                table: "CoinListRequest");

            migrationBuilder.DropColumn(
                name: "Phone",
                table: "CoinListRequest");

            migrationBuilder.DropColumn(
                name: "Premine",
                table: "CoinListRequest");

            migrationBuilder.DropColumn(
                name: "ProjectName",
                table: "CoinListRequest");

            migrationBuilder.DropColumn(
                name: "ProjectWebsiteLink",
                table: "CoinListRequest");

            migrationBuilder.DropColumn(
                name: "State",
                table: "CoinListRequest");

            migrationBuilder.DropColumn(
                name: "StreetAddress",
                table: "CoinListRequest");

            migrationBuilder.DropColumn(
                name: "TrnFee",
                table: "CoinListRequest");

            migrationBuilder.DropColumn(
                name: "WebsiteFAQ",
                table: "CoinListRequest");

            migrationBuilder.DropColumn(
                name: "WhitePaper",
                table: "CoinListRequest");

            migrationBuilder.DropColumn(
                name: "ZipCode",
                table: "CoinListRequest");

            migrationBuilder.AlterColumn<long>(
                name: "TotalSupply",
                table: "CoinListRequest",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18, 8)");

            migrationBuilder.AlterColumn<long>(
                name: "MaxSupply",
                table: "CoinListRequest",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18, 8)");

            migrationBuilder.AlterColumn<string>(
                name: "Explorer",
                table: "CoinListRequest",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<long>(
                name: "CirculatingSupply",
                table: "CoinListRequest",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18, 8)");
        }
    }
}
