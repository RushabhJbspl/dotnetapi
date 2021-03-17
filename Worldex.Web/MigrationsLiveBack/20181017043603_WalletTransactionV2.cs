using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CleanArchitecture.Web.Migrations
{
    public partial class WalletTransactionV2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "TrnType",
                table: "WalletTransactionQueues",
                nullable: false,
                oldClrType: typeof(byte));

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "WalletTransactionQueues",
                nullable: false,
                oldClrType: typeof(byte));

            migrationBuilder.AddColumn<decimal>(
                name: "SettedAmt",
                table: "WalletTransactionQueues",
                type: "decimal(18, 8)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "WalletTransactionOrders",
                nullable: false,
                oldClrType: typeof(byte));

            //migrationBuilder.AlterColumn<long>(
            //    name: "Id",
            //    table: "TrnAcBatch",
            //    nullable: false,
            //    oldClrType: typeof(long))
            //    .OldAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AddColumn<long>(
                name: "Id",
                table: "TrnAcBatch",
                nullable: false
                );

            migrationBuilder.AlterColumn<long>(
                name: "StatusCode",
                table: "TransactionQueue",
                nullable: false,
                oldClrType: typeof(short));

            migrationBuilder.AddColumn<long>(
                name: "Id",
                table: "TradePairMaster",
                nullable: false);

            migrationBuilder.Sql("DBCC CHECKIDENT ('ServiceMaster', RESEED, 1000);");
            migrationBuilder.Sql("DBCC CHECKIDENT ('ServiceProviderDetail', RESEED, 3000);");
            migrationBuilder.Sql("DBCC CHECKIDENT ('ServiceProviderMaster', RESEED, 2000);");

            //migrationBuilder.AlterColumn<long>(
            //    name: "Id",
            //    table: "TradePairMaster",
            //    nullable: false,
            //    oldClrType: typeof(long))
            //    .OldAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            migrationBuilder.CreateTable(
                name: "TradeBuyRequest",
                columns: table => new
                {
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<long>(nullable: false),
                    UpdatedBy = table.Column<long>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: false),
                    Status = table.Column<short>(nullable: false),
                    Id = table.Column<long>(nullable: false),
                    TrnDate = table.Column<DateTime>(nullable: false),
                    PickupDate = table.Column<DateTime>(nullable: false),
                    MemberID = table.Column<long>(nullable: false),
                    TrnNo = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    PairID = table.Column<long>(nullable: false),
                    ServiceID = table.Column<long>(nullable: false),
                    Qty = table.Column<decimal>(type: "decimal(18, 8)", nullable: false),
                    BidPrice = table.Column<decimal>(type: "decimal(18, 8)", nullable: false),
                    PaidQty = table.Column<decimal>(type: "decimal(18, 8)", nullable: false),
                    PaidServiceID = table.Column<long>(nullable: false),
                    DeliveredQty = table.Column<decimal>(type: "decimal(18, 8)", nullable: false),
                    PendingQty = table.Column<decimal>(type: "decimal(18, 8)", nullable: false),
                    IsCancel = table.Column<short>(nullable: false),
                    IsPartialProceed = table.Column<short>(nullable: false),
                    IsProcessing = table.Column<short>(nullable: false),
                    SellStockID = table.Column<long>(nullable: false),
                    BuyStockID = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TradeBuyRequest", x => x.TrnNo);
                });

            migrationBuilder.CreateTable(
                name: "WalletLimitConfiguration",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<long>(nullable: false),
                    UpdatedBy = table.Column<long>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: false),
                    Status = table.Column<short>(nullable: false),
                    WalletId = table.Column<long>(nullable: false),
                    TrnType = table.Column<int>(nullable: false),
                    LimitPerHour = table.Column<decimal>(nullable: false),
                    LimitPerDay = table.Column<decimal>(nullable: false),
                    LimitPerTransaction = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WalletLimitConfiguration", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TradeBuyRequest");

            migrationBuilder.DropTable(
                name: "WalletLimitConfiguration");

            migrationBuilder.DropColumn(
                name: "SettedAmt",
                table: "WalletTransactionQueues");

            migrationBuilder.AlterColumn<byte>(
                name: "TrnType",
                table: "WalletTransactionQueues",
                nullable: false,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<byte>(
                name: "Status",
                table: "WalletTransactionQueues",
                nullable: false,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<byte>(
                name: "Status",
                table: "WalletTransactionOrders",
                nullable: false,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "TrnAcBatch",
                nullable: false,
                oldClrType: typeof(long))
                .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<short>(
                name: "StatusCode",
                table: "TransactionQueue",
                nullable: false,
                oldClrType: typeof(long));

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "TradePairMaster",
                nullable: false,
                oldClrType: typeof(long))
                .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);
        }
    }
}
