using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CleanArchitecture.Web.Migrations
{
    public partial class closepositionwallet : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DepositionRequired",
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
                    SMSCode = table.Column<string>(nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    LoanID = table.Column<long>(nullable: false),
                    WalletID = table.Column<long>(nullable: false),
                    TrnType = table.Column<int>(nullable: false),
                    TrnRefNo = table.Column<string>(nullable: true),
                    ReceivedDate = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DepositionRequired", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MarginCloseUserPositionWallet",
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
                    WalletTypeID = table.Column<long>(nullable: false),
                    SMSCode = table.Column<string>(nullable: true),
                    TrnRefNo = table.Column<string>(nullable: true),
                    LoanID = table.Column<long>(nullable: false),
                    ErrorCode = table.Column<long>(nullable: false),
                    ErrorMessage = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MarginCloseUserPositionWallet", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DepositionRequired");

            migrationBuilder.DropTable(
                name: "MarginCloseUserPositionWallet");
        }
    }
}
