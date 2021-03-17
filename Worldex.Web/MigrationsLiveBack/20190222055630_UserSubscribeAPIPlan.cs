using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CleanArchitecture.Web.Migrations
{
    public partial class UserSubscribeAPIPlan : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SubScribePlan");

            migrationBuilder.AddColumn<short>(
                name: "Visibility",
                table: "FieldMaster",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.CreateTable(
                name: "TradeCancelQueueMargin",
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
                    DeliverServiceID = table.Column<long>(nullable: false),
                    TrnDate = table.Column<DateTime>(nullable: false),
                    PendingBuyQty = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    DeliverQty = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    OrderType = table.Column<short>(nullable: true),
                    DeliverBidPrice = table.Column<decimal>(type: "decimal(28, 18)", nullable: true),
                    StatusMsg = table.Column<string>(nullable: false),
                    OrderID = table.Column<long>(nullable: false),
                    SettledDate = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TradeCancelQueueMargin", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserSubscribeAPIPlan",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<long>(nullable: false),
                    UpdatedBy = table.Column<long>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    Status = table.Column<short>(nullable: false),
                    APIPlanMasterID = table.Column<long>(nullable: false),
                    ExpiryDate = table.Column<DateTime>(nullable: true),
                    Price = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    Charge = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    TotalAmt = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    UserID = table.Column<long>(nullable: false),
                    RenewalStatus = table.Column<short>(nullable: true),
                    ActivationDate = table.Column<DateTime>(nullable: true),
                    Perticuler = table.Column<string>(nullable: true),
                    DebitedAccountID = table.Column<string>(nullable: true),
                    DebitedCurrency = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserSubscribeAPIPlan", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TradeCancelQueueMargin");

            migrationBuilder.DropTable(
                name: "UserSubscribeAPIPlan");

            migrationBuilder.DropColumn(
                name: "Visibility",
                table: "FieldMaster");

            migrationBuilder.CreateTable(
                name: "SubScribePlan",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    APIPlanMasterID = table.Column<long>(nullable: false),
                    ActivationDate = table.Column<DateTime>(nullable: true),
                    Charge = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    CreatedBy = table.Column<long>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    DebitedAccountID = table.Column<string>(nullable: true),
                    DebitedCurrency = table.Column<string>(nullable: true),
                    ExpiryDate = table.Column<DateTime>(nullable: true),
                    Perticuler = table.Column<string>(nullable: true),
                    Price = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    RenewalStatus = table.Column<short>(nullable: false),
                    Status = table.Column<short>(nullable: false),
                    SubscribeStatus = table.Column<short>(nullable: false),
                    TotalAmt = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    UpdatedBy = table.Column<long>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubScribePlan", x => x.Id);
                });
        }
    }
}
