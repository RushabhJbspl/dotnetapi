using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CleanArchitecture.Web.Migrations
{
    public partial class commmaster19122018_2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_CommissionTypeMaster",
                table: "CommissionTypeMaster");

            migrationBuilder.DropColumn(
                name: "CommissionTypeId",
                table: "CommissionTypeMaster");

            migrationBuilder.AlterColumn<long>(
                name: "Type",
                table: "StatasticsDetail",
                nullable: false,
                oldClrType: typeof(string));

            //migrationBuilder.AlterColumn<long>(
            //    name: "Id",
            //    table: "CommissionTypeMaster",
            //    nullable: false,
            //    oldClrType: typeof(long))
            //    .OldAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AddPrimaryKey(
                name: "PK_CommissionTypeMaster",
                table: "CommissionTypeMaster",
                column: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_CommissionTypeMaster",
                table: "CommissionTypeMaster");

            migrationBuilder.AlterColumn<string>(
                name: "Type",
                table: "StatasticsDetail",
                nullable: false,
                oldClrType: typeof(long));

            //migrationBuilder.AlterColumn<long>(
            //    name: "Id",
            //    table: "CommissionTypeMaster",
            //    nullable: false,
            //    oldClrType: typeof(long))
            //    .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AddColumn<long>(
                name: "CommissionTypeId",
                table: "CommissionTypeMaster",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddPrimaryKey(
                name: "PK_CommissionTypeMaster",
                table: "CommissionTypeMaster",
                column: "CommissionTypeId");
        }
    }
}
