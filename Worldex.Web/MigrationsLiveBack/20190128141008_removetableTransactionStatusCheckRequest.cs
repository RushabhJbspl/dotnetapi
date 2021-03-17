using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CleanArchitecture.Web.Migrations
{
    public partial class removetableTransactionStatusCheckRequest : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TransactionStatusCheckRequest");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TransactionStatusCheckRequest",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedBy = table.Column<long>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    OprTrnID = table.Column<string>(nullable: true),
                    RequestData = table.Column<string>(nullable: true),
                    ResponseData = table.Column<string>(nullable: true),
                    ResponseTime = table.Column<DateTime>(nullable: false),
                    SerProDetailID = table.Column<long>(nullable: false),
                    Status = table.Column<short>(nullable: false),
                    TrnID = table.Column<string>(nullable: true),
                    TrnNo = table.Column<long>(nullable: false),
                    UpdatedBy = table.Column<long>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransactionStatusCheckRequest", x => x.Id);
                });
        }
    }
}
