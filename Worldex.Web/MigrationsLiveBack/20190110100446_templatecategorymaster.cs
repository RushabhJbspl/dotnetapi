using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CleanArchitecture.Web.Migrations
{
    public partial class templatecategorymaster : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsOnOff",
                table: "TemplateMaster");

            migrationBuilder.DropColumn(
                name: "ParameterInfo",
                table: "TemplateMaster");

            migrationBuilder.CreateTable(
                name: "TemplateCategoryMaster",
                columns: table => new
                {
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<long>(nullable: false),
                    UpdatedBy = table.Column<long>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    Status = table.Column<short>(nullable: false),
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    IsOnOff = table.Column<short>(nullable: false),
                    ParameterInfo = table.Column<string>(maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TemplateCategoryMaster", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TemplateCategoryMaster");

            migrationBuilder.AddColumn<short>(
                name: "IsOnOff",
                table: "TemplateMaster",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddColumn<string>(
                name: "ParameterInfo",
                table: "TemplateMaster",
                maxLength: 200,
                nullable: true);
        }
    }
}
