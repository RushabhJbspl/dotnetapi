using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CleanArchitecture.Web.Migrations
{
    public partial class AddNewEntityBlockUnblockUserAddress : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FlushTrnHash",
                table: "DepositHistory",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "IsFlushAddProcess",
                table: "DepositHistory",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "FlushAddressEnable",
                table: "DepositCounterMaster",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "BlockUnblockUserAddress",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<long>(nullable: false),
                    UpdatedBy = table.Column<long>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    Status = table.Column<short>(nullable: false),
                    UserID = table.Column<long>(nullable: false),
                    WalletID = table.Column<long>(nullable: false),
                    Address = table.Column<string>(maxLength: 50, nullable: false),
                    WalletTypeID = table.Column<long>(nullable: false),
                    TrnHash = table.Column<string>(nullable: true),
                    Remarks = table.Column<string>(maxLength: 150, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BlockUnblockUserAddress", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BlockUnblockUserAddress");

            migrationBuilder.DropColumn(
                name: "FlushTrnHash",
                table: "DepositHistory");

            migrationBuilder.DropColumn(
                name: "IsFlushAddProcess",
                table: "DepositHistory");

            migrationBuilder.DropColumn(
                name: "FlushAddressEnable",
                table: "DepositCounterMaster");
        }
    }
}
