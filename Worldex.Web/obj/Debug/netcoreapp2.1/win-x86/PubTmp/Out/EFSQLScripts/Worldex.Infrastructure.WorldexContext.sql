IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20190722103839_NewColumnInThirpartyArbitrage')
BEGIN
    ALTER TABLE [ArbitrageThirdPartyAPIConfiguration] ADD [ValidateMethodType] nvarchar(max) NULL;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20190722103839_NewColumnInThirpartyArbitrage')
BEGIN
    ALTER TABLE [ArbitrageThirdPartyAPIConfiguration] ADD [ValidateRequestBody] nvarchar(max) NULL;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20190722103839_NewColumnInThirpartyArbitrage')
BEGIN
    ALTER TABLE [AppType] ADD [AppTypeID] int NOT NULL DEFAULT 0;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20190722103839_NewColumnInThirpartyArbitrage')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20190722103839_NewColumnInThirpartyArbitrage', N'2.2.4-servicing-10062');
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20190723100609_addNewColToThirdPartyArbitrage')
BEGIN
    ALTER TABLE [ThirdPartyAPIResponseConfiguration] ADD [Param4Regex] nvarchar(max) NULL;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20190723100609_addNewColToThirdPartyArbitrage')
BEGIN
    ALTER TABLE [ThirdPartyAPIResponseConfiguration] ADD [Param5Regex] nvarchar(max) NULL;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20190723100609_addNewColToThirdPartyArbitrage')
BEGIN
    ALTER TABLE [ThirdPartyAPIResponseConfiguration] ADD [Param6Regex] nvarchar(max) NULL;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20190723100609_addNewColToThirdPartyArbitrage')
BEGIN
    ALTER TABLE [ThirdPartyAPIResponseConfiguration] ADD [Param7Regex] nvarchar(max) NULL;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20190723100609_addNewColToThirdPartyArbitrage')
BEGIN
    ALTER TABLE [CryptoWatcherArbitrage] ADD [UpdateDate] bigint NOT NULL DEFAULT CAST(0 AS bigint);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20190723100609_addNewColToThirdPartyArbitrage')
BEGIN
    ALTER TABLE [CryptoWatcherArbitrage] ADD [UpdatedBy] bigint NOT NULL DEFAULT CAST(0 AS bigint);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20190723100609_addNewColToThirdPartyArbitrage')
BEGIN
    ALTER TABLE [ArbitrageThirdPartyAPIConfiguration] ADD [APITickerURL] nvarchar(max) NULL;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20190723100609_addNewColToThirdPartyArbitrage')
BEGIN
    ALTER TABLE [ArbitrageThirdPartyAPIConfiguration] ADD [TickerMethodType] nvarchar(max) NULL;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20190723100609_addNewColToThirdPartyArbitrage')
BEGIN
    ALTER TABLE [ArbitrageThirdPartyAPIConfiguration] ADD [TickerResponseBody] nvarchar(max) NULL;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20190723100609_addNewColToThirdPartyArbitrage')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20190723100609_addNewColToThirdPartyArbitrage', N'2.2.4-servicing-10062');
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20190723111653_UpdateColNameToThirdPartyArbitrage')
BEGIN
    EXEC sp_rename N'[ArbitrageThirdPartyAPIConfiguration].[TickerResponseBody]', N'TickerRequestBody', N'COLUMN';
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20190723111653_UpdateColNameToThirdPartyArbitrage')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20190723111653_UpdateColNameToThirdPartyArbitrage', N'2.2.4-servicing-10062');
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20190724062339_AddnewColtoArbitrageAPIResponseEntity')
BEGIN
    DECLARE @var0 sysname;
    SELECT @var0 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ThirdPartyAPIResponseConfiguration]') AND [c].[name] = N'Param4Regex');
    IF @var0 IS NOT NULL EXEC(N'ALTER TABLE [ThirdPartyAPIResponseConfiguration] DROP CONSTRAINT [' + @var0 + '];');
    ALTER TABLE [ThirdPartyAPIResponseConfiguration] DROP COLUMN [Param4Regex];
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20190724062339_AddnewColtoArbitrageAPIResponseEntity')
BEGIN
    DECLARE @var1 sysname;
    SELECT @var1 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ThirdPartyAPIResponseConfiguration]') AND [c].[name] = N'Param5Regex');
    IF @var1 IS NOT NULL EXEC(N'ALTER TABLE [ThirdPartyAPIResponseConfiguration] DROP CONSTRAINT [' + @var1 + '];');
    ALTER TABLE [ThirdPartyAPIResponseConfiguration] DROP COLUMN [Param5Regex];
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20190724062339_AddnewColtoArbitrageAPIResponseEntity')
BEGIN
    DECLARE @var2 sysname;
    SELECT @var2 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ThirdPartyAPIResponseConfiguration]') AND [c].[name] = N'Param6Regex');
    IF @var2 IS NOT NULL EXEC(N'ALTER TABLE [ThirdPartyAPIResponseConfiguration] DROP CONSTRAINT [' + @var2 + '];');
    ALTER TABLE [ThirdPartyAPIResponseConfiguration] DROP COLUMN [Param6Regex];
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20190724062339_AddnewColtoArbitrageAPIResponseEntity')
BEGIN
    DECLARE @var3 sysname;
    SELECT @var3 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ThirdPartyAPIResponseConfiguration]') AND [c].[name] = N'Param7Regex');
    IF @var3 IS NOT NULL EXEC(N'ALTER TABLE [ThirdPartyAPIResponseConfiguration] DROP CONSTRAINT [' + @var3 + '];');
    ALTER TABLE [ThirdPartyAPIResponseConfiguration] DROP COLUMN [Param7Regex];
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20190724062339_AddnewColtoArbitrageAPIResponseEntity')
BEGIN
    ALTER TABLE [ArbitrageThirdPartyAPIResponseConfiguration] ADD [Param4Regex] nvarchar(max) NULL;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20190724062339_AddnewColtoArbitrageAPIResponseEntity')
BEGIN
    ALTER TABLE [ArbitrageThirdPartyAPIResponseConfiguration] ADD [Param5Regex] nvarchar(max) NULL;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20190724062339_AddnewColtoArbitrageAPIResponseEntity')
BEGIN
    ALTER TABLE [ArbitrageThirdPartyAPIResponseConfiguration] ADD [Param6Regex] nvarchar(max) NULL;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20190724062339_AddnewColtoArbitrageAPIResponseEntity')
BEGIN
    ALTER TABLE [ArbitrageThirdPartyAPIResponseConfiguration] ADD [Param7Regex] nvarchar(max) NULL;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20190724062339_AddnewColtoArbitrageAPIResponseEntity')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20190724062339_AddnewColtoArbitrageAPIResponseEntity', N'2.2.4-servicing-10062');
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20190725113531_ArbitrageThirdpartyCancellationUrl')
BEGIN
    DECLARE @var4 sysname;
    SELECT @var4 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[RouteConfigurationArbitrage]') AND [c].[name] = N'StatusCheckUrl');
    IF @var4 IS NOT NULL EXEC(N'ALTER TABLE [RouteConfigurationArbitrage] DROP CONSTRAINT [' + @var4 + '];');
    ALTER TABLE [RouteConfigurationArbitrage] DROP COLUMN [StatusCheckUrl];
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20190725113531_ArbitrageThirdpartyCancellationUrl')
BEGIN
    DECLARE @var5 sysname;
    SELECT @var5 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[RouteConfigurationArbitrage]') AND [c].[name] = N'TransactionUrl');
    IF @var5 IS NOT NULL EXEC(N'ALTER TABLE [RouteConfigurationArbitrage] DROP CONSTRAINT [' + @var5 + '];');
    ALTER TABLE [RouteConfigurationArbitrage] DROP COLUMN [TransactionUrl];
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20190725113531_ArbitrageThirdpartyCancellationUrl')
BEGIN
    DECLARE @var6 sysname;
    SELECT @var6 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[RouteConfigurationArbitrage]') AND [c].[name] = N'ValidationUrl');
    IF @var6 IS NOT NULL EXEC(N'ALTER TABLE [RouteConfigurationArbitrage] DROP CONSTRAINT [' + @var6 + '];');
    ALTER TABLE [RouteConfigurationArbitrage] DROP COLUMN [ValidationUrl];
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20190725113531_ArbitrageThirdpartyCancellationUrl')
BEGIN
    ALTER TABLE [ArbitrageThirdPartyAPIConfiguration] ADD [APICancellationURL] nvarchar(max) NULL;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20190725113531_ArbitrageThirdpartyCancellationUrl')
