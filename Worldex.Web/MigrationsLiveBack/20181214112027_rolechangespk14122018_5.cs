using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CleanArchitecture.Web.Migrations
{
    public partial class rolechangespk14122018_5 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_WalletAuthorizeUserMaster",
                table: "WalletAuthorizeUserMaster");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TransactionPolicy",
                table: "TransactionPolicy");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OrganizationUserMaster",
                table: "OrganizationUserMaster");

            migrationBuilder.AddColumn<DateTime>(
                name: "ExpiryDate",
                table: "WalletMasters",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "OrgID",
                table: "WalletMasters",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "RoleId",
                table: "TransactionPolicy",
                nullable: false,
                defaultValue: 0L);

            //migrationBuilder.AlterColumn<long>(
            //    name: "ChargeTypeId",
            //    table: "ChargeTypeMaster",
            //    nullable: false,
            //    oldClrType: typeof(long))
            //    .OldAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AddPrimaryKey(
                name: "PK_WalletAuthorizeUserMaster",
                table: "WalletAuthorizeUserMaster",
                columns: new[] { "UserID", "WalletID", "RoleID" });

            migrationBuilder.AddUniqueConstraint(
                name: "AK_WalletAuthorizeUserMaster_Id_RoleID_UserID_WalletID",
                table: "WalletAuthorizeUserMaster",
                columns: new[] { "Id", "RoleID", "UserID", "WalletID" });

            migrationBuilder.AddUniqueConstraint(
                name: "AK_TransactionPolicy_RoleId_TrnType",
                table: "TransactionPolicy",
                columns: new[] { "RoleId", "TrnType" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_TransactionPolicy",
                table: "TransactionPolicy",
                columns: new[] { "TrnType", "RoleId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_OrganizationUserMaster",
                table: "OrganizationUserMaster",
                columns: new[] { "RoleID", "UserID" });

            migrationBuilder.AddUniqueConstraint(
                name: "AK_OrganizationUserMaster_Id_RoleID_UserID",
                table: "OrganizationUserMaster",
                columns: new[] { "Id", "RoleID", "UserID" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_WalletAuthorizeUserMaster",
                table: "WalletAuthorizeUserMaster");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_WalletAuthorizeUserMaster_Id_RoleID_UserID_WalletID",
                table: "WalletAuthorizeUserMaster");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_TransactionPolicy_RoleId_TrnType",
                table: "TransactionPolicy");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TransactionPolicy",
                table: "TransactionPolicy");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OrganizationUserMaster",
                table: "OrganizationUserMaster");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_OrganizationUserMaster_Id_RoleID_UserID",
                table: "OrganizationUserMaster");

            migrationBuilder.DropColumn(
                name: "ExpiryDate",
                table: "WalletMasters");

            migrationBuilder.DropColumn(
                name: "OrgID",
                table: "WalletMasters");

            migrationBuilder.DropColumn(
                name: "RoleId",
                table: "TransactionPolicy");

            //migrationBuilder.AlterColumn<long>(
            //    name: "ChargeTypeId",
            //    table: "ChargeTypeMaster",
            //    nullable: false,
            //    oldClrType: typeof(long))
            //    .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AddPrimaryKey(
                name: "PK_WalletAuthorizeUserMaster",
                table: "WalletAuthorizeUserMaster",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TransactionPolicy",
                table: "TransactionPolicy",
                column: "TrnType");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OrganizationUserMaster",
                table: "OrganizationUserMaster",
                column: "Id");
        }
    }
}
