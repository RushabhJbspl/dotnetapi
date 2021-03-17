using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CleanArchitecture.Web.Migrations
{
    public partial class AddArbitrageLPAddressMaster : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ArbitrageLPAddressMaster",
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
                    Address = table.Column<string>(maxLength: 70, nullable: true),
                    IsDefaultAddress = table.Column<byte>(nullable: false),
                    SerProID = table.Column<long>(nullable: false),
                    AddressLable = table.Column<string>(maxLength: 50, nullable: false),
                    OriginalAddress = table.Column<string>(maxLength: 70, nullable: false),
                    GUID = table.Column<string>(nullable: true),
                    AddressType = table.Column<int>(nullable: false),
                    TxnID = table.Column<string>(maxLength: 150, nullable: true),
                    WalletTypeId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArbitrageLPAddressMaster", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ArbitrageLPAddressMaster");
        }
    }
}
