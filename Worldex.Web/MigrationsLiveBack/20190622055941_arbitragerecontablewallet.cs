using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CleanArchitecture.Web.Migrations
{
    public partial class arbitragerecontablewallet : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "Confirmations",
                table: "ArbitrageDepositFund",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<decimal>(
                name: "Value",
                table: "ArbitrageDepositFund",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateTable(
                name: "LPArbitrageWalletMismatch",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<long>(nullable: false),
                    UpdatedBy = table.Column<long>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    Status = table.Column<short>(nullable: false),
                    WalletID = table.Column<long>(nullable: false),
                    TPBalance = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    SystemBalance = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    MismatchaingAmount = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    SettledAmount = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    ProviderBalance = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    ResolvedBy = table.Column<long>(nullable: false),
                    ResolvedDate = table.Column<DateTime>(nullable: false),
                    ResolvedRemarks = table.Column<string>(maxLength: 150, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LPArbitrageWalletMismatch", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LPArbitrageWalletMismatch");

            migrationBuilder.DropColumn(
                name: "Confirmations",
                table: "ArbitrageDepositFund");

            migrationBuilder.DropColumn(
                name: "Value",
                table: "ArbitrageDepositFund");
        }
    }
}
