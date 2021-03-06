using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CleanArchitecture.Web.Migrations
{
    public partial class arbitragechargeentity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ChargeConfigurationDetailArbitrage",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<long>(nullable: false),
                    UpdatedBy = table.Column<long>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    Status = table.Column<short>(nullable: false),
                    ChargeConfigurationMasterID = table.Column<long>(nullable: false),
                    ChargeDistributionBasedOn = table.Column<short>(nullable: false),
                    ChargeType = table.Column<long>(nullable: false),
                    DeductionWalletTypeId = table.Column<long>(nullable: false),
                    ChargeValue = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    ChargeValueType = table.Column<short>(nullable: false),
                    MakerCharge = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    TakerCharge = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    MinAmount = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    MaxAmount = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    Remarks = table.Column<string>(nullable: true),
                    IsCurrencyConverted = table.Column<short>(nullable: false),
                    DeductChargetType = table.Column<short>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChargeConfigurationDetailArbitrage", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ChargeConfigurationMasterArbitrage",
                columns: table => new
                {
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<long>(nullable: false),
                    UpdatedBy = table.Column<long>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    Status = table.Column<short>(nullable: false),
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    WalletTypeID = table.Column<long>(nullable: false),
                    TrnType = table.Column<long>(nullable: false),
                    KYCComplaint = table.Column<short>(nullable: false),
                    SlabType = table.Column<short>(nullable: false),
                    SpecialChargeConfigurationID = table.Column<long>(nullable: false),
                    Remarks = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChargeConfigurationMasterArbitrage", x => new { x.WalletTypeID, x.TrnType, x.KYCComplaint, x.SpecialChargeConfigurationID });
                    table.UniqueConstraint("AK_ChargeConfigurationMasterArbitrage_KYCComplaint_SpecialChargeConfigurationID_TrnType_WalletTypeID", x => new { x.KYCComplaint, x.SpecialChargeConfigurationID, x.TrnType, x.WalletTypeID });
                });

            migrationBuilder.CreateTable(
                name: "TrnChargeLogArbitrage",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<long>(nullable: false),
                    UpdatedBy = table.Column<long>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    Status = table.Column<short>(nullable: false),
                    BatchNo = table.Column<string>(nullable: true),
                    TrnNo = table.Column<long>(nullable: false),
                    TrnType = table.Column<long>(nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    MakerCharge = table.Column<decimal>(type: "decimal(28, 18)", nullable: true),
                    TakerCharge = table.Column<decimal>(type: "decimal(28, 18)", nullable: true),
                    Charge = table.Column<decimal>(type: "decimal(28, 18)", nullable: true),
                    StakingChargeMasterID = table.Column<long>(nullable: true),
                    ChargeConfigurationDetailID = table.Column<long>(nullable: true),
                    TimeStamp = table.Column<string>(nullable: true),
                    DWalletID = table.Column<long>(nullable: false),
                    OWalletID = table.Column<long>(nullable: false),
                    DUserID = table.Column<long>(nullable: false),
                    OuserID = table.Column<long>(nullable: false),
                    WalletTypeID = table.Column<long>(nullable: false),
                    SlabType = table.Column<short>(nullable: false),
                    Remarks = table.Column<string>(nullable: true),
                    ChargeConfigurationMasterID = table.Column<long>(nullable: true),
                    IsMaker = table.Column<short>(nullable: true),
                    TrnRefNo = table.Column<long>(nullable: true),
                    OriginalAmount = table.Column<decimal>(type: "decimal(28, 18)", nullable: true),
                    IsReferCommGiven = table.Column<short>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrnChargeLogArbitrage", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChargeConfigurationDetailArbitrage");

            migrationBuilder.DropTable(
                name: "ChargeConfigurationMasterArbitrage");

            migrationBuilder.DropTable(
                name: "TrnChargeLogArbitrage");
        }
    }
}
