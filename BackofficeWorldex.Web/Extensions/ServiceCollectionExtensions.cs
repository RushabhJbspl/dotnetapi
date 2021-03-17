using System;
using System.Collections.Generic;
using System.Globalization;
using AspNet.Security.OpenIdConnect.Primitives;
using AutoMapper.Configuration;
using Worldex.Core.Entities.User;
using Worldex.Core.Interfaces;
using Worldex.Core.Interfaces.Repository;
using Worldex.Core.Interfaces.Session;
using Worldex.Core.Interfaces.User;
using Worldex.Core.Services;
using Worldex.Infrastructure;
using Worldex.Infrastructure.Data;
using Worldex.Infrastructure.Data.Transaction;
using Worldex.Infrastructure.EFLocalizer;
using Worldex.Infrastructure.Services.Transaction;
using Worldex.Infrastructure.Services.User;
using Worldex.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Configuration;
using Worldex.Core.Interfaces.Configuration;
using Worldex.Infrastructure.Services.Configuration;
using Worldex.Core.Services.RadisDatabase;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Worldex.Infrastructure.Services.Log;
using Worldex.Core.Interfaces.Log;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using System.Threading.Tasks;
using Worldex.Core.Interfaces.UserChangeLog;
using Worldex.Infrastructure.Services.UserChangeLog;
using Worldex.Infrastructure.Interfaces;
using Worldex.Core.Interfaces.Profile_Management;
using Worldex.Infrastructure.Services.Profile_Management;
using Worldex.Core.Interfaces.Complaint;
using Worldex.Infrastructure.Services.Complaint;
using Worldex.Core.Interfaces.KYC;
using Worldex.Infrastructure.Services.KYC;
using Worldex.Infrastructure.Services.ControlPanel;
using Worldex.Core.Interfaces.EmailMaster;
using Worldex.Core.Interfaces.PhoneMaster;
using Worldex.Infrastructure.Services.UserEmailMaster;
using Worldex.Infrastructure.Services.UserWisePhonemaster;
using Worldex.Core.Interfaces.Organization;
using Worldex.Infrastructure.Services.Organization;
using Worldex.Infrastructure.Services.SecurityQuestion;
using Worldex.Core.Interfaces.KYCConfiguration;
using Worldex.Infrastructure.Services.KYCConfiguration;
using Worldex.Core.Interfaces.Activity_Log;
using Worldex.Infrastructure.Services.ActivityLog;
using Worldex.Infrastructure.Services.BackofficeReport;
using Worldex.Core.Interfaces.BackOfficeReport;
using Worldex.Core.Interfaces.BackOffice;
using Worldex.Infrastructure.Services.BackOffice;
using Worldex.Infrastructure.Services.BackOffice.ComplaintSALConfiguration;
using Worldex.Core.Interfaces.BackOffice.ComplaintSALConfiguration;
using Worldex.Core.Interfaces.Application;
using Worldex.Infrastructure.Services.Application;
using Worldex.Core.Interfaces.PasswordPolicy;
using Worldex.Infrastructure.Services.BackOffice.PasswordPolicy;
using Worldex.Core.Interfaces.BackOffice.ProfileConfiguration;
using Worldex.Core.Interfaces.SocialProfile;
using Worldex.Infrastructure.Services.SocialProfile;
using Worldex.Core.Interfaces.Affiliate;
using Worldex.Infrastructure.Services.Affiliate;
using Worldex.Infrastructure.Data.Affiliate;
using Worldex.Infrastructure.Services.Handlers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Cors.Internal;
using BackofficeWorldex.Web.Filters;
using Worldex.Infrastructure.Services.Wallet;
using Worldex.Core.Interfaces.MarginWallet;
using Worldex.Infrastructure.Data.LPWallet;

namespace BackofficeWorldex.Web.Extensions
{
    public static class ServiceCollectionExtensions
    {

        public static IServiceCollection AddPreRenderDebugging(this IServiceCollection services, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                services.AddNodeServices(options =>
                {
                    options.LaunchWithDebugging = true;
                    options.DebuggingPort = 9230;
                });
            }

            return services;
        }