BEGIN
    ALTER TABLE [ArbitrageThirdPartyAPIConfiguration] ADD [CancellationMethodType] nvarchar(max) NULL;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20190725113531_ArbitrageThirdpartyCancellationUrl')
BEGIN
    ALTER TABLE [ArbitrageThirdPartyAPIConfiguration] ADD [CancellationRequestBody] nvarchar(max) NULL;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20190725113531_ArbitrageThirdpartyCancellationUrl')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20190725113531_ArbitrageThirdpartyCancellationUrl', N'2.2.4-servicing-10062');
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20190726052844_removeColumnUpdateByCryptoWAtcherArbitrage')
BEGIN
    DECLARE @var7 sysname;
    SELECT @var7 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[CryptoWatcherArbitrage]') AND [c].[name] = N'UpdateDate');
    IF @var7 IS NOT NULL EXEC(N'ALTER TABLE [CryptoWatcherArbitrage] DROP CONSTRAINT [' + @var7 + '];');
    ALTER TABLE [CryptoWatcherArbitrage] DROP COLUMN [UpdateDate];
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20190726052844_removeColumnUpdateByCryptoWAtcherArbitrage')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20190726052844_removeColumnUpdateByCryptoWAtcherArbitrage', N'2.2.4-servicing-10062');
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20190726053021_AddColumnUpdateByCryptoWAtcherArbitrage')
BEGIN
    ALTER TABLE [CryptoWatcherArbitrage] ADD [UpdateDate] datetime2 NULL;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20190726053021_AddColumnUpdateByCryptoWAtcherArbitrage')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20190726053021_AddColumnUpdateByCryptoWAtcherArbitrage', N'2.2.4-servicing-10062');
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20190726131116_AddIEOTables')
BEGIN
    CREATE TABLE [IEOPurchaseHistory] (
        [Id] bigint NOT NULL IDENTITY,
        [CreatedDate] datetime2 NOT NULL,
        [CreatedBy] bigint NOT NULL,
        [UpdatedBy] bigint NULL,
        [UpdatedDate] datetime2 NULL,
        [Status] smallint NOT NULL,
        [UserID] bigint NOT NULL,
        [Guid] nvarchar(450) NOT NULL,
        [RoundID] bigint NOT NULL,
        [PaidQuantity] decimal(28, 18) NOT NULL,
        [DeliveredQuantity] decimal(28, 18) NOT NULL,
        [CurrencyRate] decimal(28, 18) NOT NULL,
        [PaidCurrency] nvarchar(max) NOT NULL,
        [DeliveredCurrency] nvarchar(max) NOT NULL,
        [InstantQuantity] decimal(28, 18) NOT NULL,
        [MaximumDeliveredQuantiy] decimal(28, 18) NOT NULL,
        [OrgWalletID] bigint NOT NULL,
        [UserWalletID] bigint NOT NULL,
        [Remarks] nvarchar(max) NOT NULL,
        CONSTRAINT [PK_IEOPurchaseHistory] PRIMARY KEY ([Id])
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20190726131116_AddIEOTables')
BEGIN
    CREATE TABLE [IEOPurchaseWalletMaster] (
        [Id] bigint NOT NULL IDENTITY,
        [CreatedDate] datetime2 NOT NULL,
        [CreatedBy] bigint NOT NULL,
        [UpdatedBy] bigint NULL,
        [UpdatedDate] datetime2 NULL,
        [Status] smallint NOT NULL,
        [Guid] nvarchar(450) NOT NULL,
        [IEOWalletTypeId] bigint NOT NULL,
        [PurchaseWalletTypeId] bigint NOT NULL,
        [PurchaseRate] decimal(28, 18) NOT NULL,
        [ConvertCurrencyType] smallint NOT NULL,
        CONSTRAINT [PK_IEOPurchaseWalletMaster] PRIMARY KEY ([Id])
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20190726131116_AddIEOTables')
BEGIN
    CREATE TABLE [IEORoundMaster] (
        [Id] bigint NOT NULL IDENTITY,
        [CreatedDate] datetime2 NOT NULL,
        [CreatedBy] bigint NOT NULL,
        [UpdatedBy] bigint NULL,
        [UpdatedDate] datetime2 NULL,
        [Status] smallint NOT NULL,
        [Guid] nvarchar(450) NOT NULL,
        [StartDate] datetime2 NOT NULL,
        [EndDate] datetime2 NOT NULL,
        [IEOCurrencyId] bigint NOT NULL,
        [TotalSupply] decimal(28, 18) NOT NULL,
        [MinimumPurchaseAmt] decimal(28, 18) NOT NULL,
        [MaximumPurchaseAmt] decimal(28, 18) NOT NULL,
        [AllocatedSupply] decimal(28, 18) NOT NULL,
        [CurrencyRate] decimal(28, 18) NOT NULL,
        CONSTRAINT [PK_IEORoundMaster] PRIMARY KEY ([Id])
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20190726131116_AddIEOTables')
BEGIN
    CREATE TABLE [IEOSlabMaster] (
        [Id] bigint NOT NULL IDENTITY,
        [CreatedDate] datetime2 NOT NULL,
        [CreatedBy] bigint NOT NULL,
        [UpdatedBy] bigint NULL,
        [UpdatedDate] datetime2 NULL,
        [Status] smallint NOT NULL,
        [Guid] nvarchar(450) NOT NULL,
        [RoundId] bigint NOT NULL,
        [Priority] bigint NOT NULL,
        [Value] decimal(28, 18) NOT NULL,
        [Duration] bigint NOT NULL,
        [DurationType] smallint NOT NULL,
        CONSTRAINT [PK_IEOSlabMaster] PRIMARY KEY ([Id])
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20190726131116_AddIEOTables')
BEGIN
    CREATE TABLE [IROCronMaster] (
        [Id] bigint NOT NULL IDENTITY,
        [CreatedDate] datetime2 NOT NULL,
        [CreatedBy] bigint NOT NULL,
        [UpdatedBy] bigint NULL,
        [UpdatedDate] datetime2 NULL,
        [Status] smallint NOT NULL,
        [Guid] nvarchar(450) NOT NULL,
        [IEOPurchaseHistoryId] bigint NOT NULL,
        [MaturityDate] datetime2 NOT NULL,
        [RoundId] bigint NOT NULL,
        [UserId] bigint NOT NULL,
        [DeliveryQuantity] decimal(28, 18) NOT NULL,
        [DeliveryCurrency] nvarchar(max) NOT NULL,
        [CrWalletId] bigint NOT NULL,
        [DrWalletId] bigint NOT NULL,
        CONSTRAINT [PK_IROCronMaster] PRIMARY KEY ([Id])
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20190726131116_AddIEOTables')
BEGIN
    CREATE INDEX [IX_IEOPurchaseHistory_Guid] ON [IEOPurchaseHistory] ([Guid]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20190726131116_AddIEOTables')
BEGIN
    CREATE INDEX [IX_IEOPurchaseWalletMaster_Guid] ON [IEOPurchaseWalletMaster] ([Guid]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20190726131116_AddIEOTables')
BEGIN
    CREATE INDEX [IX_IEORoundMaster_Guid] ON [IEORoundMaster] ([Guid]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20190726131116_AddIEOTables')
BEGIN
    CREATE INDEX [IX_IEOSlabMaster_Guid] ON [IEOSlabMaster] ([Guid]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20190726131116_AddIEOTables')
BEGIN
    CREATE INDEX [IX_IROCronMaster_Guid] ON [IROCronMaster] ([Guid]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20190726131116_AddIEOTables')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20190726131116_AddIEOTables', N'2.2.4-servicing-10062');
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20190726134216_IEONameChanges')
BEGIN
    DROP TABLE [IROCronMaster];
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20190726134216_IEONameChanges')
BEGIN
    DECLARE @var8 sysname;
    SELECT @var8 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[IEOPurchaseHistory]') AND [c].[name] = N'PaidCurrency');
    IF @var8 IS NOT NULL EXEC(N'ALTER TABLE [IEOPurchaseHistory] DROP CONSTRAINT [' + @var8 + '];');
    ALTER TABLE [IEOPurchaseHistory] ALTER COLUMN [PaidCurrency] nvarchar(450) NOT NULL;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20190726134216_IEONameChanges')
BEGIN
    DECLARE @var9 sysname;
    SELECT @var9 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[IEOPurchaseHistory]') AND [c].[name] = N'DeliveredCurrency');
    IF @var9 IS NOT NULL EXEC(N'ALTER TABLE [IEOPurchaseHistory] DROP CONSTRAINT [' + @var9 + '];');
    ALTER TABLE [IEOPurchaseHistory] ALTER COLUMN [DeliveredCurrency] nvarchar(450) NOT NULL;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20190726134216_IEONameChanges')
BEGIN
    CREATE TABLE [IEOCronMaster] (
        [Id] bigint NOT NULL IDENTITY,
        [CreatedDate] datetime2 NOT NULL,
        [CreatedBy] bigint NOT NULL,
        [UpdatedBy] bigint NULL,
        [UpdatedDate] datetime2 NULL,
        [Status] smallint NOT NULL,
        [Guid] nvarchar(450) NOT NULL,
        [IEOPurchaseHistoryId] bigint NOT NULL,
        [MaturityDate] datetime2 NOT NULL,
        [RoundId] bigint NOT NULL,
        [UserId] bigint NOT NULL,
        [DeliveryQuantity] decimal(28, 18) NOT NULL,
        [DeliveryCurrency] nvarchar(max) NOT NULL,
        [CrWalletId] bigint NOT NULL,
        [DrWalletId] bigint NOT NULL,
        CONSTRAINT [PK_IEOCronMaster] PRIMARY KEY ([Id])
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20190726134216_IEONameChanges')
BEGIN
    CREATE INDEX [IX_IEOPurchaseHistory_DeliveredCurrency] ON [IEOPurchaseHistory] ([DeliveredCurrency]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20190726134216_IEONameChanges')
BEGIN
    CREATE INDEX [IX_IEOPurchaseHistory_PaidCurrency] ON [IEOPurchaseHistory] ([PaidCurrency]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20190726134216_IEONameChanges')
BEGIN
    CREATE INDEX [IX_IEOPurchaseHistory_UserID] ON [IEOPurchaseHistory] ([UserID]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20190726134216_IEONameChanges')
BEGIN
    CREATE INDEX [IX_IEOCronMaster_Guid] ON [IEOCronMaster] ([Guid]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20190726134216_IEONameChanges')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20190726134216_IEONameChanges', N'2.2.4-servicing-10062');
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20190726135947_AddTableAEOCurrencymaster')
BEGIN
    CREATE TABLE [IEOCurrencyMaster] (
        [Id] bigint NOT NULL IDENTITY,
        [CreatedDate] datetime2 NOT NULL,
        [CreatedBy] bigint NOT NULL,
        [UpdatedBy] bigint NULL,
        [UpdatedDate] datetime2 NULL,
        [Status] smallint NOT NULL,
        [Guid] nvarchar(450) NOT NULL,
        [IEOTokenTypeName] nvarchar(max) NOT NULL,
        [CurrencyName] nvarchar(max) NOT NULL,
        [Description] nvarchar(max) NULL,
        [Rounds] smallint NOT NULL,
        [IconPath] nvarchar(max) NULL,
        CONSTRAINT [PK_IEOCurrencyMaster] PRIMARY KEY ([Id])
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20190726135947_AddTableAEOCurrencymaster')
BEGIN
    CREATE INDEX [IX_IEOCurrencyMaster_Guid] ON [IEOCurrencyMaster] ([Guid]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20190726135947_AddTableAEOCurrencymaster')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20190726135947_AddTableAEOCurrencymaster', N'2.2.4-servicing-10062');
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20190727063551_TableRenameAndColumnAdd')
BEGIN
    DROP TABLE [IEOPurchaseWalletMaster];
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20190727063551_TableRenameAndColumnAdd')
BEGIN
    CREATE TABLE [IEOCurrencyPairMapping] (
        [Id] bigint NOT NULL IDENTITY,
        [CreatedDate] datetime2 NOT NULL,
        [CreatedBy] bigint NOT NULL,
        [UpdatedBy] bigint NULL,
        [UpdatedDate] datetime2 NULL,
        [Status] smallint NOT NULL,
        [Guid] nvarchar(450) NOT NULL,
        [IEOWalletTypeId] bigint NOT NULL,
        [PaidWalletTypeId] bigint NOT NULL,
        [PurchaseRate] decimal(28, 18) NOT NULL,
        [ConvertCurrencyType] smallint NOT NULL,
        [InstantPercentage] decimal(28, 18) NOT NULL,
        CONSTRAINT [PK_IEOCurrencyPairMapping] PRIMARY KEY ([Id])
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20190727063551_TableRenameAndColumnAdd')
BEGIN
    CREATE INDEX [IX_IEOCurrencyPairMapping_Guid] ON [IEOCurrencyPairMapping] ([Guid]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20190727063551_TableRenameAndColumnAdd')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20190727063551_TableRenameAndColumnAdd', N'2.2.4-servicing-10062');
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20190727114839_AddColumninIEORoundMaster')
BEGIN
    ALTER TABLE [IEORoundMaster] ADD [OccurrenceLimit] smallint NOT NULL DEFAULT CAST(0 AS smallint);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20190727114839_AddColumninIEORoundMaster')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20190727114839_AddColumninIEORoundMaster', N'2.2.4-servicing-10062');
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20190801071007_addIEOBannerMaster')
BEGIN
    CREATE TABLE [IEOBannerMaster] (
        [Id] bigint NOT NULL IDENTITY,
        [CreatedDate] datetime2 NOT NULL,
        [CreatedBy] bigint NOT NULL,
        [UpdatedBy] bigint NULL,
        [UpdatedDate] datetime2 NULL,
        [Status] smallint NOT NULL,
        [GUID] nvarchar(50) NOT NULL,
        [BannerPath] nvarchar(max) NOT NULL,
        [BannerName] nvarchar(50) NOT NULL,
        [Description] nvarchar(max) NOT NULL,
        [Message] nvarchar(max) NOT NULL,
        [TermsAndCondition] nvarchar(max) NOT NULL,
        [IsKYCReuired] smallint NOT NULL,
        CONSTRAINT [PK_IEOBannerMaster] PRIMARY KEY ([Id])
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20190801071007_addIEOBannerMaster')
BEGIN
    CREATE INDEX [IX_IEOBannerMaster_GUID] ON [IEOBannerMaster] ([GUID]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20190801071007_addIEOBannerMaster')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20190801071007_addIEOBannerMaster', N'2.2.4-servicing-10062');
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20190801101528_ChangeColSizeInStakingHistory')
BEGIN
    DECLARE @var10 sysname;
    SELECT @var10 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[TokenStakingHistory]') AND [c].[name] = N'MaturityAmount');
    IF @var10 IS NOT NULL EXEC(N'ALTER TABLE [TokenStakingHistory] DROP CONSTRAINT [' + @var10 + '];');
    ALTER TABLE [TokenStakingHistory] ALTER COLUMN [MaturityAmount] decimal(28, 18) NOT NULL;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20190801101528_ChangeColSizeInStakingHistory')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20190801101528_ChangeColSizeInStakingHistory', N'2.2.4-servicing-10062');
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20190802124208_AddSlabID')
BEGIN
    ALTER TABLE [IEOCronMaster] ADD [SlabID] bigint NOT NULL DEFAULT CAST(0 AS bigint);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20190802124208_AddSlabID')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20190802124208_AddSlabID', N'2.2.4-servicing-10062');
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20190805081912_addbonuscoulminIEO')
BEGIN
    ALTER TABLE [IEOSlabMaster] ADD [Bonus] decimal(28, 18) NOT NULL DEFAULT 0.0;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20190805081912_addbonuscoulminIEO')
BEGIN
    ALTER TABLE [IEORoundMaster] ADD [Bonus] decimal(28, 18) NOT NULL DEFAULT 0.0;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20190805081912_addbonuscoulminIEO')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20190805081912_addbonuscoulminIEO', N'2.2.4-servicing-10062');
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20190805102713_addpathAndRoundid')
BEGIN
    ALTER TABLE [IEORoundMaster] ADD [BGPath] nvarchar(max) NULL;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20190805102713_addpathAndRoundid')
BEGIN
    ALTER TABLE [IEOCurrencyPairMapping] ADD [RoundId] bigint NOT NULL DEFAULT CAST(0 AS bigint);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20190805102713_addpathAndRoundid')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20190805102713_addpathAndRoundid', N'2.2.4-servicing-10062');
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20190806060907_addIEOWalletAdminDepositTbl')
BEGIN
    CREATE TABLE [IEOWalletAdminDeposit] (
        [Id] bigint NOT NULL IDENTITY,
        [CreatedDate] datetime2 NOT NULL,
        [CreatedBy] bigint NOT NULL,
        [UpdatedBy] bigint NULL,
        [UpdatedDate] datetime2 NULL,
        [Status] smallint NOT NULL,
        [GUID] nvarchar(450) NOT NULL,
        [WalletId] bigint NOT NULL,
        [CurrencyName] nvarchar(7) NOT NULL,
        [UserId] bigint NOT NULL,
        [Remarks] nvarchar(500) NOT NULL,
        [SystemRemarks] nvarchar(500) NOT NULL,
        [Amount] decimal(28, 18) NOT NULL,
        [ApprovedBy] bigint NULL,
        [ApprovedDate] datetime2 NULL,
        CONSTRAINT [PK_IEOWalletAdminDeposit] PRIMARY KEY ([Id])
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20190806060907_addIEOWalletAdminDepositTbl')
BEGIN
    CREATE INDEX [IX_IEOWalletAdminDeposit_GUID] ON [IEOWalletAdminDeposit] ([GUID]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20190806060907_addIEOWalletAdminDepositTbl')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20190806060907_addIEOWalletAdminDepositTbl', N'2.2.4-servicing-10062');
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20190806120530_AddEmailBatchNo')
BEGIN
    ALTER TABLE [IEOCronMaster] ADD [EmailBatchNo] bigint NOT NULL DEFAULT CAST(0 AS bigint);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20190806120530_AddEmailBatchNo')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20190806120530_AddEmailBatchNo', N'2.2.4-servicing-10062');
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20190810080715_MinMaxNotional')
BEGIN
    ALTER TABLE [TradePairDetailMargin] ADD [MaxNotional] decimal(28, 18) NOT NULL DEFAULT 0.0;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20190810080715_MinMaxNotional')
BEGIN
    ALTER TABLE [TradePairDetailMargin] ADD [MinNotional] decimal(28, 18) NOT NULL DEFAULT 0.0;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20190810080715_MinMaxNotional')
BEGIN
    ALTER TABLE [TradePairDetail] ADD [MaxNotional] decimal(28, 18) NOT NULL DEFAULT 0.0;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20190810080715_MinMaxNotional')
BEGIN
    ALTER TABLE [TradePairDetail] ADD [MinNotional] decimal(28, 18) NOT NULL DEFAULT 0.0;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20190810080715_MinMaxNotional')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20190810080715_MinMaxNotional', N'2.2.4-servicing-10062');
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20190812092433_addnewcolinhistorytable')
BEGIN
    ALTER TABLE [IEOPurchaseHistory] ADD [BonusAmount] decimal(28, 18) NOT NULL DEFAULT 0.0;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20190812092433_addnewcolinhistorytable')
BEGIN
    ALTER TABLE [IEOPurchaseHistory] ADD [BonusPercentage] decimal(28, 18) NOT NULL DEFAULT 0.0;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20190812092433_addnewcolinhistorytable')
BEGIN
    ALTER TABLE [IEOPurchaseHistory] ADD [MaximumDeliveredQuantiyWOBonus] decimal(28, 18) NOT NULL DEFAULT 0.0;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20190812092433_addnewcolinhistorytable')
BEGIN
    ALTER TABLE [IEOPurchaseHistory] ADD [Rate] decimal(28, 18) NOT NULL DEFAULT 0.0;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20190812092433_addnewcolinhistorytable')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20190812092433_addnewcolinhistorytable', N'2.2.4-servicing-10062');
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20190813063424_addrateincurrencymaster')
BEGIN
    ALTER TABLE [IEOCurrencyMaster] ADD [Rate] decimal(28, 18) NOT NULL DEFAULT 0.0;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20190813063424_addrateincurrencymaster')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20190813063424_addrateincurrencymaster', N'2.2.4-servicing-10062');
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20190814071547_addIsConfirmedindeposithistory')
BEGIN
    ALTER TABLE [DepositHistory] ADD [IsConfirmed] bigint NOT NULL DEFAULT CAST(0 AS bigint);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20190814071547_addIsConfirmedindeposithistory')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20190814071547_addIsConfirmedindeposithistory', N'2.2.4-servicing-10062');
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20190826055442_addIsDescendingInDepositHistory')
BEGIN
    ALTER TABLE [DepositHistory] ADD [IsDescending] smallint NOT NULL DEFAULT CAST(0 AS smallint);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20190826055442_addIsDescendingInDepositHistory')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20190826055442_addIsDescendingInDepositHistory', N'2.2.4-servicing-10062');
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20190826060731_AddColsInTradePairDetail')
BEGIN
    ALTER TABLE [TradePairDetail] ADD [AmtLength] int NOT NULL DEFAULT 0;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20190826060731_AddColsInTradePairDetail')
BEGIN
    ALTER TABLE [TradePairDetail] ADD [PriceLength] int NOT NULL DEFAULT 0;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20190826060731_AddColsInTradePairDetail')
BEGIN
    ALTER TABLE [TradePairDetail] ADD [QtyLength] int NOT NULL DEFAULT 0;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20190826060731_AddColsInTradePairDetail')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20190826060731_AddColsInTradePairDetail', N'2.2.4-servicing-10062');
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20190827120932_AddColInTradePairDetailMargin')
BEGIN
    ALTER TABLE [TradePairDetailMargin] ADD [AmtLength] int NOT NULL DEFAULT 0;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20190827120932_AddColInTradePairDetailMargin')
BEGIN
    ALTER TABLE [TradePairDetailMargin] ADD [PriceLength] int NOT NULL DEFAULT 0;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20190827120932_AddColInTradePairDetailMargin')
BEGIN
    ALTER TABLE [TradePairDetailMargin] ADD [QtyLength] int NOT NULL DEFAULT 0;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20190827120932_AddColInTradePairDetailMargin')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20190827120932_AddColInTradePairDetailMargin', N'2.2.4-servicing-10062');
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20190903104449_chnageaddresslength')
BEGIN
    DECLARE @var11 sysname;
    SELECT @var11 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[AddressMasters]') AND [c].[name] = N'OriginalAddress');
    IF @var11 IS NOT NULL EXEC(N'ALTER TABLE [AddressMasters] DROP CONSTRAINT [' + @var11 + '];');
    ALTER TABLE [AddressMasters] ALTER COLUMN [OriginalAddress] nvarchar(160) NOT NULL;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20190903104449_chnageaddresslength')
BEGIN
    DECLARE @var12 sysname;
    SELECT @var12 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[AddressMasters]') AND [c].[name] = N'Address');
    IF @var12 IS NOT NULL EXEC(N'ALTER TABLE [AddressMasters] DROP CONSTRAINT [' + @var12 + '];');
    ALTER TABLE [AddressMasters] ALTER COLUMN [Address] nvarchar(160) NULL;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20190903104449_chnageaddresslength')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20190903104449_chnageaddresslength', N'2.2.4-servicing-10062');
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20190906072742_addDestinationTagAddressMaster')
BEGIN
    ALTER TABLE [AddressMasters] ADD [DestinationTag] nvarchar(max) NULL;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20190906072742_addDestinationTagAddressMaster')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20190906072742_addDestinationTagAddressMaster', N'2.2.4-servicing-10062');
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20190916063242_AddNewChargeTable')
BEGIN
    ALTER TABLE [WalletTransactionQueues] ADD [TradingChargeType] int NOT NULL DEFAULT 0;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20190916063242_AddNewChargeTable')
BEGIN
    DECLARE @var13 sysname;
    SELECT @var13 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[TradingChargeTypeMaster]') AND [c].[name] = N'Type');
    IF @var13 IS NOT NULL EXEC(N'ALTER TABLE [TradingChargeTypeMaster] DROP CONSTRAINT [' + @var13 + '];');
    ALTER TABLE [TradingChargeTypeMaster] ALTER COLUMN [Type] int NOT NULL;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20190916063242_AddNewChargeTable')
BEGIN
    ALTER TABLE [TradingChargeTypeMaster] ADD [DeductCurrency] nvarchar(max) NOT NULL DEFAULT N'';
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20190916063242_AddNewChargeTable')
BEGIN
    ALTER TABLE [TradingChargeTypeMaster] ADD [DiscountPercent] decimal(28, 18) NOT NULL DEFAULT 0.0;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20190916063242_AddNewChargeTable')
BEGIN
    ALTER TABLE [TradingChargeTypeMaster] ADD [IsChargeFreeMarketEnabled] smallint NOT NULL DEFAULT CAST(0 AS smallint);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20190916063242_AddNewChargeTable')
BEGIN
    ALTER TABLE [TradingChargeTypeMaster] ADD [IsCommonCurrencyDeductEnable] smallint NOT NULL DEFAULT CAST(0 AS smallint);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20190916063242_AddNewChargeTable')
BEGIN
    ALTER TABLE [TradingChargeTypeMaster] ADD [IsDeductChargeMarketCurrency] smallint NOT NULL DEFAULT CAST(0 AS smallint);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20190916063242_AddNewChargeTable')
BEGIN
    CREATE TABLE [ChargeFreeMarketCurrencyMaster] (
        [MarketCurrency] nvarchar(7) NOT NULL,
        [CreatedDate] datetime2 NOT NULL,
        [CreatedBy] bigint NOT NULL,
        [UpdatedBy] bigint NULL,
        [UpdatedDate] datetime2 NULL,
        [Status] smallint NOT NULL,
        [Id] bigint NOT NULL IDENTITY,
        CONSTRAINT [PK_ChargeFreeMarketCurrencyMaster] PRIMARY KEY ([MarketCurrency])
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20190916063242_AddNewChargeTable')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20190916063242_AddNewChargeTable', N'2.2.4-servicing-10062');
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20190917051507_updatechargelogentity')
BEGIN
    ALTER TABLE [TrnChargeLog] ADD [DeductCurrency] nvarchar(max) NULL;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20190917051507_updatechargelogentity')
BEGIN
    ALTER TABLE [TrnChargeLog] ADD [DiscountPercent] decimal(28, 18) NOT NULL DEFAULT 0.0;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20190917051507_updatechargelogentity')
BEGIN
    ALTER TABLE [TrnChargeLog] ADD [Type] int NOT NULL DEFAULT 0;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20190917051507_updatechargelogentity')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20190917051507_updatechargelogentity', N'2.2.4-servicing-10062');
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20191002140540_AddColInBizUser')
BEGIN
    DECLARE @var14 sysname;
    SELECT @var14 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[LPWalletMismatch]') AND [c].[name] = N'ResolvedDate');
    IF @var14 IS NOT NULL EXEC(N'ALTER TABLE [LPWalletMismatch] DROP CONSTRAINT [' + @var14 + '];');
    ALTER TABLE [LPWalletMismatch] ALTER COLUMN [ResolvedDate] datetime2 NULL;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20191002140540_AddColInBizUser')
BEGIN
    DECLARE @var15 sysname;
    SELECT @var15 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[LPWalletMismatch]') AND [c].[name] = N'ResolvedBy');
    IF @var15 IS NOT NULL EXEC(N'ALTER TABLE [LPWalletMismatch] DROP CONSTRAINT [' + @var15 + '];');
    ALTER TABLE [LPWalletMismatch] ALTER COLUMN [ResolvedBy] bigint NULL;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20191002140540_AddColInBizUser')
BEGIN
    ALTER TABLE [LPWalletMismatch] ADD [Guid] nvarchar(max) NULL;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20191002140540_AddColInBizUser')
BEGIN
    ALTER TABLE [LPWalletMismatch] ADD [SettledAmount] decimal(28, 18) NOT NULL DEFAULT 0.0;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20191002140540_AddColInBizUser')
BEGIN
    ALTER TABLE [LPWalletMismatch] ADD [StatusMsg] nvarchar(max) NULL;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20191002140540_AddColInBizUser')
BEGIN
    ALTER TABLE [BizUser] ADD [IsDeviceAuthEnable] smallint NOT NULL DEFAULT CAST(0 AS smallint);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20191002140540_AddColInBizUser')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20191002140540_AddColInBizUser', N'2.2.4-servicing-10062');
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20191003103934_addIsPrivateAPITardeColToTrdaeTransactionQueue')
BEGIN
    ALTER TABLE [TradeTransactionQueueMargin] ADD [IsPrivateAPITarde] smallint NOT NULL DEFAULT CAST(0 AS smallint);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20191003103934_addIsPrivateAPITardeColToTrdaeTransactionQueue')
