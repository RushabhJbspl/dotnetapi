using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CleanArchitecture.Web.Migrations
{
    public partial class AffiliatePromotionLimitConfiguration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropForeignKey(
            //    name: "FK_UserAssignFieldRights_UserAssignSubModule_UserAssignSubModuleID",
            //    table: "UserAssignFieldRights");

            //migrationBuilder.DropForeignKey(
            //    name: "FK_UserAssignModule_UserAccessRights_UserAccessRightsId",
            //    table: "UserAssignModule");

            //migrationBuilder.DropForeignKey(
            //    name: "FK_UserAssignSubModule_UserAssignModule_UserAssignModuleID",
            //    table: "UserAssignSubModule");

            //migrationBuilder.DropForeignKey(
            //    name: "FK_UserAssignToolRights_UserAssignSubModule_UserAssignSubModuleID",
            //    table: "UserAssignToolRights");

            //migrationBuilder.DropColumn(
            //    name: "PermissionGroupID",
            //    table: "BizRoles");

            migrationBuilder.AddColumn<short>(
                name: "IPType",
                table: "WhiteListIPEndPoint",
                nullable: false,
                defaultValue: (short)0);

            //migrationBuilder.AlterColumn<long>(
            //    name: "UserAssignSubModuleID",
            //    table: "UserAssignToolRights",
            //    nullable: false,
            //    oldClrType: typeof(long),
            //    oldNullable: true);

            //migrationBuilder.AlterColumn<long>(
            //    name: "UserAssignModuleID",
            //    table: "UserAssignSubModule",
            //    nullable: false,
            //    oldClrType: typeof(long),
            //    oldNullable: true);

            //migrationBuilder.AlterColumn<long>(
            //    name: "UserAccessRightsId",
            //    table: "UserAssignModule",
            //    nullable: false,
            //    oldClrType: typeof(long),
            //    oldNullable: true);

            //migrationBuilder.AlterColumn<long>(
            //    name: "UserAssignSubModuleID",
            //    table: "UserAssignFieldRights",
            //    nullable: false,
            //    oldClrType: typeof(long),
            //    oldNullable: true);

            migrationBuilder.AddColumn<short>(
                name: "IsVisibility",
                table: "UserAssignFieldRights",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddUniqueConstraint(
                name: "AK_GroupRoleMapping_RoleId",
                table: "GroupRoleMapping",
                column: "RoleId");

            migrationBuilder.CreateTable(
                name: "AffiliatePromotionLimitConfiguration",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<long>(nullable: false),
                    UpdatedBy = table.Column<long>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    Status = table.Column<short>(nullable: false),
                    PromotionType = table.Column<long>(nullable: false),
                    HourlyLimit = table.Column<long>(nullable: false),
                    DailyLimit = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AffiliatePromotionLimitConfiguration", x => x.Id);
                });

            //migrationBuilder.AddForeignKey(
            //    name: "FK_UserAssignFieldRights_UserAssignSubModule_UserAssignSubModuleID",
            //    table: "UserAssignFieldRights",
            //    column: "UserAssignSubModuleID",
            //    principalTable: "UserAssignSubModule",
            //    principalColumn: "ID",
            //    onDelete: ReferentialAction.Cascade);

            //migrationBuilder.AddForeignKey(
            //    name: "FK_UserAssignModule_UserAccessRights_UserAccessRightsId",
            //    table: "UserAssignModule",
            //    column: "UserAccessRightsId",
            //    principalTable: "UserAccessRights",
            //    principalColumn: "Id",
            //    onDelete: ReferentialAction.Cascade);

            //migrationBuilder.AddForeignKey(
            //    name: "FK_UserAssignSubModule_UserAssignModule_UserAssignModuleID",
            //    table: "UserAssignSubModule",
            //    column: "UserAssignModuleID",
            //    principalTable: "UserAssignModule",
            //    principalColumn: "ID",
            //    onDelete: ReferentialAction.Cascade);

            //migrationBuilder.AddForeignKey(
            //    name: "FK_UserAssignToolRights_UserAssignSubModule_UserAssignSubModuleID",
            //    table: "UserAssignToolRights",
            //    column: "UserAssignSubModuleID",
            //    principalTable: "UserAssignSubModule",
            //    principalColumn: "ID",
            //    onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropForeignKey(
            //    name: "FK_UserAssignFieldRights_UserAssignSubModule_UserAssignSubModuleID",
            //    table: "UserAssignFieldRights");

            //migrationBuilder.DropForeignKey(
            //    name: "FK_UserAssignModule_UserAccessRights_UserAccessRightsId",
            //    table: "UserAssignModule");

            //migrationBuilder.DropForeignKey(
            //    name: "FK_UserAssignSubModule_UserAssignModule_UserAssignModuleID",
            //    table: "UserAssignSubModule");

            //migrationBuilder.DropForeignKey(
            //    name: "FK_UserAssignToolRights_UserAssignSubModule_UserAssignSubModuleID",
            //    table: "UserAssignToolRights");

            migrationBuilder.DropTable(
                name: "AffiliatePromotionLimitConfiguration");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_GroupRoleMapping_RoleId",
                table: "GroupRoleMapping");

            migrationBuilder.DropColumn(
                name: "IPType",
                table: "WhiteListIPEndPoint");

            migrationBuilder.DropColumn(
                name: "IsVisibility",
                table: "UserAssignFieldRights");

            //migrationBuilder.AlterColumn<long>(
            //    name: "UserAssignSubModuleID",
            //    table: "UserAssignToolRights",
            //    nullable: true,
            //    oldClrType: typeof(long));

            //migrationBuilder.AlterColumn<long>(
            //    name: "UserAssignModuleID",
            //    table: "UserAssignSubModule",
            //    nullable: true,
            //    oldClrType: typeof(long));

            //migrationBuilder.AlterColumn<long>(
            //    name: "UserAccessRightsId",
            //    table: "UserAssignModule",
            //    nullable: true,
            //    oldClrType: typeof(long));

            //migrationBuilder.AlterColumn<long>(
            //    name: "UserAssignSubModuleID",
            //    table: "UserAssignFieldRights",
            //    nullable: true,
            //    oldClrType: typeof(long));

            //migrationBuilder.AddColumn<long>(
            //    name: "PermissionGroupID",
            //    table: "BizRoles",
            //    nullable: false,
            //    defaultValue: 0L);

            //migrationBuilder.AddForeignKey(
            //    name: "FK_UserAssignFieldRights_UserAssignSubModule_UserAssignSubModuleID",
            //    table: "UserAssignFieldRights",
            //    column: "UserAssignSubModuleID",
            //    principalTable: "UserAssignSubModule",
            //    principalColumn: "ID",
            //    onDelete: ReferentialAction.Restrict);

            //migrationBuilder.AddForeignKey(
            //    name: "FK_UserAssignModule_UserAccessRights_UserAccessRightsId",
            //    table: "UserAssignModule",
            //    column: "UserAccessRightsId",
            //    principalTable: "UserAccessRights",
            //    principalColumn: "Id",
            //    onDelete: ReferentialAction.Restrict);

            //migrationBuilder.AddForeignKey(
            //    name: "FK_UserAssignSubModule_UserAssignModule_UserAssignModuleID",
            //    table: "UserAssignSubModule",
            //    column: "UserAssignModuleID",
            //    principalTable: "UserAssignModule",
            //    principalColumn: "ID",
            //    onDelete: ReferentialAction.Restrict);

            //migrationBuilder.AddForeignKey(
            //    name: "FK_UserAssignToolRights_UserAssignSubModule_UserAssignSubModuleID",
            //    table: "UserAssignToolRights",
            //    column: "UserAssignSubModuleID",
            //    principalTable: "UserAssignSubModule",
            //    principalColumn: "ID",
            //    onDelete: ReferentialAction.Restrict);
        }
    }
}
