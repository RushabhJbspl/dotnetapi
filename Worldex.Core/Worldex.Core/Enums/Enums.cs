namespace Worldex.Core.Enums
{
    public enum ExternalLoginStatus
    {
        Ok = 0,
        Error = 1,
        Invalid = 2,
        TwoFactor = 3,
        Lockout = 4,
        CreateAccount = 5
    }

    public enum EnUserActivity
    {
        Top_10_Gainer = 1,
        Top_10_Looser = 2,
        Inactive_Users = 3,
        Email_Not_Confirmed_Users = 4,
        Email_Confirmed_Users = 5,
        Users_Who_Reset_Password = 6,
        Users_Who_Forgot_Password = 7,
        Kyc_Verified_Users = 8,
        Kyc_Unverified_Users = 9,
        Low_Balance = 10,
        New_Registed_Users = 11,
        Today_Logged_In_Users = 12,
        Last_Login_Before_30_Days = 13,
        Highest_Trading_Volume = 14,
        Lowest_Trading_Volume = 15,
        Highest_Withdraw_Volume = 16,
        Lowest_Withdraw_Volume = 17,
        Highest_Deposition_Volume = 18,
        Lowest_Deposition_Volume = 19
    }

    public enum EnAffiCommisionTypeInterval
    {
        PerTransaction = 0,
        Hourly = 1,
        Daily = 2,
        Weekly = 3,
    }

    public enum EnAccessStatus
    {
        Read = 0,
        Write = 1,
        Invisible = 9
    }

    public enum EnConfigChargeType
    {
        Regular = 1,
        Recurring = 2
    }

    public enum EnChargeDistributionType
    {
        Regular = 1,
        Volume = 2,
        DayEndBalance = 3
    }
    public enum EnAppType
    {
        BitGoAPI = 1,
        CryptoAPI = 2,
        EtherScan = 3
    }
    public enum EnMyAccountUserStatus
    {
        InActive = 0,
        Active = 1,
        Confirmed = 2,
        UnConfirmed = 3,
        UnAssigned = 4,
        Suspended = 5,
        Blocked = 6,
        RequestDeleted = 7,
        Suspicious = 8,
        Delete = 9,
        PolicyViolated = 10,
        UnBlocked = 11
    }

    public enum ApprovalStatus
    {
        Accept = 1,
        Reject = 9
    }

    public enum ServiceStatus
    {
        Disable = 9,
        Active = 1,
        InActive = 0
    }
    public enum UserAddressStatus
    {
        Block = 1,
        Unblock = 2
    }
    public enum enTransactionStatus
    {
        Initialize = 0,
        Success = 1,
        OperatorFail = 2,
        SystemFail = 3,
        Hold = 4,
        Refunded = 5,
        Pending = 6,
        InActive = 9
    }
    public enum enTrnType
    {
        Transaction = 1,
        Buy_Trade = 4,
        Sell_Trade = 5,
        Withdraw = 6,
        Shoping_Cart = 7,
        Deposit = 8,
        Generate_Address = 9,
        Topup = 10,
        Charge = 11,
        Commission = 12,
        ExportWallet = 13,
        GenerateColdWallet = 14,
        BlockUserAddress = 15,
        UnblockUser = 16,
        DestroyBlackFund = 17,
        TransferToken = 18,
        IncreaseTokenSupply = 19,
        DecreaseTokenSupply = 20,
        SetTransferFee = 21,
        UpdateCointLimit = 22,
        Configure_Coin = 23,
    }
    public enum enOrderStatus
    {
        Rejected = 9,
        Success = 1,
        Pending = 0
    }
    public enum enOrderType
    {
        BuyOrder = 1,
        SellOrder = 2,
        CancelOrder = 3,
        DepositOrder = 4,
        WithdrawalOrder = 5,
        CashOnBank = 6,
        Cheque = 7,
        Transfer = 8
    }
    public enum enServiceType
    {
        Recharge = 1,
        DTH = 2,
        BillPayment = 3,
        FlightBooking = 4,
        RailwayBooking = 5,
        BusBooking = 6,
        HotelBooking = 7,
        DataCard = 8,
        DMRSERVICE = 13,
        CAB = 14,
        WalletService = 15,
        LoanAPI = 16,
        Trading = 17,
        SiteTokenConversation = 18,
    }
    public enum MessageStatusType
    {
        Pending = 6,
        Success = 1,
        Initialize = 0,
        Fail = 9
    }
    public enum enWebAPIRouteType //ThirdParty Apptype-Types of API //ProTypeID
    {
        TransactionAPI = 1,
        CommunicationAPI = 2,
        LiquidityProvider = 3,
        PaymentGateway = 4,
        MarketData = 5,
        TradeServiceLocal = 6,
        CCXTLpProvider = 7 //Rita 22-7-19 for CCXT API call base on thirdparty API
    }

    public enum enMessageService
    {
        Init = 0,
        Success = 1,
        Fail = 2,
        Pending = 6
    }

    public enum EnModuleType
    {
        Role = 1,
        Groups = 2,
        Permissions = 3,
        Users = 4
    }

    public enum EnOperationType
    {
        Add = 1,
        Update = 2
    }

    public enum enErrorCode
    {
        //Wallet Remaining Error Code
        #region Wallet
        //2019-7-25 vsolanki added new errorcode
        SameEMail = 17101,
        EmailVerifyPending = 17102,

        BannerNotUpload = 17200,
        NotFoundBannerName = 17201,
        NotFoundBannerDescription = 17202,
        NotFoundBannerMessage = 17203,
        NotFoundBannerTermsAndCondition = 17204,
        NotFoundBannerIsKYCReuired = 17205,
        NotFoundBannerStatus = 17206,

        WalletImageNotUpload = 17207,
        NotFoundWalletName = 17208,
        NotFoundShortCode = 17209,
        NotFoundCoinType = 17210,
        NotFoundStatus = 17211,
        NotFoundId = 17212,
        NotFoundRounds = 17213,
        MaxShortCode = 17214,
        NotFoundBannerId = 17215,

        InValidRoundRequest = 17216,
        InValidRoundJsonRequest = 17217,
        NotFondRoundId = 17218,

        IEOInvalidTotal = 17219,
        IEOTotalRoundExceed = 17244,
        IEOTotalBonusExceed = 17245,
        IEONotFoundRate = 17246,
        IEONotFoundDescription = 17247,
        InvalidFromDate_ToDate = 17248,

        RequiredBankName = 17249,
        InvalidBankNameLen = 17250,
        RequiredBankCode = 17251,
        InvalidBankCodeLen = 17252,
        RequiredBankAccNo = 17253,
        InvalidBankAccNoLen = 17254,
        RequiredAccHolderName = 17255,
        InvalidAccHolderNameLen = 17256,
        RequiredCurrencyCode = 17257,
        InvalidCurrencyCodeLen = 17258,
        RequiredCountryCode = 17259,
        InvalidCountryCodeLen = 17260,
        PrevReqAlreadyInProcess = 17261,
        MultipleRequestNotAllowed = 17262,
        RequiredGUID = 17263,
        RequiredApprovalBit = 17264,
        InvalidApprovalBit = 17265,
        InvalidGUIDValue = 17266,
        ReqNotPending = 17267,
        FiatBuyFeeRequired = 17268,
        FiatSellFeeRequired = 17269,
        FiatTermsnConditionRequired = 17270,
        FiateIsBuyEnableRequired = 17271,
        FiatIsSellEnableRequired = 17272,
        FiatBuyFeeTypeRequired = 17273,
        FiatSellFeeTypeRequired = 17274,
        FiatInvalidLenBuyNotifyURL = 17275,
        FiatInvalidLenSellNotifyURL = 17276,
        FiatInvalidLenCallbackURL = 17277,
        FiatInvalidLenEncryptionKey = 17278,
        FiatInvalidLenBuyFee = 17279,
        FiatInvalidLenSellFee = 17280,
        FiatInvalidStatus = 17281,
        FiatInvalidMinLimit = 17291,
        FiatInvalidIDRecordNotFound = 17292,
        FiatTransactionIDNotFound = 17293,
        FiatTradeConfigurationNotFound = 17294,
        FiatTransactionHashUpdateFail = 17295,
        FiatTransactionHashAlredyExist = 17296,
        FiatDuplicateTransactionHash = 17297,
        FiatWithdrawTransactionNotFound = 17298,
        FiatBankIDRequired = 17306,
        FiatInvalidBankID = 17307,
        FiatBankReqNoRecFound = 17308,

        //add by akshay kothari 13/02/2020
        FileCreateSuccessfully=31000,





        FiatStatusRequired = 14201,
        FiatNameRequired = 14202,
        FiatCurrencyCodeRequired = 14203,
        FiatUSDRateRequired = 14204,
        InvalideFiatUSDRate = 14205,
        FiatCurrencyBuyFeeRequired = 14206,
        FiatBuyFeeRangeInvalid = 14207,
        FiatCurrencySellFeeRequired = 14208,
        FiatSellFeeRangeInvalid = 14209,
        RequiredFiatBuyFeeType = 14210,
        RequiredFiatSellFeeType = 14211,






        InvalidAmount = 2251,
        InsufficientBalance = 2252,
        Success = 2253,

        ItemNotFoundForGenerateAddress = 2254,
        InvalidThirdpartyID = 2255,
        AddressGenerationFailed = 2256,

        PleaseEnterWalletTypeName = 4200,
        PleaseEnterValidWalletTypeName = 4201,
        PleaseEnterDiscription = 4202,
        PleaseEnterValidDiscription = 4203,
        PleaseEnterIsDepositionAllow = 4204,
        PleaseEnterIsWithdrawalAllow = 4205,
        PleaseEnterIsTransactionWallet = 4206,
        PleaseEnterWalletName = 4207,
        PleaseEnterValidWalletName = 4208,
        PleaseEnterOTP = 4209,
        TransactionTypeRequired = 4210,
        LimitPerHourRequired = 4211,
        LimitPerDayRequired = 4212,
        LimitPerTransactionRequired = 4213,
        InvalidTransactionType = 4214,
        PleaseEnterDestinationAddress = 4215,
        PleaseEnterValidDestinationAddress = 4216,
        PleaseEnterAddressLabel = 4217,
        PleaseEnterValidAddressLabel = 4218,
        PleaseEnterAmount = 4219,
        PleaseEnterSourceAddress = 4220,
        PleaseEnterValidSourceAddress = 4221,
        PleaseEnterValidAmount = 4222,
        PleaseEnterAccWalletID = 4223,
        PleaseEnterBeneficiaryAddress = 4224,
        PleaseEnterBeneficiaryIDArray = 4225,
        PleaseEnterIsWhitelistingArray = 4226,
        //NoRecordFoundForUpdate = 4227,
        PleaseEnterIsWhitelistingBit = 4228,
        PleaseEnterStartTime = 4229,
        PleaseEnterEndTime = 4230,
        PleaseEnterBeneficiaryName = 4231,
        PleaseEnterBeneficiaryStatus = 4232,
        InvalidLimit = 4233,
        PleaseEnterUserID = 4234,
        PleaseEnterUserType = 4235,
        InvalidAddress = 4236,
        AddressNotFoundOrWhitelistingBitIsOff = 4237,

        BeneficiaryNotFound = 4238,
        AddressNotMatch = 4239,
        GlobalBitNotFound = 4240,
        WalletNotFound = 4241,
        MemberTypeNotFound = 4242,
        InvalidLimitPerDay = 4243,
        InvalidLimitPerHour = 4244,
        InvalidLimitPerTransaction = 4245,
        InvalidLifeTimeValue = 4246,
        PleaseEnterLifeTime = 4247,
        PleaseEnterWalletTypeID = 4248,
        PleaseEnterTrnTypeID = 4249,
        PleaseEnterStatus = 4250,
        InvalidAmt = 4251,
        InvalidTradeRefNoCr = 4252,
        InvalidTradeRefNoDr = 4253,
        InsufficantBalFirstCur = 4254,
        InsufficantBalSecondCur = 4255,

        //4256-4260 Not Found In File And Also In Enum
        CntUpdateWalletNTrnType = 4260,
        PleaseEnterPolicyName = 4261,
        PleaseEnterValidPolicyName = 4262,
        PleaseEnterWalletTrnType = 4263,
        PleaseEnterMinAmount = 4264,
        PleaseEnterValidMinAmount = 4265,
        PleaseEnterMaxAmount = 4266,
        PleaseEnterValidMaxAmount = 4267,
        PleaseEnterWalletType = 4268,
        PleaseEnterType = 4269,
        PleaseEnterChargeType = 4270,
        PleaseEnterChargeValue = 4271,
        //PleaseEnterStatus = 4272, duplicate
        InvalidWalletTrnType = 4273,
        PleaseEnterCommissionType = 4274,
        PleaseEnterCommissionValue = 4275,
        //YouCanNotInsertDuplicateRecord = 4276,
        Alredy_Exist = 4276,
        //AmountIsGreaterThanDailyLimit = 4277,
        DailyLimitValidationFail = 4277,
        //AmountIsGreaterThanHourlyLimit = 4278,
        HourlyLimitValidationFail = 4278,
        //AmountIsGreaterThanPerTransactionLimit = 4279,
        TransactionLimitValidationFail = 4279,
        LimitValidationFail = 4280,
        //AmountIsGreaterThanLimits = 4280,
        TimeValidationFail = 4281,
        //TransactionTimeExceedTheTimeLimit = 4281,
        LifetimeLimitValidationFail = 4282,
        //LifetimeSpendingLimitExceed = 4282,
        PleaseEnterAllowedIP = 4283,
        PleaseEnterAllowedLocation = 4284,
        PleaseEnterAuthenticationType = 4285,
        PleaseEnterDailyTrnCount = 4286,
        PleaseEnterDailyTrnAmount = 4287,
        PleaseEntervalidWeeklyTrnAmount = 4288,
        PleaseEnterMonthlyTrnCount = 4289,
        PleaseEnterMonthlyTrnAmount = 4290,
        PleaseEntervalidMonthlyTrnAmount = 4291,
        PleaseEnterWeeklyTrnCount = 4292,
        PleaseEnterWeeklyTrnAmount = 4293,
        //PleaseEntervalidWeeklyTrnAmount = 4294,
        PleaseEnterYearlyTrnCount = 4295,
        PleaseEnterYearlyTrnAmount = 4296,
        PleaseEntervalidYearlyTrnAmount = 4297,
        PleaseEnterAuthorityType = 4298,
        MasterDailyLimitValidationFail = 4299,
        //AmountIsGreaterThanMasterDailyLimit = 4299,
        MasterHourlyLimitValidationFail = 4300,
        //AmountIsGreaterThanMasterHourlyLimit = 4300,
        MasterTransactionLimitValidationFail = 4301,
        //AmountIsGreaterThanMasterPerTransactionLimit = 4301,
        MasterLimitValidationFail = 4302,
        //AmountIsGreaterThanMasterLimits = 4302,
        MasterTimeValidationFail = 4303,
        //TransactionTimeExceedTheMasterTimeLimit = 4303,
        MasterLifetimeLimitValidationFail = 4304,
        //MasterLifetimeSpendingLimit Exceed = 4304,       
        PleaseEnterWalletId = 4305,
        PleaseEnterTypeId = 4306,
        PleaseenterTypeName = 4307,
        PleaseEnterHourlyTrnAmount = 4308,
        //PleaseEnterHourlyTrnAmount = 4309,
        PleaseEntervalidHourlyTrnAmount = 4310,
        InvalidWalletType = 4311,
        PleaseLifeTimeTrnCount = 4312,
        PleaseEnterLifeTimeTrnAmount = 4313,
        PleaseEnterValidLifeTimeTrnAmount = 4314,
        PleaseEnrterRequiredParameterAppName = 4315,
        PleaseEnrterRequiredParameterSecreteKey = 4316,
        PleaseEnrterRequiredParameterSiteURL = 4317,
        PleaseEnrterRequiredParameterChannelID = 4318,
        PleaseEnrterRequiredParameterChannelName = 4319,
        PleaseEnrterRequiredParameterTransactionType = 4320,
        //InvalidTransactionType = 4321, duplicate
        PleaseEnterRequiredParameterCurrencyTypeName = 4322,
        PleaseEnterRequiredParameterID = 4323,
        PleaseEnterRequiredParameterIDOfWalletPolicyAllowedDay = 4324,
        InvalidRequiredParameterPolicyID = 4325,
        PleaseEnterRequiredParameterDayNo = 4326,
        InvalidRequiredParameterDayNo = 4327,
        DataAlreadyExist = 4328,
        PleaseEnterWalletID = 4329,
        PleaseEnterRoleID = 4330,
        PleaseEnterMessage = 4331,
        PleaseEnterValidMessage = 4332,
        PleaseEnterEmail = 4333,
        PleaseEnterValidEmail = 4334,
        EmailNotExist = 4335,
        AlredyExistWithRole = 4336,
        RequestPending = 4337,
        RemainingRecordNotUpdated = 4338,
        FirstCurrWalletStatusDisable = 4339,
        SecondCurrWalletStatusDisable = 4340,
        SettedOutgoingBalanceMismatch = 4341,
        NotFoundLimit = 4342,
        DuplicateRecord = 4343,
        SettedIncomingBalanceMismatch = 4344,
        InsufficientOutgoingBalFirstCur = 4345,
        InsufficietOutgoingBalSecondCur = 4346,
        InsufficientMarketInternalBalanceCheckFirstCurrencyForDifferentialAmountFailed = 4347,
        InsufficientMarketInternalBalanceCheckSecondCurrencyForDifferentialAmountFailed = 4348,
        FirstCurrDifferentialAmountHoldFailed = 4349,
        SecondCurrDifferentialAmountHoldFailed = 4350,
        InvalidChannelWalletHold = 4351,
        InvalidWalletTransactionWalletHold = 4352,
        InvalidChannelWalletDeduction = 4353,
        InvalidWalletTransactionWalletDeduction = 4354,
        InvalidChannelFirstCurrDr = 4355,
        InvalidChannelSecondCurrDr = 4356,
        InvalidChannelFirstCurrCr = 4357,
        InvalidChannelSecondCurrCr = 4358,
        InvalidWalletTransactionFirstCurrDr = 4359,
        InvalidWalletTransactionSecondCurrDr = 4360,
        InvalidWalletTransactionFirstCurrCr = 4361,
        InvalidWalletTransactionSecondCurrCr = 4362,
        PleaseEnterAllowedUserType = 4363,
        CrDrCredit_SettledBalMismatchCrWallet = 4364,
        CrDrCredit_SettledBalMismatchCrWalletSecCur = 4365,
        CrDrCredit_SettledBalMismatchDrWallet = 4366,
        CrDrCredit_SettledBalMismatchDrWalletSecDr = 4367,
        InvalidTradeRefNoFirCur = 4368,
        InsufficientOutboundBalance = 4369,
        FirstCurrDrWalletNotFound = 4370,
        SecondCurrDrWalletNotFound = 4371,
        FirstCurrCrWalletNotFound = 4372,
        SecondCurrCrWalletNotFound = 4373,
        MaxIdGenerationFail = 4374,
        AlredyExist = 4375,
        InvalidBeneficiaryID = 4376,
        InvalidTradeRefNo = 4377,
        SettedBalanceMismatch = 4378,
        ShadowBalanceExceed = 4379,
        UnAssignedAddressFetchFail = 4380,
        InvalidTradeRefNoSecCur = 4381,
        WalletTypeNameNotFound = 4382,
        InvalidCurrencyTypeID = 4384,
        InvalidRole = 4385,
        NotAddUser = 4387,
        ApprovedByOwner = 4388,
        NotRequestApproved = 4389,
        AlredyApproved = 4390,
        InvalidStatus = 4391,

        CurrencyTypeRequired = 4392,
        StakingTypeRequired = 4393,
        InvalidStakingType = 4394,
        SlabTypeRequired = 4395,
        InvalidSlabType = 4396,
        DurationWeekRequired = 4397,
        DurationMonthRequired = 4398,
        AutoUnstakingEnableRequired = 4399,
        InvalidStakingAmount = 4400,
        InvalidStakingMinAmount = 4401,
        InvalidStackingMaxAmount = 4402,
        InvalidMinOrMaxStakingAmount = 4403,
        InvalidDuration = 4404,
        InvalidInterestValue = 4405,
        MaturityCurrencyRequired = 4406,
        InterestTypeRequired = 4407,
        MakerChargeRequired = 4408,
        TakerChargeRequired = 4409,
        StakingRangeValidationFail = 4410,

        PolicyDetailIDRequired = 4415,
        InvalidStakingPolicyAmount = 4416,
        NotRemoveUser = 4417,
        ChannelIDRequired = 4418,
        DegradePolicyDetailIDrequired = 4419,
        UnstakingTypeRequired = 4420,
        StakingHistoryIDRequired = 4421,
        UnstakingNotAllowed = 4422,
        StakingFixedRangeValidationFail = 4423,
        ExportWalletFail = 4424,
        InvalidFileData = 4425,
        WalletTypeNameRequired = 4426,
        IsDefaultAddressRequired = 4427,
        EmailRequired = 4428,
        AddressLableRequired = 4429,
        AddressRequired = 4430,
        ServiceProviderNameRequired = 4431,
        CheckWithdrawBeneficiary_AddressNotFound = 4432,
        CheckWithdrawBeneficiary_SelfAddressFound = 4433,
        CheckWithdrawBeneficiary_InternalAddressFound = 4434,
        CheckWithdrawBeneficiary_WalletNotFound = 4435,
        CheckWithdrawBeneficiary_WalletNotEnable = 4436,
        CheckWithdrawBeneficiary_CoinNameNotMatch = 4437,
        NotShareWallet = 4440,
        FileRequiredToImport = 4441,
        NotMatchCoin = 4442,
        MaxNoOfRecordLimitExceed = 4443,
        InvalidWeekValue = 4444,
        AddressExist = 4445,
        InvalidMinAmount = 4446,
        InvalidSlabSelection = 4447,
        MasterDataNotFound = 4448,
        InvalidTimeDuration = 4449,
        EnableStakingBeforeMaturityRequired = 4450,
        EnableStakingBeforeMaturityChargeRequired = 4451,
        InvalidMinAndMaxValue = 4452,
        PositiValue = 4455,
        LessLimit = 4456,
        MarginWalletCanNotBeUsedForThis = 4457,

        LimitPerDayMinExceed = 4476,
        LimitPerDayMaxExceed = 4477,
        LimitPerHourMinExceed = 4478,
        LimitPerHourMaxExceed = 4479,
        LimitPerTransactionMinExceed = 4480,
        LimitPerTransactionMaxExceed = 4481,
        MLimitPerDayMaxExceed = 4482,
        MLimitPerHourMaxExceed = 4483,
        MLimitPerTransactionMaxExceed = 4484,
        MLimitPerTransactionMinExceed = 4485,

        //Rushabh WalletTrnLimitConfiguration Codes 
        WTrnTypeRequired = 4486,
        WWalletTypeRequired = 4487,
        WIsKYCEnableRequired = 4488,
        WPerTranMinAmount = 4489,
        WPerTranMaxAmount = 4490,
        WHourlyTrnCount = 4491,
        WHourlyTrnAmount = 4492,
        WDailyTrnCount = 4493,
        WDailyTrnAmount = 4494,
        WWeeklyTrnCount = 4495,
        WWeeklyTrnAmount = 4496,
        WMonthlyTrnCount = 4497,
        WMonthlyTrnAmount = 4498,
        WYearlyTrnCount = 4499,
        WYearlyTrnAmount = 17000,
        WInvalidTrnType = 17001,
        WInvalidIsKYCEnable = 17002,
        DepoReconInvalidTransactionType = 17003,
        DepoReconInvalidIsKYCEnable = 17004,
        DepoReconStatusRequired = 17005,
        DepoReconInvalidStatus = 17006,
        DepoReconIdRequired = 17007,
        DepoReconTransactionNoRequired = 17008,
        DepoReconActionTypeRequired = 17009,
        DepoReconActionRemarksRequired = 17010,
        InvalidConfirmationId = 17011,
        UserDetailNotFound = 17012,
        DownloadFileNotFound = 17013,
        DownloadFilePathNotFound = 17014,
        LinkExpired = 17015,
        emailConfirmCodeRequired = 17016,
        TrnAccNoRequired = 17017,
        //TrnAccNoRequired = 17017,
        InvalidTrnAccNoOnlyDigit = 17018,
        InvalidTrnAccNoOnlyAlphabet = 17019,
        InvalidTrnAccNoOnlySpecialChars = 17020,
        InvalidTrnAccNoLength = 17021,
        InvalidTrnAccNoStartsWithChar = 17022,
        InvalidUnstakeType = 17023,
        LeverageValueExceeded = 17024,
        FullUnstakingNotAvailable = 17025,
        InvalidSlabTypeSelecttion = 17026,
        AppTypeIdRequired = 17027,
        ImageUploadedSuccessfully = 17028,
        ImageUploadCurrencyNameRequired = 17029,
        ImageNotFoundInRequest = 17030,
        InvalidImageFormat = 17031,

        TransactionProviderDataNotFound = 17032,
        ThirdPartyDataNotFound = 17033,
        InvalidThirdPartyAPIID = 17034,
        NullResponseFromAPI = 17035,
        BalanceIsNull = 17036,
        ReportDataNotFound = 17037,
        AddressRegexValidationFail = 17038,
        sp_MarginDepositionProcess_AdminDepositionAllowed = 17039,
        InvalidTranxType = 17040,// only withdraw and address generation is allowed service provider
        BlockUserAddressOperationFail = 17049,
        InvalidTronAddress = 17050,
        WalletTypeRecNotFound = 17051,
        WalletRecNotFound = 17052,
        BlackFundAlreadyDestroyed = 17054,
        DestroyBlackFundOperationFail = 17055,
        TokenTransferOperationFail = 17061,
        IncreaseTokenSupplyOperationFail = 17062,
        DecreaseTokenSupplyOperationFail = 17063,
        SetTransferFeeOperationFail = 17070,
        AddressAlreadyUnblocked = 17072,
        AddressAlreadyBlocked = 17073,
        AdminAprrovalPendingForWithdrawTrn = 17074,
        WithdrawalReqSentToAdmin = 17075,
        RequestAlreadyProcessedByAdmin = 17076,
        WithdrawTrnRejectByAdmin = 17077,
        InvalidTrnStatusForProcess = 17078,
        CreateTrnInvalidIntAmountMsg = 17079,
        WithdrawNotAllowdBeforehrBene = 17096,
        WithdrawNotAllowdBeforehrForgotPswd = 17097,
        WithdrawNotAllowdBeforehrResetPswd = 17098,
        WithdrawNotAllowdBeforehrChangeDevice = 17099,
        InvalidTrnNo = 17100,

        InvalidDate = 17300,
        #endregion
        //====================Transactional
        #region Transaction

        InvalidPairName = 4500,
        NoDataFound = 4501,
        InvalidStatusType = 4502,
        InValidTrnType = 4503,
        InvalidFromDateFormate = 4504,
        InvalidToDateFormate = 4505,
        InvalidMarketType = 4506,
        InValidPage = 4507,
        InValid_ID = 4565,
        TransactionProcessSuccess = 4566,
        TransactionInsertSuccess = 4567,
        TransactionInsertFail = 4568,
        TransactionInsertInternalError = 4569,
        CreateTrn_NoPairSelected = 4570,
        CreateTrnInvalidQtyPrice = 4571,
        CreateTrnInvalidQtyNAmount = 4572,
        CreateTrn_NoCreditAccountFound = 4573,
        CreateTrn_NoDebitAccountFound = 4574,
        CreateTrnInvalidAmount = 4575,
        CreateTrnDuplicateTrn = 4576,
        FiatCreateTrnDuplicateTrn = 14215,
        CreateTrn_NoSelfAddressWithdrawAllow = 4577,
        ProcessTrn_InsufficientBalance = 4578,
        ProcessTrn_AmountBetweenMinMax = 4579,
        ProcessTrn_PriceBetweenMinMax = 4580,
        ProcessTrn_InvalidBidPriceValue = 4581,
        ProcessTrn_PoolOrderCreateFail = 4582,
        ProcessTrn_Initialize = 4583,
        TransactionProcessInternalError = 4584,
        ProcessTrn_ServiceProductNotAvailable = 4585,
        ProcessTrn_WalletDebitFail = 4586,
        ProcessTrn_Hold = 4587,
        ProcessTrn_ThirdPartyDataNotFound = 4588,
        ProcessTrn_GettingResponseBlank = 4589,
        ProcessTrn_OprFail = 4589,
        ProcessTrn_OprFails = 17331,
        ProcessTrn_Oprexpired = 17332,
       // ProcessTrn_OprFails = 17331,
        TradeRecon_InvalidTransactionNo = 4591,
        TradeRecon_After7DaysTranDontTakeAction = 4593,
        TradeRecon_InvalidTransactionStatus = 4594,
        TradeRecon_CancelRequestAlreayInProcess = 4595,
        TradeRecon_TransactionAlreadyInProcess = 4596,
        TradeRecon_OrderIsFullyExecuted = 4597,
        TradeRecon_InvalidDeliveryAmount = 4598,
        TradeRecon_CencelRequestSuccess = 4600,
        TradeRecon_InvalidActionType = 4618,
        FavPair_InvalidPairId = 4619,
        FavPair_AlreadyAdded = 4620,
        FavPair_AddedSuccess = 4621,
        FavPair_RemoveSuccess = 4622,
        FavPair_NoPairFound = 4623,
        InValidDebitAccountID = 4624,
        InValidCreditAccountID = 4625,
        Graph_InvalidIntervalTime = 4626,
        CreateTrn_WithdrawAmountBetweenMinAndMax = 4627,
        Settlement_AlreadySettled = 4628,
        Settlement_PartialSettlementDone = 4629,
        Settlement_FullSettlementDone = 4630,
        Settlement_SettlementInternalError = 4631,
        Settlement_NoSettlementRecordFound = 4632,
        CancelOrder_NoRecordFound = 4633,
        CancelOrder_InternalError = 4634,
        CancelOrder_TrnNotHold = 4635,
        CancelOrder_SystemOrderCanNotCancelByUser = 11175,
        Settlement_PartialSettlementRollback = 4636,
        Settlement_FullSettlementRollback = 4637,
        CancelOrder_OrderalreadyCancelled = 4638,
        CancelOrder_YourOrderInProcessMode = 4639,
        CancelOrder_Yourorderfullyexecuted = 4640,
        CancelOrder_InvalidDeliveryamount = 4669,
        ProcessTrn_APICallInternalError = 4641,
        CancelOrder_StockNotAvilable = 4642,
        CancelOrder_ProccedSuccess = 4643,
        CancelOrder_CancelProcessFail = 4644,
        CancelOrder_FailInTransactionDataUpdate = 11181,
        CancelOrder_UnderProcessing = 4645,
        CancelOrder_StockErrorOccured = 4646,
        CancelOrder_APICallFail = 11040,
        Settlement_SettlementInternalErrorBuy = 4647,
        Settlement_SettlementInternalErrorSell = 4648,
        WithdrawalRecon_Success = 4650,
        WithdrawalRecon_NoRecordFound = 4651,
        WithdrawalRecon_InvalidActionType = 4652,
        WithdrawalRecon_ProcessFail = 4653,
        WithdrawalRecon_InvalidTrnType = 4654,
        DataInsertFail = 4663,
        DataUpdateFail = 4665,
        AddPairConfiguration_PairAlreadyAvailable = 4666,
        MarketAlreadyExist = 4667,
        TradeRouteAlreadyAvailable = 4668,
        InValidOrderType = 4670,
        InvalidTimeType = 4671,
        InValidTopLossGainerFilterType = 4672,
        InValidRange = 4673,
        CoinReqIDNotFound = 4676,
        InvalidCoinOrToken = 4691,
        CoinAlreadyAvailable = 4692,
        CoinNotContainSpecialCharacter = 4693,
        Withdrwal_TranNoNotFound = 11008,
        Withdrwal_NotAuthorizeToDoingTransaction = 11009,
        Withdrwal_TranNoHoldState = 11010,
        Withdrwal_InvalidTransactionBitValue = 11011,
        Withdrwal_TransactionTimeOut = 11012,
        Withdrwal_TransactionAlreadyVerified = 11015,
        Withdrwal_TransactionConfirmSuccess = 11016,
        Withdrwal_TransactionCancelSuccess = 11017,
        Withdrwal_TransactionCancelFail = 11018,
        Withdrwal_TransactionConfirmFail = 11019,
        Withdrwal_SendEmailAfterSomeTime = 11020,
        Withdrwal_ResendConfirmationEmailSuccess = 11021,
        Withdrwal_TransactionAlreadyRequested = 11022,
        Service_Already_Exist = 11027,
        WithdrwalCancelByUser = 11023,
        WithdrwalInternalTransactionSuccess = 11024,
        WithdrwalOutsideTransactionSuccess = 11025,
        WithdrwalTransactionHold = 11026,
        WithdrwalCancelProcessFail = 11028,
        CancelOrder_EnterValidTransactionNo = 11029,
        HistoricalPerformance_LeaderNotFound = 11030,
        Withdrwal_FromAddressNotFound = 11031,
        API_OrderNotPending = 11041,//Rita 29-1-19 API status check transaction data update
        API_BuyerOrderNotPending = 11042,
        API_NotAPIOrder = 11043,//Rita 1-2-19
        API_NoRemainQtyChange = 11044,
        API_NoSettledQtyChange = 11045,
        API_InvalidSettlementQty = 11046,
        API_InvalidSettledRemainQty = 11047,
        API_IsProcessing = 11048,
        API_Respose_Fail = 11032,
        CancelOrder_ThirdPartyDataNotFound = 11033,
        CancelOrder_ProviderDetailIDNotFound = 11034,
        CancelOrder_TrnRefNo_Not_Found = 11035,
        InValid_CurrencyID = 11049,
        InValid_BaseCurrencyID = 11050,
        InValid_Currency_Pair = 11051,
        Invalid_Site_Token = 11052,
        InvalidSourceCurrency = 11053,
        NoPairforUserCurrency = 11054,
        NoLTPfound = 11055,
        InvalidTokenRate = 11056,
        MinimumQtyRequired = 11057,
        MaximumQtyRequired = 11058,
        DailyQtyReached = 11059,
        WeeklyQtyReached = 11060,
        MonthlyQtyReached = 11061,

        //Feed Configuration
        DataSizeValidationFail = 11062,
        DataRowSizeValidationFail = 11063,
        DataPerSecValidationFail = 11064,
        DataPerMinValidationFail = 11065,
        DataPerHrValidationFail = 11066,
        DataPerWeekValidationFail = 11067,
        DataPerMonthValidationFail = 11068,
        DataPerYearValidationFail = 11069,
        DataPerUserValidationFail = 11070,
        MethodLimit_Alredy_Exist = 11071,
        InvalidInputQty = 11072,

        //APIConfiguration
        PleaseEnterPlanName = 11077,
        PleaseEnterMethodName = 11078,
        PleaseEnterPlanMasterID = 11079,
        PriorityExist = 11080,
        SubscribeAPIPlan_InValidRequestedPlan = 11081,
        SubscribeAPIPlan_RequestedPlanIsActivated = 11082,
        SubscribeAPIPlan_RequestedPlanInProcess = 11083,
        SubscribeAPIPlan_RequestedPlanIsInPending = 11084,
        InValidValidityType = 11089,
        SubscribeAPIPlan_InValidSubscribeID = 11090,
        SubscribeAPIPlan_InValidRequestedPlanOrDisable = 11091,
        SubscribeAPIPlan_PlanIsNotRecursive = 11092,
        SubscribeAPIPlan_PlanAlreadyRenew = 11093,
        SubscribeAPIPlan_InvalidNextAutoRenewID = 11094,
        SubscribeAPIPlan_CurrentPlanIsActive = 11095,
        SubscribeAPIPlan_InValidIPAccess = 11096,
        SubscribeAPIPlan_ActivePlanNotFound = 11097,
        SubscribeAPIPlan_DuplicateAliasName = 11098,
        SubscribeAPIPlan_InValidAPIKey = 11099,
        SubscribeAPIPlan_SettingFreezed = 11100,
        SubscribeAPIPlan_AddLimitExceed = 11101,
        SubscribeAPIPlan_PerDayAddLimitExceed = 11102,
        SubscribeAPIPlan_AddFrequencyLimitExceed = 11103,
        SubscribeAPIPlan_DeleteLimitExceed = 11104,
        SubscribeAPIPlan_PerDayDeleteLimitExceed = 11105,
        SubscribeAPIPlan_DeleteFrequencyLimitExceed = 11106,
        SubscribeAPIPlan_IPWhitelistLimitExceed = 11111,
        SubscribeAPIPlan_InvalidIP = 11112,
        SubscribeAPIPlan_EnterCurrentSubscribeID = 11113,
        SubscribeAPIPlan_CustomeLimitAlreadyExist = 11114,
        SubscribeAPIPlan_MaxPerMinuteLimitExceed = 11115,
        SubscribeAPIPlan_MaxPerDayLimitExceed = 11116,
        SubscribeAPIPlan_MaxPerMonthLimitExceed = 11117,
        SubscribeAPIPlan_MaxOrderPerSecLimitExceed = 11118,
        SubscribeAPIPlan_MaxRecPerRequestLimitExceed = 11119,
        SubscribeAPIPlan_MaxReqSizeLimitExceed = 11120,
        SubscribeAPIPlan_MaxResSizeLimitExceed = 11121,
        SubscribeAPIPlan_WhitelistedEndPointsLimitExceed = 11122,
        SubscribeAPIPlan_ConcurrentEndPointsLimitExceed = 11123,
        SubscribeAPIPlan_HistoricalDataMonthLimitExceed = 11124,
        SubscribeAPIPlan_InvalidCustimeLimitID = 11125,
        API_LP_Fail = 11126,
        API_LP_Success = 11127,
        API_LP_InternalError = 11128,
        SubscribeAPIPlan_InvalidFrequencyType = 11129,
        SubscribeAPIPlan_CurrentPlanNotFound = 11130,
        SubscribeAPIPlan_DuplicateIP = 11131,
        SubscribeAPIPlan_PlanAlreadyAutoRenew = 11132,
        SubscribeAPIPlan_PlanIsNotAutoRenew = 11133,
        SubscribeAPIPlan_DuplicateIPAddress = 11134,
        InValidPriority = 11135,
        InvalidTransactionStatus = 11136,
        TransactionStatusNotSuccess = 11137,
        TransactionStatusNotFail = 11138,
        TransactionStatusNotActive = 11139,
        TransactionStatusNotInActive = 11140,
        TransactionStatusNotHold = 11141,
        TradeRecon_RequestCancel = 11142,
        TradeRecon_RequestFail = 11143,
        TradeRecon_RequestActive = 11144,
        TradeRecon_RequestInActive = 11145,
        TradeRecon_RequestSuccess = 11146,
        TradeRecon_RequestSuccessAndDebit = 11147,
        TradeRecon_After5MinTranDontTakeAction = 11148,
        SubscribeAPIPlan_InValidChannelType = 11149,
        ServiceNotExist = 11150,
        IPAccessIsNotRestrict = 11151,
        SubscribeAPIPlan_ConCurrentIPLimitExceed = 11152,
        APIKeyFreezed = 11153,
        TradeRecon_ActionFailed = 11154,
        TradeRecon_RequestInProcess = 11156,
        IPListRequirer = 11157,
        AuthenticationFail = 401,
        TransactionNotInIsProcessing = 11158,
        TradeRecon_RequestReleaseStuckOrder = 11159,
        TradeRecon_RequestReInit = 11160,
        ReconInsufficientBalanceForCharge = 11161,
        ReconInsufficientBalance = 11162,
        TradeRecon_ActionFailedForLocalTrade = 11163,
        MarginTxn_AlreadyOpenPosition = 11164,
        ProcessTrn_RouteTypeDefinedWrong = 11165,
        Margin_ClosePositionNoHoldOrderFound = 11166,
        Margin_ClosePositionUnderProcessing = 11167,
        InvalidAccNoStartsWithLength = 11168,
        InvalidAccNoValidationRegexLength = 11169,
        Site_Token_SourceCurrencyShoulBeFromBaseMarket = 11172,
        Site_Token_TargetCurrencyShoulNotBeFromBaseMarket = 11173,
        Margin_TradingNotAllowed = 11174,
        CreateTxnTradingIsStopped = 11178,//rita 27-5-19 change from 11175 as already exist
        API_LP_Filled = 11179,
        API_LP_PartialFilled = 11180,
        InvalidSmartArbitrageOrder = 11182,
        SmartArbitrageOrder_ShouldHave_BothBuyNSell = 11183,
        API_LP_Cancel = 11184,
        API_LP_Hold = 11185,
        API_LP_Order_Not_Found = 11186,
        Binance_PRICE_FILTER = 11187,
        Binance_PERCENT_PRICE = 11188,
        Binance_LOT_SIZE = 11189,
        Binance_MIN_NOTIONAL = 11190,
        Binance_ICEBERG_PARTS = 11191,
        Binance_MARKET_LOT_SIZE = 11192,
        Binance_MAX_NUM_ORDERS = 11193,
        Binance_MAX_ALGO_ORDERS = 11194,
        Binance_MAX_NUM_ICEBERG_ORDERS = 11195,
        Binance_EXCHANGE_MAX_NUM_ORDERS = 11196,
        Binance_EXCHANGE_MAX_ALGO_ORDERS = 11197,
        Binance_Timeout = 11198,
        Bittrex_MIN_TRADE_REQUIREMENT_NOT_MET = 11199,
        Bittrex_INSUFFICIENT_FUNDS = 11200,
        Tradesatoshi_INSUFFICIENT_FUNDS = 11201,
        Binance_INSUFFICIENT_FUNDS = 11202,
        Bitfinex_INSUFFICIENT_FUNDS = 11203,
        Bitfinex_MIN_TRADE_REQUIREMENT_NOT_MET = 11204,

        //Kraken Place Order Error Codes by Pushpraj as on 11-07-2019
        Kraken_INVALID_ARGUMENTS = 11205,
        Kraken_UNAVAILABLE = 11206,
        Kraken_INVALID_REQUEST = 11207,
        Kraken_CANNOT_OPEN_POSITION = 11208,
        Kraken_CANNOT_OPEN_OPPOSING_POSITION = 11209,
        Kraken_MARGIN_ALLOWED_EXCEEDED = 11210,
        Kraken_MARGIN_LEVEL_TO_LOW = 11211,
        Kraken_INSUFFICIENT_MARGIN = 11212,
        Kraken_INSUFFICIENT_FUNDS = 11213,
        Kraken_ORDER_MINIMUM_NOT_MET = 11214,
        Kraken_ORDER_LIMIT_EXCEEDED = 11215,
        Kraken_POSITION_LIMIT_EXCEEDED = 11216,
        Kraken_RATE_LIMIT_EXCEEDED = 11217,
        Kraken_SCHEDULE_ORDER_LIMIT_EXCEEDED = 11218,
        Kraken_UNKNOWN_POSITION = 11219,
        ProcessTrn_QtyBadPrecision = 11220,
        ProcessTrn_PriceBadPrecision = 11221,
        ProcessTrn_NotionalBadPrecision = 11229,
        ProcessTrn_TotalFailure = 11222,
        API_AuthenticationFail = 11223,
        API_LPCreateOrderInternalError = 11224,
        API_APICallfail = 11225,
        API_APICallProceeWithNoResponse = 11226,
        API_ResponseFail = 11227,
        API_ResponseParsingFail = 11228,
        Invalid_TradingType = 11229,
        ProcessTrn_ArbitrageFirstTransactionFail = 11230,
        OrderTypeNotAvailable = 11231,
        CurrentlystoppedWithdrawRecon = 11232,
        Bot_minAmountNotFulfill = 18000,

        MobileIsRequired = 11233,

        MarketMakerUserDoesNotExist = 11234,
        WithdrawConfirmationRefNoRequired = 11235,
        WithdrawConfirmationInvalidRefNo = 11236,
        WithdrawConfirmationLinkDecryptionFail = 11237,
        WithdrawConfirmationReqDataIsNull = 11238,
        WithdrawConfirmationUserNotFound = 11239,
        ItemNotFoundForNewCoinConfig = 11240,
        InvalidThirdPartyIdForNewCoinConfig = 11241,
        ThirdPartyDataNotFoundForNewCoinConfig = 11242,
        WebRequestCreationFailForNewCoinConfig = 11243,
        OperatorFailForNewCoinConfig = 11244,
        ResponseParseErrorForNewCoinConfig = 11245,
        RequiredServiceProviderIdForNewCoinConfig = 11246,
        InvalidServiceProviderIdForNewCoinConfig = 11247,
        ServiceProviderDataNotFoundForNewCoinConfig = 11248,
        InvalidServiceProDetailId = 11249,
        ServiceProDetailNoDataFound = 11250,
        #endregion
        //=======================
        //=====================MyAccount
        #region MyAccount
        Status500InternalServerError = 500,
        Status400BadRequest = 400,
        Status423Locked = 423,
        Status14214UserCreatedTimeNotValid = 14214,
        Status4013MobileInvalid = 4013,
        Status4020IpInvalid = 4020,
        Status4032LoginFailed = 4032,
        Status4033NotFoundRecored = 4033,
        Status4034UnableUpdateUser = 4034,
        Status4035UnableToAddIpAddress = 4035,
        Status4036VerifyPending = 4036,
        Status4037UserNotAvailable = 4037,
        Status4038ResetUserNotAvailable = 4038,
        Status4039ResetPasswordLinkExpired = 4039,
        Status4040ResetPasswordLinkempty = 4040,
        Status4041ResetPasswordConfirm = 4041,
        Status4042ResetConfirmPassMatch = 4042,
        Status4701newandoldPassMatch = 4701,
        Status4043ResetConfirmOldNotMatch = 4043,
        Status4044UserSelectedIpNotFound = 4044,
        Status4045InvalidUserSelectedIp = 4045,
        Status4046NotUpdateIpStatus = 4046,
        Status4047NotDeleteIp = 4047,
        Status4048Invalidappkey = 4048,
        Status4049appkey = 4049,
        Status4050InvalidUser = 4050,
        Status4051RefreshToken = 4051,
        Status4052UserToken = 4052,
        Status4053Granttype = 4053,
        Status4054FactorFail = 4054,
        Status4055DisableTroFactorError = 4055,
        Status4056TwoFactorVerification = 4056,
        Status4057DeviceIdNotAdd = 4057,
        Status4058DeviceAddress = 4058,
        Status4059NotDeleteDevice = 4059,
        Status4060VerifyMethod = 4060,
        Status4061Userpasswordnotupdated = 4061,
        Status4062UseralreadRegister = 4062,
        Status4063UserNotRegister = 4063,
        Status4064EmailLinkBlanck = 4064,
        Status1450invalidelinkdata = 1450,
        Status4066UserRoleNotAvailable = 4066,
        Status4067InvalidOTPorexpired = 4067,
        Status4068InvalidGoogleToken = 4068,
        Status4069InvalidGoogleProviderKey = 4069,
        Status4070SocialUserInsertError = 4070,
        Status4071TwoFactorVerificationDisable = 4071,
        Status4074SignUPMobileValidation = 4074,
        Status4075SignUPOTP = 4075,
        Status4076SignUpReSendOTP = 4076,
        Status4077UserUnlockError = 4077,
        Status4078SignUpRole = 4078,
        Status4081IpAddressNotInsert = 4081,
        Status4083IpAddressExist = 4083,
        Status4084DeviceIdExist = 4084,
        Status4079TwoFAcodeInvalide = 4079,
        Status4085LoginWithOtpDatanotSend = 4085,
        Status4086LoginFailEmailNotAvailable = 4086,
        Status4087EmailFail = 4087,
        Status4088LoginWithOtpInvalidAttempt = 4088,
        Status4089LoginEmailOTPNotsend = 4089,
        Status4090OTPSendOnMobile = 4090,
        Status4091LoginMobileNumberInvalid = 4091,
        Status4096InvalidFaceBookToken = 4096,
        Status4097InvalidFaceBookProviderKey = 4097,
        Status4072UserIdNotValid = 4072,
        Status4098BizUserEmailExist = 4098,
        Status4099BizUserNameExist = 4099,
        Status4100provideDetailNotAvailable = 4100,
        Status4101InputProvider = 4101,
        Status4102SignUpUserRegisterError = 4102,

        Status4103UserDataNotAvailable = 4103,
        Status4104TempUserNameExist = 4104,
        Status4105TempUserMobileExist = 4105,
        Status4106LoginFailMobileNotAvailable = 4106,
        Status4107TwoFAKeyinvalid = 4107,
        Status4108TwoFAalreadydisable = 4108,
        Typemasterrequired4109 = 4109,
        Status4112ProfilePlan = 4112,
        Status4122NotAddedProfile = 4122,
        Status4129InvalidProfileId = 13004, //Chirag 05/07/2019 4129 Changed to 13004 bcoz of duplication
        Status4110TypemasterInsertError = 4110,
        Status4111AddCompainrequired = 4111,
        Status4113AddCompainTrail = 4113,
        Status4114CompainTrailInsertError = 4114,
        status4116Complaintdatanotavailable = 4116,
        status4121complaintTypeNotavailable = 4121,

        Status4115ImageNotUpload = 4115,
        Status4123FrontImageLarger = 4123,
        Status4124BackImageLarger = 4124,
        Status4125SelfiImageLarger = 4125,
        Status4126SuranName = 4126,
        Status4127GivenName = 4127,
        Status4128ValidIdentityCard = 4128,
        Status4129PersonalIdentityNotInserted = 4129,
        Status4130PersonalIdentityNotavailable = 4130,
        Status4131RequiredCountryCode = 4131,
        Status4132RequiredCountryCodeValideRange = 4132,
        Status4015DeviceIdNotFound = 4015,
        Status4017ModeNotFound = 4017,
        Status4021HostNameNotFound = 4021,
        Status4133ResetAuthorizedLinkExpired = 4133,
        Status4134AuthorizedLinkBlanck = 4134,
        Status4135AuthorizedUser = 4135,
        Status4136InvalidIPNDevice = 4136,
        Status4137UnAuthorizedDevice = 4137,
        Status4140TwoFactorRequired = 4140,
        Status4141FrontImageLarger = 4141,
        Status4142FrontImageFormatNotValid = 4142,
        Status4143BackImageFormatNotValid = 4143,
        Status4144SelfieImageFormatNotValid = 4144,
        Status4145PersonalIdentityAllreadyApprove = 4145,
        Status4146PersonalIdentityAllreadySubmited = 4146,
        Status4147IdentityNotFound = 4147,
        Status4148FileFormatnotfound = 4148,
        Status4149ActivityDataNotAvailable = 4149,
        Status4150AlreadyEmailVef = 4150,
        Status4151AlreadyMobileVef = 4151,
        Status4152IPNotValid = 4152,
        Status4158PasswordExpiration = 4158,
        Status4159PasswordForgotinday = 4159,
        Status4160PasswordForgotinMonth = 4160,
        Status4161DayModeAlreadyActivated = 4161,
        Status4162NightModeAlreadyActivated = 4162,
        Status4163AllreadyLinkVerify = 4163,
        Status4164UnableChangePassword = 4164,
        Status4165UserWiseGetProfileNotavailable = 4165,
        Status4166UserWiseProfileHistoryNotavailable = 4166,
        Status4167SignUPAlreadyConfirm = 4167,
        Status4168UserNotActive = 4168,

        RoleNotExist = 4172,
        Status4173DeviceIdNotDisable = 4173,
        Status4174DeviceIdNotEnable = 4174,
        Status4175DeviceIdNotEnable = 13005, //Chirag 05/07/2019 4175 Changed to 13005 bcoz of duplication
        //RecordCreatedSuccessfully = 4184,
        //RecordUpdatedSuccessfully = 4185,
        //RecordDeletedSuccessfully = 4186,
        //RecordFoundSuccessfully   = 4187,
        //NoRecordFound = 4188,
        Status4188RequestedRemoteIPNotFound = 4188,
        RecordCreatedSuccessfully = 14000,
        RecordUpdatedSuccessfully = 14001,
        RecordDeletedSuccessfully = 14002,
        RecordFoundSuccessfully = 14003,
        NoRecordFound = 14004,
        RecordAlreadyExist = 14016,
        EmailAlreadyExistWithUser = 14029,
        UserNameAlreadyExist = 14030,
        MobileNoAlreadyExist = 14031,
        ModuleNameAlreadyExist = 14032,
        SubModuleNameAlreadyExist = 14033,
        FieldNameAlreadyExist = 14034,
        ToolNameAlreadyExist = 14035,
        RoleIdRequiredAndCanNotBeZero = 14041,
        UserIdRequiredAndCanNotBeZero = 14042,
        RoleNameAlreadyExist = 14043,
        PermissionGrpIdRequiredAndCanNotBeZero = 14044,
        RuleDataUpdationFail = 14045,
        YouCanNotAssignDisabledUser = 14046,
        YouCanNotAssignDisabledRole = 14047,
        PermissionGroupIsDisable = 14048,
        YouCanNotUpdateDisableUser = 14049,
        UserRoleNoDataFound = 14050,
        NoPermissionGroupByLinkedRole = 14051,
        PermissionGroupNotActive = 14052,
        MultipleRoleNotAllow = 14053,
        SuccessReInviteUser = 14054,
        SuccessInviteUser = 14056,
        Status14058InvalidemailConfirmCode = 14058,
        Status14059ConfigurePermissionsNotFound = 14059,
        Status14060AdminUserNotAvailable = 14060,
        InvalidLang = 14061,
        EmailAlreadySubscribed = 14062,
        EnterValidEmail = 14063,
        EmailNotFoundEnterValidEmail = 14064,
        UnAuthorize = 14065,
        Status14071AlreadyDisable = 14071,
        Status14072AlreadyEnable = 14072,
        Status14074deviceauthupdate = 14074,
        Status14073OTPDataNotFound = 14073,
        #endregion
        ///// wallet ///////////
        #region Wallet
        DeductWalletNullWalletIDorCoinType = 424,
        DefaultWalletNotFound = 425,
        BatchNoGenerationFailed = 246,
        InvalidWalletId = 247,
        InvalidTrnType = 248,
        TrnTypeNotAllowed = 249,
        RecordNotFound = 250,
        InvalidCoinName = 4031,
        StandardLoginfailed = 4032,

        NotFound = 13001, //Chirag 05/07/2019 4032 Changed to 13001 bcoz of duplication
        InvalidWalletOrUserIDorCoinName = 13002, //Chirag 05/07/2019 4034 Changed to 13002 bcoz of duplication
        InvalidWallet = 13006, //Chirag 05/07/2019 4235 Changed to 13006 bcoz of duplication
        InsufficantBal = 13007, //Chirag 05/07/2019 4432 Changed to 13007 bcoz of duplication
        UserIDWalletIDDidNotMatch = 13003, //Chirag 05/07/2019 4039 Changed to 13003 bcoz of duplication
        //InvalidLimit = 4233,
        InternalError = 9,
        InvalidBank = 14186,
        InvalidMaxLimit = 14187,
        InvalidMinLimit = 14188,
        MinAmtExccedd = 14189,
        MaxAmtExccedd = 14190,
        MinQtyExccedd = 14192,
        MaxQtyExccedd = 14193,
        CoinConfigurationDataNotFond = 14191,

        //AddressNotFoundOrWhitelistingBitIsOff = 4237,
        //BeneficiaryNotFound = 4238,
        //AddressNotMatch = 4239,
        //GlobalBitNotFound = 4240,
        //WalletNotFound = 4241,
        //NotFoundLimit=4234,
        //DuplicateRecord`=4235,   
        OrgIDNotFound = 2427,
        NeedtoCmpltKYC = 14165,
        KYCDataNotFound = 14166,
        //MemberTypeNotFound = 4242,

        PushNotificationSubscriptionSuccess = 5001,
        PushNotificationunsubscriptionSuccess = 5002,
        PushNotificationSubscriptionFail = 5003,
        PushNotificationUnsubscriptionFail = 5004,
        InvalidInput = 5005,
        InvalidMasterID = 4916,
        MoreDays = 5006,
        BlockUser = 5007,
        UnblockUser = 5008,
        BlockUserFail = 5009,
        UnblockUserFail = 5010,
        Invalid_MobileNo = 5014,
        Empty_EmailList = 5020,
        Empty_DeviceList = 5021,
        MessageInQueue = 5022,

        CreditWalletMsgNotification = 6000,
        DebitWalletMsgNotification = 6001,
        GenerateAddressNotification = 6002,
        CWalletLimitNotification = 6003,
        UWalletLimitNotification = 6004,
        AddBeneNotification = 6005,
        UpBeneNotification = 6006,
        UserPreferencesNotification = 6007,
        DefaultCreateWalletSuccessMsg = 6008,
        NewCreateWalletSuccessMsg = 6009,
        SignalRTrnSuccessfullyCreated = 6010,
        SignalRWithdrawTrnSuccessfullyCreated = 6018,
        SignalRTrnSuccessfullySettled = 6011,
        SignalRTrnSuccessPartialSettled = 6012,
        SignalRCancelOrder = 6014,
        ConvertFund = 6013,
        TransactionValidationFail = 6015,
        TransactionFailed = 6016,
        TransactionSuccess = 6017,
        UpdateBeneNotificationActive = 6043,
        AddUserNotification = 13008, //Chirag 05/07/2019 6010 Changed to 13008 bcoz of duplication
        //UpdateBeneNotificationInactive = 6044,
        //UpdateBeneNotificationDelete = 6045,
        // NullTradeRefNoCr = 6022,
        //NullTradeRefNoDr = 6023,
        HoldBalanceNotification = 6035, //ntrivedi 04-12-2018
        HoldBalanceReleaseNotification = 6042, //ntrivedi 04-12-2018
        //CrDrCredit_SettledBalMismatchDrWalletSecDr = 6021,
        // MyAccount Notification
        UserProfileUpdateSuccess = 6036,
        UserLoginNotification = 6037,
        ResendOTPNotification = 6038,
        SendOTPNotification = 6039,
        TwoFAActiveNotification = 6040,
        TwoFADeactiveNotification = 6041,
        SessionExpired = 6046,
        ChargeHoldWallet = 6047,
        ChargeReleasedWallet = 6048,
        ChargeDeductedWallet = 6049,
        ChargeRefundedWallet = 6050,
        UWalletLimitNotificationErr = 6051,

        SignalR_LimitPerDayMinExceed = 6052,
        SignalR_LimitPerDayMaxExceed = 6053,
        SignalR_LimitPerHourMinExceed = 6054,
        SignalR_LimitPerHourMaxExceed = 6055,
        SignalR_LimitPerTransactionMinExceed = 6056,
        SignalR_LimitPerTransactionMaxExceed = 6057,
        // SignalR_MLimitPerDayMaxExceed = 6058,
        //SignalR_MLimitPerHourMaxExceed = 6059,
        //  SignalR_MLimitPerTransactionMaxExceed = 6060,
        // SignalR_MLimitPerTransactionMinExceed = 6061,
        Undermaintenance = 6062,
        Production = 6063,
        //sp error code
        sp_ReleaseHoldWallet_AmountMismatch = 7001, //sp_ReleaseHoldWallet
        sp_ReleaseHoldWallet_Success = 7002, //sp_ReleaseHoldWallet
        sp_ReleaseHoldWallet_InternalError = 7003,//sp_ReleaseHoldWallet
        sp_CrDrWalletForHold_Success = 7004,
        sp_CrDrWalletForHold_InternalError = 7005,//sp_ReleaseHoldWallet
        sp_DebitWallet_Scccess = 7006,
        sp_DebitWallet_InternalError = 7007,
        sp_HoldWallet_Scccess = 7008,
        sp_HoldWallet_InternalError = 7009,
        sp_CreditWallet_InvalidRefNo = 7010,
        sp_CreditWallet_Sucess = 7011,
        sp_CreditWallet_InternalError = 7012,
        Sp_InsertUpdateStatatics_InternalError = 7013,
        //Khushali 23-03-2019 for sp_ReconSuccessAndDebitWalletWithCharge
        sp_ReconSuccessAndDebitWalletWithCharge_Scccess = 7014,
        sp_ReconSuccessAndDebitWalletWithCharge_InternalError = 7015,
        //ntrivedi 26-12-2018 added
        sp_StakingSchemeRequest_StakingPolicyDetailNotFound = 7028,
        sp_StakingSchemeRequest_StakingPolicyMasterNotFound = 7029,
        sp_StakingSchemeRequest_OrginationWalletnotfound = 7030,
        sp_StakingSchemeRequest_InvalidWalletUser = 7033,
        sp_StakingSchemeRequest_InsufficientWalletBalance = 7034,
        sp_StakingSchemeRequest_InvalidRequest = 7035,
        sp_StakingSchemeRequest_InvalidSlabSelection = 7036,
        sp_StakingSchemeRequest_InvalidStakingAmountUpgradeUpgrade = 7037,
        sp_StakingSchemeRequest_InvalidStakingAmountFirstTime = 7038,
        sp_StakingSchemeRequest_InvalidMaturityAmount_FD = 7039,
        sp_StakingSchemeRequest_OtherCurrency = 7040,
        sp_StakingSchemeRequest_Success = 7041,
        sp_StakingSchemeRequest_InternalError = 13009, //Chirag 05/07/2019 7041 Changed to 13009 bcoz of duplication
        sp_IsValidWalletTransaction_WalletAuthorizeUserNotFound = 7050,
        sp_IsValidWalletTransaction_WTrnTypeMasterNotFound = 7051,
        sp_IsValidWalletTransaction_WalletTypeMastersNotFound = 7052,
        sp_IsValidWalletTransaction_BlockWalletTrnTypeMasterFound = 7053,
        sp_IsValidWalletTransaction_TransactionBlockedChannel = 7054,
        sp_IsValidWalletTransaction_UserWalletBlockTrnTypeMasterFound = 7055,
        sp_IsValidWalletTransaction_AllowTrnTypeRoleWiseNotFound = 7056,
        sp_IsValidWalletTransaction_success = 7057,
        sp_CrDrWalletTransaction_InvalidRefNoTQ = 7058,
        sp_CrDrWalletTransaction_InvalidRefNoTO = 7059,
        sp_GetMemberBalance_InvalidDrWallet = 7060,
        sp_CrDrWalletTransaction_InvalidCrWallet = 7061,
        sp_CrDrWalletTransaction_InsufficientBalance = 7062,
        sp_CrDrWalletTransaction_Success = 7063,
        sp_CrDrWalletTransaction_InternalError = 7064,
        sp_WithdrawInternalTransfer_InvalidTransaction = 7065,
        sp_WithdrawInternalTransfer_SelfAddressFound = 7066,
        sp_WithdrawInternalTransfer_InternalAddressMismatch = 7067,
        sp_WithdrawInternalTransfer_Success = 7068,
        sp_MonthlyBalanceStatisticInsert_Fail = 7069,
        // SP_NoDataFound = 7070,
        sp_CalculateCharge_ChargeConfigurationNotFound = 7071,
        sp_CalculateCharge_Success = 7072,
        Sp_CalculateCharge_InternalError = 7070,
        Sp_CalculateCharge_InvalidMakerTakerBit = 7073,
        sp_DeductionChargeWallet_InternalError = 7074,
        sp_DeductionChargeWallet_Fail = 7075,
        sp_DeductionChargeWallet_InValidAmount = 7076,
        sp_CalculateAndDeductCharge_InternalError = 7077,
        sp_CalculateAndDeductCharge_Fail = 7078,
        sp_DeductionChargeWallet_InvalidTransaction = 7079,
        sp_DeductionWallet_Scccess = 7080,
        //---
        sp_CheckShadowLimit_InternalError = 7081,
        sp_CheckUserBalance_InternalError = 7082,
        sp_CheckUserBalance_Success = 7083,
        sp_CheckUserBalance_Fail = 7084,
        SP_IsVAlidPolicy_InternalError = 7085,
        sp_StakingSchemeRequest_DuplicateStaking = 7086,
        SP_InsertUpdateProfit_Success = 7087,
        SP_InsertUpdateProfit_Fail = 7088,
        SP_InsertUpdateProfit_InternalError = 7089,
        sp_InsufficientBalance = 7090,
        //sp_DeductionChargeWallet_ReleaseAmount0 = 7091,
        //sp_DeductionChargeWallet_ReleaseAmount0 = 21072, //ntrivedi 13-03-2019 errorcode changes for devang bhai charge balance 
        sp_InsufficientBalanceForCharge = 7091,
        SP_MarginFundTransfer_InternalError = 7092,
        SP_MarginFundTransfer_Success = 7093,
        SP_MarginFundTransfer_Invalid_Trading_Wallet = 7094,
        SP_MarginFundTransfer_InSufficientBalance = 7095,
        SP_MarginFundTransfer_InvalidAmt = 7096,

        sp_CrDrMarginWallet_InvalidTradingAmt = 7097,
        sp_CrDrMarginWallet_InvalidMarginAmt = 7098,
        sp_CrDrMarginWallet_InvalidSafetyAmt = 7099,
        SP_UnstakingSchemeReq_UnableUnstakingBeforeMaturity = 7100,
        sp_CrDrMarginWallet_InvalidTradingBal = 21000,
        sp_CrDrMarginWallet_InvalidMarginBal = 21001,
        sp_CrDrMarginWallet_InvalidSafetyBal = 21002,
        sp_CrDrMarginWallet_Success = 21003,
        sp_CrDrMarginWallet_InternalError = 21004,
        SP_MarginProcess_InternalError = 21005,
        SP_MarginProcess_Sucess = 21006,
        SP_MarginProcess_Pending = 21036,
        SP_CreateMarginWallet_Alreadyexistwallet = 21007,
        SP_CreateMarginWallet_InvalidWalletType = 21008,
        SP_CreateMarginWallet_InternalError = 21009,
        sp_ConvertFundWalletOperation_InternalError = 21010,
        sp_ConvertFundWalletOperation_InsufficientUserWalletBalance = 21011,
        sp_ConvertFundWalletOperation_InsufficientOrgWalletBalance = 21012,
        sp_ConvertFundWalletOperation_InvalidAmount = 21013,
        sp_ConvertFundWalletOperation_InValidCreditCurrency = 21014,
        sp_ConvertFundWalletOperation_InValidDebitCurrency = 21015,
        sp_ConvertFundWalletOperation_InValidUserCreditWalletNotFound = 21016,
        sp_ConvertFundWalletOperation_InValidUserDebitWalletNotFound = 21017,
        sp_ConvertFundWalletOperation_InValidOrgCreditWalletNotFound = 21018,
        sp_ConvertFundWalletOperation_InValidOrgDebitWalletNotFound = 21019,
        sp_ConvertFundWalletOperation_DuplicateTrxn = 21020,
        //sp_ConvertFundWalletOperation_InValidOrgWalletID = 21020,
        sp_IsValidWalletTransaction_MarginWalletNotUsedForThisTransaction = 21021,
        sp_IsValidWalletTransaction_WalletNotFound = 21022,
        sp_MarginChargeWalletBGTask_OrganizationNotFound = 21023,
        sp_MarginChargeWalletBGTask_OrganizationMarginWalletNotFound = 21024,
        sp_MarginChargeWalletBGTask_OrganizationSafertyWalletNotFound = 21025,
        sp_MarginChargeWalletBGTask_ChargesNotFound = 21026,
        sp_MarginChargeWalletBGTask_ChargesGreaterThanZero = 21027,
        sp_MarginChargeWalletBGTask_WalletNotFOund = 21028,
        sp_MarginChargeWalletBGTask_NotMatch = 21029,
        sp_MarginChargeWalletBGTask_Success = 21030,
        sp_MarginChargeWalletBGTask_InternalError = 21031,
        sp_MarginChargeWalletCallBGTask_InternalError = 21032,
        sp_MarginChargeWalletCallBGTask_Fail = 21033,
        sp_MarginChargeWalletBGTask_ToptalZero = 21034,
        sp_MarginReleaseHoldWalletWithCharge_Insufficient_Balance = 21035,
        SP_CreateMarginWallet_Success = 21037,
        SP_MarginProfitCrDr_InternalError = 21038,
        SP_MarginProfitCrDr_Invalid_Request = 21039,
        SP_MarginProfitCrDr_Org_Not_Found = 21040,
        SP_MarginProfitCrDr_CrDrAmountDontSame = 21041,
        SP_MarginProfitCrDr_Success = 21042,
        sp_ConvertCurrency_Success = 21043,
        sp_ConvertCurrency_PairNotFound = 21044,
        sp_ConvertCurrency_InternalError = 21045,
        SP_CalculateCharge_OrgNotFoun = 21046,
        SP_CalculateCharge_MarketCurrencyNotfound = 22046,
        SP_CalculateCharge_DeductCurrencyNotfound = 22047,
        sp_CheckWalletTranLimitTemp_WithdrawLimitExceed = 21047,
        SP_MarginCalculateCharge_OrgNotFoun = 21048,

        #region Deposition Recon Error Codes From 21049 to 21070

        SP_DepositionRecon_InvalidOldStatusForActionTypeOne = 21049,
        SP_DepositionRecon_AlreadyProcessed = 21050,
        SP_DepositionRecon_InvalidOldStatusForActionTypeThree = 21051,
        SP_DepositionRecon_TrnNoNotFound = 21052,
        SP_DepositionRecon_DepositionForAdminAddress = 21053,
        SP_DepositionRecon_DepositionAddressNotFound = 21054,
        SP_DepositionRecon_Success = 21055,
        SP_DepositionRecon_Fail = 21056,
        SP_DepositionRecon_InternalError = 21057,
        SP_DepositionRecon_WalletIdNotFond = 21058,
        SP_DepositionRecon_InvalidOldStatusForActionTypeFive = 21059,
        SP_DebitWalletWithChargeForDepositRecon_Success = 21060,

        #endregion
        sp_DebitWalletWithCharge_NotChargeLimitExceed = 21071,
        #region Unstaking Error Codes From 21147 to 21160
        sp_UnStakingSchemeRequest_StakingHistoryDataNotFound = 21147,
        sp_UnStakingSchemeRequest_RequestAlreadySentToAdmin = 21148,
        sp_UnStakingSchemeRequest_PolicyDetailDataNotFound = 21149,
        sp_UnStakingSchemeRequest_PolicyMasterDataNotFound = 21150,
        sp_UnStakingSchemeRequest_UnableToUnstakBeforeMaturityFromUserSide = 21151,
        sp_UnStakingSchemeRequest_NewPolicyDetailDataNotFound = 21152,
        sp_UnStakingSchemeRequest_OnlyDegradeAllow = 21153,
        sp_UnStakingSchemeRequest_InsufficientBalanceInDebiterAccountWithInterest = 21154,
        sp_UnStakingSchemeRequest_InsufficientBalanceInDebiterAccountWithoutInterest = 21155,
        sp_UnStakingSchemeRequest_ChargeDeductionIsMoreThenCrAmount = 21156,
        sp_UnStakingSchemeRequest_NegetiveAmountNotCredited = 21157,
        #endregion

        #endregion

        #region Affiliate

        sp_AffiliateCommissionSlabSignUp_No_Record_Found = 21072,
        sp_AffiliateCommissionSlabSignUp_CountIsZero = 21073,
        sp_DeductAffiliateCommission_OrganizationNotFound = 21074,
        sp_DeductAffiliateCommission_Insufficientbalance = 21075,
        sp_DeductAffiliateCommission_Success = 21076,
        sp_DeductAffiliateCommission_InternalError = 21077,
        sp_AffiliateCommissionSlabSignUp_Success = 21078,
        sp_AffiliateCommissionSlabSignUp_InternalError = 21079,
        sp_CalculateAndDeductAffiliateCommission_InternalError = 21080,
        sp_CalculateAffiliateCommission_NoRecordFound = 21081,
        sp_CalculateAffiliateCommission_Success = 21082,
        sp_CalculateAffiliateCommission_InternalError = 21083,
        sp_AffiliateCommissionSlabDeposition_InternalError = 21084,
        sp_AffiliateCommissionSlabDeposition_Success = 21085,
        sp_AffiliateCommissionSlabDeposition_ZeroDeposit = 21086,
        sp_AffiliateCommissionFlatSignUp_NotFound = 21087,
        sp_AffiliateCommissionFlatSignUp_ZeroAffiliate = 21088,
        sp_AffiliateCommissionFlatSignUp_Success = 21089,
        sp_AffiliateCommissionFlatSignUp_InternalError = 21090,
        sp_AffiliateCommissionSlabBuyTrading_InternalError = 21091,
        sp_AffiliateCommissionSlabBuyTrading_Success = 21092,
        sp_AffiliateCommissionSlabBuyTrading_NotFoundButrading = 21093,
        sp_AffiliateCommissionSlabSellTrading_InternalError = 21094,
        sp_AffiliateCommissionSlabSellTrading_Success = 21095,
        sp_AffiliateCommissionSlabSellTrading_NotFoundButrading = 21096,
        sp_CalculateAndDeductAffiliateCommission_NACommision = 21097,
        sp_AffiliateCommissionMLMDeposition_NotFound = 21098,
        sp_AffiliateCommissionMLMDeposition_Success = 21099,
        sp_AffiliateCommissionMLMDeposition_InternalError = 21100,
        sp_AffiliateCommissionMLMBuyTrading_NotFound = 21101,
        sp_AffiliateCommissionMLMBuyTrading_Success = 21102,
        sp_AffiliateCommissionMLMBuyTrading_InternalError = 21103,
        sp_AffiliateCommissionMLMSellTrading_NotFound = 21104,
        sp_AffiliateCommissionMLMSellTrading_Success = 21105,
        sp_AffiliateCommissionMLMSellTrading_InternalError = 21106,
        sp_AffiliateCron_InternalError = 21107,
        SP_ReferCommissionSignUp_InternalError = 21108,
        SP_ReferCommissionSignUp_Success = 21109,
        sp_DeductReferralCommission_OrgNotFound = 21110,
        sp_DeductReferralCommission_InsuffiecientBal = 21111,
        sp_DeductReferralCommission_Success = 21112,
        sp_DeductReferralCommission_InternalError = 21113,

        sp_CreditArbitrageProviderInitialBalance_InvalidWalletTypename = 21114,
        sp_CreditArbitrageProviderInitialBalance_InvalidSerPro = 21115,
        sp_CreditArbitrageProviderInitialBalance_InvalidInput = 21116,
        sp_CreditArbitrageProviderInitialBalance_AlreadyCredit = 21117,
        sp_CreditArbitrageProviderInitialBalance_OrderAlreadyCredit = 21118,
        sp_CreditArbitrageProviderInitialBalance_Sucess = 21119,
        sp_CreditArbitrageProviderInitialBalance_InternalError = 21120,

        Sp_ArbitrageCalculateCharge_OrgNotFound = 21121,
        Sp_ArbitrageCalculateCharge_NotFound = 21122,
        Sp_ArbitrageCalculateCharge_InvalidMakerTaker = 21123,
        Sp_ArbitrageCalculateCharge_Success = 21124,
        Sp_ArbitrageCalculateCharge_InternalError = 21125,

        sp_ArbitrageWalletFundTransfer_InvalidWalletTypename = 21126,
        sp_ArbitrageWalletFundTransfer_LeveargeNotAllow = 21127,
        sp_ArbitrageWalletFundTransfer_InvalidArbitrageWallet = 21128,
        sp_ArbitrageWalletFundTransfer_InvalidTradingWallet = 21129,
        sp_ArbitrageWalletFundTransfer_AlreadyCredit = 21130,
        sp_ArbitrageWalletFundTransfer_Sucess = 21131,
        sp_ArbitrageWalletFundTransfer_InternalError = 21132,


        sp_CreditArbitrageProviderBalance_InvalidWalletTypename = 21133,
        sp_CreditArbitrageProviderBalance_InvalidSerPro = 21134,
        sp_CreditArbitrageProviderBalance_InvalidInput = 21135,
        sp_CreditArbitrageProviderBalance_WalletNotFound = 21136,
        sp_CreditArbitrageProviderBalance_Sucess = 21137,
        sp_CreditArbitrageProviderBalance_InternalError = 21138,

        sp_ArbitrageRecon_InternalError = 21139,
        sp_ArbitrageRecon_Success = 21140,
        sp_ArbitrageRecon_InvalidRequestId = 21141,
        sp_ArbitrageRecon_InvalidInput = 21142,
        sp_ArbitrageRecon_InvalidWallet = 21143,

        sp_ReleaseHoldWalletWithCharge_Invalid_Trn = 21144,
        sp_ReleaseHoldWalletWithCharge_Invalid_Amount = 21145,
        sp_ReleaseHoldWalletWithCharge_Insuffiecnt_Amount = 21146,

        #endregion
        // nntrivedi end 26-12-2018

        #region MarginTradingWalletSP
        sp_MarginHoldWalletWithCharge_TransactionAlreadyProcessedInvalidRefNo = 51000,
        sp_MarginHoldWalletWithCharge_InsufficientBalance = 51001,
        sp_MarginHoldWalletWithCharge_CanNotExceedChargeLimit = 51002,
        sp_MarginHoldWalletWithCharge_Success = 51003,
        sp_MarginHoldWalletWithCharge_InternalError = 51004,
        sp_MarginHoldWalletWithCharge_Invalid_Amount = 51005,
        sp_MarginGetMemberBalance_Wallet_Not_Found_fromUserandWalletType = 51006,
        sp_MarginGetMemberBalance_Wallet_Not_Found = 51007,
        sp_MarginGetMemberBalance_Success = 51008,
        sp_MarginCheckUserBalance_FAILED = 51009,
        sp_MarginCheckUserBalance_Success = 51010,
        sp_MarginCheckUserBalance_FAIL = 51011,
        sp_MarginCheckUserBalance_InternalError = 51012,
        sp_MarginIsValidWalletTransaction_Invalid_User_Wallet = 51013,
        sp_MarginIsValidWalletTransaction_Transaction_isnot_allowed = 51014,
        sp_MarginIsValidWalletTransaction_Currency_isnot_valid = 51015,
        sp_MarginIsValidWalletTransaction_Transaction_is_blocked = 51016,
        sp_MarginIsValidWalletTransaction_Channel_is_blocked = 51017,
        sp_MarginIsValidWalletTransaction_Trntype_is_blocked = 51018,
        sp_MarginIsValidWalletTransaction_Success = 51019,
        sp_MarginIsValidWalletTransaction_InternalError = 51020,

        sp_MarginCrDrWalletForHoldWithCharge_CrDrCredit_SettledBalMismatchCrWallet = 51021,
        sp_MarginCrDrWalletForHoldWithCharge_CrDrCredit_SettledBalMismatchCrWalletSecCur = 51022,
        sp_MarginCrDrWalletForHoldWithCharge_CrDrCredit_SettledBalMismatchDrWallet = 51023,
        sp_MarginCrDrWalletForHoldWithCharge_CrDrCredit_SettledBalMismatchDrWalletSecCur = 51024,
        sp_MarketHoldWallet_MarketTrade_Insufficient_Balance_for_MarketTrade = 51025,
        Sp_MarginCalculateCharge_Not_Found = 51026,
        Sp_MarginCalculateCharge_InValidMakerBit = 51027,
        Sp_MarginCalculateCharge_Success = 51028,
        Sp_MarginCalculateCharge_InternalError = 51029,
        SP_MarginCalculateAndDeductCharge_InternalError = 51030,
        sp_MarginDeductionChargeWallet_Invalid_Transaction = 51031,
        sp_MarginDeductionChargeWallet_Fail = 51032,
        sp_MarginDeductionChargeWallet_Invalid_Amount = 51033,
        sp_MarginDeductionChargeWallet_InsuffientAmount = 51034,
        sp_MarginDeductionChargeWallet_Success = 51035,
        sp_MarginDeductionChargeWallet_InternalError = 51036,
        //ntrivedi 03-04-2019
        OpenPositionNotFound = 51037,
        InvalidBaseCurrency = 51038,
        SafetyWalletNotFound = 51039,
        MarginPriceCalculationisSuccess = 51040,
        LoanNotFound = 51041,
        SP_MarginFundTransferCalculation_LeverageMasterConfig_Not_found = 51042,
        SP_MarginFundTransferCalculation_Leverage_Zero_or_negative = 51043,
        SP_MarginWithdrawCalc_ProfitAmountMismatching = 51044,
        sp_MarginConvertFundWalletOperation_OpenPosition_Found_For_other_Currency = 51045,
        SP_AdminMarginChargeRequestApproval_InternalError = 51046,
        SP_AdminMarginChargeRequestApproval_Success = 51047,
        SP_AdminMarginChargeRequestApproval_Admin_approval_rejected = 51048,
        SP_AdminMarginChargeRequestApproval_Request_Not_Found = 51049,
        sp_MarginHoldWalletWithCharge_InsufficientSafetyBalance = 51050,
        OpenPositionLossMoreThanSafetyWallet = 51051,
        Safety_Wallet_Not_Found = 51052,
        SP_MarginFundTransferCalculation_Previous_Loan_Deposition_Required_for_Loss = 51053,
        sp_MarginCrDrWalletForHoldWithCharge_SettledQtyInvalid_TwoTranxParallelFetchFirstCurr = 51054,
        sp_MarginCrDrWalletForHoldWithCharge_SettledQtyInvalid_TwoTranxParallelFetchSecondCurr = 51055,






        #endregion

        #region APIPlanSubscription
        sp_APIPlanDepositProcess_WalletTypeNotFound = 13014, //Chirag 05/07/2019 21098 Changed to 13014 bcoz of duplication
        sp_APIPlanDepositProcess_OrgWalletNotFound = 13015, //Chirag 05/07/2019 21099 Changed to 13015 bcoz of duplication
        sp_APIPlanDepositProcess_UserWalletNotFound = 13016, //Chirag 05/07/2019 21100 Changed to 13016 bcoz of duplication
        sp_APIPlanDepositProcess_OrgUserNotFound = 13017, //Chirag 05/07/2019 21101 Changed to 13017 bcoz of duplication
        sp_APIPlanDepositProcess_Success = 13018, //Chirag 05/07/2019 21102 Changed to 13018 bcoz of duplication
        #endregion
        sp_DepositionProcess_InvalidWalletUsgeType = 13019,

        SP_IEOCrDrWalletTransaction_OrgNotFound = 13020,
        SP_IEOCrDrWalletTransaction_OrgCrWalletNotFound = 13021,
        SP_IEOCrDrWalletTransaction_OrgDrWalletNotFound = 13022,
        SP_IEOCrDrWalletTransaction_UserCrWalletNotFound = 13024,
        SP_IEOCrDrWalletTransaction_UserDrWalletNotFound = 13023,
        SP_IEOCrDrWalletTransaction_InvalidAmt = 13025,
        SP_IEOCrDrWalletTransaction_InSuffientUserAmt = 13026,
        SP_IEOCrDrWalletTransaction_InSuffientOrgAmt = 13027,
        SP_IEOCrDrWalletTransaction_Success = 13028,
        SP_IEOCrDrWalletTransaction_InternalError = 13029,

        SP_IEOTransactionPreConfirm_OrgNotFound = 13030,
        SP_IEOTransactionPreConfirm_UserWalletNotFound = 13031,
        SP_IEOTransactionPreConfirm_userInvalidAmt = 13032,
        SP_IEOTransactionPreConfirm_userInsuffAmt = 13033,
        SP_IEOTransactionPreConfirm_RoundNotFound = 13034,
        SP_IEOTransactionPreConfirm_CurrentRateNotFound = 13035,
        SP_IEOTransactionPreConfirm_MinMaxamt = 13036,
        SP_IEOTransactionPreConfirm_SlabNotFound = 13037,
        SP_IEOTransactionPreConfirm_InternalError = 13038,
        SP_IEOConfirmTransaction_Success = 13039,
        SP_IEOConfirmTransaction_InternalError = 13040,
        SP_IEOTransactionPreConfirm_allocate = 13041,
        SP_IEOTransactionPreConfirm_ExideLimit = 13042,

        sp_IEOAdminWalletCreditBalance_Success = 13043,
        sp_IEOAdminWalletCreditBalance_InternalError = 13044,
        sp_IEOAdminWalletCreditBalance_InvalidInput = 13045,
        sp_IEOAdminWalletCreditBalance_InvalidCurrency = 13046,
        sp_IEOAdminWalletCreditBalance_InvalidWallet = 13047,
        sp_IEOAdminWalletCreditBalance_SelfOrder = 13048,

        //Create Error Code for Referral User by Rushali (24-12-2019)
        Status14200DataNotInsert=14200,
        Status14199PassDataNotFound = 14199,



        // Back Office Organization Admin Panel
        Status9005DomainAlreadyExist = 9005,
        Status9006DomainInsertError = 9006,
        Status9007DomainNotAvailable = 9007,
        Status9008ActiveDomainNotAvailable = 9008,
        Status9009DisActiveDomainNotAvailable = 9009,
        Status9010DomainUpdateError = 9010,
        Status9011DomainCountNotAvailable = 9011,
        Status9017OrganizationUpdateError = 9017,
        Status9021BackOffComDataNotAvailable = 9021,
        Status9022BackOffComIdDataNotAvailable = 9022,
        Status9023SuccessBackOffComTrailError = 9023,
        Status9026ComplainCountNotAvailable = 9026,
        Status9027BackOffComIdNotAvailable = 9027,
        Status9028ActivityDataNotAvailable = 9028,
        Status9029IpInvalid = 9029,
        Status9030BizUserEmailExist = 9030,
        Status9031VerifyPending = 9031,
        Status9032UseralreadRegister = 9032,
        Status9033SignUpUserRegisterError = 9033,
        Status9034MobileInvalid = 9034,
        Status9035SignUPMobileValidation = 9035,
        Status9036VerifyPending = 9036,
        Status9037TempUserMobileExist = 9037,
        Status9038BizUserNameExist = 9038,
        Status9039VerifyPending = 9039,
        Status9040TempUserNameExist = 9040,
        Status9041EmailOrPhone = 9041,
        Status9042NotFoundRecored = 9042,
        Status9043UserNotRegister = 9043,
        Status9044ResetPasswordLinkExpired = 9044,
        Status9045EmailLinkBlanck = 9045,
        Status9046NotFoundRecored = 9046,
        Status9047SignUpRole = 9047,
        Status9048SignUPOTP = 9048,
        Status9049SignUpReSendOTP = 9049,
        Status9050BackOfficeInvalidEmail = 9050,
        Status9051UseralreadRegister = 9051,
        Status9052ApplicationAlreadyExist = 9052,
        Status9053ApplicationInsertError = 9053,
        Status9054ApplicationUpdateError = 9054,
        Status9055AppCountNotAvailable = 9055,
        Status9056AppNotAvailable = 9056,
        Status9057ActiveAppNotAvailable = 9057,
        Status9058DisActiveAppNotAvailable = 9058,
        Status9059BackOffReqDomainId = 9059,
        Status9060BackOffReqAppId = 9060,
        Status9061BackOffInvalidDomainorApp = 9061,
        Status9062UserWiseApplicationInsertError = 9062,
        Status9063BackOffAppIdNotAvailable = 9063,
        Status9064BackOffAppIdDataNotAvailable = 9064,
        Status9065BackOffAppUpdateError = 9065,
        Status9066UserWiseAppNotAvailable = 9066,
        Status9067UserWiseApplicationAlreadyExist = 9067,
        Status9071UserProfileConfigurationNotInsert = 9071,
        Status9072UserProfileConfigurationNotupdate = 9072,
        Status9073BackOffReqValidDomainId = 9073,
        Status9074BackOffReqValidAppId = 9074,
        Status9075UserProfileConfigurationNotDelete = 9075,
        Status9076UserProfileLevelExist = 9076,
        Sataus9077SubscriptionAmountShouldBeGraterThan0 = 9077,
        Status9078LevelNameAllowString = 9078,
        ///User backoffice  
        Status8001AddEmailInsertFail = 8001,
        Status8003AddEmailUpdateFail = 8003,
        Status8006AddPhoneUpdateFail = 8006,
        Status8007PnoneNumberUpdateFail = 8007,
        Status8008EmailDeleteFail = 8008,
        Status8009SecurityQuestionFail = 8009,
        Status8013KYCConfigurationExist = 8013,
        Status8014KYCConfigurationUpdateFail = 8014,
        Status8016KYCConfigurationMappingUpdateFail = 8016,
        Status8017KYCConfigurationUserMappingAllreadyExist = 8017,
        Status8019KYCConfigurationUserMappingnotupdate = 8019,
        Status8021DocumentmasterExist = 8021,
        Status8022DocumentMasterNotAdded = 8022,
        Status8023DocumentMasterNotupdate = 8023,
        Status8025LevekMasterNotAdded = 8025,
        Status8026IsKYCNameExist = 8026,
        Status8027AddKYCLevelNotUpdated = 8027,
        Status8028AddKYCRemark = 8028,
        Status8029KYCrecordnotfound = 8029,
        Status8034KYCrecordnotfound = 8034,
        Status8035KYCrecordnotfound = 8035,
        Status8036InvalidIPRange = 8036,
        Status8037NotInsertIpRange = 8037,
        Status8038IPrangeAlreadyExist = 8038,
        Status8039IPrangeNotDeleted = 8039,
        Status8040ComplaintPriorityExist = 8040,
        Status8041NotInsertComplaintPriority = 8041,
        Status8042NotupdateComplaintPriority = 8042,
        Status8043NotDeleteComplaintPriority = 8043,


        /// Added by pankaj for display error message for password policy configuration

        Status8044PasswordPolicyExist = 8044,
        Status8045NotAddPasswordPolicy = 8045,
        Status8046NotUpdatePasswordPolicy = 8046,
        Status8047NotDeletePasswordPolicy = 8047,

        Status8053LinkExpiryTime = 8053,
        Status8054MaxfppwdDay = 8054,
        Status8055MaxfppwdMonth = 8055,
        Status8056OTPExpiryTime = 8056,
        Status8057PwdExpiretime = 8057,
        SuccessfullUpdateUserLang = 8058,




        // Social Profile Error  Code
        Status12002Max_Number_Followers_can_Follow = 12002,
        Status12003Min_Balance_Require_in_Follower_Account_to_Follow = 12003,
        Status12007Default_Visibility_of_Profile = 12007,
        Status12008Min_Trade_Volume_Requir_in_Time = 12008,
        Status12009Subscription_Charge_Frequency = 12009,
        Status12010Can_Add_Pair_to_Watchlist = 12010,
        Status12014Minimum_Copy_Trade_Percentage = 12014,
        Status12015Default_Copy_Trade_Percentage = 12015,
        Status12016Maximum_Copy_Trade_Percentage = 12016,
        Status12017Can_Copy_Trade = 12017,
        Status12018Can_Mirror_Trade = 12018,
        Status12019Enable_Auto_Copy_Trade_Functionality = 12019,
        Status12020UnsubscribOtherSubscription = 12020,
        Status12021InvalidSocialProfile = 12021,
        Status12022SubscribeplanNotAvailable = 12022,
        Status12023Set_only_max_number_of_follower = 12023,
        Status12024Minimum_Balamce_required_Leader = 12024,
        Status12025Default_Copy_Trade_Percentageforfront = 12025,
        Status12026Maximum_Copy_Trade_PercentageforFront = 12026,
        Status12027Maximum_Transaction_Amount_Limit = 12027,
        Status12028Maximum_Number_of_Transactions_Limit = 12028,
        Status12029Min_Number_of_Followers_can_Follow = 12029,
        Status12030Maximum_Number_of_Leaders_to_Allow_Follow = 12030,
        Status12031Min_Number_of_Followers_can_Follow_front = 12031,
        TempalteAlreadyMapwithCategory = 12032,
        Status12033Can_Copy_Trade_Percentage = 12033,
        Status12034Can_Mirror_Trade_Percentage = 12034,
        Status12035Copy_Mirror_Or_Trade_Any_One = 12035,
        Status12036FollowerNotAvailable = 12036,
        Status12037LeaderId_NotNull = 12037,
        Status12038UserNotSubscibeSocialProfile = 12038,
        Status12039UserNotSubscribAnyProfile = 12039,
        Status12041Already_Exsits_Group = 12041,
        Status12042Already_Exsits_Watch = 12042,
        Status12045UnFollowWatchNotAvailable = 12045,
        Status12046LederlimitExide = 12046,

        //Referral Program        
        Status32001InvalidReferralPayType = 32001,
        Status32002GetFailReferralPayType = 32002,
        Status32003FailReferralPayTypeList = 32003,
        Status32004FailReferralPayTypeDropDown = 32004,
        Status32005InvalidUpdateReferralPayType = 32005,
        Status32006FailReferralPayTypeStatus = 32006,
        Status32007InvalidReferralChannelType = 32007,
        Status32008GetFailReferralChannelType = 32008,
        Status32009FailReferralChannelTypeList = 32009,
        Status32010FailReferralChannelTypeDropDown = 32010,
        Status32011InvalidUpdateReferralChannelType = 32011,
        Status32012FailReferralChannelTypeStatus = 32012,
        Status32013InvalidReferralServiceType = 32013,
        Status32014GetFailReferralServiceType = 32014,
        Status32015FailReferralSeriesTypeList = 32015,
        Status32016FailReferralSeriesTypeDropDown = 32016,
        Status32017InvalidUpdateReferralServiceType = 32017,
        Status32018FailReferralServiceTypeStatus = 32018,
        Status32019ReferralPayTypeExist = 32019,
        Status32020ReferralChannelTypeExist = 32020,
        Status32021ReferralServiceTypeExist = 32021,
        Status32022ReferralCodeNotAvailable = 32022,
        Status32023InvalidReferralService = 32023,
        Status32024InvalidUpdateReferralService = 32024,
        Status32025FailDisableReferralServiceStatus = 32025,
        Status32026FailEnableReferralServiceStatus = 32026,
        Status32027GetFailReferralServiceById = 32027,
        Status32028FailReferralServiceList = 32028,
        Status32029InvalidReferralChannel = 32029,
        Status32030FailReferralChannelList = 32030,
        Status32031FailReferralChannelCountForAdmin = 32031,
        Status32032FailReferralChannelListByChannelTypeId = 32032,
        Status32033FailReferralChannelListByChannelTypeId = 32033,
        Status32034FailReferralChannelCountForUserInvite = 32034,
        Status32035FailReferralChannelListByUserChannelTypeId = 32035,
        Status32036FailReferralChannelCountForUser = 32036,
        Status32037ExpireDateMustBeGraterThanActiveDate = 32037,
        Status32038FailReferralUserListForAdmin = 32038,
        Status32039FailReferralUserListForUser = 32039,
        Status32040FailReferralServiecsDropDown = 32040,
        Status32041ReferralServiceNotExist = 32041,
        Status32042ReferralChannelTypeNotExist = 32042,
        Status32043ReferralSendEmailBlankRequest = 32043,
        Status32044ReferralSendSMSBlankRequest = 32044,
        Status32045ReferralEmailHourlyLimitExceed = 32045,
        Status32046ReferralEmailDailyLimitExceed = 32046,
        Status32047ReferralEmailWeeklyLimitExceed = 32047,
        Status32048ReferralEmailMonthlyLimitExceed = 32048,
        Status32049ReferralSMSHourlyLimitExceed = 32049,
        Status32050ReferralSMSDailyLimitExceed = 32050,
        Status32051ReferralSMSWeeklyLimitExceed = 32051,
        Status32052ReferralSMSMonthlyLimitExceed = 32052,
        Status32053ReferralHourlyLimitRequired = 32053,
        Status32054ReferralDailyLimitRequired = 32054,
        Status32055ReferralWeeklyLimitRequired = 32055,
        Status32056ReferralMonthlyLimitRequired = 32056,
        Status32057FailReferralURL = 32057,
        Status32058FailReferralUserClickListForAdmin = 32058,
        Status32059FailReferralUserClickListForUser = 32059,
        Status32060GetFailReferralService = 32060,
        Status32061FailReferralRewardsListForAdmin = 32061,
        Status32062FailReferralRewardsListForUser = 32062,
        Status32063InvalidReferralRewards = 32063,
        Status32064InvalidReferralUserClickData = 32064,
        Status32065ReferralFromDateGreterThanToday = 32065,
        Status32066ReferralToDateGreterThanToday = 32066,
        Status32067ReferralCompareFromDateTodate = 32067,
        Status32068ReferralBothDateFromDateTodateRequired = 32068,
        Status32069ReferralRequiredServiceTypeName = 32069,
        Status32070ReferralRequiredValidServiceTypeName = 32070,
        Status32071ReferralRequiredPayTypeName = 32071,
        Status32072ReferralRequiredValidPayTypeName = 32072,
        Status32073ReferralRequiredChannelTypeName = 32073,
        Status32074ReferralRequiredValidChannelTypeName = 32074,
        ReferralRecordAlreadyExist = 32081,
        ReferralRecordNotFound = 32082,
        CreateTrn_CheckWithdrawBeneficiary_Fail = 32083,
        RoleIsDisabled = 4175,
        UserWithRoleAlreadyExist = 4177,
        InvalidAccessRightOrRoleId = 4181,
        PermissionGroupIsAssignToGroup = 4183,
        Status4189InValidReferralCode = 4189,

        //Affiliate System Error Code  Uday 14-02-2019
        UserIsNotAsffiliateUser = 31001,
        InvalidAffliateSchemeType = 31002,
        InvalidAffliateReferCode = 31003,
        InvalidAffiliatePromotionType = 31004,
        AffiliateSendEmailBlankRequest = 31005,
        AffiliateSendSMSBlankRequest = 31006,
        AffiliateEmailSendSuccess = 31007,
        UserIsNotSelectEmailPromotion = 31008,
        AffiliateEmailHourlyLimitExceed = 31009,
        AffiliateSMSSendSuccess = 31010,
        UserIsNotSelectSMSPromotion = 31011,
        AffiliateSMSHourlyLimitExceed = 31012,
        AffiliateSMSDailyLimitExceed = 31013,
        AffiliateEmailDailyLimitExceed = 31014,
        AfRecordAlreadyExist = 31015,
        //AfNoRecordFound = 31016,
        //AfDuplicateRecord = 31017,
        //AfSchemeMasterIdRequired = 31018,
        Status31019FailToAddAffiliateSchemeTypeMapping = 31019,
        //Status31020FailAffiliateSchemeTypeMappingList = 31020, //Chirag 05/07/2019 bcoz of duplication and no reference in code
        //Status31021GetFailAffiliateSchemeTypeMapping = 31021, //Chirag 05/07/2019 bcoz of duplication and no reference in code
        Status31022FailToUpdateAffiliateSchemeTypeMapping = 31022,
        AfNoRecordFound = 31020,
        AfDuplicateRecord = 31021,
        AfSchemeMasterIdRequired = 31028,
        AfSchemeTypeIdRequired = 31023,
        AfProIdRequired = 31040,
        AfdetailIdRequired = 31042,
        AfSchemeTypeMappingIdRequired = 31035,
        AfInvalidLevel = 31052,
        AfInvalidMinMaxValue = 31053,
        AfRecordAdded = 31054,
        AfRecordUpdated = 31055,
        AfRecordFound = 34056,



        //Depostion Interval

        Status33001FailAddOrUpdateDepositionInterval = 33001,
        Status33002FailDepositionIntervalList = 33002,
        Status33003FailDisableDepositionIntervalStatus = 33003,
        Status33004FailEnableDepositionIntervalStatus = 33004,
        Status33005FailMultichainAddressList = 33005,
        Status33006FailMultichainConnection = 33006,
        Status33007FailMultichainConnectionNode = 33007,
        SelfAddressFound = 33008,
        //ntrivedi 05-03-2018

        //ntrivedi 26-03-2019
        SP_MarginFundTransferCalculation_CanNotExceedLeverage = 15000,
        SP_MarginFundTransferCalculation_InvalidSafetyPercentage = 15001,
        SP_MarginFundTransferCalculation_You_can_only_take_Leverage_To_SecondCurrency = 15002,
        SP_ManageMarginPosition_InternalError = 15003,
        SP_ManageMarginPosition_Success = 15004,
        SP_ManageMarginPosition_OpenPositionNotFound = 15005,
        SP_ManageMarginPosition_InvalidCurrency = 15006,
        sp_MarginEODChargeCollection_UnexpectedError = 15007,
        sp_MarginEODChargeCollection_Success = 15008,
        sp_MarginEODChargeCollection_InternalError = 15009,
        sp_MarginEODChargeCollection_OrgRecordNotFound = 15010,
        sp_MarginProcessLeverageAccountEOD_LoanRecordNotFound = 15011,
        sp_MarginProcessLeverageAccountEOD_Success = 15012,
        sp_MarginProcessLeverageAccountEOD_InternalError = 15013,
        sp_MarginProcessLeverageAccountEOD_Open_Position_Not_found_Unable_to_Settle_Loan_Account = 15014,
        sp_MarginProcessLeverageAccountEOD_SuccessDoNothing = 15015,//margin balance + hold balance(open order) + open position < max leverage amount
        sp_MarginProcessLeverageAccountEOD_OrganizationNotFound = 15016,
        sp_MarginEODChargeCollection_ChargeAlreadyDeducted = 15017,
        SP_MarginWithdrawCalc_Success = 15018,
        SP_MarginWithdrawCalc_Insufficient_balance_for_Withdraw = 15019,
        SP_MarginWithdrawCalc_Profit_Wallet_Not_found = 15020,
        SP_MarginWithdrawCalc_Safety_Wallet_Not_found = 15021,
        SP_MarginWithdrawCalc_Margin_Trading_Wallet_Not_found = 15022,
        SP_MarginWithdrawCalc_Open_Loan_Not_Found = 15023,
        SP_MarginWithdrawCalc_Invalid_Currency = 15024,
        SP_MarginWithdrawCalc_Invalid_Currency_Margin = 15025,
        SP_MarginWithdrawCalc_OpenPositionFound = 15026,
        SP_MarginWithdrawCalc_OpenOrderFound = 15027,
        SP_MarginWithdraw_Open_Loan_Not_Found = 15028,
        SP_MarginWithdraw_Withdraw_Amount_mustbe_greater_than_Zero = 15029,
        SP_MarginWithdraw_Invalid_amount = 15030,
        SP_MarginWithdraw_WalletBalanceMismatch_profit = 15031,
        SP_MarginWithdraw_WalletBalanceMismatch_safety = 15032,
        SP_MarginWithdraw_Success = 15033,
        SP_MarginWithdraw_InternalError = 15034,
        SP_UpgradeLoan_InternalError = 15035,
        SP_UpgradeLoan_Success = 15036,
        SP_UpgradeLoan_Insufficient_Balance = 15037,
        SP_UpgradeLoan_Invalid_Leverage_Zero_Negative = 15038,
        SP_UpgradeLoan_Invalid_Leverage_LessThanOrEqualOld = 15039,
        SP_UpgradeLoan_Invalid_Leverage_GreaterThanMax = 15040,
        SP_UpgradeLoan_LoanNotFound = 15041,
        SP_UpgradeLoan_Invalid_Status_Loan = 15042,
        MarginFirstCurrencyNotFound = 15043,
        MarginSecondCurrencyNotFound = 15044,
        MarginFirstCurrecyUserWalletNotFound = 15045,
        MarginSecondCurrecyUserWalletNotFound = 15046,
        MarginFirstCurrecyLeverageDetailNotFound = 15047,
        MarginSecondCurrecyLeverageDetailNotFound = 15048,
        MarginLeverageDetailFoundSuccess = 15049,
        MarginCurrencyNull = 15050,
        LoanIDNull = 15051,
        SP_ManageMarginPosition_Open_Loan_Not_Found_Sell = 15052,
        SP_ManageMarginPosition_Open_Loan_Not_Found_buy = 15053,
        SP_ManageMarginPosition_Safety_Wallet_NotFound = 15054,
        SP_MarginFundTransferCalculation_OnlyOneLoanAllowedPerUser = 15055,//Your Loan is Open for other Currency.Only one open loan is allowed
        SP_MarginFundTransferCalculation_OpenLoanNotFound = 15056,
        sp_MarginHoldWalletWithCharge_InternalOldOrderReleaseFail = 15057,//square off order second time then release old order
        sp_DebitWalletWithCharge_UserNotAllowedWithdrw = 15058,
        SP_MarginWithdrawCalc_UserNotAllowWithdrLoan = 15059,
        SP_MarginReleaseTransactin_InProcessing = 15060,
        sp_MarginCrDrWalletForHoldWithCharge_InProcessingFirstCurr = 15061,
        sp_MarginCrDrWalletForHoldWithCharge_InProcessingSecondCurr = 15062,
        SP_MarginFundTransferCalculation_SixHourTimeBetweenTwoLeverage = 15063,

        #region LPWalletErrorCode
        sp_LPHoldWalletWithCharge_UserOrderNotFound = 16000,
        SP_ArbitrageCreateWallet_Alreadyexistwallet = 16001,
        SP_ArbitrageCreateWallet_InvalidCurrency = 16002,
        SP_ArbitrageCreateWallet_InternalError = 16003,
        SP_ArbitrageCreateWallet_Success = 16004,
        sp_ArbitrageReleaseHoldWalletWithCharge_ProviderInsufficientBalance = 16005,
        sp_ArbitrageLPHoldWalletWithCharge_DuplicateTrnNo = 16006,
        sp_ArbitrageReleaseHoldWalletWithCharge_RecordNotFound = 16007,
        sp_ArbitrageGetLPBalance_WalletNotFound = 16008,
        sp_ArbitrageGetLPBalance_WalletNotFound_Provider_Currency = 16009,
        InvalidArbitarageWallet = 16010,
        sp_LPArbitrageCrDrWalletForHoldWithCharge_LPBalanceCrWalletMismatch = 16011,
        sp_LPArbitrageCrDrWalletForHoldWithCharge_LPBalanceDrWalletMismatch = 16012,
        ArbitrageLPWalletBalanceMismatch = 16013,

        sp_CheckWalletTranLimitTemp_MonthlyLimitExceed = 16014,
        sp_CheckWalletTranLimitTemp_WeeklyLimitExceed = 16015,
        sp_CheckWalletTranLimitTemp_velocityWeeklyLimitExceed = 16016,
        sp_CheckWalletTranLimitTemp_VelocityMonthlyLimitExceed = 16017,
        sp_CheckWalletTranLimitTemp_VelocityMonthlyCountExceed = 16018,
        sp_CheckWalletTranLimitTemp_VelocityWeeklyCountExceed = 16019,
        sp_CheckWalletTranLimitTemp_VelocityMinMax = 16020,

        sp_ArbitrageToTradingWalletFundTransfer_Invalid_WalletTypeName = 16021,
        sp_ArbitrageToTradingWalletFundTransfer_TransferNotAllowed = 16022,
        sp_ArbitrageToTradingWalletFundTransfer_Invalid_Arbitrage_Wallet = 16023,
        sp_ArbitrageToTradingWalletFundTransfer_Invalid_Trading_Wallet = 16024,
        sp_ArbitrageToTradingWalletFundTransfer_InsufficientAmount = 16025,
        sp_ArbitrageToTradingWalletFundTransfer_Success = 16026,
        sp_ArbitrageToTradingWalletFundTransfer_InternalError = 16027,
        sp_ArbitrageLPHoldWalletWithCharge_InsufficientBalance = 16028,

        sp_ReconSuccessAndDebitWalletWithChargeArbitrage_InsufficientBalance = 16029,
        sp_ReconSuccessAndDebitWalletWithChargeArbitrage_Success = 16030,
        sp_ReconSuccessAndDebitWalletWithChargeArbitrage_InternalError = 16031,

        #endregion

        InsertUpdateChargeConfigurationMasterArbitrage_Req_Null = 13010, //Chirag 05/07/2019 16028 Changed to 13010 bcoz of duplication
        SpecialChargeConfiguration_NoRecordFound = 13011, //Chirag 05/07/2019 16029 Changed to 13011 bcoz of duplication
        KYCComplaint_InvalidInput = 13012, //Chirag 05/07/2019 16030 Changed to 13012 bcoz of duplication
        ArbitrageWalletTypeMaster_NoRecordFound = 13013, //Chirag 05/07/2019 16031 Changed to 13013 bcoz of duplication
        ChargeConfigurationMasterArbitrage_AlredyExist = 16032,
        ChargeConfigurationMasterArbitrage_NoRecordFound = 16033,
        InvalidChargeConfigurationMasterID = 16034,
        sp_NormalTradingFullSettlementReleaseProfitAmountCron_Success = 16035,
        sp_NormalTradingFullSettlementReleaseProfitAmountCron_InternalError = 16036,
        sp_NormalTradingFullSettlementReleaseProfitAmount_Success = 16037,
        sp_NormalTradingFullSettlementReleaseProfitAmount_InternalError = 16038,

        sp_CrDrWalletForHoldWithCharge_ParallelSettlementFirstCurr = 16039,
        sp_CrDrWalletForHoldWithCharge_ParallelSettlementSecondCurr = 16040,
        NoDataFound_StakingScheme = 16041,//ntrivedi 24-07-2019 for new errorcode staking scheme
        InternalWithdrwStopped = 16042,
        OrderCancelFail= 16043,
        OrderCancelAllSuccess= 16044

    }

    //Rushabh 05-10-2018 as per instruction by nupoora mam change Enum 0 for Success and 1 for Fail
    public enum enResponseCodeService
    {
        Success = 0,
        Fail = 1,
        InternalError = 9
    }

    public enum enActivityType
    {
        ResetPassword = 0,
        ForgotPassword = 1,
        DeviceChange = 2,
        Benificiary = 3
    }

    public enum enResponseCode
    {
        Success = 0,
        Fail = 1,
        InternalError = 9
    }

    public enum enRegisterType
    {
        Mobile = 1,
        Email = 2,
        Standerd = 3,
    }

    public enum enCommunicationServiceType
    {
        SMS = 1,
        Email = 2,
        PushNotification = 3,
    }

    public enum enAppType //Use in SerProDetail table
    {
        WebSocket = 1,
        JsonRPC = 2,
        TCPSocket = 3,
        RestAPI = 4,
        HttpApi = 5,
        SocketApi = 6,
        BitcoinDeamon = 7,
        COINTTRADINGLocal = 8,
        Binance = 9,
        Bittrex = 10,
        TradeSatoshi = 11,
        Poloniex = 12,
        Coinbase = 13,
        Erc20Withdraw = 14,
        Twillio = 15,
        Multichain = 16,
        Plivo = 17,
        UpBit = 18,
        Huobi = 19,
        OKEx = 20,
        Kraken = 21,
        Bitfinex = 22,
        Gemini = 23,
        CEXIO = 24,
        Yobit = 25,
        EXMO = 26
    }

    public enum enWalletTrnType
    {
        Topup = 1,
        Deposit = 2,
        BuyTrade = 3, // ntrivedi 06-12-2018 
        Refund = 4,
        Commission = 5,
        Trans_IN = 7,
        SellTrade = 8,
        Withdrawal = 9,
        Charge = 11,
        Bonus = 16,
        ReleaseBlockAmount = 18, // for cancellation 
        AddUser = 19,
        StakingRequest = 20,
        InternalWithraw = 21,
        SelfOrderOrg = 22,
        AddressGeneration = 23,
        ConvertFund = 24,
        PublicAPIPlan = 32,
        MarginWalletTransfer = 25,
        UnblockDiffAmount = 34,
        ArbitrageWalletTransfer = 34,
        ArbitrageToTradingWalletTransfer = 35,
        IEOTransaction = 36
    }

    public enum enMarginWalletTrnType
    {
        ConvertFund = 24,
        MarginWalletTransfer = 25, // trading to margin wallet transfer
        BuyTrade = 3,
        SellTrade = 8,
        ReleaseBlockAmount = 18, // for cancellation 
        MarginTradingWalletChargeCollectionSafety = 29,
        TradingWalletTransfer = 30,
        MarginProfit = 31,
        MarginTradingWalletChargeCollectionMargin = 32,
        WithdrawLeverage = 33,
    }

    public enum enTransactionMarketType
    {
        LIMIT = 1,
        MARKET = 2,//No price , take from statestic table
        SPOT = 3,//No price , take from statestic table
        STOP_Limit = 4,//take extra para
        //buy at a specific price or lower when that event occurs.
        //sell at a specific price or higher when that event occurs.
        STOP = 5, //(stop-loss) same as Market + LIMIT
    }
    public enum enTransactionMarketType1
    {
        LimitBuy = 0,
        LimitSell = 1,
        MarketBuy = 2,
        MarketSell = 3,
        IOCBuy = 4,
        IOCSell = 5,
        LimitMakerBuy = 6,
        LimitMakerSell = 7,
        StopLimitBuy = 8,
        StopLimitSell = 9
    }
    public enum enWalletTranxOrderType
    {
        Debit = 1,
        Credit = 2
    }

    public enum enWalletLimitType
    {
        //Rushabh 13-02-2019
        TradingLimit = 1,
        WithdrawLimit = 9, //old value = 2 
        DepositLimit = 2, //old value = 3
        APICallLimit = 4
    }

    public enum enSignalREventType
    {
        Alert = 1,
        Nofification = 2,
        GrpMsg = 3,
        Channel = 4,
        BroadCast = 5
    }

    public enum enSubscriptionType
    {
        OneToOne = 1,
        Broadcast = 2
    }

    public enum enMethodName
    {
        ActiveOrder = 1,
        TradeHistory = 2,
        RecentOrder = 3,
        ActivityNotification = 4,
        //PairWise
        BuyerBook = 5,
        SellerBook = 6,
        OrderHistory = 7,
        MarketData = 8,
        Price = 9,
        BuyerSideWallet = 10,
        SellerSideWallet = 11,
        ChartData = 12,
        //Market wise
        PairData = 13,
        MarketTicker = 14,
        //Global 
        Chat = 15,
        News = 16,
        Announcement = 17,
        Time = 18,
        SendGroupMessage = 19,
        MarkBlockUnblockUser = 20,
        BlockUserUpdate = 21,
        ChatHistory = 22,
        OnlineUserCount = 23,
        OpenOrder = 24,
        StopLimitBuyerBook = 25,
        StopLimitSellerBook = 26,
        WalletActivity = 27,
        SessionExpired = 28,
        BulkBuyerBook = 29,
        BulkSellerBook = 30,
        EnvironmentMode = 31,
        BulkOrderHistory = 32,
        //Arbitrage Method
        ActiveOrderArbitrage = 41,
        TradeHistoryArbitrage = 42,
        RecentOrderArbitrage = 43,
        ActivityNotificationArbitrage = 44,
        BuyerBookArbitrage = 45,
        SellerBookArbitrage = 46,
        OrderHistoryArbitrage = 47,
        MarketDataArbitrage = 48,
        PriceArbitrage = 49,
        ChartDataArbitrage = 50,
        PairDataArbitrage = 51,
        MarketTickerArbitrage = 52,
        StopLimitBuyerBookArbitrage = 53,
        StopLimitSellerBookArbitrage = 54,
        WalletActivityArbitrage = 55,
        WalletBalArbitrage = 56,
        ProviderMarketDataArbitrage = 57,
        ProfitIndicatorArbitrage = 58,
        ExchangeListSmartArbitrage = 59,
        ActiveOrderRemove=60
    }
    public enum enTokenType
    {
        ByAccessToken = 1,
        ByUserID = 2
    }

    public enum enSignalRParmType
    {
        PairName = 1,
        Base = 2,
        AccessToken = 3,
        UserInfo = 4
    }
    public enum enReturnMethod
    {
        //user specific
        RecieveActiveOrder = 1,
        RecieveTradeHistory = 2,
        RecieveRecentOrder = 3,
        //RecieveBuyerSideWalletBal = 4,
        RecieveWalletBal = 4,
        RecieveSellerSideWalletBal = 5,
        RecieveNotification = 14,

        //pair wise
        RecieveBuyerBook = 6,
        RecieveSellerBook = 7,
        RecieveOrderHistory = 8,
        RecieveMarketData = 9,
        RecieveChartData = 10,
        RecieveLastPrice = 11,

        //Base Market
        RecievePairData = 12,
        RecieveMarketTicker = 13,
        BroadcastMessage = 16,
        SetTime = 15,
        RecieveBlockUnblockUser = 20,
        RecieveBlockUserUpdate = 21,
        RecieveChatHistory = 22,
        GetOnlineUserCount = 23,

        RecieveOpenOrder = 24,
        RecieveStopLimitBuyerBook = 25,
        RecieveStopLimitSellerBook = 26,
        RecieveWalletActivity = 27,
        ReceiveSessionExpired = 28,
        ReceiveBulkBuyerBook = 29,
        ReceiveBulkSellerBook = 30,
        ReceiveEnvironmentMode = 31,
        ReceiveBulkOrderHistory = 32,

        //Arbitrage Method
        RecieveActiveOrderArbitrage = 41,
        RecieveTradeHistoryArbitrage = 42,
        RecieveRecentOrderArbitrage = 43,
        RecieveNotificationArbitrage = 44,
        RecieveBuyerBookArbitrage = 45,
        RecieveSellerBookArbitrage = 46,
        RecieveOrderHistoryArbitrage = 47,
        RecieveMarketDataArbitrage = 48,
        RecieveLastPriceArbitrage = 49,
        RecieveChartDataArbitrage = 50,
        RecievePairDataArbitrage = 51,
        RecieveMarketTickerArbitrage = 52,
        RecieveStopLimitBuyerBookArbitrage = 53,
        RecieveStopLimitSellerBookArbitrage = 54,
        RecieveWalletActivityArbitrage = 55,
        RecieveWalletBalArbitrage = 56,
        RecieveProviderMarketDataArbitrage = 57,
        RecieveProfitIndicatorArbitrage = 58,
        RecieveExchangeListSmartArbitrage = 59,
        RecieveActiveOrderRemove = 60,

    }
    public enum enCheckWithdrawalBene
    {
        Success = 1,
        WalletNotFound = 2,
        GlobalBitNotFound = 3,
        BeneficiaryNotFound = 4,
        AddressNotFoundOrWhitelistingBitIsOff = 5,
        AddressNotMatch = 6
    }
    //vsolanki 2018-10-29
    public enum enUserType
    {
        Admin = 0, // super admin 
        User = 1,
        OrganizationAdmin = 2
    }

    public enum enWhiteListingBit
    {
        OFF = 0,
        ON = 1
    }

    public enum EnuserChangeLog
    {
        UserProfile = 1,
        TwofactoreChange = 2,
        ChangePassword = 3,
        SetPassword = 4
    }

    public enum enTradeReconActionType
    {
        //Refund = 1,
        SuccessAndDebit = 2,
        SuccessMark = 3,
        InProccessMark = 4,
        FailMark = 5,
        //PartialRefund = 7,
        CancelMark = 8,
        ForceCancel = 9,
        InactiveMark = 10,
        ActiveMark = 11,
        ReleaseStuckOrder = 12,
        ReInitMark = 13
    }

    public enum enSP_TradeSettlementActionType
    {
        Buy = 11,
        Sell = 13
    }

    public enum enChargeType
    {
        Fixed = 1,
        Pecentage = 2
    }

    public enum EnDeviceSubsscrptionType
    {
        Subsscribe = 1,
        UnSubsscribe = 2
    }
    public enum EnNotificationType
    {
        Success = 1,
        Fail = 2,
        Info = 3
    }
    public enum EnTemplateType
    {
        Registration = 1,
        TransactionSuccess = 2,
        ConfirmationMail = 3,
        ForgotPassword = 4,
        LoginWithOTP = 5,
        ResetPassword = 6,
        SMS_VerificationCode = 7,
        SMS_WelcomeToExchange = 8,
        SMS_PasswordChnages = 9,
        SMS_WalletCreate = 10,
        SMS_ConvertFund = 11,
        SMS_DefaultWalletCreate = 12,
        SMS_WalletLimitCreated = 13,
        SMS_WalletAddressCreated = 14,
        SMS_WalletBeneficiaryAdded = 15,
        SMS_BeneficiaryUpdated = 16,
        SMS_WhitelistingOnOff = 17,
        SMS_WalletCredited = 18,
        SMS_WalletDebited = 19,
        SMS_TransactionCreated = 20,
        SMS_TransactionSettled = 21,
        SMS_TransactionPartiallySettled = 22,
        SMS_OrderCancel = 23,
        SMS_TransactionFailed = 24,
        SMS_WithdrwalTransactionSuccess = 25,
        SuccessfulLogin = 26,
        SMS_WithdrwalTransactionFailed = 27,
        SMS_WithdrwalTransactionCreate = 28,
        SMS_PartialOrderCancel = 29,
        SMS_OrderCancelFailed = 30,
        EMAIL_WalletCreate = 31,
        EMAIL_WithdrwalTransactionCreate = 32,
        AuthorizedNewDevice = 33,
        EMAIL_WithdrwalTransactionSuccess = 34,
        EMAIL_WithdrwalTransactionFailed = 35,
        EMAIL_ConvertFund = 36,
        EMAIL_DefaultWalletCreate = 37,
        EMAIL_WalletLimitCreated = 38,
        EMAIL_WalletAddressCreated = 39,
        EMAIL_WalletBeneficiaryAdded = 40,
        EMAIL_BeneficiaryUpdated = 41,
        EMAIL_WhitelistingOnOff = 42,
        EMAIL_WalletCredited = 43,
        EMAIL_WalletDebited = 44,
        EMAIL_OrderCancel = 45,
        EMAIL_PartialOrderCancel = 46,
        EMAIL_OrderCancelFailed = 47,
        EMAIL_TransactionFailed = 48,
        EMAIL_TransactionPartialSuccess = 49,
        EmailRegistration = 50,
        SignupEmailWithOTP = 51,
        Email_Reciever = 54,
        Email_RequestApproval = 52,
        //  Email_InvitationRejected = 53,
        Email_Sender = 55,
        Email_AddUserOwnerApproval = 56,
        Email_OwnerApproval = 57,
        Email_Market_TransactionSuccess = 58,     // Uday 04-01-2019 For Market Order Price is 0 so new not display price, so new template added for full and partial success
        Email_MArket_TransactionPartialSuccess = 59,
        LoginPassword = 106,
        Email_ExportAddress = 60,
        EMAIL_WithdrwalTransactionConfirmation = 61,
        EMAIL_WithdrwalTransactionStatus = 62,
        EMAIL_AffiliatePromotion = 63,
        SMS_AffiliatePromotion = 64,
        EMAIL_Leverage_Req_AdminAccept = 65,
        EMAIL_Leverage_Req_AdminReject = 66,
        EMAIL_Leverage_Req_DirectAccept = 67,
        EMAIL_Leverage_Req_Pending = 68,
        EMAIL_ChrgesApply = 69,
        EMAIL_ChrgesApplyrefund = 70,
        SMS_Referral = 71,
        Email_Referral = 72,
        EMAIL_SendInvitation = 73,
        EMAIL_IEOTransaction = 74,
        EMAIL_IEOTransactionCron = 75,
        EMAIL_FiatBuyRequest = 77,
        EMAIL_FiatSellRequest = 78,
        EMAIL_FiatBuyRequestSuccess = 79,
        EMAIL_FiatSellRequestSuccess = 80,
        EMAIL_BankRequestAccepted = 81,
        EMAIL_BankRequestRejected = 82,
        AuthorizeNewIPAddress = 83
    }

    public enum EnCurrencyType
    {
        Crypto = 1,
        Fiat = 2
    }

    public enum EnAllowedChannels
    {
        Web = 21,
        App = 31,
        LocalTest = 11,
        TrnSystemInternal = 41,
        WalletSystemInternal = 51
    }

    public enum EnTradingChargeType
    {
        BaseCurrency = 1,
        HoldCurrency = 2,
        //  ChargeFreeCurrency = 3
    }

    public enum enWalletDeductionType
    {
        Normal = 0,
        Limit = 1,
        Market = 2,
        Trading = 3,
        Margin = 4,
        Safety = 5,
        InternalSquareOffOrder = 6
    }

    public enum EnStakeUnStake
    {
        Staking = 1,
        Unstaking = 2
    }
    public enum EnStakingType
    {
        FixedDeposit = 1,
        Charge = 2//DiscountOnCharge
    }

    public enum EnStakingSlabType
    {
        Fixed = 1,
        Range = 2
    }

    public enum EnInterestType
    {
        Fixed = 1,
        Percentage = 2
    }

    public enum EnUnstakeType
    {
        Full = 1,
        Partial = 2
    }

    public enum EnChargeType
    {
        Default = 1
    }

    public enum EnCommissionType
    {
        Default = 1
    }

    public enum enBalanceType
    {
        AvailableBalance = 1,
        OutBoundBalance = 2,
        InBoundBalance = 3
    }

    public enum EnWalletStatus
    {
        Disable = 0,
        Active = 1,
        Inactive = 2,
        Freeze = 3,
        Inoperative = 4,
        Suspended = 5,
        Blocked = 6,
        Deleted = 9
    }

    public enum enWithdrawalReconActionType
    {
        Refund = 1,
        SuccessAndDebit = 2,
        Success = 4,
        FailedMark = 5
    }
    public enum enComplainStatusType
    {
        Open = 1,
        Close = 2,
        Pending = 3
    }
    public enum EnEmailType
    {
        Template = 0,
        Formatted = 1
    }
    public enum EnWeekDays
    {
        Monday = 1,
        Tuesday = 2,
        Wednesday = 3,
        Thursday = 4,
        Friday = 5,
        Saturday = 6,
        Sunday = 7
    }
    public enum EnUserActivityType
    {
        GenerateAddress = 1,
        SetUserPreference = 2,
        AddBeneficiary = 3,
        SetWalletLimit = 4,
        CreateWallet = 5
    }

    public enum EnIsKYCEnable
    {
        No = 0,
        Yes = 1
    }

    #region "Social Profile"
    public enum EnProfileConfigKey
    {
        Profile_Visiblity_Leader,
        Can_Have_Followers_Leader,
        Can_Follow_Leaders_Lead,
        Can_Copy_Trade_Leader,
        Can_Mirror_Trade_Leader,
        Minimum_Trade_Volume_Leader,
        Profile_Visiblity_Follower,
        Can_Have_Followers_Follow,
        Can_Follow_Leaders_Follower,
        Can_Copy_Trade_Folower,
        Can_Mirror_Trade_Follower,
        Minimum_Trade_Volume_Follower,

        Default_Visibility_of_Profile,
        Min_Number_of_Followers_can_Follow,
        Max_Number_Followers_can_Follow,


        Min_Balance_Require_in_Follower_Account_to_Follow,
        Min_Trade_Volume_Requir,
        Min_Trade_Volume_Requir_in_Time,
        Subscription_Charge_Leader,
        Subscription_Charge_Frequency_Leader,
        Can_Add_Pair_to_Watchlist_Leader,
        Max_Number_of_Pairs_to_Allow_in_Watchlist_Leader,
        Can_Copy_Trade,
        Can_Mirror_Trade,
        Enable_Auto_Copy_Trade_Functionality,
        Minimum_Copy_Trade_Percentage,
        Default_Copy_Trade_Percentage,
        Maximum_Copy_Trade_Percentage,
        Maximum_Number_of_Leaders_to_Allow_Follow,
        Maximum_Transaction_Amount_Limit,
        Maximum_Number_of_Transactions_Limit,
        Subscription_Charge_Follower,
        Subscription_Charge_Frequency_Follower,
        Can_Add_Leader_to_Watchlist_Follower,
        Max_Number_of_Leader_to_Allow_in_Watchlist_Follower

    }

    public enum EnVisibleProfile
    {
        Public = 1,
        Private = 2
    }

    public enum EnYesNo
    {
        Yes = 1,
        No = 2
    }

    public enum ProfileSocialCongifType
    {
        Leader_Profile,
        Follower_Profile,
        Leader_Admin_Policy,
        Follower_Admin_Policy,
        Leader,
        Follower,
    }

    public enum EnmGroupName
    {
        MyGroup,
    }

    #endregion

    public enum EnTopLossGainerFilterType
    {
        VolumeWise = 1,
        ChangePerWise = 2,
        LTPWise = 3,
        ChangeValueWise = 4
    }

    public enum KYCStatus
    {
        Approval = 1,
        Reject = 2,
        Pending = 4
    }
    public enum SignupReportfiltration
    {
        Today = 0,
        Weekly = -7,
        Monthly = -30
    }

    public enum EnWithdrwalConfirmationStatus
    {
        ConfirmationPending = 0,
        Confirm = 1,
        Cancelled = 9
    }

    public enum EnWithdrwalInternalTransaction
    {
        InternalTransaction = 1,
        OutSideTransaction = 2
    }

    public enum EnWalletUsageType
    {
        Trading_Wallet = 0,
        Market_Wallet = 1,
        Cold_Wallet = 2,
        Charge_Cr_Wallet_ORG = 3,
        Deposition_Admin_Wallet = 4,
        Margin_Trading_Wallet = 5,
        Margin_Safety_Wallet = 6,
        Margin_Profit_Wallet = 7,
        //Margin_Charge_Cr_Wallet_ORG = 8, not in use ntrivedi 18-04-2019
        Commission_Org_Wallet = 9,
        OrganizationStakingWallet = 10, //ntrivedi 23-07-2019 added for staking
        IEOWallet = 11
    }

    public enum enLiquidityProvider
    {
        Local = 0,
        Binance = 1,
        Bittrex = 2,
        TradeSatoshi = 3,
        Poloniex = 4,
        Coinbase = 5,
        Huobi = 6,
        Upbit = 7,
        OKEx = 8,
        Kraken = 9,
        Bitfinex = 10,
        Gemini = 11,
        CEXIO = 12,
        EXMO = 13,
        Yobit = 25
    }

    public enum enMarginLoanStatus
    {
        Success = 1,
        Initialize = 0,
        Rejected = 9,//by admin
        Failed = 3,
        Refunded = 5, //loan closed
        AdminApprovalPending = 6
    }
    public enum enSiteTokenRateType//Rita 8-2-18 added for Site token conversion
    {
        API = 1,
        Market = 2,
        UserSpecific = 3
    }

    public enum enMethodLimitType
    {
        None = 0,
        PerSec = 1,
        PerMin = 2,
        PerHr = 3,
        PerDay = 4,
        PerWeek = 5,
        PerMonth = 6,
        PerYear = 7,
        PerUser = 8,
        AllTime = 10
    }

    public enum enLeverageChargeDeductionType
    {
        Monthly = 0,
        Daily = 1
    }

    public enum enVelocityRuleStatus
    {
        Active = 1,
        InActive = 0
    }

    public enum enAPIIPAccess
    {
        Restrict = 1,
        UnRestrict = 0

    }
    public enum enFrequencyType //PlanValidityType
    {
        Day = 1,
        Month = 2,
        Year = 3,
        week = 4
    }
    public enum enIPType
    {
        Whitelist = 1,
        Concurrent = 2
    }

    public enum MarginChargeCase
    {
        SufficientLeverage = 1,
        InsufficinetLeverage_PlaceOrder = 2,
        InsufficientLeverage_ButLessLeverageMax = 3,
        NewOrderForHoldBuyOrder = 4 // existing order amount tranx change 
    }
    public enum enAddressType
    {
        DepositionAddress = 0,
        AdminAddress = 1,
    }

    public enum enCronMaster
    {
        ReleaseOrderHandler = 1,
        StuckOrderHandler = 2,
        MarginStuckOrderHandler = 3,
        CryptoWatcher = 4,
        CryptoWatcherArbitrage = 5,
        LPstatusCheck = 6,
        LPStatusCheckArbitrage = 7,
        NormalTradingFullSettlementReleaseProfitAmount = 8, //ntrivedi 15-07-2019
        ReleaseArbitrageLPOrder = 9,
        MarketCap = 10,
        TradingProfit = 11,
        RecurringCharge = 12,
        ReferralCommission = 13,
        Staking = 14,
        ForceWithdrwLoan = 15,
        MarginTradingFullSettlementReleaseProfitAmount = 16, //ntrivedi 17-07-2019
        LPHoldLPFailed = 15,
        CCXTTicker = 17,
        CCXTStatusCheck = 18,
        StopLossArbitrage = 19,
        ArbitrageTradingProfit = 20,
        IEOBGProcess = 21,
        FiatSellWithdraw = 22,
        FiatLTPChangeBinnance = 23,
        FiatPendingHashUpdate=24
    }

    public enum enTradingType
    {
        Regular = 1,
        Margin = 2,
        Liquidity = 3,
        MarketMaking = 4,
        Arbitrage = 5,
        MaxProfit = 6,
        ArbitrageTrading = 7
    }

    public enum LPOrderType
    {
        LPHoldProvider = 1,
        LPHoldUser = 2
    }
    //test commit

    public enum enLoginType
    {
        WithPassword = 101,
        WithMailOTP = 102,
        WithMobileOTP = 103
    }

    public enum enArbitrageTradingAllowToUser
    {
        None = 0,
        BuyFirst = 1,
        SellFirst = 3
    }

    /// <summary>
    /// MarketMaker user buy and sell range selection preference
    /// </summary>
    /// <remarks>-Sahil 25-09-2019</remarks>
    /// <remarks>migrate from market maker project for put preference and price validation -Sahil 12-10-2019 12:10 PM</remarks>
    public enum RangeType
    {
        Percentage = 1,
        Fix = 2
    }

    //add by akshay kothari 14/02/2020 9:47 am UrlType using Create Text File 
    public enum UrlType
    {
        DepositeStatusCheck=1,
        WithdrawApp=2,
        WalletDeposite=3
    }
}