BEGIN
    ALTER TABLE [TradeTransactionQueue] ADD [IsPrivateAPITarde] smallint NOT NULL DEFAULT CAST(0 AS smallint);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20191003103934_addIsPrivateAPITardeColToTrdaeTransactionQueue')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20191003103934_addIsPrivateAPITardeColToTrdaeTransactionQueue', N'2.2.4-servicing-10062');
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20191009063217_AddBuySellTopUpRequestEntity')
BEGIN
    CREATE TABLE [BuySellTopUpRequest] (
        [Id] bigint NOT NULL IDENTITY,
        [CreatedDate] datetime2 NOT NULL,
        [CreatedBy] bigint NOT NULL,
        [UpdatedBy] bigint NULL,
        [UpdatedDate] datetime2 NULL,
        [Status] smallint NOT NULL,
        [Guid] nvarchar(450) NOT NULL,
        [FromAmount] decimal(28, 18) NOT NULL,
        [ToAmount] decimal(28, 18) NOT NULL,
        [CoinRate] decimal(28, 18) NOT NULL,
        [FiatConverationRate] decimal(28, 18) NOT NULL,
        [Fee] decimal(28, 18) NOT NULL,
        [UserId] bigint NOT NULL,
        [FromCurrency] nvarchar(max) NOT NULL,
        [ToCurrency] nvarchar(max) NOT NULL,
        [Address] nvarchar(max) NOT NULL,
        [TransactionHash] nvarchar(max) NOT NULL,
        [NotifyUrl] nvarchar(max) NOT NULL,
        [TransactionId] nvarchar(max) NOT NULL,
        [TransactionCode] nvarchar(max) NOT NULL,
        [UserGuid] nvarchar(max) NOT NULL,
        [Platform] nvarchar(max) NOT NULL,
        [Type] smallint NOT NULL,
        [FromBankId] bigint NOT NULL,
        [ToBankId] bigint NOT NULL,
        [Code] nvarchar(max) NOT NULL,
        CONSTRAINT [PK_BuySellTopUpRequest] PRIMARY KEY ([Id])
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20191009063217_AddBuySellTopUpRequestEntity')
BEGIN
    CREATE INDEX [IX_BuySellTopUpRequest_Guid] ON [BuySellTopUpRequest] ([Guid]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20191009063217_AddBuySellTopUpRequestEntity')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20191009063217_AddBuySellTopUpRequestEntity', N'2.2.4-servicing-10062');
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20191009081648_EntityUserBankRequest')
BEGIN
    CREATE TABLE [UserBankRequest] (
        [Id] bigint NOT NULL IDENTITY,
        [CreatedDate] datetime2 NOT NULL,
        [CreatedBy] bigint NOT NULL,
        [UpdatedBy] bigint NULL,
        [UpdatedDate] datetime2 NULL,
        [Status] smallint NOT NULL,
        [GUID] nvarchar(450) NULL,
        [UserId] bigint NOT NULL,
        [BankName] nvarchar(100) NOT NULL,
        [BankCode] nvarchar(50) NOT NULL,
        [BankAccountNumber] nvarchar(50) NOT NULL,
        [BankAcountHolderName] nvarchar(100) NOT NULL,
        [CurrencyCode] nvarchar(5) NOT NULL,
        [CountryCode] nvarchar(5) NOT NULL,
        CONSTRAINT [PK_UserBankRequest] PRIMARY KEY ([Id])
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20191009081648_EntityUserBankRequest')
BEGIN
    CREATE INDEX [IX_UserBankRequest_GUID] ON [UserBankRequest] ([GUID]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20191009081648_EntityUserBankRequest')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20191009081648_EntityUserBankRequest', N'2.2.4-servicing-10062');
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20191009125439_addremarkscol')
BEGIN
    ALTER TABLE [BuySellTopUpRequest] ADD [Remarks] nvarchar(max) NULL;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20191009125439_addremarkscol')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20191009125439_addremarkscol', N'2.2.4-servicing-10062');
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20191010052448_EntityUserBankMaster')
BEGIN
    CREATE TABLE [UserBankMaster] (
        [Id] bigint NOT NULL IDENTITY,
        [CreatedDate] datetime2 NOT NULL,
        [CreatedBy] bigint NOT NULL,
        [UpdatedBy] bigint NULL,
        [UpdatedDate] datetime2 NULL,
        [Status] smallint NOT NULL,
        [GUID] nvarchar(450) NULL,
        [UserId] bigint NOT NULL,
        [BankName] nvarchar(100) NOT NULL,
        [BankCode] nvarchar(50) NOT NULL,
        [BankAccountNumber] nvarchar(50) NOT NULL,
        [BankAccountHolderName] nvarchar(100) NOT NULL,
        [CurrencyCode] nvarchar(5) NOT NULL,
        [CountryCode] nvarchar(5) NOT NULL,
        CONSTRAINT [PK_UserBankMaster] PRIMARY KEY ([Id])
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20191010052448_EntityUserBankMaster')
BEGIN
    CREATE INDEX [IX_UserBankMaster_GUID] ON [UserBankMaster] ([GUID]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20191010052448_EntityUserBankMaster')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20191010052448_EntityUserBankMaster', N'2.2.4-servicing-10062');
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20191010060542_AddFiatTradeConfigurationMaster')
BEGIN
    CREATE TABLE [FiatTradeConfigurationMaster] (
        [Id] bigint NOT NULL IDENTITY,
        [CreatedDate] datetime2 NOT NULL,
        [CreatedBy] bigint NOT NULL,
        [UpdatedBy] bigint NULL,
        [UpdatedDate] datetime2 NULL,
        [Status] smallint NOT NULL,
        [BuyFee] decimal(28, 18) NOT NULL,
        [SellFee] decimal(28, 18) NOT NULL,
        [TermsAndCondition] nvarchar(max) NOT NULL,
        [IsBuyEnable] smallint NOT NULL,
        [IsSellEnable] smallint NOT NULL,
        [BuyFeeType] smallint NOT NULL,
        [SellFeeType] smallint NOT NULL,
        CONSTRAINT [PK_FiatTradeConfigurationMaster] PRIMARY KEY ([Id])
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20191010060542_AddFiatTradeConfigurationMaster')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20191010060542_AddFiatTradeConfigurationMaster', N'2.2.4-servicing-10062');
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20191010093737_AddNewResponseParam')
BEGIN
    ALTER TABLE [BuySellTopUpRequest] ADD [Bank] nvarchar(max) NULL;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20191010093737_AddNewResponseParam')
BEGIN
    ALTER TABLE [BuySellTopUpRequest] ADD [Currency] nvarchar(max) NULL;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20191010093737_AddNewResponseParam')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20191010093737_AddNewResponseParam', N'2.2.4-servicing-10062');
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20191012070357_addnewparamforFiat')
BEGIN
    EXEC sp_rename N'[BuySellTopUpRequest].[Currency]', N'CurrencyName', N'COLUMN';
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20191012070357_addnewparamforFiat')
BEGIN
    EXEC sp_rename N'[BuySellTopUpRequest].[Bank]', N'CurrencyId', N'COLUMN';
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20191012070357_addnewparamforFiat')
BEGIN
    ALTER TABLE [BuySellTopUpRequest] ADD [BankId] nvarchar(max) NULL;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20191012070357_addnewparamforFiat')
BEGIN
    ALTER TABLE [BuySellTopUpRequest] ADD [BankName] nvarchar(max) NULL;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20191012070357_addnewparamforFiat')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20191012070357_addnewparamforFiat', N'2.2.4-servicing-10062');
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20191012074504_AddColInFiatTradeConfigurationMaster')
BEGIN
    ALTER TABLE [FiatTradeConfigurationMaster] ADD [BuyNotifyURL] nvarchar(250) NULL;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20191012074504_AddColInFiatTradeConfigurationMaster')
