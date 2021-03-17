using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CleanArchitecture.Web.Migrations
{
    public partial class margintradingrequesttbl : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<short>(
                name: "IsAutoApprove",
                table: "LeverageMaster",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddColumn<decimal>(
                name: "SafetyMarginPer",
                table: "LeverageMaster",
                type: "decimal(28, 18)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateTable(
                name: "MarginWalletTopupRequest",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<long>(nullable: false),
                    UpdatedBy = table.Column<long>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    WalletTypeID = table.Column<long>(nullable: false),
                    FromWalletID = table.Column<long>(nullable: false),
                    ToWalletID = table.Column<long>(nullable: false),
                    UserID = table.Column<long>(nullable: false),
                    LeverageID = table.Column<long>(nullable: false),
                    IsAutoApprove = table.Column<short>(nullable: false),
                    RequestRemarks = table.Column<string>(maxLength: 500, nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    TrnDate = table.Column<DateTime>(nullable: false),
                    LeverageAmount = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    ChargeAmount = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    SafetyMarginAmount = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    CreditAmount = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    ApprovedBy = table.Column<long>(nullable: false),
                    ApprovedDate = table.Column<DateTime>(nullable: false),
                    ApprovedRemarks = table.Column<string>(maxLength: 500, nullable: true),
                    SystemRemarks = table.Column<string>(maxLength: 500, nullable: true),
                    Status = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MarginWalletTopupRequest", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MarginWalletTopupRequest");

            migrationBuilder.DropColumn(
                name: "IsAutoApprove",
                table: "LeverageMaster");

            migrationBuilder.DropColumn(
                name: "SafetyMarginPer",
                table: "LeverageMaster");
        }
    }
}
