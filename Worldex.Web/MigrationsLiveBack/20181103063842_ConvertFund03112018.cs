using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CleanArchitecture.Web.Migrations
{
    public partial class ConvertFund03112018 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "MemberMobile",
                table: "TransactionQueue",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.CreateTable(
                name: "ConvertFundHistory",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<long>(nullable: false),
                    UpdatedBy = table.Column<long>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    Status = table.Column<short>(nullable: false),
                    FromWalletId = table.Column<long>(nullable: false),
                    ToWalletId = table.Column<long>(nullable: false),
                    DestinationPrice = table.Column<decimal>(type: "decimal(18, 8)", nullable: false),
                    SourcePrice = table.Column<decimal>(type: "decimal(18, 8)", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18, 8)", nullable: false),
                    TrnDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConvertFundHistory", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ConvertFundHistory");

            migrationBuilder.AlterColumn<string>(
                name: "MemberMobile",
                table: "TransactionQueue",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);
        }
    }
}