BEGIN
    ALTER TABLE [FiatTradeConfigurationMaster] ADD [CallBackURL] nvarchar(250) NULL;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20191012074504_AddColInFiatTradeConfigurationMaster')
BEGIN
    ALTER TABLE [FiatTradeConfigurationMaster] ADD [EncryptionKey] nvarchar(50) NULL;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20191012074504_AddColInFiatTradeConfigurationMaster')
BEGIN
    ALTER TABLE [FiatTradeConfigurationMaster] ADD [SellNotifyURL] nvarchar(250) NULL;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20191012074504_AddColInFiatTradeConfigurationMaster')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20191012074504_AddColInFiatTradeConfigurationMaster', N'2.2.4-servicing-10062');
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20191014070103_addratein')
BEGIN
    ALTER TABLE [WalletTypeMasters] ADD [Rate] decimal(28, 18) NOT NULL DEFAULT 0.0;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20191014070103_addratein')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20191014070103_addratein', N'2.2.4-servicing-10062');
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20191014105312_addSellTopUpRequest')
BEGIN
    ALTER TABLE [FiatTradeConfigurationMaster] ADD [Platform] nvarchar(250) NULL;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20191014105312_addSellTopUpRequest')
BEGIN
    ALTER TABLE [FiatTradeConfigurationMaster] ADD [SellCallBackURL] nvarchar(250) NULL;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20191014105312_addSellTopUpRequest')
