using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CleanArchitecture.Web.Migrations
{
    public partial class newmigration15oct2018 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropColumn(
            //    name: "Balance",
            //    table: "BizUser");

            //migrationBuilder.DropColumn(
            //    name: "OTP",
            //    table: "BizUser");

            //migrationBuilder.CreateTable(
            //    name: "AddressMasters",
            //    columns: table => new
            //    {
            //        Id = table.Column<long>(nullable: false)
            //            .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
            //        CreatedDate = table.Column<DateTime>(nullable: false),
            //        CreatedBy = table.Column<long>(nullable: false),
            //        UpdatedBy = table.Column<long>(nullable: true),
            //        UpdatedDate = table.Column<DateTime>(nullable: false),
            //        Status = table.Column<short>(nullable: false),
            //        WalletId = table.Column<long>(nullable: false),
            //        Address = table.Column<string>(maxLength: 50, nullable: false),
            //        IsDefaultAddress = table.Column<byte>(nullable: false),
            //        SerProID = table.Column<long>(nullable: false),
            //        AddressLable = table.Column<string>(maxLength: 50, nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_AddressMasters", x => x.Id);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "AppType",
            //    columns: table => new
            //    {
            //        Id = table.Column<long>(nullable: false)
            //            .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
            //        CreatedDate = table.Column<DateTime>(nullable: false),
            //        CreatedBy = table.Column<long>(nullable: false),
            //        UpdatedBy = table.Column<long>(nullable: true),
            //        UpdatedDate = table.Column<DateTime>(nullable: false),
            //        Status = table.Column<short>(nullable: false),
            //        AppTypeName = table.Column<string>(maxLength: 20, nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_AppType", x => x.Id);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "CommAPIServiceMaster",
            //    columns: table => new
            //    {
            //        Id = table.Column<long>(nullable: false)
            //            .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
            //        CreatedDate = table.Column<DateTime>(nullable: false),
            //        CreatedBy = table.Column<long>(nullable: false),
            //        UpdatedBy = table.Column<long>(nullable: true),
            //        UpdatedDate = table.Column<DateTime>(nullable: false),
            //        Status = table.Column<short>(nullable: false),
            //        APID = table.Column<long>(nullable: false),
            //        CommServiceID = table.Column<long>(nullable: false),
            //        SenderID = table.Column<string>(maxLength: 60, nullable: false),
            //        SMSSendURL = table.Column<string>(maxLength: 200, nullable: false),
            //        SMSBalURL = table.Column<string>(maxLength: 200, nullable: false),
            //        Priority = table.Column<int>(nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_CommAPIServiceMaster", x => x.Id);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "CommServiceMaster",
            //    columns: table => new
            //    {
            //        Id = table.Column<long>(nullable: false)
            //            .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
            //        CreatedDate = table.Column<DateTime>(nullable: false),
            //        CreatedBy = table.Column<long>(nullable: false),
            //        UpdatedBy = table.Column<long>(nullable: true),
            //        UpdatedDate = table.Column<DateTime>(nullable: false),
            //        Status = table.Column<short>(nullable: false),
            //        CommServiceID = table.Column<long>(nullable: false),
            //        RequestID = table.Column<long>(nullable: false),
            //        CommSerproID = table.Column<long>(nullable: false),
            //        ServiceName = table.Column<string>(maxLength: 60, nullable: false),
            //        ResponseSuccess = table.Column<string>(nullable: true),
            //        ResponseFailure = table.Column<string>(nullable: true),
            //        ParsingDataID = table.Column<long>(nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_CommServiceMaster", x => x.Id);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "CommServiceproviderMaster",
            //    columns: table => new
            //    {
            //        Id = table.Column<long>(nullable: false)
            //            .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
            //        CreatedDate = table.Column<DateTime>(nullable: false),
            //        CreatedBy = table.Column<long>(nullable: false),
            //        UpdatedBy = table.Column<long>(nullable: true),
            //        UpdatedDate = table.Column<DateTime>(nullable: false),
            //        Status = table.Column<short>(nullable: false),
            //        CommSerproID = table.Column<long>(nullable: false),
            //        CommServiceTypeID = table.Column<long>(nullable: false),
            //        SerproName = table.Column<string>(maxLength: 60, nullable: false),
            //        UserID = table.Column<string>(maxLength: 50, nullable: false),
            //        Password = table.Column<string>(maxLength: 50, nullable: false),
            //        Balance = table.Column<decimal>(type: "decimal(18, 2)", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_CommServiceproviderMaster", x => x.Id);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "CommServiceTypeMaster",
            //    columns: table => new
            //    {
            //        Id = table.Column<long>(nullable: false)
            //            .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
            //        CreatedDate = table.Column<DateTime>(nullable: false),
            //        CreatedBy = table.Column<long>(nullable: false),
            //        UpdatedBy = table.Column<long>(nullable: true),
            //        UpdatedDate = table.Column<DateTime>(nullable: false),
            //        Status = table.Column<short>(nullable: false),
            //        CommServiceTypeID = table.Column<long>(nullable: false),
            //        ServiceTypeID = table.Column<long>(nullable: false),
            //        CommServiceTypeName = table.Column<string>(maxLength: 60, nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_CommServiceTypeMaster", x => x.Id);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "DemonConfiguration",
            //    columns: table => new
            //    {
            //        Id = table.Column<long>(nullable: false)
            //            .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
            //        CreatedDate = table.Column<DateTime>(nullable: false),
            //        CreatedBy = table.Column<long>(nullable: false),
            //        UpdatedBy = table.Column<long>(nullable: true),
            //        UpdatedDate = table.Column<DateTime>(nullable: false),
            //        Status = table.Column<short>(nullable: false),
            //        IPAdd = table.Column<string>(maxLength: 15, nullable: false),
            //        PortAdd = table.Column<int>(nullable: false),
            //        Url = table.Column<string>(maxLength: 200, nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_DemonConfiguration", x => x.Id);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "DepositHistorys",
            //    columns: table => new
            //    {
            //        CreatedDate = table.Column<DateTime>(nullable: false),
            //        CreatedBy = table.Column<long>(nullable: false),
            //        UpdatedBy = table.Column<long>(nullable: true),
            //        UpdatedDate = table.Column<DateTime>(nullable: false),
            //        Status = table.Column<short>(nullable: false),
            //        Id = table.Column<long>(nullable: false)
            //            .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
            //        TrnID = table.Column<string>(maxLength: 100, nullable: false),
            //        SMSCode = table.Column<string>(nullable: false),
            //        Address = table.Column<string>(maxLength: 50, nullable: false),
            //        Confirmations = table.Column<long>(nullable: false),
            //        Amount = table.Column<decimal>(type: "decimal(18, 8)", nullable: false),
            //        StatusMsg = table.Column<string>(maxLength: 100, nullable: false),
            //        TimeEpoch = table.Column<string>(nullable: false),
            //        ConfirmedTime = table.Column<string>(nullable: false),
            //        EpochTimePure = table.Column<string>(nullable: false),
            //        OrderID = table.Column<long>(nullable: false),
            //        IsProcessing = table.Column<byte>(nullable: false),
            //        FromAddress = table.Column<string>(maxLength: 50, nullable: false),
            //        APITopUpRefNo = table.Column<string>(nullable: true),
            //        SystemRemarks = table.Column<string>(nullable: true),
            //        RouteTag = table.Column<string>(nullable: true),
            //        SerProID = table.Column<long>(nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_DepositHistorys", x => x.TrnID);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "EmailQueue",
            //    columns: table => new
            //    {
            //        Id = table.Column<long>(nullable: false)
            //            .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
            //        CreatedDate = table.Column<DateTime>(nullable: false),
            //        CreatedBy = table.Column<long>(nullable: false),
            //        UpdatedBy = table.Column<long>(nullable: true),
            //        UpdatedDate = table.Column<DateTime>(nullable: false),
            //        Status = table.Column<short>(nullable: false),
            //        Recepient = table.Column<string>(maxLength: 50, nullable: false),
            //        Body = table.Column<string>(nullable: false),
            //        Subject = table.Column<string>(maxLength: 50, nullable: false),
            //        CC = table.Column<string>(maxLength: 500, nullable: true),
            //        BCC = table.Column<string>(maxLength: 500, nullable: true),
            //        Attachment = table.Column<string>(maxLength: 500, nullable: true),
            //        SendBy = table.Column<short>(nullable: false),
            //        EmailType = table.Column<short>(nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_EmailQueue", x => x.Id);
            //    });

            migrationBuilder.CreateTable(
                name: "IpMaster",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<long>(nullable: false),
                    UpdatedBy = table.Column<long>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: false),
                    Status = table.Column<short>(nullable: false),
                    UserId = table.Column<int>(nullable: false),
                    IpAddress = table.Column<string>(maxLength: 15, nullable: false),
                    IsEnable = table.Column<bool>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IpMaster", x => x.Id);
                });

            //migrationBuilder.CreateTable(
            //    name: "Limits",
            //    columns: table => new
            //    {
            //        Id = table.Column<long>(nullable: false)
            //            .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
            //        CreatedDate = table.Column<DateTime>(nullable: false),
            //        CreatedBy = table.Column<long>(nullable: false),
            //        UpdatedBy = table.Column<long>(nullable: true),
            //        UpdatedDate = table.Column<DateTime>(nullable: false),
            //        Status = table.Column<short>(nullable: false),
            //        MinAmt = table.Column<decimal>(type: "decimal(18, 8)", nullable: false),
            //        MaxAmt = table.Column<decimal>(type: "decimal(18, 8)", nullable: false),
            //        MinAmtDaily = table.Column<decimal>(type: "decimal(18, 8)", nullable: false),
            //        MaxAmtDaily = table.Column<decimal>(type: "decimal(18, 8)", nullable: false),
            //        MinAmtWeekly = table.Column<decimal>(type: "decimal(18, 8)", nullable: false),
            //        MaxAmtWeekly = table.Column<decimal>(type: "decimal(18, 8)", nullable: false),
            //        MinAmtMonthly = table.Column<decimal>(type: "decimal(18, 8)", nullable: false),
            //        MaxAmtMonthly = table.Column<decimal>(type: "decimal(18, 8)", nullable: false),
            //        MinRange = table.Column<long>(nullable: false),
            //        Maxrange = table.Column<long>(nullable: false),
            //        MinRangeDaily = table.Column<long>(nullable: false),
            //        MaxRangeDaily = table.Column<long>(nullable: false),
            //        MinRangeWeekly = table.Column<long>(nullable: false),
            //        MaxRangeWeekly = table.Column<long>(nullable: false),
            //        MinRangeMonthly = table.Column<long>(nullable: false),
            //        MaxRangeMonthly = table.Column<long>(nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_Limits", x => x.Id);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "MessagingQueue",
            //    columns: table => new
            //    {
            //        Id = table.Column<long>(nullable: false)
            //            .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
            //        CreatedDate = table.Column<DateTime>(nullable: false),
            //        CreatedBy = table.Column<long>(nullable: false),
            //        UpdatedBy = table.Column<long>(nullable: true),
            //        UpdatedDate = table.Column<DateTime>(nullable: false),
            //        Status = table.Column<short>(nullable: false),
            //        MobileNo = table.Column<long>(nullable: false),
            //        SMSText = table.Column<string>(maxLength: 200, nullable: false),
            //        RespText = table.Column<string>(maxLength: 1000, nullable: true),
            //        SMSServiceID = table.Column<short>(nullable: false),
            //        SMSSendBy = table.Column<short>(nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_MessagingQueue", x => x.Id);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "NotificationQueue",
            //    columns: table => new
            //    {
            //        Id = table.Column<long>(nullable: false)
            //            .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
            //        CreatedDate = table.Column<DateTime>(nullable: false),
            //        CreatedBy = table.Column<long>(nullable: false),
            //        UpdatedBy = table.Column<long>(nullable: true),
            //        UpdatedDate = table.Column<DateTime>(nullable: false),
            //        Status = table.Column<short>(nullable: false),
            //        Subject = table.Column<string>(maxLength: 50, nullable: true),
            //        Message = table.Column<string>(maxLength: 200, nullable: false),
            //        DeviceID = table.Column<string>(maxLength: 500, nullable: false),
            //        TickerText = table.Column<string>(maxLength: 200, nullable: false),
            //        ContentTitle = table.Column<string>(maxLength: 200, nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_NotificationQueue", x => x.Id);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "OtpMaster",
            //    columns: table => new
            //    {
            //        Id = table.Column<long>(nullable: false)
            //            .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
            //        CreatedDate = table.Column<DateTime>(nullable: false),
            //        CreatedBy = table.Column<long>(nullable: false),
            //        UpdatedBy = table.Column<long>(nullable: true),
            //        UpdatedDate = table.Column<DateTime>(nullable: false),
            //        Status = table.Column<short>(nullable: false),
            //        UserId = table.Column<int>(nullable: false),
            //        RegTypeId = table.Column<int>(nullable: false),
            //        OTP = table.Column<string>(maxLength: 6, nullable: false),
            //        CreatedTime = table.Column<DateTime>(nullable: false),
            //        ExpirTime = table.Column<DateTime>(nullable: false),
            //        EnableStatus = table.Column<bool>(nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_OtpMaster", x => x.Id);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "ProductConfiguration",
            //    columns: table => new
            //    {
            //        Id = table.Column<long>(nullable: false)
            //            .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
            //        CreatedDate = table.Column<DateTime>(nullable: false),
            //        CreatedBy = table.Column<long>(nullable: false),
            //        UpdatedBy = table.Column<long>(nullable: true),
            //        UpdatedDate = table.Column<DateTime>(nullable: false),
            //        Status = table.Column<short>(nullable: false),
            //        ProductName = table.Column<string>(maxLength: 30, nullable: false),
            //        ServiceID = table.Column<long>(nullable: false),
            //        StateID = table.Column<long>(nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_ProductConfiguration", x => x.Id);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "RegisterType",
            //    columns: table => new
            //    {
            //        Id = table.Column<long>(nullable: false)
            //            .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
            //        CreatedDate = table.Column<DateTime>(nullable: false),
            //        CreatedBy = table.Column<long>(nullable: false),
            //        UpdatedBy = table.Column<long>(nullable: true),
            //        UpdatedDate = table.Column<DateTime>(nullable: false),
            //        Status = table.Column<short>(nullable: false),
            //        Type = table.Column<string>(nullable: true),
            //        ActiveStatus = table.Column<bool>(nullable: false),
            //        IsDeleted = table.Column<bool>(nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_RegisterType", x => x.Id);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "RequestFormatMaster",
            //    columns: table => new
            //    {
            //        Id = table.Column<long>(nullable: false)
            //            .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
            //        CreatedDate = table.Column<DateTime>(nullable: false),
            //        CreatedBy = table.Column<long>(nullable: false),
            //        UpdatedBy = table.Column<long>(nullable: true),
            //        UpdatedDate = table.Column<DateTime>(nullable: false),
            //        Status = table.Column<short>(nullable: false),
            //        RequestID = table.Column<long>(maxLength: 60, nullable: false),
            //        contentType = table.Column<string>(maxLength: 60, nullable: false),
            //        MethodType = table.Column<string>(maxLength: 20, nullable: false),
            //        RequestFormat = table.Column<string>(maxLength: 500, nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_RequestFormatMaster", x => x.Id);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "RouteConfiguration",
            //    columns: table => new
            //    {
            //        Id = table.Column<long>(nullable: false)
            //            .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
            //        CreatedDate = table.Column<DateTime>(nullable: false),
            //        CreatedBy = table.Column<long>(nullable: false),
            //        UpdatedBy = table.Column<long>(nullable: true),
            //        UpdatedDate = table.Column<DateTime>(nullable: false),
            //        Status = table.Column<short>(nullable: false),
            //        RouteName = table.Column<string>(maxLength: 30, nullable: false),
            //        ServiceID = table.Column<long>(nullable: false),
            //        SerProID = table.Column<long>(nullable: false),
            //        ProductID = table.Column<long>(nullable: false),
            //        Priority = table.Column<short>(nullable: false),
            //        StatusCheckUrl = table.Column<string>(nullable: true),
            //        ValidationUrl = table.Column<string>(nullable: true),
            //        TransactionUrl = table.Column<string>(nullable: true),
            //        LimitId = table.Column<long>(nullable: false),
            //        OpCode = table.Column<string>(maxLength: 50, nullable: true),
            //        TrnType = table.Column<int>(nullable: false),
            //        IsDelayAddress = table.Column<byte>(nullable: false),
            //        ProviderWalletID = table.Column<string>(maxLength: 100, nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_RouteConfiguration", x => x.Id);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "ServiceDetail",
            //    columns: table => new
            //    {
            //        Id = table.Column<long>(nullable: false)
            //            .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
            //        CreatedDate = table.Column<DateTime>(nullable: false),
            //        CreatedBy = table.Column<long>(nullable: false),
            //        UpdatedBy = table.Column<long>(nullable: true),
            //        UpdatedDate = table.Column<DateTime>(nullable: false),
            //        Status = table.Column<short>(nullable: false),
            //        ServiceId = table.Column<long>(nullable: false),
            //        ServiceDetailJson = table.Column<string>(type: "text", nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_ServiceDetail", x => x.Id);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "ServiceMaster",
            //    columns: table => new
            //    {
            //        Id = table.Column<long>(nullable: false)
            //            .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
            //        CreatedDate = table.Column<DateTime>(nullable: false),
            //        CreatedBy = table.Column<long>(nullable: false),
            //        UpdatedBy = table.Column<long>(nullable: true),
            //        UpdatedDate = table.Column<DateTime>(nullable: false),
            //        Status = table.Column<short>(nullable: false),
            //        Name = table.Column<string>(maxLength: 30, nullable: false),
            //        SMSCode = table.Column<string>(maxLength: 5, nullable: false),
            //        ServiceType = table.Column<short>(nullable: false),
            //        LimitId = table.Column<long>(nullable: false),
            //        WalletTypeID = table.Column<long>(nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_ServiceMaster", x => x.Id);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "ServiceProConfiguration",
            //    columns: table => new
            //    {
            //        Id = table.Column<long>(nullable: false)
            //            .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
            //        CreatedDate = table.Column<DateTime>(nullable: false),
            //        CreatedBy = table.Column<long>(nullable: false),
            //        UpdatedBy = table.Column<long>(nullable: true),
            //        UpdatedDate = table.Column<DateTime>(nullable: false),
            //        Status = table.Column<short>(nullable: false),
            //        AppKey = table.Column<string>(maxLength: 50, nullable: false),
            //        APIKey = table.Column<string>(maxLength: 50, nullable: false),
            //        SecretKey = table.Column<string>(maxLength: 50, nullable: false),
            //        UserName = table.Column<string>(maxLength: 50, nullable: false),
            //        Password = table.Column<string>(maxLength: 50, nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_ServiceProConfiguration", x => x.Id);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "ServiceProviderDetail",
            //    columns: table => new
            //    {
            //        Id = table.Column<long>(nullable: false)
            //            .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
            //        CreatedDate = table.Column<DateTime>(nullable: false),
            //        CreatedBy = table.Column<long>(nullable: false),
            //        UpdatedBy = table.Column<long>(nullable: true),
            //        UpdatedDate = table.Column<DateTime>(nullable: false),
            //        Status = table.Column<short>(nullable: false),
            //        ServiceProID = table.Column<long>(nullable: false),
            //        ProTypeID = table.Column<long>(nullable: false),
            //        AppTypeID = table.Column<long>(nullable: false),
            //        TrnTypeID = table.Column<long>(nullable: false),
            //        LimitID = table.Column<long>(nullable: false),
            //        DemonConfigID = table.Column<long>(nullable: false),
            //        ServiceProConfigID = table.Column<long>(nullable: false),
            //        ThirPartyAPIID = table.Column<long>(nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_ServiceProviderDetail", x => x.Id);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "ServiceProviderMaster",
            //    columns: table => new
            //    {
            //        Id = table.Column<long>(nullable: false)
            //            .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
            //        CreatedDate = table.Column<DateTime>(nullable: false),
            //        CreatedBy = table.Column<long>(nullable: false),
            //        UpdatedBy = table.Column<long>(nullable: true),
            //        UpdatedDate = table.Column<DateTime>(nullable: false),
            //        Status = table.Column<short>(nullable: false),
            //        ProviderName = table.Column<string>(maxLength: 60, nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_ServiceProviderMaster", x => x.Id);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "ServiceProviderType",
            //    columns: table => new
            //    {
            //        Id = table.Column<long>(nullable: false)
            //            .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
            //        CreatedDate = table.Column<DateTime>(nullable: false),
            //        CreatedBy = table.Column<long>(nullable: false),
            //        UpdatedBy = table.Column<long>(nullable: true),
            //        UpdatedDate = table.Column<DateTime>(nullable: false),
            //        Status = table.Column<short>(nullable: false),
            //        ServiveProTypeName = table.Column<string>(maxLength: 20, nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_ServiceProviderType", x => x.Id);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "ServiceStastics",
            //    columns: table => new
            //    {
            //        Id = table.Column<long>(nullable: false)
            //            .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
            //        CreatedDate = table.Column<DateTime>(nullable: false),
            //        CreatedBy = table.Column<long>(nullable: false),
            //        UpdatedBy = table.Column<long>(nullable: true),
            //        UpdatedDate = table.Column<DateTime>(nullable: false),
            //        Status = table.Column<short>(nullable: false),
            //        ServiceId = table.Column<long>(nullable: false),
            //        MarketCap = table.Column<decimal>(type: "decimal(18, 8)", nullable: false),
            //        VolGlobal = table.Column<decimal>(type: "decimal(18, 8)", nullable: false),
            //        MaxSupply = table.Column<long>(nullable: false),
            //        CirculatingSupply = table.Column<long>(nullable: false),
            //        IssuePrice = table.Column<decimal>(nullable: false),
            //        IssueDate = table.Column<DateTime>(nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_ServiceStastics", x => x.Id);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "ServiceTypeMaster",
            //    columns: table => new
            //    {
            //        Id = table.Column<long>(nullable: false)
            //            .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
            //        CreatedDate = table.Column<DateTime>(nullable: false),
            //        CreatedBy = table.Column<long>(nullable: false),
            //        UpdatedBy = table.Column<long>(nullable: true),
            //        UpdatedDate = table.Column<DateTime>(nullable: false),
            //        Status = table.Column<short>(nullable: false),
            //        ServiceTypeID = table.Column<long>(nullable: false),
            //        ServiceTypeName = table.Column<string>(maxLength: 60, nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_ServiceTypeMaster", x => x.Id);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "TemplateMaster",
            //    columns: table => new
            //    {
            //        Id = table.Column<long>(nullable: false)
            //            .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
            //        CreatedDate = table.Column<DateTime>(nullable: false),
            //        CreatedBy = table.Column<long>(nullable: false),
            //        UpdatedBy = table.Column<long>(nullable: true),
            //        UpdatedDate = table.Column<DateTime>(nullable: false),
            //        Status = table.Column<short>(nullable: false),
            //        TemplateID = table.Column<long>(nullable: false),
            //        CommServiceTypeID = table.Column<long>(nullable: false),
            //        TemplateName = table.Column<string>(maxLength: 50, nullable: false),
            //        Content = table.Column<string>(maxLength: 1024, nullable: false),
            //        AdditionalInfo = table.Column<string>(maxLength: 200, nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_TemplateMaster", x => x.Id);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "TempOtpMaster",
            //    columns: table => new
            //    {
            //        Id = table.Column<long>(nullable: false)
            //            .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
            //        CreatedDate = table.Column<DateTime>(nullable: false),
            //        CreatedBy = table.Column<long>(nullable: false),
            //        UpdatedBy = table.Column<long>(nullable: true),
            //        UpdatedDate = table.Column<DateTime>(nullable: false),
            //        Status = table.Column<short>(nullable: false),
            //        UserId = table.Column<int>(nullable: false),
            //        RegTypeId = table.Column<int>(nullable: false),
            //        OTP = table.Column<string>(maxLength: 6, nullable: false),
            //        CreatedTime = table.Column<DateTime>(nullable: false),
            //        ExpirTime = table.Column<DateTime>(nullable: false),
            //        EnableStatus = table.Column<bool>(nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_TempOtpMaster", x => x.Id);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "TempUserRegister",
            //    columns: table => new
            //    {
            //        Id = table.Column<long>(nullable: false)
            //            .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
            //        CreatedDate = table.Column<DateTime>(nullable: false),
            //        CreatedBy = table.Column<long>(nullable: false),
            //        UpdatedBy = table.Column<long>(nullable: true),
            //        UpdatedDate = table.Column<DateTime>(nullable: false),
            //        Status = table.Column<short>(nullable: false),
            //        RegTypeId = table.Column<int>(nullable: false),
            //        UserName = table.Column<string>(maxLength: 100, nullable: true),
            //        Email = table.Column<string>(nullable: true),
            //        PasswordHash = table.Column<string>(nullable: true),
            //        SecurityStemp = table.Column<string>(nullable: true),
            //        ConcurrencyStamp = table.Column<string>(nullable: true),
            //        PhoneNumber = table.Column<string>(nullable: true),
            //        FirstName = table.Column<string>(maxLength: 250, nullable: true),
            //        LastName = table.Column<string>(maxLength: 250, nullable: true),
            //        Mobile = table.Column<string>(nullable: true),
            //        RegisterStatus = table.Column<bool>(nullable: false),
            //        IsDeleted = table.Column<bool>(nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_TempUserRegister", x => x.Id);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "ThirdPartyAPIConfiguration",
            //    columns: table => new
            //    {
            //        Id = table.Column<long>(nullable: false)
            //            .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
            //        CreatedDate = table.Column<DateTime>(nullable: false),
            //        CreatedBy = table.Column<long>(nullable: false),
            //        UpdatedBy = table.Column<long>(nullable: true),
            //        UpdatedDate = table.Column<DateTime>(nullable: false),
            //        Status = table.Column<short>(nullable: false),
            //        APIName = table.Column<string>(maxLength: 30, nullable: false),
            //        APISendURL = table.Column<string>(nullable: false),
            //        APIValidateURL = table.Column<string>(nullable: true),
            //        APIBalURL = table.Column<string>(nullable: true),
            //        APIStatusCheckURL = table.Column<string>(nullable: true),
            //        APIRequestBody = table.Column<string>(nullable: true),
            //        TransactionIdPrefix = table.Column<string>(nullable: true),
            //        MerchantCode = table.Column<string>(nullable: true),
            //        SerProConfigurationID = table.Column<long>(nullable: false),
            //        ResponseSuccess = table.Column<string>(nullable: true),
            //        ResponseFailure = table.Column<string>(nullable: true),
            //        ResponseHold = table.Column<string>(nullable: true),
            //        AuthHeader = table.Column<string>(nullable: true),
            //        ContentType = table.Column<string>(nullable: true),
            //        MethodType = table.Column<string>(nullable: true),
            //        HashCode = table.Column<string>(nullable: true),
            //        HashCodeRecheck = table.Column<string>(nullable: true),
            //        HashType = table.Column<short>(nullable: false),
            //        AppType = table.Column<short>(nullable: false),
            //        ParsingDataID = table.Column<long>(nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_ThirdPartyAPIConfiguration", x => x.Id);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "ThirdPartyAPIResponseConfiguration",
            //    columns: table => new
            //    {
            //        Id = table.Column<long>(nullable: false)
            //            .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
            //        CreatedDate = table.Column<DateTime>(nullable: false),
            //        CreatedBy = table.Column<long>(nullable: false),
            //        UpdatedBy = table.Column<long>(nullable: true),
            //        UpdatedDate = table.Column<DateTime>(nullable: false),
            //        Status = table.Column<short>(nullable: false),
            //        BalanceRegex = table.Column<string>(nullable: true),
            //        StatusRegex = table.Column<string>(nullable: true),
            //        StatusMsgRegex = table.Column<string>(nullable: true),
            //        ResponseCodeRegex = table.Column<string>(nullable: true),
            //        ErrorCodeRegex = table.Column<string>(nullable: true),
            //        TrnRefNoRegex = table.Column<string>(nullable: true),
            //        OprTrnRefNoRegex = table.Column<string>(nullable: true),
            //        Param1Regex = table.Column<string>(nullable: true),
            //        Param2Regex = table.Column<string>(nullable: true),
            //        Param3Regex = table.Column<string>(nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_ThirdPartyAPIResponseConfiguration", x => x.Id);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "ToDoItems",
            //    columns: table => new
            //    {
            //        Id = table.Column<int>(nullable: false)
            //            .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
            //        Title = table.Column<string>(nullable: true),
            //        Description = table.Column<string>(nullable: true),
            //        IsDone = table.Column<bool>(nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_ToDoItems", x => x.Id);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "TradeBitGoDelayAddressess",
            //    columns: table => new
            //    {
            //        Id = table.Column<long>(nullable: false)
            //            .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
            //        CreatedDate = table.Column<DateTime>(nullable: false),
            //        CreatedBy = table.Column<long>(nullable: false),
            //        UpdatedBy = table.Column<long>(nullable: true),
            //        UpdatedDate = table.Column<DateTime>(nullable: false),
            //        Status = table.Column<short>(nullable: false),
            //        WalletId = table.Column<long>(nullable: false),
            //        WalletTypeId = table.Column<long>(nullable: false),
            //        TrnID = table.Column<string>(maxLength: 100, nullable: false),
            //        Address = table.Column<string>(maxLength: 100, nullable: false),
            //        GenerateBit = table.Column<byte>(nullable: false),
            //        CoinName = table.Column<string>(maxLength: 5, nullable: false),
            //        BitgoWalletId = table.Column<string>(maxLength: 100, nullable: false),
            //        CoinSpecific = table.Column<string>(maxLength: 250, nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_TradeBitGoDelayAddressess", x => x.Id);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "TradePairDetail",
            //    columns: table => new
            //    {
            //        Id = table.Column<long>(nullable: false)
            //            .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
            //        CreatedDate = table.Column<DateTime>(nullable: false),
            //        CreatedBy = table.Column<long>(nullable: false),
            //        UpdatedBy = table.Column<long>(nullable: true),
            //        UpdatedDate = table.Column<DateTime>(nullable: false),
            //        Status = table.Column<short>(nullable: false),
            //        PairId = table.Column<long>(nullable: false),
            //        Currentrate = table.Column<decimal>(type: "decimal(18, 8)", nullable: false),
            //        BuyMinQty = table.Column<decimal>(type: "decimal(18, 8)", nullable: false),
            //        BuyMaxQty = table.Column<decimal>(type: "decimal(18, 8)", nullable: false),
            //        SellMinQty = table.Column<decimal>(type: "decimal(18, 8)", nullable: false),
            //        SellMaxQty = table.Column<decimal>(type: "decimal(18, 8)", nullable: false),
            //        DailyHigh = table.Column<decimal>(type: "decimal(18, 8)", nullable: false),
            //        DailyLow = table.Column<decimal>(type: "decimal(18, 8)", nullable: false),
            //        CurrencyPrice = table.Column<decimal>(type: "decimal(18, 8)", nullable: false),
            //        Volume = table.Column<decimal>(type: "decimal(18, 8)", nullable: false),
            //        SellPrice = table.Column<decimal>(type: "decimal(18, 8)", nullable: false),
            //        BuyPrice = table.Column<decimal>(type: "decimal(18, 8)", nullable: false),
            //        BuyMinPrice = table.Column<decimal>(type: "decimal(18, 8)", nullable: false),
            //        BuyMaxPrice = table.Column<decimal>(type: "decimal(18, 8)", nullable: false),
            //        SellMinPrice = table.Column<decimal>(type: "decimal(18, 8)", nullable: false),
            //        SellMaxPrice = table.Column<decimal>(type: "decimal(18, 8)", nullable: false),
            //        Fee = table.Column<decimal>(type: "decimal(18, 8)", nullable: false),
            //        FeeType = table.Column<short>(nullable: false),
            //        LastTrnNo = table.Column<long>(nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_TradePairDetail", x => x.Id);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "TradePairMaster",
            //    columns: table => new
            //    {
            //        Id = table.Column<long>(nullable: false)
            //            .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
            //        CreatedDate = table.Column<DateTime>(nullable: false),
            //        CreatedBy = table.Column<long>(nullable: false),
            //        UpdatedBy = table.Column<long>(nullable: true),
            //        UpdatedDate = table.Column<DateTime>(nullable: false),
            //        Status = table.Column<short>(nullable: false),
            //        PairName = table.Column<string>(nullable: false),
            //        SecondaryCurrencyId = table.Column<long>(nullable: false),
            //        WalletMasterID = table.Column<long>(nullable: false),
            //        BaseCurrencyId = table.Column<long>(nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_TradePairMaster", x => x.Id);
            //    });

            migrationBuilder.CreateTable(
                name: "TradePoolMaster",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<long>(nullable: false),
                    UpdatedBy = table.Column<long>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: false),
                    Status = table.Column<short>(nullable: false),
                    PairName = table.Column<string>(maxLength: 50, nullable: false),
                    ProductID = table.Column<long>(nullable: false),
                    SellServiceID = table.Column<long>(nullable: false),
                    BuyServiceID = table.Column<long>(nullable: false),
                    BidPrice = table.Column<long>(nullable: false),
                    TotalQty = table.Column<decimal>(type: "decimal(18, 8)", nullable: false),
                    Landing = table.Column<decimal>(type: "decimal(37, 16)", nullable: false),
                    OnProcessing = table.Column<short>(nullable: false),
                    TPSPickupStatus = table.Column<short>(nullable: false),
                    IsSleepMode = table.Column<short>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TradePoolMaster", x => new { x.Id, x.SellServiceID, x.BuyServiceID, x.BidPrice });
                    table.UniqueConstraint("AK_TradePoolMaster_BidPrice_BuyServiceID_Id_SellServiceID", x => new { x.BidPrice, x.BuyServiceID, x.Id, x.SellServiceID });
                });

            migrationBuilder.CreateTable(
                name: "TradeStopLoss",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<long>(nullable: false),
                    UpdatedBy = table.Column<long>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: false),
                    Status = table.Column<short>(nullable: false),
                    TrnNo = table.Column<long>(nullable: false),
                    ordertype = table.Column<short>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TradeStopLoss", x => x.Id);
                });

            //migrationBuilder.CreateTable(
            //    name: "TradeTransactionQueue",
            //    columns: table => new
            //    {
            //        Id = table.Column<long>(nullable: false)
            //            .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
            //        CreatedDate = table.Column<DateTime>(nullable: false),
            //        CreatedBy = table.Column<long>(nullable: false),
            //        UpdatedBy = table.Column<long>(nullable: true),
            //        UpdatedDate = table.Column<DateTime>(nullable: false),
            //        Status = table.Column<short>(nullable: false),
            //        TrnNo = table.Column<long>(nullable: false),
            //        TrnDate = table.Column<DateTime>(nullable: false),
            //        MemberID = table.Column<long>(nullable: false),
            //        TrnType = table.Column<short>(nullable: false),
            //        TrnTypeName = table.Column<string>(nullable: true),
            //        PairID = table.Column<long>(nullable: false),
            //        PairName = table.Column<string>(nullable: false),
            //        OrderWalletID = table.Column<long>(nullable: false),
            //        DeliveryWalletID = table.Column<long>(nullable: false),
            //        BuyQty = table.Column<decimal>(nullable: false),
            //        BidPrice = table.Column<decimal>(nullable: false),
            //        SellQty = table.Column<decimal>(nullable: false),
            //        AskPrice = table.Column<decimal>(nullable: false),
            //        Order_Currency = table.Column<string>(nullable: true),
            //        OrderTotalQty = table.Column<decimal>(nullable: false),
            //        Delivery_Currency = table.Column<string>(nullable: true),
            //        DeliveryTotalQty = table.Column<decimal>(nullable: false),
            //        StatusCode = table.Column<int>(nullable: false),
            //        StatusMsg = table.Column<string>(nullable: true),
            //        ServiceID = table.Column<long>(nullable: false),
            //        ProductID = table.Column<long>(nullable: false),
            //        SerProID = table.Column<long>(nullable: false),
            //        RoutID = table.Column<int>(nullable: false),
            //        TrnRefNo = table.Column<long>(nullable: true),
            //        IsCancelled = table.Column<short>(nullable: false),
            //        SettledBuyQty = table.Column<decimal>(nullable: false),
            //        SettledSellQty = table.Column<decimal>(nullable: false),
            //        SettledDate = table.Column<DateTime>(nullable: true),
            //        TakerPer = table.Column<decimal>(nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_TradeTransactionQueue", x => new { x.Id, x.TrnNo });
            //    });

            //migrationBuilder.CreateTable(
            //    name: "TransactionAccounts",
            //    columns: table => new
            //    {
            //        Id = table.Column<long>(nullable: false)
            //            .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
            //        CreatedDate = table.Column<DateTime>(nullable: false),
            //        CreatedBy = table.Column<long>(nullable: false),
            //        UpdatedBy = table.Column<long>(nullable: true),
            //        UpdatedDate = table.Column<DateTime>(nullable: false),
            //        Status = table.Column<short>(nullable: false),
            //        BatchNo = table.Column<long>(nullable: false),
            //        RefNo = table.Column<long>(nullable: false),
            //        TrnDate = table.Column<DateTime>(nullable: false),
            //        WalletID = table.Column<long>(nullable: false),
            //        CrAmt = table.Column<decimal>(type: "decimal(18, 8)", nullable: false),
            //        DrAmt = table.Column<decimal>(type: "decimal(18, 8)", nullable: false),
            //        Remarks = table.Column<string>(maxLength: 150, nullable: false),
            //        IsSettled = table.Column<short>(nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_TransactionAccounts", x => x.Id);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "TransactionQueue",
            //    columns: table => new
            //    {
            //        Id = table.Column<long>(nullable: false)
            //            .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
            //        CreatedDate = table.Column<DateTime>(nullable: false),
            //        CreatedBy = table.Column<long>(nullable: false),
            //        UpdatedBy = table.Column<long>(nullable: true),
            //        UpdatedDate = table.Column<DateTime>(nullable: false),
            //        Status = table.Column<short>(nullable: false),
            //        GUID = table.Column<Guid>(nullable: false),
            //        TrnDate = table.Column<DateTime>(nullable: false),
            //        TrnMode = table.Column<short>(nullable: false),
            //        TrnType = table.Column<short>(nullable: false),
            //        MemberID = table.Column<long>(nullable: false),
            //        MemberMobile = table.Column<string>(nullable: false),
            //        SMSCode = table.Column<string>(maxLength: 10, nullable: false),
            //        TransactionAccount = table.Column<string>(maxLength: 200, nullable: false),
            //        Amount = table.Column<decimal>(type: "decimal(18, 8)", nullable: false),
            //        ServiceID = table.Column<long>(nullable: false),
            //        SerProID = table.Column<long>(nullable: false),
            //        ProductID = table.Column<int>(nullable: false),
            //        RoutID = table.Column<int>(nullable: false),
            //        StatusCode = table.Column<short>(nullable: false),
            //        StatusMsg = table.Column<string>(nullable: true),
            //        VerifyDone = table.Column<short>(nullable: false),
            //        TrnRefNo = table.Column<string>(nullable: true),
            //        AdditionalInfo = table.Column<string>(nullable: true),
            //        ChargePer = table.Column<decimal>(type: "decimal(18, 8)", nullable: true),
            //        ChargeRs = table.Column<decimal>(type: "decimal(18, 8)", nullable: true),
            //        ChargeType = table.Column<short>(nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_TransactionQueue", x => x.Id);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "WalletAllowTrns",
            //    columns: table => new
            //    {
            //        Id = table.Column<long>(nullable: false)
            //            .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
            //        CreatedDate = table.Column<DateTime>(nullable: false),
            //        CreatedBy = table.Column<long>(nullable: false),
            //        UpdatedBy = table.Column<long>(nullable: true),
            //        UpdatedDate = table.Column<DateTime>(nullable: false),
            //        Status = table.Column<short>(nullable: false),
            //        WalletId = table.Column<long>(nullable: false),
            //        TrnType = table.Column<byte>(nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_WalletAllowTrns", x => x.Id);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "WalletLedgers",
            //    columns: table => new
            //    {
            //        Id = table.Column<long>(nullable: false)
            //            .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
            //        CreatedDate = table.Column<DateTime>(nullable: false),
            //        CreatedBy = table.Column<long>(nullable: false),
            //        UpdatedBy = table.Column<long>(nullable: true),
            //        UpdatedDate = table.Column<DateTime>(nullable: false),
            //        Status = table.Column<short>(nullable: false),
            //        WalletId = table.Column<long>(nullable: false),
            //        ToWalletId = table.Column<long>(nullable: false),
            //        TrnDate = table.Column<DateTime>(nullable: false),
            //        ServiceTypeID = table.Column<int>(nullable: false),
            //        TrnType = table.Column<int>(nullable: false),
            //        TrnNo = table.Column<long>(nullable: false),
            //        CrAmt = table.Column<decimal>(type: "decimal(18, 8)", nullable: false),
            //        DrAmt = table.Column<decimal>(type: "decimal(18, 8)", nullable: false),
            //        PreBal = table.Column<decimal>(type: "decimal(18, 8)", nullable: false),
            //        PostBal = table.Column<decimal>(type: "decimal(18, 8)", nullable: false),
            //        Remarks = table.Column<string>(maxLength: 100, nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_WalletLedgers", x => x.Id);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "WalletMasters",
            //    columns: table => new
            //    {
            //        CreatedDate = table.Column<DateTime>(nullable: false),
            //        CreatedBy = table.Column<long>(nullable: false),
            //        UpdatedBy = table.Column<long>(nullable: true),
            //        UpdatedDate = table.Column<DateTime>(nullable: false),
            //        Status = table.Column<short>(nullable: false),
            //        Id = table.Column<long>(nullable: false)
            //            .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
            //        Walletname = table.Column<string>(maxLength: 50, nullable: false),
            //        Balance = table.Column<decimal>(type: "decimal(18, 8)", nullable: false),
            //        WalletTypeID = table.Column<long>(nullable: false),
            //        IsValid = table.Column<bool>(nullable: false),
            //        AccWalletID = table.Column<string>(maxLength: 16, nullable: false),
            //        UserID = table.Column<long>(nullable: false),
            //        PublicAddress = table.Column<string>(maxLength: 50, nullable: false),
            //        IsDefaultWallet = table.Column<byte>(nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_WalletMasters", x => x.Id);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "WalletOrders",
            //    columns: table => new
            //    {
            //        Id = table.Column<long>(nullable: false)
            //            .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
            //        CreatedDate = table.Column<DateTime>(nullable: false),
            //        CreatedBy = table.Column<long>(nullable: false),
            //        UpdatedBy = table.Column<long>(nullable: true),
            //        UpdatedDate = table.Column<DateTime>(nullable: false),
            //        OrderDate = table.Column<DateTime>(nullable: false),
            //        OrderType = table.Column<int>(nullable: false),
            //        OWalletMasterID = table.Column<long>(nullable: false),
            //        DWalletMasterID = table.Column<long>(nullable: false),
            //        OrderAmt = table.Column<decimal>(type: "decimal(18, 8)", nullable: false),
            //        Status = table.Column<int>(nullable: false),
            //        ORemarks = table.Column<string>(maxLength: 100, nullable: false),
            //        DeliveryAmt = table.Column<decimal>(type: "decimal(18, 8)", nullable: false),
            //        DRemarks = table.Column<string>(nullable: true),
            //        DeliveryGivenBy = table.Column<long>(nullable: true),
            //        DeliveryGivenDate = table.Column<DateTime>(nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_WalletOrders", x => x.Id);
            //    });

            migrationBuilder.CreateTable(
                name: "WalletTransactionOrders",
                columns: table => new
                {
                    OrderID = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    UpdatedDate = table.Column<DateTime>(nullable: false),
                    TrnDate = table.Column<DateTime>(nullable: false),
                    OWalletID = table.Column<long>(nullable: false),
                    DWalletID = table.Column<long>(nullable: false),
                    Amount = table.Column<decimal>(nullable: false),
                    WalletType = table.Column<string>(maxLength: 5, nullable: false),
                    OTrnNo = table.Column<long>(nullable: false),
                    DTrnNo = table.Column<long>(nullable: false),
                    Status = table.Column<byte>(nullable: false),
                    StatusMsg = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WalletTransactionOrders", x => x.OrderID);
                });

            migrationBuilder.CreateTable(
                name: "WalletTransactionQueues",
                columns: table => new
                {
                    TrnNo = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Guid = table.Column<string>(maxLength: 50, nullable: false),
                    TrnType = table.Column<byte>(nullable: false),
                    Amount = table.Column<decimal>(nullable: false),
                    TrnRefNo = table.Column<long>(nullable: false),
                    TrnDate = table.Column<DateTime>(nullable: false),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    WalletID = table.Column<long>(nullable: false),
                    WalletType = table.Column<string>(maxLength: 5, nullable: false),
                    MemberID = table.Column<long>(nullable: false),
                    TimeStamp = table.Column<string>(maxLength: 5, nullable: false),
                    Status = table.Column<byte>(nullable: false),
                    StatusMsg = table.Column<string>(maxLength: 5, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WalletTransactionQueues", x => x.TrnNo);
                });

            //migrationBuilder.CreateTable(
            //    name: "WalletTypeMasters",
            //    columns: table => new
            //    {
            //        Id = table.Column<long>(nullable: false)
            //            .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
            //        CreatedDate = table.Column<DateTime>(nullable: false),
            //        CreatedBy = table.Column<long>(nullable: false),
            //        UpdatedBy = table.Column<long>(nullable: true),
            //        UpdatedDate = table.Column<DateTime>(nullable: false),
            //        Status = table.Column<short>(nullable: false),
            //        WalletTypeName = table.Column<string>(maxLength: 50, nullable: false),
            //        Discription = table.Column<string>(maxLength: 100, nullable: false),
            //        IsDepositionAllow = table.Column<short>(nullable: false),
            //        IsWithdrawalAllow = table.Column<short>(nullable: false),
            //        IsTransactionWallet = table.Column<short>(nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_WalletTypeMasters", x => x.Id);
            //    });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropTable(
            //    name: "AddressMasters");

            //migrationBuilder.DropTable(
            //    name: "AppType");

            //migrationBuilder.DropTable(
            //    name: "CommAPIServiceMaster");

            //migrationBuilder.DropTable(
            //    name: "CommServiceMaster");

            //migrationBuilder.DropTable(
            //    name: "CommServiceproviderMaster");

            //migrationBuilder.DropTable(
            //    name: "CommServiceTypeMaster");

            //migrationBuilder.DropTable(
            //    name: "DemonConfiguration");

            //migrationBuilder.DropTable(
            //    name: "DepositHistorys");

            //migrationBuilder.DropTable(
            //    name: "EmailQueue");

            //migrationBuilder.DropTable(
            //    name: "IpMaster");

            //migrationBuilder.DropTable(
            //    name: "Limits");

            //migrationBuilder.DropTable(
            //    name: "MessagingQueue");

            //migrationBuilder.DropTable(
            //    name: "NotificationQueue");

            //migrationBuilder.DropTable(
            //    name: "OtpMaster");

            //migrationBuilder.DropTable(
            //    name: "ProductConfiguration");

            //migrationBuilder.DropTable(
            //    name: "RegisterType");

            //migrationBuilder.DropTable(
            //    name: "RequestFormatMaster");

            //migrationBuilder.DropTable(
            //    name: "RouteConfiguration");

            //migrationBuilder.DropTable(
            //    name: "ServiceDetail");

            //migrationBuilder.DropTable(
            //    name: "ServiceMaster");

            //migrationBuilder.DropTable(
            //    name: "ServiceProConfiguration");

            //migrationBuilder.DropTable(
            //    name: "ServiceProviderDetail");

            //migrationBuilder.DropTable(
            //    name: "ServiceProviderMaster");

            //migrationBuilder.DropTable(
            //    name: "ServiceProviderType");

            //migrationBuilder.DropTable(
            //    name: "ServiceStastics");

            //migrationBuilder.DropTable(
            //    name: "ServiceTypeMaster");

            //migrationBuilder.DropTable(
            //    name: "TemplateMaster");

            //migrationBuilder.DropTable(
            //    name: "TempOtpMaster");

            //migrationBuilder.DropTable(
            //    name: "TempUserRegister");

            //migrationBuilder.DropTable(
            //    name: "ThirdPartyAPIConfiguration");

            //migrationBuilder.DropTable(
            //    name: "ThirdPartyAPIResponseConfiguration");

            //migrationBuilder.DropTable(
            //    name: "ToDoItems");

            //migrationBuilder.DropTable(
            //    name: "TradeBitGoDelayAddressess");

            //migrationBuilder.DropTable(
            //    name: "TradePairDetail");

            //migrationBuilder.DropTable(
            //    name: "TradePairMaster");

            //migrationBuilder.DropTable(
            //    name: "TradePoolMaster");

            //migrationBuilder.DropTable(
            //    name: "TradeStopLoss");

            //migrationBuilder.DropTable(
            //    name: "TradeTransactionQueue");

            //migrationBuilder.DropTable(
            //    name: "TransactionAccounts");

            //migrationBuilder.DropTable(
            //    name: "TransactionQueue");

            //migrationBuilder.DropTable(
            //    name: "WalletAllowTrns");

            //migrationBuilder.DropTable(
            //    name: "WalletLedgers");

            //migrationBuilder.DropTable(
            //    name: "WalletMasters");

            //migrationBuilder.DropTable(
            //    name: "WalletOrders");

            //migrationBuilder.DropTable(
            //    name: "WalletTransactionOrders");

            //migrationBuilder.DropTable(
            //    name: "WalletTransactionQueues");

            //migrationBuilder.DropTable(
            //    name: "WalletTypeMasters");

        //    migrationBuilder.AddColumn<decimal>(
        //        name: "Balance",
        //        table: "BizUser",
        //        nullable: false,
        //        defaultValue: 0m);

        //    migrationBuilder.AddColumn<long>(
        //        name: "OTP",
        //        table: "BizUser",
        //        nullable: false,
        //        defaultValue: 0L);
        }
    }
}
