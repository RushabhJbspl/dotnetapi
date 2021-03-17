using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CleanArchitecture.Web.Migrations
{
    public partial class VelocityruleRemovePrimarykey : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropForeignKey(
            //    name: "FK_UserAssignFieldRights_UserAssignSubModule_UserAssignSubModuleSubModuleID",
            //    table: "UserAssignFieldRights");

            //migrationBuilder.DropForeignKey(
            //    name: "FK_UserAssignSubModule_UserAssignModule_UserAssignModuleModuleID",
            //    table: "UserAssignSubModule");

            //migrationBuilder.DropForeignKey(
            //    name: "FK_UserAssignToolRights_UserAssignSubModule_UserAssignSubModuleSubModuleID",
            //    table: "UserAssignToolRights");

            //migrationBuilder.DropTable(
            //    name: "LimitRuleMaster");

            //migrationBuilder.DropTable(
            //    name: "WalletLimitConfigurationMaster");

            //migrationBuilder.DropPrimaryKey(
            //    name: "PK_UserAssignToolRights",
            //    table: "UserAssignToolRights");

            //migrationBuilder.DropPrimaryKey(
            //    name: "PK_UserAssignSubModule",
            //    table: "UserAssignSubModule");

            //migrationBuilder.DropPrimaryKey(
            //    name: "PK_UserAssignModule",
            //    table: "UserAssignModule");

            //migrationBuilder.DropPrimaryKey(
            //    name: "PK_UserAssignFieldRights",
            //    table: "UserAssignFieldRights");

            //migrationBuilder.RenameColumn(
            //    name: "UserAssignSubModuleSubModuleID",
            //    table: "UserAssignToolRights",
            //    newName: "UserAssignSubModuleID");

            //migrationBuilder.RenameIndex(
            //    name: "IX_UserAssignToolRights_UserAssignSubModuleSubModuleID",
            //    table: "UserAssignToolRights",
            //    newName: "IX_UserAssignToolRights_UserAssignSubModuleID");

            //migrationBuilder.RenameColumn(
            //    name: "UserAssignModuleModuleID",
            //    table: "UserAssignSubModule",
            //    newName: "UserAssignModuleID");

            //migrationBuilder.RenameIndex(
            //    name: "IX_UserAssignSubModule_UserAssignModuleModuleID",
            //    table: "UserAssignSubModule",
            //    newName: "IX_UserAssignSubModule_UserAssignModuleID");

            //migrationBuilder.RenameColumn(
            //    name: "UserAssignSubModuleSubModuleID",
            //    table: "UserAssignFieldRights",
            //    newName: "UserAssignSubModuleID");

            //migrationBuilder.RenameIndex(
            //    name: "IX_UserAssignFieldRights_UserAssignSubModuleSubModuleID",
            //    table: "UserAssignFieldRights",
            //    newName: "IX_UserAssignFieldRights_UserAssignSubModuleID");

            //migrationBuilder.AlterColumn<long>(
            //    name: "ToolID",
            //    table: "UserAssignToolRights",
            //    nullable: false,
            //    oldClrType: typeof(long))
            //    .OldAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            //migrationBuilder.AddColumn<long>(
            //    name: "ID",
            //    table: "UserAssignToolRights",
            //    nullable: false,
            //    defaultValue: 0L)
            //    .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            //migrationBuilder.AlterColumn<long>(
            //    name: "SubModuleID",
            //    table: "UserAssignSubModule",
            //    nullable: false,
            //    oldClrType: typeof(long))
            //    .OldAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            //migrationBuilder.AddColumn<long>(
            //    name: "ID",
            //    table: "UserAssignSubModule",
            //    nullable: false,
            //    defaultValue: 0L)
            //    .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            //migrationBuilder.AlterColumn<long>(
            //    name: "ModuleID",
            //    table: "UserAssignModule",
            //    nullable: false,
            //    oldClrType: typeof(long))
            //    .OldAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            //migrationBuilder.AddColumn<long>(
            //    name: "ID",
            //    table: "UserAssignModule",
            //    nullable: false,
            //    defaultValue: 0L)
            //    .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            //migrationBuilder.AlterColumn<long>(
            //    name: "FieldID",
            //    table: "UserAssignFieldRights",
            //    nullable: false,
            //    oldClrType: typeof(long))
            //    .OldAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            //migrationBuilder.AddColumn<long>(
            //    name: "ID",
            //    table: "UserAssignFieldRights",
            //    nullable: false,
            //    defaultValue: 0L)
            //    .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            //migrationBuilder.AddPrimaryKey(
            //    name: "PK_UserAssignToolRights",
            //    table: "UserAssignToolRights",
            //    column: "ID");

            //migrationBuilder.AddPrimaryKey(
            //    name: "PK_UserAssignSubModule",
            //    table: "UserAssignSubModule",
            //    column: "ID");

            //migrationBuilder.AddPrimaryKey(
            //    name: "PK_UserAssignModule",
            //    table: "UserAssignModule",
            //    column: "ID");

            //migrationBuilder.AddPrimaryKey(
            //    name: "PK_UserAssignFieldRights",
            //    table: "UserAssignFieldRights",
            //    column: "ID");

            //migrationBuilder.AddForeignKey(
            //    name: "FK_UserAssignFieldRights_UserAssignSubModule_UserAssignSubModuleID",
            //    table: "UserAssignFieldRights",
            //    column: "UserAssignSubModuleID",
            //    principalTable: "UserAssignSubModule",
            //    principalColumn: "ID",
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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserAssignFieldRights_UserAssignSubModule_UserAssignSubModuleID",
                table: "UserAssignFieldRights");

            migrationBuilder.DropForeignKey(
                name: "FK_UserAssignSubModule_UserAssignModule_UserAssignModuleID",
                table: "UserAssignSubModule");

            migrationBuilder.DropForeignKey(
                name: "FK_UserAssignToolRights_UserAssignSubModule_UserAssignSubModuleID",
                table: "UserAssignToolRights");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserAssignToolRights",
                table: "UserAssignToolRights");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserAssignSubModule",
                table: "UserAssignSubModule");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserAssignModule",
                table: "UserAssignModule");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserAssignFieldRights",
                table: "UserAssignFieldRights");

            migrationBuilder.DropColumn(
                name: "ID",
                table: "UserAssignToolRights");

            migrationBuilder.DropColumn(
                name: "ID",
                table: "UserAssignSubModule");

            migrationBuilder.DropColumn(
                name: "ID",
                table: "UserAssignModule");

            migrationBuilder.DropColumn(
                name: "ID",
                table: "UserAssignFieldRights");

            migrationBuilder.RenameColumn(
                name: "UserAssignSubModuleID",
                table: "UserAssignToolRights",
                newName: "UserAssignSubModuleSubModuleID");

            migrationBuilder.RenameIndex(
                name: "IX_UserAssignToolRights_UserAssignSubModuleID",
                table: "UserAssignToolRights",
                newName: "IX_UserAssignToolRights_UserAssignSubModuleSubModuleID");

            migrationBuilder.RenameColumn(
                name: "UserAssignModuleID",
                table: "UserAssignSubModule",
                newName: "UserAssignModuleModuleID");

            migrationBuilder.RenameIndex(
                name: "IX_UserAssignSubModule_UserAssignModuleID",
                table: "UserAssignSubModule",
                newName: "IX_UserAssignSubModule_UserAssignModuleModuleID");

            migrationBuilder.RenameColumn(
                name: "UserAssignSubModuleID",
                table: "UserAssignFieldRights",
                newName: "UserAssignSubModuleSubModuleID");

            migrationBuilder.RenameIndex(
                name: "IX_UserAssignFieldRights_UserAssignSubModuleID",
                table: "UserAssignFieldRights",
                newName: "IX_UserAssignFieldRights_UserAssignSubModuleSubModuleID");

            migrationBuilder.AlterColumn<long>(
                name: "ToolID",
                table: "UserAssignToolRights",
                nullable: false,
                oldClrType: typeof(long))
                .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "SubModuleID",
                table: "UserAssignSubModule",
                nullable: false,
                oldClrType: typeof(long))
                .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "ModuleID",
                table: "UserAssignModule",
                nullable: false,
                oldClrType: typeof(long))
                .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<long>(
                name: "FieldID",
                table: "UserAssignFieldRights",
                nullable: false,
                oldClrType: typeof(long))
                .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserAssignToolRights",
                table: "UserAssignToolRights",
                column: "ToolID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserAssignSubModule",
                table: "UserAssignSubModule",
                column: "SubModuleID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserAssignModule",
                table: "UserAssignModule",
                column: "ModuleID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserAssignFieldRights",
                table: "UserAssignFieldRights",
                column: "FieldID");

            migrationBuilder.CreateTable(
                name: "LimitRuleMaster",
                columns: table => new
                {
                    TrnType = table.Column<int>(nullable: false),
                    WalletType = table.Column<long>(nullable: false),
                    CreatedBy = table.Column<long>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    MaxAmount = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    MinAmount = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Status = table.Column<short>(nullable: false),
                    UpdatedBy = table.Column<long>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LimitRuleMaster", x => new { x.TrnType, x.WalletType });
                });

            migrationBuilder.CreateTable(
                name: "WalletLimitConfigurationMaster",
                columns: table => new
                {
                    TrnType = table.Column<int>(nullable: false),
                    CreatedBy = table.Column<long>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    EndTimeUnix = table.Column<double>(type: "float", nullable: false),
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    LifeTime = table.Column<decimal>(nullable: true),
                    LimitPerDay = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    LimitPerHour = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    LimitPerTransaction = table.Column<decimal>(type: "decimal(28, 18)", nullable: false),
                    StartTimeUnix = table.Column<double>(type: "float", nullable: false),
                    Status = table.Column<short>(nullable: false),
                    UpdatedBy = table.Column<long>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WalletLimitConfigurationMaster", x => x.TrnType);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_UserAssignFieldRights_UserAssignSubModule_UserAssignSubModuleSubModuleID",
                table: "UserAssignFieldRights",
                column: "UserAssignSubModuleSubModuleID",
                principalTable: "UserAssignSubModule",
                principalColumn: "SubModuleID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserAssignSubModule_UserAssignModule_UserAssignModuleModuleID",
                table: "UserAssignSubModule",
                column: "UserAssignModuleModuleID",
                principalTable: "UserAssignModule",
                principalColumn: "ModuleID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserAssignToolRights_UserAssignSubModule_UserAssignSubModuleSubModuleID",
                table: "UserAssignToolRights",
                column: "UserAssignSubModuleSubModuleID",
                principalTable: "UserAssignSubModule",
                principalColumn: "SubModuleID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