BEGIN
    ALTER TABLE [FiatTradeConfigurationMaster] ADD [WithdrawURL] nvarchar(250) NULL;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20191014105312_addSellTopUpRequest')
BEGIN
    CREATE TABLE [SellTopUpRequest] (
        [Id] bigint NOT NULL IDENTITY,
        [CreatedDate] datetime2 NOT NULL,
        [CreatedBy] bigint NOT NULL,
        [UpdatedBy] bigint NULL,
        [UpdatedDate] datetime2 NULL,
        [Status] smallint NOT NULL,
        [Guid] nvarchar(max) NOT NULL,
        [FromAmount] decimal(28, 18) NOT NULL,
        [ToAmount] decimal(28, 18) NOT NULL,
        [CoinRate] decimal(28, 18) NOT NULL,
        [FiatConverationRate] decimal(28, 18) NOT NULL,
        [Fee] decimal(28, 18) NOT NULL,
        [UserId] bigint NOT NULL,
        [FromCurrency] nvarchar(max) NOT NULL,
        [ToCurrency] nvarchar(max) NOT NULL,
        [Address] nvarchar(max) NOT NULL,
        [TransactionHash] nvarchar(max) NOT NULL,
        [NotifyUrl] nvarchar(max) NOT NULL,
        [TransactionId] nvarchar(max) NOT NULL,
        [TransactionCode] nvarchar(max) NOT NULL,
        [UserGuid] nvarchar(max) NOT NULL,
        [Platform] nvarchar(max) NOT NULL,
        [Type] smallint NOT NULL,
        [FromBankId] bigint NOT NULL,
        [ToBankId] bigint NOT NULL,
        [Code] nvarchar(max) NOT NULL,
        [Remarks] nvarchar(max) NULL,
        [BankName] nvarchar(max) NULL,
        [CurrencyName] nvarchar(max) NULL,
        [BankId] nvarchar(max) NULL,
        [CurrencyId] nvarchar(max) NULL,
        [user_bank_name] nvarchar(max) NULL,
        [user_bank_account_number] nvarchar(max) NULL,
        [user_bank_acount_holder_name] nvarchar(max) NULL,
        [user_currency_code] nvarchar(max) NULL,
        [payus_transaction_id] nvarchar(max) NULL,
        [payus_amount_usd] decimal(18, 2) NOT NULL,
        [payus_amount_crypto] decimal(18, 2) NOT NULL,
        [payus_mining_fees] decimal(18, 2) NOT NULL,
        [payus_total_payable_amount] decimal(18, 2) NOT NULL,
        [payus_fees] decimal(18, 2) NOT NULL,
        [payus_total_fees] decimal(18, 2) NOT NULL,
        [payus_usd_rate] decimal(18, 2) NOT NULL,
        [payus_expire_datetime] datetime2 NOT NULL,
        [payus_payment_tracking] nvarchar(max) NULL,
        CONSTRAINT [PK_SellTopUpRequest] PRIMARY KEY ([Id])
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20191014105312_addSellTopUpRequest')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20191014105312_addSellTopUpRequest', N'2.2.4-servicing-10062');
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20191014115456_AddNewColInFiatTradeConfigurationMaster')
BEGIN
    ALTER TABLE [FiatTradeConfigurationMaster] ADD [FiatCurrencyName] nvarchar(50) NOT NULL DEFAULT N'';
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20191014115456_AddNewColInFiatTradeConfigurationMaster')
BEGIN
    ALTER TABLE [FiatTradeConfigurationMaster] ADD [FiatCurrencyRate] decimal(28, 18) NOT NULL DEFAULT 0.0;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20191014115456_AddNewColInFiatTradeConfigurationMaster')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20191014115456_AddNewColInFiatTradeConfigurationMaster', N'2.2.4-servicing-10062');
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20191014120804_Add2NewColInFiatTradeConfigurationMaster')
BEGIN
    ALTER TABLE [FiatTradeConfigurationMaster] ADD [MaxLimit] decimal(28, 18) NOT NULL DEFAULT 0.0;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20191014120804_Add2NewColInFiatTradeConfigurationMaster')
