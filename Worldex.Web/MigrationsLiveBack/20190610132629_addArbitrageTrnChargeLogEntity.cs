using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CleanArchitecture.Web.Migrations
{
    public partial class addArbitrageTrnChargeLogEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ArbitrageTrnChargeLog",
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
                    Type = table.Column<short>(nullable: false),
                    TrnNo = table.Column<long>(nullable: false),
                    TrnType = table.Column<long>(nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    MakerCharge = table.Column<decimal>(type: "decimal(28, 18)", nullable: true),
                    TakerCharge = table.Column<decimal>(type: "decimal(28, 18)", nullable: true),
                    Charge = table.Column<decimal>(type: "decimal(28, 18)", nullable: true),
                    MarkUpValue = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    ChargeConfigurationDetailID = table.Column<long>(nullable: true),
                    TimeStamp = table.Column<string>(nullable: true),
                    DWalletID = table.Column<long>(nullable: false),
                    OWalletID = table.Column<long>(nullable: false),
                    DUserID = table.Column<long>(nullable: false),
                    OuserID = table.Column<long>(nullable: false),
                    WalletTypeID = table.Column<long>(nullable: false),
                    PairId = table.Column<long>(nullable: false),
                    Remarks = table.Column<string>(nullable: true),
                    ChargeConfigurationMasterID = table.Column<long>(nullable: true),
                    IsMaker = table.Column<short>(nullable: true),
                    TrnRefNo = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArbitrageTrnChargeLog", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ArbitrageTrnChargeLog");
        }
    }
}
