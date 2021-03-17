using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Worldex.Web.Migrations
{
    public partial class addIEOWalletAdminDepositTbl : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "IEOWalletAdminDeposit",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<long>(nullable: false),
                    UpdatedBy = table.Column<long>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    Status = table.Column<short>(nullable: false),
                    GUID = table.Column<string>(nullable: false),
                    WalletId = table.Column<long>(nullable: false),
                    CurrencyName = table.Column<string>(maxLength: 7, nullable: false),
                    UserId = table.Column<long>(nullable: false),
                    Remarks = table.Column<string>(maxLength: 500, nullable: false),
                    SystemRemarks = table.Column<string>(maxLength: 500, nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    ApprovedBy = table.Column<long>(nullable: true),
                    ApprovedDate = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IEOWalletAdminDeposit", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_IEOWalletAdminDeposit_GUID",
                table: "IEOWalletAdminDeposit",
                column: "GUID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IEOWalletAdminDeposit");
        }
    }
}
