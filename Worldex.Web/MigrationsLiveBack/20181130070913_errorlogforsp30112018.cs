using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CleanArchitecture.Web.Migrations
{
    public partial class errorlogforsp30112018 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "InBoundBalance",
                table: "WalletMasters",
                type: "decimal(18, 8)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "OutBoundBalance",
                table: "WalletMasters",
                type: "decimal(18, 8)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "WalletLedgers",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "TransactionAccounts",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "ErrorInfo",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<long>(nullable: false),
                    UpdatedBy = table.Column<long>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    Status = table.Column<short>(nullable: false),
                    FunctionName = table.Column<string>(maxLength: 50, nullable: false),
                    RefNo = table.Column<long>(nullable: false),
                    ErrorMsg = table.Column<string>(maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ErrorInfo", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ErrorInfo");

            migrationBuilder.DropColumn(
                name: "InBoundBalance",
                table: "WalletMasters");

            migrationBuilder.DropColumn(
                name: "OutBoundBalance",
                table: "WalletMasters");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "WalletLedgers");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "TransactionAccounts");
        }
    }
}
