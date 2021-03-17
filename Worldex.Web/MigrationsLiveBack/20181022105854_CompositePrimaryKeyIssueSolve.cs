using Microsoft.EntityFrameworkCore.Migrations;

namespace CleanArchitecture.Web.Migrations
{
    public partial class CompositePrimaryKeyIssueSolve : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_DepositCounterMaster",
                table: "DepositCounterMaster");

            migrationBuilder.AddUniqueConstraint(
                name: "AK_DepositCounterMaster_SerProId_WalletTypeID",
                table: "DepositCounterMaster",
                columns: new[] { "SerProId", "WalletTypeID" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_DepositCounterMaster",
                table: "DepositCounterMaster",
                columns: new[] { "WalletTypeID", "SerProId" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropUniqueConstraint(
                name: "AK_DepositCounterMaster_SerProId_WalletTypeID",
                table: "DepositCounterMaster");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DepositCounterMaster",
                table: "DepositCounterMaster");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DepositCounterMaster",
                table: "DepositCounterMaster",
                column: "Id");
        }
    }
}
