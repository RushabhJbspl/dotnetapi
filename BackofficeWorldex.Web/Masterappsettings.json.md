{
    "Data": {
        "useInMemory": "false",
        "useSqLite": "false",
        "SqlLiteConnectionString": "Data Source=Worldex.db",
        "SqlServerConnectionString": "server=<DatabaseServerName>;UID=<UserName>;pwd=<Password>;database=<DatabaseName>;"
    },
  "AzureSignalR":"false",
  "Azure": {
        "SignalR": {
            "ConnectionString": "Endpoint=<EndPointURL>;AccessKey=<AccessKey>;"
        }
    },
  "HostUrl": "<APIURL>",
  "Logging": {
    "IncludeScopes": false,
    "LogLevel": {
      "Default": "Warning"
    }
  },
  "Email": {
      "PrimaryDomain": "smtp.gmail.com",
      "PrimaryPort": "587",
      "SecondayDomain": "smtp.live.com",
      "SecondaryPort": "587",
      "UsernameEmail": "",
      "UsernamePassword": "",
      "FromEmail": "",
      "ToEmail": "toEmail",
      "CcEmail": "ccEmail"
  },
  "SMS": {
    "SMSAccountIdentification": "",
    "SMSAccountPassword": "",
    "SMSAccountFrom": ""
  },
  "AllowedHosts": "*",
  "Authentication": {
    "Google": {
      "ClientId": "ChangeMe",
      "ClientSecret": "ChangeMe"
    },
    "Facebook": {
      "AppId": "ChangeMe",
      "AppSecret": "ChangeMe"
    },
    "Microsoft": {
      "ClientId": "ChangeMe",
      "ClientSecret": "ChangeMe"
    },
    "Twitter": {
      "ConsumerKey": "ChangeMe",
      "ConsumerSecret": "ChangeMe"
    }
  },
  "redis": {
      "host": "<RedisHost>",
      "port": 6379,
      "name": "RemoteServer"
  },
  "SignalRKey": {
    "DefaultPair": "ADA_BTC",
    "DefaultBaseCurrency": "BTC",
    "RedisPairs": "CoolDexPairs:",
    "RedisMarket": "CoolDexMarkets:",
    "RedisUsers": "CoolDexUsers:",
    "RedisToken": "CoolDexTokens:",
    "RedisMarketwithoutExpr": "CoolDexMarkets",
    "RedisPairswithoutExpr": "CoolDexPairs",
    "RedisChatHistory": "CoolDexGroupChatHistory",
    "RedisOnlineUserList": "CoolDexOnlineUserList",
    "RedisBlockUserList": "CoolDexBlockUserList"
  },
  "LogPath": "LogFiles/LogFile_{Data}.txt",
  "AESSalt": "rz8LuOtFBXphj9WQfvFh",
  "MaxFailedAttempts": "3",
  "DefaultLockoutTimeSpan": "6",
  "DefaultValidateLinkTimeSpan": "2",
  "SwaggerPath": "/FrontAPI/swagger/v1/swagger.json",
  "SocialGoogle": "https://www.googleapis.com/oauth2/v1/tokeninfo?access_token=",
  "SocialFacebook": "https://graph.facebook.com/v2.8/me?fields=id,email,first_name,last_name,name,gender,locale,birthday&access_token=",
  "SocialFacebookToken": "https://graph.facebook.com/oauth/access_token?client_id=",
  "ConfirmMailURL": "<FrontURL>/register-confirm?emailConfirmCode=",
  "ResetPaswword": "<FrontURL>/forgot-confirm?emailConfirmCode=",
  "AuthorizedDeviceURL": "<FrontURL>/device-authorize?authorizecode=",
  "ConfirmEmailURL": "<FrontURL>/email-confirm?emailConfirmCode=",
  "ASOSToken": "/connect/token",
  "KYCImagePath": "Image/KYC",
  "KYCImageSize": "2097152",
  "ReportValidDays": 2,
  "DefaultValidateAuthorizedDevice": "2",
  "KYCImageGetPath": "Image/FrontAPI/",
  "FrontUrl": "<APIURL>/FrontAPI/",
  "ExportFilePath": "D:/home/site/wwwroot/DotNet/FrontAPI/ExportedFiles/",
  "DownloadFilePath": "<APIURL>/FrontAPI/ExportedFiles/",
  "ExportWalletFilePath": "C:/Users/BcAdmin102/AppData/Roaming/MultiChain/",
  "ImportedFilePath": "D:/home/site/wwwroot/DotNet/FrontAPI/ImportedFiles/",
  "WithdrawConfirmUrl": "<FrontURL>/app/withdraw-confirmation",
  "WithdrawTimeLimit": 15,
  "BaseCurrencyName": "USD",
  "MaxImportRecords": "101",
  "sitename": "CoolDex",
  "site_id": "ebca4304-3954-4209-b603-4dbc78114969",
  "Forgotverifylink": "<FrontURL>/forgot-confirm?Forgotverifylink=",
  "SiteNameForWelcomeMsg": "CoolDex",
  "ConfirmAffiliateMailURL": "<FrontURL>/affiliate-confirm?emailConfirmCode=",
  "AffiliatePromotionShare": "<FrontURL>/affiliate-signup?code=",
  "AffiliateShortifyAPIKey": "AIzaSyDNenZHL66cY1zmzlW2iWrfFeVrTYdqi9Q",
  "AffiliateShortifyBitlyAPIKey": "R_82195e9b037447478de82d24a7b2abf7",
  "AffiliateShortifyBitlyUser": "o_6iei5mtfq2",
  "AffiliateFacebookShare": "https://www.facebook.com/dialog/feed?app_id=1156417341207675&description=",
  "AffiliateGoogleShare": "https://plus.google.com/share?url=",
  "AffiliateTwitterShare": "https://twitter.com/intent/tweet?text=",
  "Hours": "24",
  "RecurringChargeHour": "19",
  "InviteMailURL": "<FrontURL>/Invite-confirm?emailConfirmCode=",
  "AdminSafeList": "",
  "QrCodeProvider": "GoogleQrCodeProvider",
  "ReferralCommissionHour" : "24"
}