        public static IServiceCollection AddCustomizedMvc(this IServiceCollection services)
        {
            services.AddMvc(options =>
            {
                options.Filters.Add(typeof(ModelValidationFilter));
                options.Filters.Add(typeof(ApiResultFilter));
            })
            .AddJsonOptions(options =>
            {
                options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
            })
            .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
            .AddDataAnnotationsLocalization()
            .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            return services;
        }

        public static IServiceCollection RegisterRedisServer(this IServiceCollection services)
        {
            services.AddDistributedRedisCache(option =>
            {
                option.Configuration = "localhost";
                //option.Configuration = ConnectionMultiplexer.Connect("server=JBSPL-0062\\SQLEXPRESS;UID=Test;pwd=admin@123;database=AspNetCoreSpa;integrated security=true;MultipleActiveResultSets=true;");
                ///option.Configuration = ConnectionMultiplexer.Connect("localhost:53232,allowAdmin=True,connectTimeout=1000,defaultDatabase=0");
                //option.Configuration = Configuration.GetConnectionString("RedisConnection");
                option.InstanceName = "master";
            });
            services.AddSession(options =>
            {
                // Set a short timeout for easy testing.
                // options.IdleTimeout = TimeSpan.FromSeconds(10);
                options.Cookie.Name = ".Worldex.Session";
                options.Cookie.HttpOnly = true;
                //options.IdleTimeout = TimeSpan.FromSeconds(1);
                //  options.Cookie.IsEssential = true;
            });
            return services;
        }

        public static IServiceCollection AddCustomIdentity(this IServiceCollection services, Microsoft.Extensions.Configuration.IConfiguration configuration)
        {
            services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
            {
                // options for user and password can be set here
                // options.Password.RequiredLength = 4;
                // options.Password.RequireNonAlphanumeric = false;


                // Start Pawword Related Setting
                //.AddTokenProvider<ConfirmEmailDataProtectorTokenProvider<ApplicationUser>>(EmailConfirmationTokenProviderName);

                // Password settings.
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireUppercase = true;
                options.Password.RequiredLength = 6;
                options.Password.RequiredUniqueChars = 1;

                // Lockout settings.
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromHours(Convert.ToInt64(configuration["DefaultLockoutTimeSpan"]));
                options.Lockout.MaxFailedAccessAttempts = Convert.ToInt16(configuration["MaxFailedAttempts"]);
                options.Lockout.AllowedForNewUsers = true;

                // User settings.
                options.User.AllowedUserNameCharacters =
                "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
                options.User.RequireUniqueEmail = false;

                // End Pawword Related Setting

            })
            .AddEntityFrameworkStores<WorldexContext>()
            .AddDefaultTokenProviders();

