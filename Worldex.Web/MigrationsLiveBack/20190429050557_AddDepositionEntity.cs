using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CleanArchitecture.Web.Migrations
{
    public partial class AddDepositionEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MarginAddressMaster",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<long>(nullable: false),
                    UpdatedBy = table.Column<long>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    Status = table.Column<short>(nullable: false),
                    WalletId = table.Column<long>(nullable: false),
                    Address = table.Column<string>(maxLength: 50, nullable: true),
                    IsDefaultAddress = table.Column<byte>(nullable: false),
                    SerProID = table.Column<long>(nullable: false),
                    AddressLable = table.Column<string>(maxLength: 50, nullable: false),
                    OriginalAddress = table.Column<string>(maxLength: 50, nullable: false),
                    GUID = table.Column<string>(nullable: true),
                    AddressType = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MarginAddressMaster", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MarginDepositCounterLog",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<long>(nullable: false),
                    UpdatedBy = table.Column<long>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    Status = table.Column<short>(nullable: false),
                    NewTxnID = table.Column<string>(nullable: true),
                    PreviousTrnID = table.Column<string>(nullable: true),
                    LastTrnID = table.Column<string>(nullable: true),
                    LastLimit = table.Column<long>(nullable: false),
                    NextBatchPrvID = table.Column<string>(nullable: true),
                    DepositCounterMasterId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MarginDepositCounterLog", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MarginDepositCounterMaster",
                columns: table => new
                {
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<long>(nullable: false),
                    UpdatedBy = table.Column<long>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    Status = table.Column<short>(nullable: false),
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    RecordCount = table.Column<int>(nullable: false),
                    Limit = table.Column<long>(nullable: false),
                    LastTrnID = table.Column<string>(nullable: true),
                    MaxLimit = table.Column<long>(nullable: false),
                    WalletTypeID = table.Column<long>(nullable: false),
                    SerProId = table.Column<long>(nullable: false),
                    PreviousTrnID = table.Column<string>(nullable: true),
                    prevIterationID = table.Column<string>(nullable: true),
                    TPSPickupStatus = table.Column<long>(nullable: false),
                    AppType = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MarginDepositCounterMaster", x => new { x.WalletTypeID, x.SerProId });
                    table.UniqueConstraint("AK_MarginDepositCounterMaster_SerProId_WalletTypeID", x => new { x.SerProId, x.WalletTypeID });
                });

            migrationBuilder.CreateTable(
                name: "MarginDepositHistory",
                columns: table => new
                {
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<long>(nullable: false),
                    UpdatedBy = table.Column<long>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    Status = table.Column<short>(nullable: false),
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    TrnID = table.Column<string>(maxLength: 100, nullable: false),
                    SMSCode = table.Column<string>(nullable: false),
                    Address = table.Column<string>(maxLength: 50, nullable: false),
                    Confirmations = table.Column<long>(nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    StatusMsg = table.Column<string>(maxLength: 100, nullable: false),
                    TimeEpoch = table.Column<string>(nullable: false),
                    ConfirmedTime = table.Column<string>(nullable: false),
                    EpochTimePure = table.Column<string>(nullable: true),
                    OrderID = table.Column<long>(nullable: false),
                    IsProcessing = table.Column<byte>(nullable: false),
                    FromAddress = table.Column<string>(maxLength: 50, nullable: false),
                    APITopUpRefNo = table.Column<string>(nullable: true),
                    SystemRemarks = table.Column<string>(nullable: true),
                    RouteTag = table.Column<string>(nullable: true),
                    SerProID = table.Column<long>(nullable: false),
                    UserId = table.Column<long>(nullable: false),
                    IsInternalTrn = table.Column<short>(nullable: true),
                    WalletId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MarginDepositHistory", x => new { x.TrnID, x.Address });
                    table.UniqueConstraint("AK_MarginDepositHistory_Address_TrnID", x => new { x.Address, x.TrnID });
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MarginAddressMaster");

            migrationBuilder.DropTable(
                name: "MarginDepositCounterLog");

            migrationBuilder.DropTable(
                name: "MarginDepositCounterMaster");

            migrationBuilder.DropTable(
                name: "MarginDepositHistory");
        }
    }
}
