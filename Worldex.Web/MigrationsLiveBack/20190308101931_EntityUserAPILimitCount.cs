using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CleanArchitecture.Web.Migrations
{
    public partial class EntityUserAPILimitCount : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserAPILimitCount",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<long>(nullable: false),
                    UpdatedBy = table.Column<long>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    Status = table.Column<short>(nullable: false),
                    PerDayCount = table.Column<long>(nullable: false),
                    PerDayUpdatedDate = table.Column<DateTime>(nullable: false),
                    PerMonthCount = table.Column<long>(nullable: false),
                    PerMonthUpdatedDate = table.Column<DateTime>(nullable: false),
                    TotalCall = table.Column<long>(nullable: false),
                    UserID = table.Column<long>(nullable: false),
                    PlanID = table.Column<long>(nullable: false),
                    SubscribeID = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserAPILimitCount", x => x.Id);
                });

            //migrationBuilder.CreateIndex(
            //    name: "IX_UserAssignToolRights_UserAssignSubModuleID",
            //    table: "UserAssignToolRights",
            //    column: "UserAssignSubModuleID");

            //migrationBuilder.CreateIndex(
            //    name: "IX_UserAssignFieldRights_UserAssignSubModuleID",
            //    table: "UserAssignFieldRights",
            //    column: "UserAssignSubModuleID");

            //migrationBuilder.AddForeignKey(
            //    name: "FK_UserAssignFieldRights_UserAssignSubModule_UserAssignSubModuleID",
            //    table: "UserAssignFieldRights",
            //    column: "UserAssignSubModuleID",
            //    principalTable: "UserAssignSubModule",
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
            //    name: "FK_UserAssignToolRights_UserAssignSubModule_UserAssignSubModuleID",
            //    table: "UserAssignToolRights");

            migrationBuilder.DropTable(
                name: "UserAPILimitCount");

            //migrationBuilder.DropIndex(
            //    name: "IX_UserAssignToolRights_UserAssignSubModuleID",
            //    table: "UserAssignToolRights");

            //migrationBuilder.DropIndex(
            //    name: "IX_UserAssignFieldRights_UserAssignSubModuleID",
            //    table: "UserAssignFieldRights");
        }
    }
}
