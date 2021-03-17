using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CleanArchitecture.Web.Migrations
{
    public partial class ModuleGroupAccessControl : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ModuleCRUDOptMaster");

            migrationBuilder.DropTable(
                name: "ModuleDomainMaster");

            migrationBuilder.DropTable(
                name: "ModuleUtilityMaster");

            migrationBuilder.DropColumn(
                name: "ModuleDomainType",
                table: "SubModuleMaster");

            migrationBuilder.AddColumn<long>(
                name: "UserID",
                table: "UserAccessRights",
                nullable: false,
                defaultValue: 0L);

            //migrationBuilder.AlterColumn<long>(
            //    name: "Id",
            //    table: "ModuleTypeMaster",
            //    nullable: false,
            //    oldClrType: typeof(long))
            //    .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AddColumn<int>(
                name: "GroupID",
                table: "BizUser",
                nullable: false,
                defaultValue: 2);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserID",
                table: "UserAccessRights");

            migrationBuilder.DropColumn(
                name: "GroupID",
                table: "BizUser");

            migrationBuilder.AddColumn<short>(
                name: "ModuleDomainType",
                table: "SubModuleMaster",
                nullable: false,
                defaultValue: (short)0);

            //migrationBuilder.AlterColumn<long>(
            //    name: "Id",
            //    table: "ModuleTypeMaster",
            //    nullable: false,
            //    oldClrType: typeof(long))
            //    .OldAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            migrationBuilder.CreateTable(
                name: "ModuleCRUDOptMaster",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false),
                    CreatedBy = table.Column<long>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    OptName = table.Column<string>(maxLength: 20, nullable: true),
                    OptValue = table.Column<string>(maxLength: 20, nullable: true),
                    Status = table.Column<short>(nullable: false),
                    UpdatedBy = table.Column<long>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ModuleCRUDOptMaster", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ModuleDomainMaster",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedBy = table.Column<long>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    DomainName = table.Column<string>(maxLength: 20, nullable: true),
                    Status = table.Column<short>(nullable: false),
                    UpdatedBy = table.Column<long>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ModuleDomainMaster", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ModuleUtilityMaster",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false),
                    CreatedBy = table.Column<long>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    Status = table.Column<short>(nullable: false),
                    UpdatedBy = table.Column<long>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    UtilityName = table.Column<string>(maxLength: 20, nullable: true),
                    UtilityValue = table.Column<string>(maxLength: 20, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ModuleUtilityMaster", x => x.Id);
                });
        }
    }
}
