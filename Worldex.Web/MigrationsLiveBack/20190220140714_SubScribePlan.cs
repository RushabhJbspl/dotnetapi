using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CleanArchitecture.Web.Migrations
{
    public partial class SubScribePlan : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "APIPlanDeatilID",
                table: "WhiteListIPEndPoint");

            migrationBuilder.DropColumn(
                name: "WhiteListIPEndPointID",
                table: "UserAPIKeyDetails");

            migrationBuilder.AddColumn<string>(
                name: "IPAddress",
                table: "APIPlanMaster",
                nullable: true);

            migrationBuilder.AddColumn<short>(
                name: "IsPlanRecursive",
                table: "APIPlanMaster",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.CreateTable(
                name: "MarginTransactionBlockedChannel",
                columns: table => new
                {
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<long>(nullable: false),
                    UpdatedBy = table.Column<long>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    Status = table.Column<short>(nullable: false),
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ChannelID = table.Column<long>(nullable: false),
                    TrnType = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MarginTransactionBlockedChannel", x => new { x.ChannelID, x.TrnType });
                });

            migrationBuilder.CreateTable(
                name: "MarginUserWalletBlockTrnTypeMaster",
                columns: table => new
                {
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<long>(nullable: false),
                    UpdatedBy = table.Column<long>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    Status = table.Column<short>(nullable: false),
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    WalletID = table.Column<long>(nullable: false),
                    WTrnTypeMasterID = table.Column<long>(nullable: false),
                    Remarks = table.Column<string>(maxLength: 150, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MarginUserWalletBlockTrnTypeMaster", x => new { x.WalletID, x.WTrnTypeMasterID });
                });

            migrationBuilder.CreateTable(
                name: "PublicAPIKeyPolicy",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<long>(nullable: false),
                    UpdatedBy = table.Column<long>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    Status = table.Column<short>(nullable: false),
                    AddMaxLimit = table.Column<int>(nullable: false),
                    AddPerDayFrequency = table.Column<int>(nullable: false),
                    AddFrequency = table.Column<int>(nullable: false),
                    DeleteMaxLimit = table.Column<int>(nullable: false),
                    DeletePerDayFrequency = table.Column<int>(nullable: false),
                    DeleteFrequency = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PublicAPIKeyPolicy", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SubScribePlan",
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
                    Price = table.Column<long>(nullable: false),
                    Charge = table.Column<long>(nullable: false),
                    TotalAmt = table.Column<long>(nullable: false),
                    SubscribeStatus = table.Column<short>(nullable: false),
                    RenewalStatus = table.Column<short>(nullable: false),
                    ActivationDate = table.Column<DateTime>(nullable: true),
                    Perticuler = table.Column<string>(nullable: true),
                    DebitedAccountID = table.Column<string>(nullable: true),
                    DebitedCurrency = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubScribePlan", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MarginTransactionBlockedChannel");

            migrationBuilder.DropTable(
                name: "MarginUserWalletBlockTrnTypeMaster");

            migrationBuilder.DropTable(
                name: "PublicAPIKeyPolicy");

            migrationBuilder.DropTable(
                name: "SubScribePlan");

            migrationBuilder.DropColumn(
                name: "IPAddress",
                table: "APIPlanMaster");

            migrationBuilder.DropColumn(
                name: "IsPlanRecursive",
                table: "APIPlanMaster");

            migrationBuilder.AddColumn<long>(
                name: "APIPlanDeatilID",
                table: "WhiteListIPEndPoint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "WhiteListIPEndPointID",
                table: "UserAPIKeyDetails",
                nullable: false,
                defaultValue: 0L);
        }
    }
}
