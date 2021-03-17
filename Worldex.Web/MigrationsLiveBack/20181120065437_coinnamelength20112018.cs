using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CleanArchitecture.Web.Migrations
{
    public partial class coinnamelength20112018 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "WalletType",
                table: "WalletTransactionQueues",
                maxLength: 10,
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 5);

            migrationBuilder.AlterColumn<string>(
                name: "WalletType",
                table: "WalletTransactionOrders",
                maxLength: 10,
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 5);

            migrationBuilder.CreateTable(
                name: "BlockWalletTrnTypeMaster",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<long>(nullable: false),
                    UpdatedBy = table.Column<long>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    Status = table.Column<short>(nullable: false),
                    WalletTypeID = table.Column<long>(nullable: false),
                    TrnTypeID = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BlockWalletTrnTypeMaster", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ChargePolicy",
                columns: table => new
                {
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<long>(nullable: false),
                    UpdatedBy = table.Column<long>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    Status = table.Column<short>(nullable: false),
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    PolicyName = table.Column<string>(maxLength: 50, nullable: false),
                    WalletTrnType = table.Column<long>(nullable: false),
                    MinAmount = table.Column<decimal>(type: "decimal(18, 8)", nullable: false),
                    MaxAmount = table.Column<decimal>(type: "decimal(18, 8)", nullable: false),
                    WalletType = table.Column<long>(nullable: false),
                    Type = table.Column<long>(nullable: false),
                    ChargeType = table.Column<long>(nullable: false),
                    ChargeValue = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChargePolicy", x => new { x.WalletTrnType, x.WalletType });
                });

            migrationBuilder.CreateTable(
                name: "CommissionPolicy",
                columns: table => new
                {
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<long>(nullable: false),
                    UpdatedBy = table.Column<long>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    Status = table.Column<short>(nullable: false),
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    PolicyName = table.Column<string>(maxLength: 50, nullable: false),
                    WalletTrnType = table.Column<long>(nullable: false),
                    MinAmount = table.Column<decimal>(type: "decimal(18, 8)", nullable: false),
                    MaxAmount = table.Column<decimal>(type: "decimal(18, 8)", nullable: false),
                    WalletType = table.Column<long>(nullable: false),
                    Type = table.Column<long>(nullable: false),
                    CommissionType = table.Column<long>(nullable: false),
                    CommissionValue = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommissionPolicy", x => new { x.WalletTrnType, x.WalletType });
                });

            migrationBuilder.CreateTable(
                name: "CurrencyTypeMaster",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<long>(nullable: false),
                    UpdatedBy = table.Column<long>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    Status = table.Column<short>(nullable: false),
                    CurrencyTypeName = table.Column<string>(maxLength: 7, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CurrencyTypeMaster", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OrganizationMaster",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<long>(nullable: false),
                    UpdatedBy = table.Column<long>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    Status = table.Column<short>(nullable: false),
                    OrganizationName = table.Column<string>(maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganizationMaster", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OrganizationUserMaster",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<long>(nullable: false),
                    UpdatedBy = table.Column<long>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    Status = table.Column<short>(nullable: false),
                    OrganizationID = table.Column<long>(nullable: false),
                    UserID = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganizationUserMaster", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserMaster",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    UserName = table.Column<string>(nullable: true),
                    NormalizedUserName = table.Column<string>(nullable: true),
                    Email = table.Column<string>(nullable: true),
                    NormalizedEmail = table.Column<string>(nullable: true),
                    EmailConfirmed = table.Column<bool>(nullable: false),
                    PasswordHash = table.Column<string>(nullable: true),
                    SecurityStamp = table.Column<string>(nullable: true),
                    ConcurrencyStamp = table.Column<string>(nullable: true),
                    PhoneNumber = table.Column<string>(nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(nullable: false),
                    TwoFactorEnabled = table.Column<bool>(nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(nullable: true),
                    LockoutEnabled = table.Column<bool>(nullable: false),
                    AccessFailedCount = table.Column<int>(nullable: false),
                    IsEnabled = table.Column<bool>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    FirstName = table.Column<string>(maxLength: 250, nullable: true),
                    LastName = table.Column<string>(maxLength: 250, nullable: true),
                    Mobile = table.Column<string>(nullable: true),
                    ProfilePhotoId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserMaster", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserMaster_BizUserPhotos_ProfilePhotoId",
                        column: x => x.ProfilePhotoId,
                        principalTable: "BizUserPhotos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserRoleMaster",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<long>(nullable: false),
                    UpdatedBy = table.Column<long>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    Status = table.Column<short>(nullable: false),
                    RoleName = table.Column<string>(maxLength: 20, nullable: false),
                    RoleType = table.Column<string>(maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRoleMaster", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserTypeMaster",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<long>(nullable: false),
                    UpdatedBy = table.Column<long>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    Status = table.Column<short>(nullable: false),
                    UserType = table.Column<string>(maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserTypeMaster", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserWalletBlockTrnTypeMaster",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<long>(nullable: false),
                    UpdatedBy = table.Column<long>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    Status = table.Column<short>(nullable: false),
                    WalletID = table.Column<long>(nullable: false),
                    WTrnTypeMasterID = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserWalletBlockTrnTypeMaster", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserWalletMaster",
                columns: table => new
                {
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<long>(nullable: false),
                    UpdatedBy = table.Column<long>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    Status = table.Column<short>(nullable: false),
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    WalletName = table.Column<string>(maxLength: 50, nullable: false),
                    Balance = table.Column<decimal>(type: "decimal(18, 8)", nullable: false),
                    WalletTypeID = table.Column<long>(nullable: false),
                    IsValid = table.Column<bool>(nullable: false),
                    AccWalletID = table.Column<string>(maxLength: 16, nullable: false),
                    UserID = table.Column<long>(nullable: false),
                    PublicAddress = table.Column<string>(maxLength: 50, nullable: false),
                    IsDefaultWallet = table.Column<byte>(nullable: false),
                    OrganizationID = table.Column<long>(nullable: false),
                    ExpiryDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserWalletMaster", x => x.AccWalletID);
                });

            migrationBuilder.CreateTable(
                name: "Wallet_TypeMaster",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<long>(nullable: false),
                    UpdatedBy = table.Column<long>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    Status = table.Column<short>(nullable: false),
                    WalletTypeName = table.Column<string>(maxLength: 7, nullable: false),
                    Discription = table.Column<string>(maxLength: 100, nullable: false),
                    CurrencyTypeID = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Wallet_TypeMaster", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WalletAuthorizeUserMaster",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<long>(nullable: false),
                    UpdatedBy = table.Column<long>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    Status = table.Column<short>(nullable: false),
                    WalletID = table.Column<long>(nullable: false),
                    UserID = table.Column<long>(nullable: false),
                    OrgID = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WalletAuthorizeUserMaster", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WTrnTypeMaster",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<long>(nullable: false),
                    UpdatedBy = table.Column<long>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    Status = table.Column<short>(nullable: false),
                    TrnTypeName = table.Column<string>(maxLength: 7, nullable: false),
                    Discription = table.Column<string>(maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WTrnTypeMaster", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserMaster_ProfilePhotoId",
                table: "UserMaster",
                column: "ProfilePhotoId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BlockWalletTrnTypeMaster");

            migrationBuilder.DropTable(
                name: "ChargePolicy");

            migrationBuilder.DropTable(
                name: "CommissionPolicy");

            migrationBuilder.DropTable(
                name: "CurrencyTypeMaster");

            migrationBuilder.DropTable(
                name: "OrganizationMaster");

            migrationBuilder.DropTable(
                name: "OrganizationUserMaster");

            migrationBuilder.DropTable(
                name: "UserMaster");

            migrationBuilder.DropTable(
                name: "UserRoleMaster");

            migrationBuilder.DropTable(
                name: "UserTypeMaster");

            migrationBuilder.DropTable(
                name: "UserWalletBlockTrnTypeMaster");

            migrationBuilder.DropTable(
                name: "UserWalletMaster");

            migrationBuilder.DropTable(
                name: "Wallet_TypeMaster");

            migrationBuilder.DropTable(
                name: "WalletAuthorizeUserMaster");

            migrationBuilder.DropTable(
                name: "WTrnTypeMaster");

            migrationBuilder.AlterColumn<string>(
                name: "WalletType",
                table: "WalletTransactionQueues",
                maxLength: 5,
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 10);

            migrationBuilder.AlterColumn<string>(
                name: "WalletType",
                table: "WalletTransactionOrders",
                maxLength: 5,
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 10);
        }
    }
}
