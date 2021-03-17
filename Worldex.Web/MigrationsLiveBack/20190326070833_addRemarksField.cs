using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CleanArchitecture.Web.Migrations
{
    public partial class addRemarksField : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Remarks",
                table: "TokenStakingHistory",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "TransactionAmount",
                table: "ReferralRewards",
                type: "decimal(28, 18)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<DateTime>(
                name: "TrnDate",
                table: "ReferralRewards",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Remarks",
                table: "TokenStakingHistory");

            migrationBuilder.DropColumn(
                name: "TransactionAmount",
                table: "ReferralRewards");

            migrationBuilder.DropColumn(
                name: "TrnDate",
                table: "ReferralRewards");
        }
    }
}