BEGIN
    ALTER TABLE [FiatTradeConfigurationMaster] ADD [MinLimit] decimal(28, 18) NOT NULL DEFAULT 0.0;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20191014120804_Add2NewColInFiatTradeConfigurationMaster')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20191014120804_Add2NewColInFiatTradeConfigurationMaster', N'2.2.4-servicing-10062');
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20191014130411_addFiatCoinConfiguration')
BEGIN
    CREATE TABLE [FiatCoinConfiguration] (
        [FromCurrencyId] bigint NOT NULL,
        [ToCurrencyId] bigint NOT NULL,
        [CreatedDate] datetime2 NOT NULL,
        [CreatedBy] bigint NOT NULL,
        [UpdatedBy] bigint NULL,
        [UpdatedDate] datetime2 NULL,
        [Status] smallint NOT NULL,
        [Id] bigint NOT NULL IDENTITY,
        [MinQty] decimal(28, 18) NOT NULL,
        [MaxQty] decimal(28, 18) NOT NULL,
        [MinAmount] decimal(28, 18) NOT NULL,
        [MaxAmount] decimal(28, 18) NOT NULL,
        [BuyFee] decimal(28, 18) NOT NULL,
        [SellFee] decimal(28, 18) NOT NULL,
        CONSTRAINT [PK_FiatCoinConfiguration] PRIMARY KEY ([FromCurrencyId], [ToCurrencyId])
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20191014130411_addFiatCoinConfiguration')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20191014130411_addFiatCoinConfiguration', N'2.2.4-servicing-10062');
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20191016065210_FiatCurrencyMaster')
BEGIN
    EXEC sp_rename N'[FiatCoinConfiguration].[SellFee]', N'Rate', N'COLUMN';
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20191016065210_FiatCurrencyMaster')
BEGIN
    EXEC sp_rename N'[FiatCoinConfiguration].[BuyFee]', N'MinRate', N'COLUMN';
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20191016065210_FiatCurrencyMaster')
BEGIN
    CREATE TABLE [FiatCurrencyMaster] (
        [Id] bigint NOT NULL IDENTITY,
        [CreatedDate] datetime2 NOT NULL,
        [CreatedBy] bigint NOT NULL,
        [UpdatedBy] bigint NULL,
        [UpdatedDate] datetime2 NULL,
        [Status] smallint NOT NULL,
        [Name] nvarchar(max) NOT NULL,
        [CurrencyCode] nvarchar(max) NOT NULL,
        [USDRate] decimal(28, 18) NOT NULL,
        [BuyFee] decimal(28, 18) NOT NULL,
        [SellFee] decimal(28, 18) NOT NULL,
        [BuyFeeType] smallint NOT NULL,
        [SellFeeType] smallint NOT NULL,
        CONSTRAINT [PK_FiatCurrencyMaster] PRIMARY KEY ([Id])
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20191016065210_FiatCurrencyMaster')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20191016065210_FiatCurrencyMaster', N'2.2.4-servicing-10062');
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20191018084057_AddNewColInUserBankRequest')
BEGIN
    ALTER TABLE [UserBankRequest] ADD [RequestType] smallint NOT NULL DEFAULT CAST(0 AS smallint);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20191018084057_AddNewColInUserBankRequest')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20191018084057_AddNewColInUserBankRequest', N'2.2.4-servicing-10062');
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20191021130735_addTransactionTypeinFiat')
BEGIN
    ALTER TABLE [FiatCoinConfiguration] DROP CONSTRAINT [PK_FiatCoinConfiguration];
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20191021130735_addTransactionTypeinFiat')
BEGIN
    ALTER TABLE [FiatCoinConfiguration] ADD [TransactionType] smallint NOT NULL DEFAULT CAST(0 AS smallint);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20191021130735_addTransactionTypeinFiat')
