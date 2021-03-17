using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Worldex.Web.Migrations
{
    public partial class EntityUserBankRequest : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserBankRequest",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<long>(nullable: false),
                    UpdatedBy = table.Column<long>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    Status = table.Column<short>(nullable: false),
                    GUID = table.Column<string>(nullable: true),
                    UserId = table.Column<long>(nullable: false),
                    BankName = table.Column<string>(maxLength: 100, nullable: false),
                    BankCode = table.Column<string>(maxLength: 50, nullable: false),
                    BankAccountNumber = table.Column<string>(maxLength: 50, nullable: false),
                    BankAcountHolderName = table.Column<string>(maxLength: 100, nullable: false),
                    CurrencyCode = table.Column<string>(maxLength: 5, nullable: false),
                    CountryCode = table.Column<string>(maxLength: 5, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserBankRequest", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserBankRequest_GUID",
                table: "UserBankRequest",
                column: "GUID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserBankRequest");
        }
    }
}
