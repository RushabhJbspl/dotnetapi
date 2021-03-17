using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CleanArchitecture.Web.Migrations
{
    public partial class TradeRecon : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DebitAccountID",
                table: "TransactionQueue",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "TransactionRecon",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<long>(nullable: false),
                    UpdatedBy = table.Column<long>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    Status = table.Column<short>(nullable: false),
                    TrnNo = table.Column<long>(nullable: false),
                    ServiceID = table.Column<long>(nullable: false),
                    SerProID = table.Column<long>(nullable: false),
                    OldStatus = table.Column<short>(nullable: false),
                    NewStatus = table.Column<short>(nullable: false),
                    Remarks = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransactionRecon", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TransactionRecon");

            migrationBuilder.DropColumn(
                name: "DebitAccountID",
                table: "TransactionQueue");
        }
    }
}
