using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CleanArchitecture.Web.Migrations
{
    public partial class removeunusedentity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserWalletMaster");

            migrationBuilder.DropTable(
                name: "Wallet_TypeMaster");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserWalletMaster",
                columns: table => new
                {
                    AccWalletID = table.Column<string>(maxLength: 16, nullable: false),
                    Balance = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    CreatedBy = table.Column<long>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    ExpiryDate = table.Column<DateTime>(nullable: false),
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    IsDefaultWallet = table.Column<byte>(nullable: false),
                    IsValid = table.Column<bool>(nullable: false),
                    OrganizationID = table.Column<long>(nullable: false),
                    PublicAddress = table.Column<string>(maxLength: 50, nullable: false),
                    Status = table.Column<short>(nullable: false),
                    UpdatedBy = table.Column<long>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    UserID = table.Column<long>(nullable: false),
                    WalletName = table.Column<string>(maxLength: 50, nullable: false),
                    WalletTypeID = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserWalletMaster", x => x.AccWalletID);
                });

            migrationBuilder.CreateTable(
                name: "Wallet_TypeMaster",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedBy = table.Column<long>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CurrencyTypeID = table.Column<long>(nullable: false),
                    Discription = table.Column<string>(maxLength: 100, nullable: false),
                    Status = table.Column<short>(nullable: false),
                    UpdatedBy = table.Column<long>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    WalletTypeName = table.Column<string>(maxLength: 7, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Wallet_TypeMaster", x => x.Id);
                });
        }
    }
}
