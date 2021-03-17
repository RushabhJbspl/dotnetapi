using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CleanArchitecture.Web.Migrations
{
    public partial class FeedExchange : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "ApprovedDate",
                table: "MarginWalletTopupRequest",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<long>(
                name: "ApprovedBy",
                table: "MarginWalletTopupRequest",
                nullable: true,
                oldClrType: typeof(long));

            migrationBuilder.CreateTable(
                name: "FeedLimitCounts",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<long>(nullable: false),
                    UpdatedBy = table.Column<long>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    Status = table.Column<short>(nullable: false),
                    MethodID = table.Column<long>(nullable: false),
                    LimitCount = table.Column<long>(nullable: false),
                    UserID = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FeedLimitCounts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SocketFeedConfiguration",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<long>(nullable: false),
                    UpdatedBy = table.Column<long>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    Status = table.Column<short>(nullable: false),
                    MethodID = table.Column<long>(nullable: false),
                    FeedLimitID = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SocketFeedConfiguration", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SocketFeedLimits",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<long>(nullable: false),
                    UpdatedBy = table.Column<long>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    Status = table.Column<short>(nullable: false),
                    MaxSize = table.Column<long>(nullable: false),
                    MinSize = table.Column<long>(nullable: false),
                    RowLenghtSize = table.Column<long>(nullable: false),
                    MaxRowCount = table.Column<long>(nullable: false),
                    MaxRecordCount = table.Column<long>(nullable: false),
                    MinRecordCount = table.Column<long>(nullable: false),
                    MaxLimit = table.Column<long>(nullable: false),
                    MinLimit = table.Column<long>(nullable: false),
                    LimitType = table.Column<short>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SocketFeedLimits", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SocketMethods",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<long>(nullable: false),
                    UpdatedBy = table.Column<long>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    Status = table.Column<short>(nullable: false),
                    MethodName = table.Column<string>(maxLength: 30, nullable: false),
                    ReturnMethodName = table.Column<string>(maxLength: 30, nullable: false),
                    PublicOrPrivate = table.Column<short>(nullable: false),
                    EnumCode = table.Column<short>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SocketMethods", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FeedLimitCounts");

            migrationBuilder.DropTable(
                name: "SocketFeedConfiguration");

            migrationBuilder.DropTable(
                name: "SocketFeedLimits");

            migrationBuilder.DropTable(
                name: "SocketMethods");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ApprovedDate",
                table: "MarginWalletTopupRequest",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "ApprovedBy",
                table: "MarginWalletTopupRequest",
                nullable: false,
                oldClrType: typeof(long),
                oldNullable: true);
        }
    }
}
