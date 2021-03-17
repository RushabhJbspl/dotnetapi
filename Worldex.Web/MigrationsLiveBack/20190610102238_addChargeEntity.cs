using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CleanArchitecture.Web.Migrations
{
    public partial class addChargeEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_LPArbitrageLPTransactionAccount",
                table: "LPArbitrageLPTransactionAccount");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_ArbitrageLPWalletMaster_SerProID_WalletTypeID",
                table: "ArbitrageLPWalletMaster");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ArbitrageLPWalletMaster",
                table: "ArbitrageLPWalletMaster");

            migrationBuilder.RenameTable(
                name: "LPArbitrageLPTransactionAccount",
                newName: "LPArbitrageTransactionAccount");

            migrationBuilder.RenameTable(
                name: "ArbitrageLPWalletMaster",
                newName: "LPArbitrageWalletMaster");

            migrationBuilder.AddColumn<int>(
                name: "LPType",
                table: "ArbitrageWalletTransactionQueue",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_LPArbitrageTransactionAccount",
                table: "LPArbitrageTransactionAccount",
                column: "Id");

            migrationBuilder.AddUniqueConstraint(
                name: "AK_LPArbitrageWalletMaster_SerProID_WalletTypeID",
                table: "LPArbitrageWalletMaster",
                columns: new[] { "SerProID", "WalletTypeID" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_LPArbitrageWalletMaster",
                table: "LPArbitrageWalletMaster",
                columns: new[] { "WalletTypeID", "SerProID" });

            migrationBuilder.CreateTable(
                name: "ArbitrageChargeConfigurationDetail",
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
                    ChargeValue = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    ChargeValueType = table.Column<short>(nullable: false),
                    MakerCharge = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    TakerCharge = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    Remarks = table.Column<string>(nullable: true),
                    IsCurrencyConverted = table.Column<short>(nullable: false),
                    MarkUpValue = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    MarkUpValueType = table.Column<short>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArbitrageChargeConfigurationDetail", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ArbitrageChargeConfigurationMaster",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<long>(nullable: false),
                    UpdatedBy = table.Column<long>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    Status = table.Column<short>(nullable: false),
                    WalletTypeID = table.Column<long>(nullable: false),
                    PairId = table.Column<long>(nullable: false),
                    SerProId = table.Column<long>(nullable: false),
                    TrnType = table.Column<long>(nullable: false),
                    KYCComplaint = table.Column<short>(nullable: false),
                    Remarks = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArbitrageChargeConfigurationMaster", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ArbitrageChargeConfigurationDetail");

            migrationBuilder.DropTable(
                name: "ArbitrageChargeConfigurationMaster");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_LPArbitrageWalletMaster_SerProID_WalletTypeID",
                table: "LPArbitrageWalletMaster");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LPArbitrageWalletMaster",
                table: "LPArbitrageWalletMaster");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LPArbitrageTransactionAccount",
                table: "LPArbitrageTransactionAccount");

            migrationBuilder.DropColumn(
                name: "LPType",
                table: "ArbitrageWalletTransactionQueue");

            migrationBuilder.RenameTable(
                name: "LPArbitrageWalletMaster",
                newName: "ArbitrageLPWalletMaster");

            migrationBuilder.RenameTable(
                name: "LPArbitrageTransactionAccount",
                newName: "LPArbitrageLPTransactionAccount");

            migrationBuilder.AddUniqueConstraint(
                name: "AK_ArbitrageLPWalletMaster_SerProID_WalletTypeID",
                table: "ArbitrageLPWalletMaster",
                columns: new[] { "SerProID", "WalletTypeID" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_ArbitrageLPWalletMaster",
                table: "ArbitrageLPWalletMaster",
                columns: new[] { "WalletTypeID", "SerProID" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_LPArbitrageLPTransactionAccount",
                table: "LPArbitrageLPTransactionAccount",
                column: "Id");
        }
    }
}
