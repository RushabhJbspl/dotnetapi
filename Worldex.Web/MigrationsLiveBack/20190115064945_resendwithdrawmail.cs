using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CleanArchitecture.Web.Migrations
{
    public partial class resendwithdrawmail : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ConvertedAddress",
                table: "AddressMasters");

            migrationBuilder.AddColumn<short>(
                name: "WalletUsageType",
                table: "WalletMasters",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddColumn<DateTime>(
                name: "EmailSendDate",
                table: "TransactionQueue",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AlterColumn<string>(
                name: "Address",
                table: "AddressMasters",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 50);

            migrationBuilder.AddColumn<string>(
                name: "OriginalAddress",
                table: "AddressMasters",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "ColdWalletMaster",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<long>(nullable: false),
                    UpdatedBy = table.Column<long>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    Status = table.Column<short>(nullable: false),
                    KeyId1 = table.Column<string>(nullable: false),
                    KeyId2 = table.Column<string>(nullable: false),
                    KeyId3 = table.Column<string>(nullable: false),
                    BackUpKey = table.Column<string>(nullable: false),
                    PublicKey = table.Column<string>(nullable: false),
                    UserKey = table.Column<string>(nullable: true),
                    Recoverable = table.Column<short>(nullable: false),
                    WalletId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ColdWalletMaster", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TransactionMarketType",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<long>(nullable: false),
                    UpdatedBy = table.Column<long>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    Status = table.Column<short>(nullable: false),
                    MarketName = table.Column<string>(nullable: true),
                    AllowForFollowers = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransactionMarketType", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ColdWalletMaster");

            migrationBuilder.DropTable(
                name: "TransactionMarketType");

            migrationBuilder.DropColumn(
                name: "WalletUsageType",
                table: "WalletMasters");

            migrationBuilder.DropColumn(
                name: "EmailSendDate",
                table: "TransactionQueue");

            migrationBuilder.DropColumn(
                name: "OriginalAddress",
                table: "AddressMasters");

            migrationBuilder.AlterColumn<string>(
                name: "Address",
                table: "AddressMasters",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ConvertedAddress",
                table: "AddressMasters",
                maxLength: 50,
                nullable: true);
        }
    }
}
