using Microsoft.EntityFrameworkCore.Migrations;

namespace CleanArchitecture.Web.Migrations
{
    public partial class addColToTransactionQueueArbitrageEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<short>(
                name: "LPType",
                table: "TransactionQueueArbitrage",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddColumn<long>(
                name: "ProviderID",
                table: "TransactionQueueArbitrage",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<short>(
                name: "IsIntAmountAllow",
                table: "ServiceMasterArbitrage",
                nullable: false,
                defaultValue: (short)0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LPType",
                table: "TransactionQueueArbitrage");

            migrationBuilder.DropColumn(
                name: "ProviderID",
                table: "TransactionQueueArbitrage");

            migrationBuilder.DropColumn(
                name: "IsIntAmountAllow",
                table: "ServiceMasterArbitrage");
        }
    }
}
