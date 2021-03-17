using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CleanArchitecture.Web.Migrations
{
    public partial class UpdateWalletServiceV11510 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TransactionQueue");

            migrationBuilder.AlterColumn<Guid>(
                name: "Guid",
                table: "WalletTransactionQueues",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 50);

            migrationBuilder.CreateTable(
                name: "TrnAcBatch",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<long>(nullable: false),
                    UpdatedBy = table.Column<long>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: false),
                    Status = table.Column<short>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrnAcBatch", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TrnAcBatch");

            migrationBuilder.AlterColumn<string>(
                name: "Guid",
                table: "WalletTransactionQueues",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(Guid),
                oldMaxLength: 50);

            migrationBuilder.CreateTable(
                name: "TransactionQueue",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AdditionalInfo = table.Column<string>(nullable: true),
                    Amount = table.Column<decimal>(type: "decimal(18, 8)", nullable: false),
                    ChargePer = table.Column<decimal>(type: "decimal(18, 8)", nullable: true),
                    ChargeRs = table.Column<decimal>(type: "decimal(18, 8)", nullable: true),
                    ChargeType = table.Column<short>(nullable: true),
                    CreatedBy = table.Column<long>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    GUID = table.Column<Guid>(nullable: false),
                    MemberID = table.Column<long>(nullable: false),
                    MemberMobile = table.Column<string>(nullable: false),
                    ProductID = table.Column<int>(nullable: false),
                    RoutID = table.Column<int>(nullable: false),
                    SMSCode = table.Column<string>(maxLength: 10, nullable: false),
                    SerProID = table.Column<long>(nullable: false),
                    ServiceID = table.Column<long>(nullable: false),
                    Status = table.Column<short>(nullable: false),
                    StatusCode = table.Column<short>(nullable: false),
                    StatusMsg = table.Column<string>(nullable: true),
                    TransactionAccount = table.Column<string>(maxLength: 200, nullable: false),
                    TrnDate = table.Column<DateTime>(nullable: false),
                    TrnMode = table.Column<short>(nullable: false),
                    TrnRefNo = table.Column<string>(nullable: true),
                    TrnType = table.Column<short>(nullable: false),
                    UpdatedBy = table.Column<long>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: false),
                    VerifyDone = table.Column<short>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransactionQueue", x => x.Id);
                });
        }
    }
}
