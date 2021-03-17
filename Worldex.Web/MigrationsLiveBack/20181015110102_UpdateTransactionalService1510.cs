using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CleanArchitecture.Web.Migrations
{
    public partial class UpdateTransactionalService1510 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TransactionQueue",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<long>(nullable: false),
                    UpdatedBy = table.Column<long>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: false),
                    Status = table.Column<short>(nullable: false),
                    GUID = table.Column<Guid>(nullable: false),
                    TrnDate = table.Column<DateTime>(nullable: false),
                    TrnMode = table.Column<short>(nullable: false),
                    TrnType = table.Column<short>(nullable: false),
                    MemberID = table.Column<long>(nullable: false),
                    MemberMobile = table.Column<string>(nullable: false),
                    SMSCode = table.Column<string>(maxLength: 10, nullable: false),
                    TransactionAccount = table.Column<string>(maxLength: 200, nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18, 8)", nullable: false),
                    ServiceID = table.Column<long>(nullable: false),
                    SerProID = table.Column<long>(nullable: false),
                    ProductID = table.Column<int>(nullable: false),
                    RoutID = table.Column<int>(nullable: false),
                    StatusCode = table.Column<short>(nullable: false),
                    StatusMsg = table.Column<string>(nullable: true),
                    VerifyDone = table.Column<short>(nullable: false),
                    TrnRefNo = table.Column<string>(nullable: true),
                    AdditionalInfo = table.Column<string>(nullable: true),
                    ChargePer = table.Column<decimal>(type: "decimal(18, 8)", nullable: true),
                    ChargeRs = table.Column<decimal>(type: "decimal(18, 8)", nullable: true),
                    ChargeType = table.Column<short>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransactionQueue", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TransactionQueue");
        }
    }
}
