using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CleanArchitecture.Web.Migrations
{
    public partial class walletnewchanges20112018 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_WTrnTypeMaster",
                table: "WTrnTypeMaster");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserTypeMaster",
                table: "UserTypeMaster");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CurrencyTypeMaster",
                table: "CurrencyTypeMaster");

            migrationBuilder.AddColumn<long>(
                name: "TrnTypeId",
                table: "WTrnTypeMaster",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "UserTypeId",
                table: "UserTypeMaster",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "CurrencyTypeId",
                table: "CurrencyTypeMaster",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddPrimaryKey(
                name: "PK_WTrnTypeMaster",
                table: "WTrnTypeMaster",
                column: "TrnTypeId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserTypeMaster",
                table: "UserTypeMaster",
                column: "UserTypeId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CurrencyTypeMaster",
                table: "CurrencyTypeMaster",
                column: "CurrencyTypeId");

            migrationBuilder.CreateTable(
                name: "ActivityTypeMaster",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<long>(nullable: false),
                    UpdatedBy = table.Column<long>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    Status = table.Column<short>(nullable: false),
                    TypeName = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActivityTypeMaster", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AllowedChannels",
                columns: table => new
                {
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<long>(nullable: false),
                    UpdatedBy = table.Column<long>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    Status = table.Column<short>(nullable: false),
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ChannelID = table.Column<long>(nullable: false),
                    ChannelName = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AllowedChannels", x => x.ChannelID);
                });

            migrationBuilder.CreateTable(
                name: "AuditActivityLog",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<long>(nullable: false),
                    UpdatedBy = table.Column<long>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    Status = table.Column<short>(nullable: false),
                    EntityType = table.Column<string>(nullable: false),
                    ColumnName = table.Column<string>(nullable: false),
                    OldValue = table.Column<string>(nullable: false),
                    NewValue = table.Column<string>(nullable: false),
                    Remarks = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditActivityLog", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AutorizedApps",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<long>(nullable: false),
                    UpdatedBy = table.Column<long>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    Status = table.Column<short>(nullable: false),
                    AppName = table.Column<string>(nullable: false),
                    SecretKey = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AutorizedApps", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ChargeTypeMaster",
                columns: table => new
                {
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<long>(nullable: false),
                    UpdatedBy = table.Column<long>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    Status = table.Column<short>(nullable: false),
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ChargeTypeId = table.Column<long>(nullable: false),
                    ChargeName = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChargeTypeMaster", x => x.ChargeTypeId);
                });

            migrationBuilder.CreateTable(
                name: "CommissionTypeMaster",
                columns: table => new
                {
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<long>(nullable: false),
                    UpdatedBy = table.Column<long>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    Status = table.Column<short>(nullable: false),
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CommissionTypeId = table.Column<long>(nullable: false),
                    TypeName = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommissionTypeMaster", x => x.CommissionTypeId);
                });

            migrationBuilder.CreateTable(
                name: "ServiceProvider",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<long>(nullable: false),
                    UpdatedBy = table.Column<long>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    Status = table.Column<short>(nullable: false),
                    ServiceProviderName = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceProvider", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TransactionBlockedChannel",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<long>(nullable: false),
                    UpdatedBy = table.Column<long>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    Status = table.Column<short>(nullable: false),
                    ChannelID = table.Column<long>(nullable: false),
                    ChannelName = table.Column<string>(nullable: false),
                    TrnType = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransactionBlockedChannel", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TransactionPolicy",
                columns: table => new
                {
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<long>(nullable: false),
                    UpdatedBy = table.Column<long>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    Status = table.Column<short>(nullable: false),
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    TrnType = table.Column<long>(nullable: false),
                    AllowedIP = table.Column<string>(nullable: false),
                    AllowedLocation = table.Column<string>(nullable: false),
                    AuthenticationType = table.Column<int>(nullable: false),
                    StartTime = table.Column<double>(nullable: true),
                    EndTime = table.Column<double>(nullable: true),
                    DailyTrnCount = table.Column<long>(nullable: false),
                    DailyTrnAmount = table.Column<decimal>(type: "decimal(18, 8)", nullable: false),
                    MonthlyTrnCount = table.Column<long>(nullable: false),
                    MonthlyTrnAmount = table.Column<decimal>(type: "decimal(18, 8)", nullable: false),
                    WeeklyTrnCount = table.Column<long>(nullable: false),
                    WeeklyTrnAmount = table.Column<decimal>(type: "decimal(18, 8)", nullable: false),
                    YearlyTrnCount = table.Column<long>(nullable: false),
                    YearlyTrnAmount = table.Column<decimal>(type: "decimal(18, 8)", nullable: false),
                    MinAmount = table.Column<decimal>(type: "decimal(18, 8)", nullable: false),
                    MaxAmount = table.Column<decimal>(type: "decimal(18, 8)", nullable: false),
                    AuthorityType = table.Column<short>(nullable: false),
                    AllowedUserType = table.Column<short>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransactionPolicy", x => x.TrnType);
                });

            migrationBuilder.CreateTable(
                name: "TransactionPolicyAllowedRole",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<long>(nullable: false),
                    UpdatedBy = table.Column<long>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    Status = table.Column<short>(nullable: false),
                    TrnPolicyID = table.Column<long>(nullable: false),
                    RoleID = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransactionPolicyAllowedRole", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserActivityLog",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<long>(nullable: false),
                    UpdatedBy = table.Column<long>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    Status = table.Column<short>(nullable: false),
                    ActivityType = table.Column<short>(nullable: false),
                    UserID = table.Column<long>(nullable: false),
                    WalletID = table.Column<long>(nullable: false),
                    TrnDate = table.Column<DateTime>(nullable: false),
                    TrnNo = table.Column<long>(nullable: false),
                    Remarks = table.Column<string>(nullable: true),
                    WalletTrnType = table.Column<short>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserActivityLog", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WalletPolicyAllowedDay",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<long>(nullable: false),
                    UpdatedBy = table.Column<long>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    Status = table.Column<short>(nullable: false),
                    WalletPolicyID = table.Column<long>(nullable: false),
                    DayNo = table.Column<short>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WalletPolicyAllowedDay", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WalletUsagePolicy",
                columns: table => new
                {
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<long>(nullable: false),
                    UpdatedBy = table.Column<long>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    Status = table.Column<short>(nullable: false),
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    WalletType = table.Column<long>(nullable: false),
                    AllowedIP = table.Column<string>(nullable: false),
                    AllowedLocation = table.Column<string>(nullable: false),
                    AuthenticationType = table.Column<int>(nullable: false),
                    StartTime = table.Column<double>(nullable: true),
                    EndTime = table.Column<double>(nullable: true),
                    HourlyTrnCount = table.Column<long>(nullable: false),
                    HourlyTrnAmount = table.Column<decimal>(type: "decimal(18, 8)", nullable: false),
                    DailyTrnCount = table.Column<long>(nullable: false),
                    DailyTrnAmount = table.Column<decimal>(type: "decimal(18, 8)", nullable: false),
                    MonthlyTrnCount = table.Column<long>(nullable: false),
                    MonthlyTrnAmount = table.Column<decimal>(type: "decimal(18, 8)", nullable: false),
                    WeeklyTrnCount = table.Column<long>(nullable: false),
                    WeeklyTrnAmount = table.Column<decimal>(type: "decimal(18, 8)", nullable: false),
                    YearlyTrnCount = table.Column<long>(nullable: false),
                    YearlyTrnAmount = table.Column<decimal>(type: "decimal(18, 8)", nullable: false),
                    LifeTimeTrnCount = table.Column<long>(nullable: false),
                    LifeTimeTrnAmount = table.Column<decimal>(type: "decimal(18, 8)", nullable: false),
                    MinAmount = table.Column<decimal>(type: "decimal(18, 8)", nullable: false),
                    MaxAmount = table.Column<decimal>(type: "decimal(18, 8)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WalletUsagePolicy", x => x.WalletType);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ActivityTypeMaster");

            migrationBuilder.DropTable(
                name: "AllowedChannels");

            migrationBuilder.DropTable(
                name: "AuditActivityLog");

            migrationBuilder.DropTable(
                name: "AutorizedApps");

            migrationBuilder.DropTable(
                name: "ChargeTypeMaster");

            migrationBuilder.DropTable(
                name: "CommissionTypeMaster");

            migrationBuilder.DropTable(
                name: "ServiceProvider");

            migrationBuilder.DropTable(
                name: "TransactionBlockedChannel");

            migrationBuilder.DropTable(
                name: "TransactionPolicy");

            migrationBuilder.DropTable(
                name: "TransactionPolicyAllowedRole");

            migrationBuilder.DropTable(
                name: "UserActivityLog");

            migrationBuilder.DropTable(
                name: "WalletPolicyAllowedDay");

            migrationBuilder.DropTable(
                name: "WalletUsagePolicy");

            migrationBuilder.DropPrimaryKey(
                name: "PK_WTrnTypeMaster",
                table: "WTrnTypeMaster");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserTypeMaster",
                table: "UserTypeMaster");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CurrencyTypeMaster",
                table: "CurrencyTypeMaster");

            migrationBuilder.DropColumn(
                name: "TrnTypeId",
                table: "WTrnTypeMaster");

            migrationBuilder.DropColumn(
                name: "UserTypeId",
                table: "UserTypeMaster");

            migrationBuilder.DropColumn(
                name: "CurrencyTypeId",
                table: "CurrencyTypeMaster");

            migrationBuilder.AddPrimaryKey(
                name: "PK_WTrnTypeMaster",
                table: "WTrnTypeMaster",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserTypeMaster",
                table: "UserTypeMaster",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CurrencyTypeMaster",
                table: "CurrencyTypeMaster",
                column: "Id");
        }
    }
}
