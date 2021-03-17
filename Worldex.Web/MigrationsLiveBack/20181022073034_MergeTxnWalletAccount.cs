using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CleanArchitecture.Web.Migrations
{
    public partial class MergeTxnWalletAccount : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropUniqueConstraint(
                name: "AK_TradePoolMaster_BidPrice_BuyServiceID_Id_SellServiceID",
                table: "TradePoolMaster");

            migrationBuilder.DropColumn(
                name: "RoutID",
                table: "TransactionQueue");

            migrationBuilder.AlterColumn<long>(
                name: "ProductID",
                table: "TransactionQueue",
                nullable: false,
                oldClrType: typeof(int));

            migrationBuilder.AddColumn<long>(
                name: "RouteID",
                table: "TransactionQueue",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "CountPerPrice",
                table: "TradePoolMaster",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AlterColumn<long>(
                name: "CountryID",
                table: "StateMaster",
                nullable: false,
                oldClrType: typeof(string));

            migrationBuilder.AddUniqueConstraint(
                name: "AK_TradePoolMaster_BidPrice_BuyServiceID_CountPerPrice_Id_SellServiceID",
                table: "TradePoolMaster",
                columns: new[] { "BidPrice", "BuyServiceID", "CountPerPrice", "Id", "SellServiceID" });

            migrationBuilder.CreateTable(
                name: "CityMaster",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<long>(nullable: false),
                    UpdatedBy = table.Column<long>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: false),
                    Status = table.Column<short>(nullable: false),
                    CityName = table.Column<string>(maxLength: 30, nullable: false),
                    StateID = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CityMaster", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DepositCounterLog",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<long>(nullable: false),
                    UpdatedBy = table.Column<long>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: false),
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
                    table.PrimaryKey("PK_DepositCounterLog", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DepositCounterMaster",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<long>(nullable: false),
                    UpdatedBy = table.Column<long>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: false),
                    Status = table.Column<short>(nullable: false),
                    RecordCount = table.Column<int>(nullable: false),
                    Limit = table.Column<long>(nullable: false),
                    LastTrnID = table.Column<string>(nullable: true),
                    MaxLimit = table.Column<long>(nullable: false),
                    WalletTypeID = table.Column<long>(nullable: false),
                    SerProId = table.Column<long>(nullable: false),
                    PreviousTrnID = table.Column<string>(nullable: true),
                    TPSPickupStatus = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DepositCounterMaster", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ZipCodeMaster",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<long>(nullable: false),
                    UpdatedBy = table.Column<long>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: false),
                    Status = table.Column<short>(nullable: false),
                    ZipCode = table.Column<long>(nullable: false),
                    ZipAreaName = table.Column<string>(maxLength: 30, nullable: false),
                    CityID = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ZipCodeMaster", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CityMaster");

            migrationBuilder.DropTable(
                name: "DepositCounterLog");

            migrationBuilder.DropTable(
                name: "DepositCounterMaster");

            migrationBuilder.DropTable(
                name: "ZipCodeMaster");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_TradePoolMaster_BidPrice_BuyServiceID_CountPerPrice_Id_SellServiceID",
                table: "TradePoolMaster");

            migrationBuilder.DropColumn(
                name: "RouteID",
                table: "TransactionQueue");

            migrationBuilder.DropColumn(
                name: "CountPerPrice",
                table: "TradePoolMaster");

            migrationBuilder.AlterColumn<int>(
                name: "ProductID",
                table: "TransactionQueue",
                nullable: false,
                oldClrType: typeof(long));

            migrationBuilder.AddColumn<int>(
                name: "RoutID",
                table: "TransactionQueue",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "CountryID",
                table: "StateMaster",
                nullable: false,
                oldClrType: typeof(long));

            migrationBuilder.AddUniqueConstraint(
                name: "AK_TradePoolMaster_BidPrice_BuyServiceID_Id_SellServiceID",
                table: "TradePoolMaster",
                columns: new[] { "BidPrice", "BuyServiceID", "Id", "SellServiceID" });
        }
    }
}
