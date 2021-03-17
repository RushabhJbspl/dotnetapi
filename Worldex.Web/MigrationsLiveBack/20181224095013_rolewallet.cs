using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CleanArchitecture.Web.Migrations
{
    public partial class rolewallet : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AddUserWalletRequest");

            migrationBuilder.DropColumn(
                name: "Remark",
                table: "PersonalVerification");

            migrationBuilder.AlterColumn<bool>(
                name: "VerifyStatus",
                table: "PersonalVerification",
                nullable: false,
                oldClrType: typeof(int));

            migrationBuilder.CreateTable(
                name: "AddRemoveUserWalletRequest",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<long>(nullable: false),
                    UpdatedBy = table.Column<long>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    Status = table.Column<short>(nullable: false),
                    ToUserId = table.Column<long>(nullable: false),
                    WalletOwnerUserID = table.Column<long>(nullable: false),
                    WalletID = table.Column<long>(nullable: false),
                    RoleId = table.Column<long>(nullable: false),
                    Message = table.Column<string>(nullable: true),
                    Email = table.Column<string>(maxLength: 100, nullable: false),
                    RecieverApproveDate = table.Column<DateTime>(nullable: true),
                    RecieverApproveBy = table.Column<long>(nullable: false),
                    FromUserId = table.Column<long>(nullable: false),
                    OwnerApprovalStatus = table.Column<short>(nullable: false),
                    OwnerApprovalDate = table.Column<DateTime>(nullable: true),
                    OwnerApprovalBy = table.Column<long>(nullable: true),
                    Type = table.Column<short>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AddRemoveUserWalletRequest", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AddRemoveUserWalletRequest");

            migrationBuilder.AlterColumn<int>(
                name: "VerifyStatus",
                table: "PersonalVerification",
                nullable: false,
                oldClrType: typeof(bool));

            migrationBuilder.AddColumn<string>(
                name: "Remark",
                table: "PersonalVerification",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "AddUserWalletRequest",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ApproveBy = table.Column<long>(nullable: false),
                    ApproveDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<long>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    Email = table.Column<string>(maxLength: 100, nullable: false),
                    Message = table.Column<string>(maxLength: 1000, nullable: false),
                    RoleId = table.Column<long>(nullable: false),
                    Status = table.Column<short>(nullable: false),
                    ToUserId = table.Column<long>(nullable: false),
                    UpdatedBy = table.Column<long>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    WalletID = table.Column<long>(nullable: false),
                    WalletOwnerUserID = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AddUserWalletRequest", x => x.Id);
                });
        }
    }
}
