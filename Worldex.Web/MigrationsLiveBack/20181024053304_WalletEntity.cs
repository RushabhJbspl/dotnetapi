using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CleanArchitecture.Web.Migrations
{
    public partial class WalletEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropPrimaryKey(
            //    name: "PK_WalletMasters",
            //    table: "WalletMasters");

            migrationBuilder.RenameColumn(
                name: "userId",
                table: "DepositHistory",
                newName: "UserId");

            migrationBuilder.AddColumn<decimal>(
                name: "LifeTime",
                table: "WalletLimitConfiguration",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "EpochTimePure",
                table: "DepositHistory",
                nullable: true,
                oldClrType: typeof(string));

            //migrationBuilder.AddPrimaryKey(
            //    name: "PK_WalletMasters1",
            //    table: "WalletMasters",
            //    column: "AccWalletID");

            migrationBuilder.CreateTable(
                name: "BizUserTypeMapping",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    UserID = table.Column<long>(nullable: false),
                    UserType = table.Column<short>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BizUserTypeMapping", x => x.UserID);
                });

            migrationBuilder.CreateTable(
                name: "MemberShadowBalance",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<long>(nullable: false),
                    UpdatedBy = table.Column<long>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: false),
                    Status = table.Column<short>(nullable: false),
                    WalletID = table.Column<long>(nullable: false),
                    ShadowAmount = table.Column<decimal>(type: "decimal(18, 8)", nullable: false),
                    Remarks = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MemberShadowBalance", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MemberShadowLimit",
                columns: table => new
                {
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<long>(nullable: false),
                    UpdatedBy = table.Column<long>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: false),
                    Status = table.Column<short>(nullable: false),
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    MemberTypeId = table.Column<long>(nullable: false),
                    ShadowLimitAmount = table.Column<decimal>(type: "decimal(18, 8)", nullable: false),
                    Type = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MemberShadowLimit", x => x.MemberTypeId);
                });

            migrationBuilder.CreateTable(
                name: "StckingScheme",
                columns: table => new
                {
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<long>(nullable: false),
                    UpdatedBy = table.Column<long>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: false),
                    Status = table.Column<short>(nullable: false),
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    MemberTypeId = table.Column<long>(nullable: false),
                    LimitAmount = table.Column<decimal>(type: "decimal(18, 8)", nullable: false),
                    Type = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StckingScheme", x => x.MemberTypeId);
                });

            migrationBuilder.CreateTable(
                name: "UserStacking",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<long>(nullable: false),
                    UpdatedBy = table.Column<long>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: false),
                    Status = table.Column<short>(nullable: false),
                    UserId = table.Column<long>(nullable: false),
                    StackingAmount = table.Column<decimal>(type: "decimal(18, 8)", nullable: false),
                    Type = table.Column<string>(maxLength: 50, nullable: false),
                    Remarks = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserStacking", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BizUserTypeMapping");

            migrationBuilder.DropTable(
                name: "MemberShadowBalance");

            migrationBuilder.DropTable(
                name: "MemberShadowLimit");

            migrationBuilder.DropTable(
                name: "StckingScheme");

            migrationBuilder.DropTable(
                name: "UserStacking");

            //migrationBuilder.DropPrimaryKey(
            //    name: "PK_WalletMasters",
            //    table: "WalletMasters");

            migrationBuilder.DropColumn(
                name: "LifeTime",
                table: "WalletLimitConfiguration");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "DepositHistory",
                newName: "userId");

            migrationBuilder.AlterColumn<string>(
                name: "EpochTimePure",
                table: "DepositHistory",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            //migrationBuilder.AddPrimaryKey(
            //    name: "PK_WalletMasters1",
            //    table: "WalletMasters",
            //    column: "Id");
        }
    }
}