            return services;
        }

        public static IServiceCollection AddCustomOpenIddict(this IServiceCollection services, IHostingEnvironment env)
        {
            // Configure Identity to use the same JWT claims as OpenIddict instead
            // of the legacy WS-Federation claims it uses by default (ClaimTypes),
            // which saves you from doing the mapping in your authorization controller.
            services.Configure<IdentityOptions>(options =>
            {
                options.ClaimsIdentity.UserNameClaimType = OpenIdConnectConstants.Claims.Name;
                options.ClaimsIdentity.UserIdClaimType = OpenIdConnectConstants.Claims.Subject;
                options.ClaimsIdentity.RoleClaimType = OpenIdConnectConstants.Claims.Role;
            });

            // Register the OpenIddict services.
            services.AddOpenIddict(options =>
            {
                // Register the Entity Framework stores.
                options.AddEntityFrameworkCoreStores<WorldexContext>();

                // Register the ASP.NET Core MVC binder used by OpenIddict.
                // Note: if you don't call this method, you won't be able to
                // bind OpenIdConnectRequest or OpenIdConnectResponse parameters.
                options.AddMvcBinders();

                // Enable the token endpoint.
                // Form password flow (used in username/password login requests)
                options.EnableTokenEndpoint("/connect/token");

                // Enable the authorization endpoint.
                // Form implicit flow (used in social login redirects)
                options.EnableAuthorizationEndpoint("/connect/authorize");

                // Enable the password and the refresh token flows.
                options.AllowPasswordFlow()
                       .AllowRefreshTokenFlow()
                       .AllowImplicitFlow(); // To enable external logins to authenticate

                options.SetAccessTokenLifetime(TimeSpan.FromMinutes(15));
                options.SetIdentityTokenLifetime(TimeSpan.FromMinutes(15));
                options.SetRefreshTokenLifetime(TimeSpan.FromMinutes(60));
                // During development, you can disable the HTTPS requirement.
                if (env.IsDevelopment())
                {
                    options.DisableHttpsRequirement();
                }

                // Note: to use JWT access tokens instead of the default
                // encrypted format, the following lines are required:
                //
                // options.UseJsonWebTokens();
                options.AddEphemeralSigningKey();
                options.AllowClientCredentialsFlow();
            });

            // If you prefer using JWT, don't forget to disable the automatic
            // JWT -> WS-Federation claims mapping used by the JWT middleware:
            //
            // JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            // JwtSecurityTokenHandler.DefaultOutboundClaimTypeMap.Clear();
            //
            // services.AddAuthentication()
            //     .AddJwtBearer(options =>
            //     {
            //         options.Authority = "http://localhost:54895/";
            //         options.Audience = "resource_server";
            //         options.RequireHttpsMetadata = false;
            //         options.TokenValidationParameters = new TokenValidationParameters
            //         {
            //             NameClaimType = OpenIdConnectConstants.Claims.Subject,
            //             RoleClaimType = OpenIdConnectConstants.Claims.Role
            //         };
            //     });              

            // Alternatively, you can also use the introspection middleware.
            // Using it is recommended if your resource server is in a
            // different application/separated from the authorization server.
            //
            // services.AddAuthentication()
            //     .AddOAuthIntrospection(options =>
            //     {
            //         options.Authority = new Uri("http://localhost:54895/");
            //         options.Audiences.Add("resource_server");
            //         options.ClientId = "resource_server";
            //         options.ClientSecret = "875sqd4s5d748z78z7ds1ff8zz8814ff88ed8ea4z4zzd";
            //         options.RequireHttpsMetadata = false;
            //     });

            services.AddAuthentication(options =>
               {
                   // This will override default cookies authentication scheme
                   options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                   options.DefaultForbidScheme = JwtBearerDefaults.AuthenticationScheme;
                   options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
               })
                  .AddOAuthValidation()

               //.AddOAuth("Customprovider", "Custom provider", o =>
               //{
               //    o.ClientId = Startup.Configuration["Authentication:ClientId"];
               //    o.ClientSecret = Startup.Configuration["Authentication:ClientSecret"];
               //    o.CallbackPath = new PathString("/connect/Customprovider");
               //    //o.AuthorizationEndpoint = MicrosoftAccountDefaults.AuthorizationEndpoint;
               //    //o.TokenEndpoint = MicrosoftAccountDefaults.TokenEndpoint;
               //    //o.Scope.Add("https://graph.microsoft.com/user.read");
               //    o.SaveTokens = true;
               //    //o.Events = new OAuthEvents()
               //    //{
               //    //    OnRemoteFailure = HandleOnRemoteFailure
               //    //};
               //})



               // https://console.developers.google.com/projectselector/apis/library?pli=1
               .AddGoogle(options =>
               {
                   options.ClientId = Startup.Configuration["Authentication:Google:ClientId"];
                   options.ClientSecret = Startup.Configuration["Authentication:Google:ClientSecret"];

                   //// added by nirav savariya for created token with soical login on 10-22-2018
                   //options.Scope.Add("https://www.googleapis.com/auth/plus.login");
                   //options.ClaimActions.MapJsonKey(ClaimTypes.Gender, "gender");
                   //options.SaveTokens = true;
                   //options.Events.OnCreatingTicket = ctx =>
                   //{
                   //    List<AuthenticationToken> tokens = ctx.Properties.GetTokens()
                   //        as List<AuthenticationToken>;
                   //    tokens.Add(new AuthenticationToken()
                   //    {
                   //        Name = "TicketCreated",
                   //        Value = DateTime.UtcNow.ToString()
                   //    });
                   //    ctx.Properties.StoreTokens(tokens);
                   //    return Task.CompletedTask;
                   //};
               })
               // https://developers.facebook.com/apps
               .AddFacebook(options =>
               {
                   options.AppId = Startup.Configuration["Authentication:Facebook:AppId"];
                   options.AppSecret = Startup.Configuration["Authentication:Facebook:AppSecret"];
                   //options.ClaimActions.MapJsonKey(ClaimTypes.Gender, "gender");
                   //options.SaveTokens = true;
                   //options.Events.OnCreatingTicket = ctx =>
                   //{
                   //    List<AuthenticationToken> tokens = ctx.Properties.GetTokens()
                   //        as List<AuthenticationToken>;
                   //    tokens.Add(new AuthenticationToken()
                   //    {
                   //        Name = "TicketCreated",
                   //        Value = DateTime.UtcNow.ToString()
                   //    });
                   //    ctx.Properties.StoreTokens(tokens);
                   //    return Task.CompletedTask;
                   //};
               })
               // https://apps.twitter.com/
               .AddTwitter(options =>
               {
                   options.ConsumerKey = Startup.Configuration["Authentication:Twitter:ConsumerKey"];
                   options.ConsumerSecret = Startup.Configuration["Authentication:Twitter:ConsumerSecret"];
               })
               // https://apps.dev.microsoft.com/?mkt=en-us#/appList
               .AddMicrosoftAccount(options =>
               {
                   options.ClientId = Startup.Configuration["Authentication:Microsoft:ClientId"];
                   options.ClientSecret = Startup.Configuration["Authentication:Microsoft:ClientSecret"];
               });

            return services;
        }

        public static IServiceCollection AddCustomDbContext(this IServiceCollection services)
        {
            // Add framework services.
            services.AddDbContextPool<WorldexContext>(options =>
            {
                string useSqLite = Startup.Configuration["Data:useSqLite"];
                string useInMemory = Startup.Configuration["Data:useInMemory"];
                if (useInMemory.ToLower() == "true")
                {
                    options.UseInMemoryDatabase("ClearArchitechture"); // Takes database name
                }
                else if (useSqLite.ToLower() == "true")
                {
                    var connection = Startup.Configuration["Data:SqlLiteConnectionString"];
                    options.UseSqlite(connection);
                    options.UseSqlite(connection, b => b.MigrationsAssembly("Worldex.Web"));

                }
                else
                {
                    var connection = Startup.Configuration["Data:SqlServerConnectionString"];
                    options.UseSqlServer(connection);
                    options.UseSqlServer(connection, b => b.MigrationsAssembly("Worldex.Web"));
                }
                options.UseOpenIddict();
            });
            return services;
        }

        public static IServiceCollection AddCustomLocalization(this IServiceCollection services)
        {
            services.Configure<RequestLocalizationOptions>(opts =>
            {
                var supportedCultures = new List<CultureInfo>
                    {
                                new CultureInfo("en-US"),
                                new CultureInfo("fr-FR")
                    };

                opts.DefaultRequestCulture = new RequestCulture("en-US");
                // Formatting numbers, dates, etc.
                opts.SupportedCultures = supportedCultures;
                // UI strings that we have localized.
                opts.SupportedUICultures = supportedCultures;
            });

            services.AddLocalization(options => options.ResourcesPath = "Resources");

            return services;
        }

        public static IServiceCollection RegisterCustomServices(this IServiceCollection services)
        {
            // New instance every time, only configuration class needs so its ok
            // Commented for req-res time issue. -Nishit Jani on A 2019-01-10 5:22 PM
            //services.AddSingleton<IStringLocalizerFactory, EFStringLocalizerFactory>();

            //services.AddTransient<IEmailSender, MessageService>();
            services.AddTransient<IApplicationDataService, ApplicationDataService>();
            services.AddScoped<IUnitOfWork, HttpUnitOfWork>();
            services.AddTransient<IDatabaseInitializer, DatabaseInitializer>();
            services.AddTransient<WorldexContext>();
            services.AddTransient<UserResolveService>();
            services.AddScoped<ApiExceptionFilter>();

            // added by nirav savariya for getting user details on 9-19-2018
            services.AddScoped<IUserService, UserService>();

            // Commented for req-res time issue.  -Nishit Jani on A 2019-01-10 5:19 PM
            services.AddTransient<IUserSessionService, UserSessionService>();

            //// added by nirav savariya for getting message otp in mobile on 9-22-2018
            //services.AddTransient<IMessageSender, MessageService>();

            // added by birju for TempRegister Table on 02-10-2018
            services.AddScoped<IRegisterTypeService, RegisterTypeService>();

            // Comment code by Birju on 06 feb 2019 10:33 AM for Temp user register table remove code and servers is not used 
            //services.AddScoped<ITempUserRegisterService, TempUserRegisterService>();
            //services.AddScoped<ITempOtpService, TempOtpService>();

            services.AddScoped<IOtpMasterService, OtpMasterService>();
            //services.AddTransient<IMessageSender, MessageService>();
            // added by nirav savariya for Encypted Decrypted on 10-02-2018
            services.AddScoped<EncyptedDecrypted>();
            services.AddTransient<IFrontTrnService, FrontTrnService>();
            services.AddTransient<IFrontTrnRepository, FrontTrnRepository>();
            services.AddTransient<ITransactionConfigService, TransactionConfigService>();
            services.AddTransient<ISignalRService, SignalRService>();//komal 30-10-2018 // khushali 12-03-2019 chaneg AddScoped to AddTransient Resolve issoe of second Operation
            //services.AddTransient<ISignalRServiceV2, SignalRServiceV2>();//Uday 08-02-2018 
            //REDIS RELETED CLASS                  
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<IWalletConfiguration, WalletConfiguration>();
            //services.AddSingleton<IRedisConnectionFactory, RedisConnectionFactory>();
            services.AddSingleton<RedisConnectionFactory>();
            //Ipaddress Service
            services.AddScoped<IipAddressService, IpAddressService>();
            //DeviceId Service
            services.AddScoped<IDeviceIdService, DeviceIdService>();

            //Custom password Service
            services.AddScoped<ICustomPassword, CustomPasswordService>();
            services.AddScoped<IUserChangeLog, UserChangeLogServices>();
            services.AddScoped<IUserConfiguration, UserConfigurationService>();
            //https://stackoverflow.com/questions/38138100/what-is-the-difference-between-services-addtransient-service-addscoped-and-serv
            //services.AddTransient<ICancelOrderProcess, CancelOrderProcess>();
            services.AddTransient<ICancelOrderProcessV1, CancelOrderProcessV1>();//Rita 5-2-19 change for API cancellation process
            services.AddTransient<ICancelOrderProcessMarginV1, CancelOrderProcessMarginV1>();//Rita 21-2-19 for margin trading
            services.AddTransient<ITradeReconProcessV1, TradeReconProcessV1>();//khushali 18-03-2019 for trade Recon
            //Login History and Ip History added by nirav savariya on 02-11-2018
            services.AddScoped<IipHistory, IpHistoryService>();
            services.AddScoped<ILoginHistory, LoginHistoryService>();
            // Profile Master added by nirav savariya on 04-11-2018
            services.AddScoped<IProfileMaster, ProfileManagementService>();
            services.AddScoped<ISubscriptionMaster, SubscriptionMasterService>();
            ///   complaint Function added By pankaj kathiriya 05-11-2018
            ///   
            services.AddScoped<ITypemaster, TypemasterServices>();
            services.AddScoped<IComplainmaster, ComplainmasterServices>();

            services.AddScoped<ICompainTrail, CompainTrailServices>();
            services.AddScoped<IPersonalVerificationService, PersonalVerificationService>();

            //Uday 16-11-2018 Testing for singleton service
            services.AddTransient<ITrnMasterConfiguration, TrnMasterConfiguration>();

            //SignUp log added by nirav savariya on 05-12-2018
            services.AddScoped<ISignupLogService, SignUpLogService>();
            services.AddScoped<IActivityLog, ActivityLogService>();
            services.AddTransient<IBackOfficeCountTrnService, BackOfficeCountTrnService>();
            services.AddTransient<IBackOfficeCountTrnRepository, BackOfficeCountTrnRepository>();

            services.AddScoped<IEmailMaster, EmailMasterServices>();
            services.AddScoped<Iphonemaster, PhoneMasterServices>();
            services.AddScoped<ISecurityQuestion, SecurityQuestionServices>();

            //Backoffice admib panel added by nirav savariya on 12-20-2018
            services.AddScoped<IOrganization, OrganizationService>();

            // Social profile interafece
            services.AddScoped<IProfileConfigurationService, ProfileConfigurationService>();
            services.AddScoped<IKYCConfiguration, KYCConfigurationServices>();
            services.AddScoped<IDocumentMaster, DocumentFromatServices>();
            services.AddScoped<IKYCLevelMaster, KYCLevelMasterService>();
            services.AddScoped<IBackOfficeReport, BackOfficeReportServices>();
            //Activity log added by nirav savariya on 12-27-2018
            services.AddSingleton<IActivityMasterConfiguration, ActivityMasterConfigurationService>();
            //Activity log store in table on 12-27-2018
            //services.AddScoped<IActivityRegister, ActivityRegisterService>();

            //Activity log added by nirav savariya on 12-27-2018
            services.AddScoped<IActivityRegisterData, ActivityLogRegisterService>();

            // Added By pankaj for perform the iprange operation using this services
            services.AddScoped<IIPRange, IPRangeServices>();
            // Added By pankaj for complain priority  configuration for backoffice
            services.AddScoped<IComplaintPriorityMaster, ComplaintPriorityMasterServices>();
            // Added by nirav savariya for create application master configuration for back office on 01-07-2019
            services.AddScoped<IBackOfficeApplication, BackOfficeApplicationService>();
            ///Added by pankaj kathiriya for create the password policy Configuration  09-01-2019
            services.AddScoped<IUserPasswordPolicyMaster, UserPasswordPolicyMasterServices>();
            services.AddScoped<IUserLinkMaster, UserLinkMasterServices>();
            services.AddTransient<IWithdrawTransactionRepository, WithdrawTransactionRepository>(); //Uday 11-01-2019 Withdrwal transaction services
            services.AddScoped<IProfileConfigurationData, ProfileConfigurationServices>(); //Added by pankaj for perform the profileconfiguration services
            services.AddScoped<IGroupMasterService, GroupMasterService>(); //Added by Birju for Social Profile  Group
            services.AddTransient<IWithdrawProcessService, WithdrawProcessService>(); //Uday 30-01-2019 get withdrwalERC provider admin address  
            services.AddTransient<IAffiliateService, AffiliateService>(); //Uday 15-02-2019 Affiliate System Related method
            services.AddTransient<IAffiliateRepository, AffiliateRepository>(); //Uday 15-02-2019 Affiliate System Related method
            services.AddTransient<IAffiliateBackOfficeService, AffiliateBackOfficeService>(); //Uday 01-03-2019 Affiliate Backoffice System Related method
            services.AddTransient<AffiliateBackOfficeRepository, AffiliateBackOfficeRepository>(); //Uday 01-03-2019 Affiliate Backoffice System Related method 
            services.AddScoped<ClientIdCheckFilter>(); //Pratik 8-3-2019 Check for remote server IP and client IP 
            services.AddSingleton<IAuthorizationHandler, MustHaveAccessHandler>(); // Added by Nishit Jani on A 2019-03-30 12:40 PM to Register Authorization Policy
           // services.AddScoped<IArbitrageWalletServiceCharge, ArbitrageWalletServiceCharge>(); //Chirag 21/06/2019
            //services.AddScoped<IArbitrageWalletChargeRepository, ArbitrageWalletChargeRepository>(); //Chirag 21/06/2019
            return services;
        }

        public static IServiceCollection AddAuthorizationPolicies(this IServiceCollection services)
        {
            services.AddAuthorization(authOption =>
            {
                authOption.AddPolicy("MustHaveAuthority", policyOption =>
                {
                    policyOption.RequireAuthenticatedUser();
                    policyOption.AddRequirements(new MustHaveAccess());
                });
            });

            return services;
        }

        public static IServiceCollection AddCorsPolicyGlobal(this IServiceCollection services)
        {
            services.Configure<MvcOptions>(options =>
            {
                options.Filters.Add(new CorsAuthorizationFilterFactory("MyCorsPolicy"));
            });

            return services;
        }
    }
}
