using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CleanArchitecture.Web.Migrations
{
    public partial class addDepositCounterForNEO : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "NEODepositCounter",
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
                    FlushAddressEnable = table.Column<int>(nullable: false),
                    TPSPickupStatus = table.Column<long>(nullable: false),
                    AppType = table.Column<int>(nullable: false),
                    StartTime = table.Column<double>(nullable: false),
                    EndTime = table.Column<double>(nullable: false),
                    AddressId = table.Column<long>(nullable: false),
                    PickUpDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NEODepositCounter", x => new { x.WalletTypeID, x.SerProId, x.AddressId });
                    table.UniqueConstraint("AK_NEODepositCounter_AddressId_SerProId_WalletTypeID", x => new { x.AddressId, x.SerProId, x.WalletTypeID });
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "NEODepositCounter");
        }
    }
}