BEGIN
    ALTER TABLE [FiatCoinConfiguration] ADD CONSTRAINT [PK_FiatCoinConfiguration] PRIMARY KEY ([FromCurrencyId], [ToCurrencyId], [TransactionType]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20191021130735_addTransactionTypeinFiat')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20191021130735_addTransactionTypeinFiat', N'2.2.4-servicing-10062');
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20191022060358_addremarksinbankfiat')
BEGIN
    ALTER TABLE [UserBankRequest] ADD [Remarks] nvarchar(max) NULL;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20191022060358_addremarksinbankfiat')
BEGIN
    ALTER TABLE [UserBankMaster] ADD [Remarks] nvarchar(max) NULL;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20191022060358_addremarksinbankfiat')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20191022060358_addremarksinbankfiat', N'2.2.4-servicing-10062');
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20191101072202_add-TrnNoStatusCol')
BEGIN
    ALTER TABLE [SellTopUpRequest] ADD [APIStatus] smallint NOT NULL DEFAULT CAST(0 AS smallint);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20191101072202_add-TrnNoStatusCol')
BEGIN
    ALTER TABLE [SellTopUpRequest] ADD [TrnNo] bigint NOT NULL DEFAULT CAST(0 AS bigint);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20191101072202_add-TrnNoStatusCol')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20191101072202_add-TrnNoStatusCol', N'2.2.4-servicing-10062');
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20191121070053_Increasesizecoinname')
BEGIN
    DECLARE @var16 sysname;
    SELECT @var16 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[WithdrawHistory]') AND [c].[name] = N'ToAddress');
    IF @var16 IS NOT NULL EXEC(N'ALTER TABLE [WithdrawHistory] DROP CONSTRAINT [' + @var16 + '];');
    ALTER TABLE [WithdrawHistory] ALTER COLUMN [ToAddress] nvarchar(200) NOT NULL;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20191121070053_Increasesizecoinname')
