using Microsoft.EntityFrameworkCore.Migrations;

namespace CleanArchitecture.Web.Migrations
{
    public partial class bitgodelay01112018 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_UserPreferencesMaster",
                table: "UserPreferencesMaster");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TradeBitGoDelayAddressess",
                table: "TradeBitGoDelayAddressess");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserPreferencesMaster",
                table: "UserPreferencesMaster",
                column: "UserID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TradeBitGoDelayAddressess",
                table: "TradeBitGoDelayAddressess",
                column: "TrnID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_UserPreferencesMaster",
                table: "UserPreferencesMaster");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TradeBitGoDelayAddressess",
                table: "TradeBitGoDelayAddressess");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserPreferencesMaster",
                table: "UserPreferencesMaster",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TradeBitGoDelayAddressess",
                table: "TradeBitGoDelayAddressess",
                column: "Id");
        }
    }
}
