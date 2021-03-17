using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CleanArchitecture.Web.Migrations
{
    public partial class removeColumnITrnnoforStatusCheck : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropUniqueConstraint(
                name: "AK_WalletAuthorizeUserMaster_Id_UserID_WalletID",
                table: "WalletAuthorizeUserMaster");

            //migrationBuilder.AlterColumn<long>(
            //    name: "Id",
            //    table: "TransactionStatusCheckRequest",
            //    nullable: false,
            //    oldClrType: typeof(long))
            //    .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AddColumn<long>(
                name: "TrnNo",
                table: "TransactionStatusCheckRequest",
                nullable: false,
                defaultValue: 0L);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TrnNo",
                table: "TransactionStatusCheckRequest");

            //migrationBuilder.AlterColumn<long>(
            //    name: "Id",
            //    table: "TransactionStatusCheckRequest",
            //    nullable: false,
            //    oldClrType: typeof(long))
            //    .OldAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AddUniqueConstraint(
                name: "AK_WalletAuthorizeUserMaster_Id_UserID_WalletID",
                table: "WalletAuthorizeUserMaster",
                columns: new[] { "Id", "UserID", "WalletID" });
        }
    }
}
