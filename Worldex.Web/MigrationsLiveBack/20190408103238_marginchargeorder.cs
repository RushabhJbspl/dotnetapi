using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CleanArchitecture.Web.Migrations
{
    public partial class marginchargeorder : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MarginChargeOrder",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<long>(nullable: false),
                    UpdatedBy = table.Column<long>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    Status = table.Column<short>(nullable: false),
                    BatchNo = table.Column<long>(nullable: false),
                    LoanID = table.Column<long>(nullable: false),
                    UserID = table.Column<long>(nullable: false),
                    TrnDate = table.Column<DateTime>(nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    PairID = table.Column<long>(nullable: false),
                    MarginChargeCase = table.Column<int>(nullable: false),
                    TrnRefNo = table.Column<long>(nullable: false),
                    Guid = table.Column<string>(nullable: true),
                    DebitAccountID = table.Column<string>(nullable: true),
                    CreditAccountID = table.Column<string>(nullable: true),
                    SMSCode = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MarginChargeOrder", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MarginChargeOrder");
        }
    }
}
