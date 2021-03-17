using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace CleanArchitecture.Web.Migrations
{
    public partial class GroupRoleMappingResolveConflict : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "IPAddress",
                table: "WhiteListIPEndPoint",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AliasName",
                table: "WhiteListIPEndPoint",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            //migrationBuilder.DropTable(
            //    name: "RoleMaster");


            migrationBuilder.AddColumn<long>(
                name: "CreatedBy",
                table: "BizRoles",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "BizRoles",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<long>(
                name: "PermissionGroupID",
                table: "BizRoles",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<short>(
                name: "Status",
                table: "BizRoles",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddColumn<long>(
                name: "UpdatedBy",
                table: "BizRoles",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedDate",
                table: "BizRoles",
                nullable: true);


        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AliasName",
                table: "WhiteListIPEndPoint");

            migrationBuilder.AlterColumn<string>(
                name: "IPAddress",
                table: "WhiteListIPEndPoint",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "BizRoles");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "BizRoles");

            migrationBuilder.DropColumn(
                name: "PermissionGroupID",
                table: "BizRoles");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "BizRoles");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "BizRoles");

            migrationBuilder.DropColumn(
                name: "UpdatedDate",
                table: "BizRoles");

            //migrationBuilder.CreateTable(
            //    name: "RoleMaster",
            //    columns: table => new
            //    {
            //        Id = table.Column<int>(nullable: false)
            //            .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
            //        ConcurrencyStamp = table.Column<string>(nullable: true),
            //        CreatedBy = table.Column<long>(nullable: false),
            //        CreatedDate = table.Column<DateTime>(nullable: false),
            //        Name = table.Column<string>(nullable: true),
            //        NormalizedName = table.Column<string>(nullable: true),
            //        PermissionGroupID = table.Column<long>(nullable: false),
            //        RoleDescription = table.Column<string>(maxLength: 250, nullable: true),
            //        Status = table.Column<short>(nullable: false),
            //        UpdatedBy = table.Column<long>(nullable: true),
            //        UpdatedDate = table.Column<DateTime>(nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_RoleMaster", x => x.Id);
            //    });
        }
    }
}
