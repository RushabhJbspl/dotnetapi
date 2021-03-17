using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CleanArchitecture.Web.Migrations
{
    public partial class AddUserWalletRequest : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "AliasName",
                table: "Organization_Master",
                maxLength: 250,
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 250);

            //migrationBuilder.AlterColumn<long>(
            //    name: "Id",
            //    table: "CommissionTypeMaster",
            //    nullable: false,
            //    oldClrType: typeof(long))
            //    .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            migrationBuilder.CreateTable(
                name: "AddUserWalletRequest",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<long>(nullable: false),
                    UpdatedBy = table.Column<long>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    Status = table.Column<short>(nullable: false),
                    ToUserId = table.Column<long>(nullable: false),
                    WalletOwnerUserID = table.Column<long>(nullable: false),
                    WalletID = table.Column<long>(nullable: false),
                    RoleId = table.Column<long>(nullable: false),
                    Message = table.Column<string>(maxLength: 1000, nullable: false),
                    Email = table.Column<string>(maxLength: 100, nullable: false),
                    ApproveDate = table.Column<DateTime>(nullable: false),
                    ApproveBy = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AddUserWalletRequest", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AddUserWalletRequest");

            migrationBuilder.AlterColumn<string>(
                name: "AliasName",
                table: "Organization_Master",
                maxLength: 250,
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 250,
                oldNullable: true);

            //migrationBuilder.AlterColumn<long>(
            //    name: "Id",
            //    table: "CommissionTypeMaster",
            //    nullable: false,
            //    oldClrType: typeof(long))
            //    .OldAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);
        }
    }
}
