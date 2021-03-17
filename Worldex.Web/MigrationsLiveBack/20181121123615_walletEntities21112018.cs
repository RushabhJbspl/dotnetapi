using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CleanArchitecture.Web.Migrations
{
    public partial class walletEntities21112018 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserMaster_BizUserPhotos_ProfilePhotoId",
                table: "UserMaster");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserMaster",
                table: "UserMaster");

            migrationBuilder.DropIndex(
                name: "IX_UserMaster_ProfilePhotoId",
                table: "UserMaster");

            migrationBuilder.DropColumn(
                name: "AccessFailedCount",
                table: "UserMaster");

            migrationBuilder.DropColumn(
                name: "ConcurrencyStamp",
                table: "UserMaster");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "UserMaster");

            migrationBuilder.DropColumn(
                name: "EmailConfirmed",
                table: "UserMaster");

            migrationBuilder.DropColumn(
                name: "FirstName",
                table: "UserMaster");

            migrationBuilder.DropColumn(
                name: "IsEnabled",
                table: "UserMaster");

            migrationBuilder.DropColumn(
                name: "LastName",
                table: "UserMaster");

            migrationBuilder.DropColumn(
                name: "LockoutEnabled",
                table: "UserMaster");

            migrationBuilder.DropColumn(
                name: "LockoutEnd",
                table: "UserMaster");

            migrationBuilder.DropColumn(
                name: "Mobile",
                table: "UserMaster");

            migrationBuilder.DropColumn(
                name: "NormalizedEmail",
                table: "UserMaster");

            migrationBuilder.DropColumn(
                name: "NormalizedUserName",
                table: "UserMaster");

            migrationBuilder.DropColumn(
                name: "PasswordHash",
                table: "UserMaster");

            migrationBuilder.DropColumn(
                name: "PhoneNumber",
                table: "UserMaster");

            migrationBuilder.DropColumn(
                name: "PhoneNumberConfirmed",
                table: "UserMaster");

            migrationBuilder.DropColumn(
                name: "ProfilePhotoId",
                table: "UserMaster");

            migrationBuilder.DropColumn(
                name: "SecurityStamp",
                table: "UserMaster");

            migrationBuilder.DropColumn(
                name: "TwoFactorEnabled",
                table: "UserMaster");

            migrationBuilder.DropColumn(
                name: "UserName",
                table: "UserMaster");

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "UserMaster",
                nullable: false,
                oldClrType: typeof(int))
                .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn)
                .OldAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AddColumn<long>(
                name: "BizUserID",
                table: "UserMaster",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "CreatedBy",
                table: "UserMaster",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<short>(
                name: "Status",
                table: "UserMaster",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddColumn<long>(
                name: "UpdatedBy",
                table: "UserMaster",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedDate",
                table: "UserMaster",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserMaster",
                table: "UserMaster",
                column: "BizUserID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_UserMaster",
                table: "UserMaster");

            migrationBuilder.DropColumn(
                name: "BizUserID",
                table: "UserMaster");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "UserMaster");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "UserMaster");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "UserMaster");

            migrationBuilder.DropColumn(
                name: "UpdatedDate",
                table: "UserMaster");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "UserMaster",
                nullable: false,
                oldClrType: typeof(long))
                .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn)
                .OldAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AddColumn<int>(
                name: "AccessFailedCount",
                table: "UserMaster",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "ConcurrencyStamp",
                table: "UserMaster",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "UserMaster",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "EmailConfirmed",
                table: "UserMaster",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                table: "UserMaster",
                maxLength: 250,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsEnabled",
                table: "UserMaster",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "LastName",
                table: "UserMaster",
                maxLength: 250,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "LockoutEnabled",
                table: "UserMaster",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "LockoutEnd",
                table: "UserMaster",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Mobile",
                table: "UserMaster",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NormalizedEmail",
                table: "UserMaster",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NormalizedUserName",
                table: "UserMaster",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PasswordHash",
                table: "UserMaster",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PhoneNumber",
                table: "UserMaster",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "PhoneNumberConfirmed",
                table: "UserMaster",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "ProfilePhotoId",
                table: "UserMaster",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SecurityStamp",
                table: "UserMaster",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "TwoFactorEnabled",
                table: "UserMaster",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "UserName",
                table: "UserMaster",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserMaster",
                table: "UserMaster",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_UserMaster_ProfilePhotoId",
                table: "UserMaster",
                column: "ProfilePhotoId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserMaster_BizUserPhotos_ProfilePhotoId",
                table: "UserMaster",
                column: "ProfilePhotoId",
                principalTable: "BizUserPhotos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
