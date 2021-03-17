using Microsoft.EntityFrameworkCore.Migrations;

namespace CleanArchitecture.Web.Migrations
{
    public partial class addkeyinmappingtbl : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_AffiliateSchemeTypeMapping",
                table: "AffiliateSchemeTypeMapping");

            //migrationBuilder.DropColumn(
            //    name: "DegradeStakingAmount",
            //    table: "TokenUnStakingHistory");

            migrationBuilder.DropColumn(
                name: "IsFullAccess",
                table: "RestMethods");

            migrationBuilder.DropColumn(
                name: "IsReadOnly",
                table: "RestMethods");

            //migrationBuilder.DropColumn(
            //    name: "MethodType",
            //    table: "RestMethods");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AffiliateSchemeTypeMapping",
                table: "AffiliateSchemeTypeMapping",
                columns: new[] { "SchemeMstId", "SchemeTypeMstId" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_AffiliateSchemeTypeMapping",
                table: "AffiliateSchemeTypeMapping");

            //migrationBuilder.AddColumn<decimal>(
            //    name: "DegradeStakingAmount",
            //    table: "TokenUnStakingHistory",
            //    type: "decimal(28, 18)",
            //    nullable: false,
            //    defaultValue: 0m);

            migrationBuilder.AddColumn<short>(
                name: "IsFullAccess",
                table: "RestMethods",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddColumn<short>(
                name: "IsReadOnly",
                table: "RestMethods",
                nullable: false,
                defaultValue: (short)0);

            //migrationBuilder.AddColumn<short>(
            //    name: "MethodType",
            //    table: "RestMethods",
            //    nullable: false,
            //    defaultValue: (short)0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_AffiliateSchemeTypeMapping",
                table: "AffiliateSchemeTypeMapping",
                column: "Id");
        }
    }
}
