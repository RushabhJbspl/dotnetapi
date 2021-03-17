using Microsoft.EntityFrameworkCore.Migrations;

namespace CleanArchitecture.Web.Migrations
{
    public partial class addKycBit : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_WalletTrnLimitConfiguration",
                table: "WalletTrnLimitConfiguration");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_TransactionPolicy_RoleId_TrnType",
                table: "TransactionPolicy");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TransactionPolicy",
                table: "TransactionPolicy");

            migrationBuilder.AddColumn<short>(
                name: "IsKYCEnable",
                table: "WalletTrnLimitConfiguration",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddColumn<short>(
                name: "IsKYCEnable",
                table: "TransactionPolicy",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddUniqueConstraint(
                name: "AK_WalletTrnLimitConfiguration_IsKYCEnable_TrnType_WalletType",
                table: "WalletTrnLimitConfiguration",
                columns: new[] { "IsKYCEnable", "TrnType", "WalletType" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_WalletTrnLimitConfiguration",
                table: "WalletTrnLimitConfiguration",
                columns: new[] { "TrnType", "WalletType", "IsKYCEnable" });

            migrationBuilder.AddUniqueConstraint(
                name: "AK_TransactionPolicy_IsKYCEnable_RoleId_TrnType",
                table: "TransactionPolicy",
                columns: new[] { "IsKYCEnable", "RoleId", "TrnType" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_TransactionPolicy",
                table: "TransactionPolicy",
                columns: new[] { "TrnType", "RoleId", "IsKYCEnable" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropUniqueConstraint(
                name: "AK_WalletTrnLimitConfiguration_IsKYCEnable_TrnType_WalletType",
                table: "WalletTrnLimitConfiguration");

            migrationBuilder.DropPrimaryKey(
                name: "PK_WalletTrnLimitConfiguration",
                table: "WalletTrnLimitConfiguration");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_TransactionPolicy_IsKYCEnable_RoleId_TrnType",
                table: "TransactionPolicy");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TransactionPolicy",
                table: "TransactionPolicy");

            migrationBuilder.DropColumn(
                name: "IsKYCEnable",
                table: "WalletTrnLimitConfiguration");

            migrationBuilder.DropColumn(
                name: "IsKYCEnable",
                table: "TransactionPolicy");

            migrationBuilder.AddPrimaryKey(
                name: "PK_WalletTrnLimitConfiguration",
                table: "WalletTrnLimitConfiguration",
                columns: new[] { "TrnType", "WalletType" });

            migrationBuilder.AddUniqueConstraint(
                name: "AK_TransactionPolicy_RoleId_TrnType",
                table: "TransactionPolicy",
                columns: new[] { "RoleId", "TrnType" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_TransactionPolicy",
                table: "TransactionPolicy",
                columns: new[] { "TrnType", "RoleId" });
        }
    }
}
