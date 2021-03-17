using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CleanArchitecture.Web.Migrations
{
    public partial class ModuleTypeMasterAddColSubModuleMaster : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "GUID",
                table: "SubModuleMaster",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "ParentGUID",
                table: "SubModuleMaster",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<short>(
                name: "Type",
                table: "SubModuleMaster",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.CreateTable(
                name: "ModuleTypeMaster",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<long>(nullable: false),
                    UpdatedBy = table.Column<long>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    Status = table.Column<short>(nullable: false),
                    TypeName = table.Column<string>(maxLength: 20, nullable: true),
                    TypeValue = table.Column<string>(maxLength: 20, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ModuleTypeMaster", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ModuleTypeMaster");

            migrationBuilder.DropColumn(
                name: "GUID",
                table: "SubModuleMaster");

            migrationBuilder.DropColumn(
                name: "ParentGUID",
                table: "SubModuleMaster");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "SubModuleMaster");
        }
    }
}
