using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CleanArchitecture.Web.Migrations
{
    public partial class ModuleGroupAccess : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ModuleCRUDOptMaster");

            migrationBuilder.DropTable(
                name: "ModuleDomainMaster");

            migrationBuilder.DropTable(
                name: "ModuleUtilityMaster");

            //migrationBuilder.AlterColumn<long>(
            //    name: "Id",
            //    table: "ModuleTypeMaster",
            //    nullable: false,
            //    oldClrType: typeof(long))
            //    .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            migrationBuilder.CreateTable(
                name: "ModuleGroupAccess",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<long>(nullable: false),
                    UpdatedBy = table.Column<long>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    Status = table.Column<short>(nullable: false),
                    GroupID = table.Column<int>(nullable: false),
                    UtilityTypes = table.Column<string>(nullable: true),
                    CrudTypes = table.Column<string>(nullable: true),
                    SubModuleID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ModuleGroupAccess", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ModuleGroupMaster",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<long>(nullable: false),
                    UpdatedBy = table.Column<long>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    Status = table.Column<short>(nullable: false),
                    GroupName = table.Column<string>(maxLength: 50, nullable: false),
                    GroupDescription = table.Column<string>(maxLength: 100, nullable: false),
                    RoleID = table.Column<int>(nullable: false),
                    ModuleDomainID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ModuleGroupMaster", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SubModuleFormMaster",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<long>(nullable: false),
                    UpdatedBy = table.Column<long>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    Status = table.Column<short>(nullable: false),
                    ModuleGroupAccessID = table.Column<int>(nullable: false),
                    CrudTypes = table.Column<string>(nullable: true),
                    FieldID = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubModuleFormMaster", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ModuleGroupAccess");

            migrationBuilder.DropTable(
                name: "ModuleGroupMaster");

            migrationBuilder.DropTable(
                name: "SubModuleFormMaster");

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