BEGIN
    DECLARE @var17 sysname;
    SELECT @var17 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[WithdrawHistory]') AND [c].[name] = N'Address');
    IF @var17 IS NOT NULL EXEC(N'ALTER TABLE [WithdrawHistory] DROP CONSTRAINT [' + @var17 + '];');
    ALTER TABLE [WithdrawHistory] ALTER COLUMN [Address] nvarchar(200) NULL;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20191121070053_Increasesizecoinname')
BEGIN
    DECLARE @var18 sysname;
    SELECT @var18 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[WalletTypeMasters]') AND [c].[name] = N'WalletTypeName');
    IF @var18 IS NOT NULL EXEC(N'ALTER TABLE [WalletTypeMasters] DROP CONSTRAINT [' + @var18 + '];');
    ALTER TABLE [WalletTypeMasters] ALTER COLUMN [WalletTypeName] nvarchar(10) NOT NULL;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20191121070053_Increasesizecoinname')
BEGIN
    DECLARE @var19 sysname;
    SELECT @var19 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ServiceMaster]') AND [c].[name] = N'SMSCode');
    IF @var19 IS NOT NULL EXEC(N'ALTER TABLE [ServiceMaster] DROP CONSTRAINT [' + @var19 + '];');
    ALTER TABLE [ServiceMaster] ALTER COLUMN [SMSCode] nvarchar(10) NOT NULL;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20191121070053_Increasesizecoinname')
BEGIN
    DECLARE @var20 sysname;
    SELECT @var20 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[DepositHistory]') AND [c].[name] = N'FromAddress');
    IF @var20 IS NOT NULL EXEC(N'ALTER TABLE [DepositHistory] DROP CONSTRAINT [' + @var20 + '];');
    ALTER TABLE [DepositHistory] ALTER COLUMN [FromAddress] nvarchar(200) NOT NULL;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20191121070053_Increasesizecoinname')
BEGIN
    DECLARE @var21 sysname;
    SELECT @var21 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[DepositHistory]') AND [c].[name] = N'Address');
    IF @var21 IS NOT NULL EXEC(N'ALTER TABLE [DepositHistory] DROP CONSTRAINT [' + @var21 + '];');
    ALTER TABLE [DepositHistory] ALTER COLUMN [Address] nvarchar(200) NOT NULL;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20191121070053_Increasesizecoinname')
BEGIN
    DECLARE @var22 sysname;
    SELECT @var22 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[AddressMasters]') AND [c].[name] = N'OriginalAddress');
    IF @var22 IS NOT NULL EXEC(N'ALTER TABLE [AddressMasters] DROP CONSTRAINT [' + @var22 + '];');
    ALTER TABLE [AddressMasters] ALTER COLUMN [OriginalAddress] nvarchar(200) NOT NULL;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20191121070053_Increasesizecoinname')
BEGIN
    DECLARE @var23 sysname;
    SELECT @var23 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[AddressMasters]') AND [c].[name] = N'Address');
    IF @var23 IS NOT NULL EXEC(N'ALTER TABLE [AddressMasters] DROP CONSTRAINT [' + @var23 + '];');
    ALTER TABLE [AddressMasters] ALTER COLUMN [Address] nvarchar(200) NULL;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20191121070053_Increasesizecoinname')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20191121070053_Increasesizecoinname', N'2.2.4-servicing-10062');
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200106111623_AddNewColInTradePairDetail')
BEGIN
    ALTER TABLE [TradePairDetail] ADD [PairPercentage] decimal(28, 18) NOT NULL DEFAULT 0.0;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200106111623_AddNewColInTradePairDetail')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20200106111623_AddNewColInTradePairDetail', N'2.2.4-servicing-10062');
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200212100034_20200212_NewColumnAddInThirdpartyAPIResponseConfiguration')
BEGIN
    ALTER TABLE [ThirdPartyAPIResponseConfiguration] ADD [ParsingName] nvarchar(max) NULL;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200212100034_20200212_NewColumnAddInThirdpartyAPIResponseConfiguration')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20200212100034_20200212_NewColumnAddInThirdpartyAPIResponseConfiguration', N'2.2.4-servicing-10062');
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200428102458_AddGUIDColInIPMaster')
BEGIN
    ALTER TABLE [IpMaster] ADD [GUID] uniqueidentifier NULL;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200428102458_AddGUIDColInIPMaster')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20200428102458_AddGUIDColInIPMaster', N'2.2.4-servicing-10062');
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200707145714_AddedNewColInPersonalVerification')
BEGIN
    DECLARE @var24 sysname;
    SELECT @var24 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[IpMaster]') AND [c].[name] = N'GUID');
    IF @var24 IS NOT NULL EXEC(N'ALTER TABLE [IpMaster] DROP CONSTRAINT [' + @var24 + '];');
    ALTER TABLE [IpMaster] DROP COLUMN [GUID];
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200707145714_AddedNewColInPersonalVerification')
BEGIN
    ALTER TABLE [PersonalVerification] ADD [IdentityDocNumber] nvarchar(100) NOT NULL DEFAULT N'';
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200707145714_AddedNewColInPersonalVerification')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20200707145714_AddedNewColInPersonalVerification', N'2.2.4-servicing-10062');
END;

GO

