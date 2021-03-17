using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AspNet.Security.OpenIdConnect.Primitives;
using Worldex.Core.Entities;
using Worldex.Core.Entities.Culture;
using Worldex.Core.Entities.Resource;
using Worldex.Core.Entities.User;
using Worldex.Core.Entities.Wallet;
using Worldex.Core.Interfaces;
using Worldex.Core.Interfaces.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OpenIddict.Core;
using OpenIddict.Models;

namespace Worldex.Infrastructure
{
    public interface IDatabaseInitializer
    {
        Task SeedAsync(IConfiguration configuration);
    }

    public class DatabaseInitializer : IDatabaseInitializer
    {
        private readonly WorldexContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly IRegisterTypeService _registerTypeService;
        private readonly OpenIddictApplicationManager<OpenIddictApplication> _openIddictApplicationManager;
        private readonly ILogger _logger;
        private readonly ITrnMasterConfiguration _trnMasterConfiguration;

        public DatabaseInitializer(
            WorldexContext context,
            ILogger<DatabaseInitializer> logger,
            OpenIddictApplicationManager<OpenIddictApplication> openIddictApplicationManager,
            RoleManager<ApplicationRole> roleManager,
            UserManager<ApplicationUser> userManager,
            IRegisterTypeService registerTypeService,
            ITrnMasterConfiguration trnMasterConfiguration
            )
        {
            _context = context;
            _logger = logger;
            _openIddictApplicationManager = openIddictApplicationManager;
            _roleManager = roleManager;
            _userManager = userManager;
            _registerTypeService = registerTypeService;
            _trnMasterConfiguration = trnMasterConfiguration;
        }

        public async Task SeedAsync(IConfiguration configuration)
        {
            await _context.Database.MigrateAsync().ConfigureAwait(false);

            CreateRoles();
            CreateUsers();
            AddLocalisedData();
            await AddOpenIdConnectOptions(configuration);
            AddWalletType(); // ntrivedi 01-10-2018 added default wallettype master entry
            CreateRegisterType(); // Birju 02-10-2018  added by default Register type add;
            AddAppType();  //komal 10-10-2018 default Apptype entry
            AddProviderType();  //komal 10-10-2018 default ServiceProviderType entry        }
            AddTrnType();
            AddCurrencyType();
            AddCommissionTypeMaster();
            AddAllowedChannels();
            AddChargeTypeMaster();
            AddUserTypeMaster();
            //AddWTrnTpeMaster();
            AddWalletUsageType();
        }
        private void CreateRegisterType()
        {
            var TypeToAdd = new List<RegisterType>(){
                new RegisterType { CreatedDate = DateTime.Now, CreatedBy= 1,Type="Mobile",ActiveStatus=true,IsDeleted = false},
                new RegisterType { CreatedDate = DateTime.Now, CreatedBy= 1,Type="Email",ActiveStatus=true,IsDeleted = false},
                new RegisterType { CreatedDate = DateTime.Now, CreatedBy= 1,Type="Standerd",ActiveStatus=true,IsDeleted = false},
            };
            foreach (var Type in TypeToAdd)
            {
                if (!_registerTypeService.GetRegisterType(Type.Type).Result)
                {
                    _registerTypeService.AddRegisterType(Type);
                }
            }
        }

        private void CreateRoles()
        {
            var rolesToAdd = new List<ApplicationRole>(){
                new ApplicationRole { Name= "Admin", Description = "Full rights role"},
                new ApplicationRole { Name= "User", Description = "Limited rights role"}
            };
            foreach (var role in rolesToAdd)
            {
                if (!_roleManager.RoleExistsAsync(role.Name).Result)
                {
                    _roleManager.CreateAsync(role).Result.ToString();
                }
            }
        }

        private void CreateUsers()
        {
            if (!_context.Users.Any())
            {
                var adminUser = new ApplicationUser { UserName = "admin@admin.com", FirstName = "Admin first", LastName = "Admin last", Email = "admin@admin.com", Mobile = "0123456789", EmailConfirmed = true, CreatedDate = DateTime.Now, IsEnabled = true };
                _userManager.CreateAsync(adminUser, "P@ssw0rd!").Result.ToString();
                _userManager.AddClaimAsync(adminUser, new Claim(OpenIdConnectConstants.Claims.PhoneNumber, adminUser.Mobile.ToString(), ClaimValueTypes.Integer)).Result.ToString();
                _userManager.AddToRoleAsync(_userManager.FindByNameAsync("admin@admin.com").GetAwaiter().GetResult(), "Admin").Result.ToString();

                var normalUser = new ApplicationUser { UserName = "user@user.com", FirstName = "First", LastName = "Last", Email = "user@user.com", Mobile = "0123456789", EmailConfirmed = true, CreatedDate = DateTime.Now, IsEnabled = true };
                _userManager.CreateAsync(normalUser, "P@ssw0rd!").Result.ToString();
                _userManager.AddClaimAsync(adminUser, new Claim(OpenIdConnectConstants.Claims.PhoneNumber, adminUser.Mobile.ToString(), ClaimValueTypes.Integer)).Result.ToString();
                _userManager.AddToRoleAsync(_userManager.FindByNameAsync("user@user.com").GetAwaiter().GetResult(), "User").Result.ToString();
            }
        }

        private void AddLocalisedData()
        {
            if (!_context.Cultures.Any())
            {
                _context.Cultures.AddRange(
                    new Cultures
                    {
                        Name = "en-US",
                        Resources = new List<Resources>() {
                            new Resources { Key = "app_title", Value = "AspNetCoreSpa" },
                            new Resources { Key = "app_description", Value = "Single page application using aspnet core and angular" },
                            new Resources { Key = "app_repo_url", Value = "https://github.com/asadsahi/aspnetcorespa" },
                            new Resources { Key = "app_nav_home", Value = "Home" },
                            new Resources { Key = "app_nav_signalr", Value = "SignalR" },
                            new Resources { Key = "app_nav_examples", Value = "Examples" },
                            new Resources { Key = "app_nav_register", Value = "Register" },
                            new Resources { Key = "app_nav_login", Value = "Login" },
                            new Resources { Key = "app_nav_logout", Value = "Logout" },
                        }
                    },
                    new Cultures
                    {
                        Name = "fr-FR",
                        Resources = new List<Resources>() {
                            new Resources { Key = "app_title", Value = "AspNetCoreSpa" },
                            new Resources { Key = "app_description", Value = "Application d'une seule page utilisant aspnet core et angular" },
                            new Resources { Key = "app_repo_url", Value = "https://github.com/asadsahi/aspnetcorespa" },
                            new Resources { Key = "app_nav_home", Value = "Accueil" },
                            new Resources { Key = "app_nav_signalr", Value = "SignalR" },
                            new Resources { Key = "app_nav_examples", Value = "Exemples" },
                            new Resources { Key = "app_nav_register", Value = "registre" },
                            new Resources { Key = "app_nav_login", Value = "S'identifier" },
                            new Resources { Key = "app_nav_logout", Value = "Connectez - Out" },
                        }
                    }
                    );

                _context.SaveChanges();
            }

        }

        private async Task AddOpenIdConnectOptions(IConfiguration configuration)
        {
            if (await _openIddictApplicationManager.FindByClientIdAsync("Worldex") == null)
            {
                var host = configuration["HostUrl"].ToString();

                var descriptor = new OpenIddictApplicationDescriptor
                {
                    ClientId = "Worldex",
                    DisplayName = "Worldex",
                    PostLogoutRedirectUris = { new Uri($"{host}signout-oidc") },
                    RedirectUris = { new Uri(host) },
                    Permissions =
                    {
                        OpenIddictConstants.Permissions.Endpoints.Authorization,
                        OpenIddictConstants.Permissions.Endpoints.Token,
                        OpenIddictConstants.Permissions.GrantTypes.Implicit,
                        OpenIddictConstants.Permissions.GrantTypes.Password,
                        OpenIddictConstants.Permissions.GrantTypes.RefreshToken
                    }
                };

                await _openIddictApplicationManager.CreateAsync(descriptor);
            }
            
            if (await _openIddictApplicationManager.FindByClientIdAsync("4BgFgJBxJPQxNeZHbjWmo5DP1j") == null)
            {
                var descriptor = new OpenIddictApplicationDescriptor
                {
                    ClientId = "4BgFgJBxJPQxNeZHbjWmo5DP1j",
                    ClientSecret = "4289da868f769987dca0c16cc8c200881f8c5403a0dae042c703e2bf490ab632",
                    DisplayName = "My client application",
                    Permissions =
                        {
                            OpenIddictConstants.Permissions.Endpoints.Token,
                            OpenIddictConstants.Permissions.GrantTypes.ClientCredentials
                        }
                };

                await _openIddictApplicationManager.CreateAsync(descriptor);
            }

        }

        private void AddWalletType()
        {
            if (!_context.WalletTypeMasters.Any())
            {
                _context.WalletTypeMasters.AddRange(
                  new WalletTypeMaster { CreatedBy = 900, CreatedDate = DateTime.UtcNow, Description = "BTC", Status = 1, WalletTypeName = "BTC", IsDepositionAllow = 1, IsWithdrawalAllow = 1, IsTransactionWallet = 1 },
                  new WalletTypeMaster { CreatedBy = 900, CreatedDate = DateTime.UtcNow, Description = "INR", Status = 1, WalletTypeName = "INR", IsDepositionAllow = 1, IsWithdrawalAllow = 1, IsTransactionWallet = 1 },
                  new WalletTypeMaster { CreatedBy = 900, CreatedDate = DateTime.UtcNow, Description = "LTC", Status = 1, WalletTypeName = "LTC", IsDepositionAllow = 1, IsWithdrawalAllow = 1, IsTransactionWallet = 1 },
                  new WalletTypeMaster { CreatedBy = 900, CreatedDate = DateTime.UtcNow, Description = "ETH", Status = 1, WalletTypeName = "ETH", IsDepositionAllow = 1, IsWithdrawalAllow = 1, IsTransactionWallet = 1 },
                  new WalletTypeMaster { CreatedBy = 900, CreatedDate = DateTime.UtcNow, Description = "XRP", Status = 1, WalletTypeName = "XRP", IsDepositionAllow = 1, IsWithdrawalAllow = 1, IsTransactionWallet = 1 },
                  new WalletTypeMaster { CreatedBy = 900, CreatedDate = DateTime.UtcNow, Description = "ATCC", Status = 1, WalletTypeName = "ATCC", IsDepositionAllow = 1, IsWithdrawalAllow = 1, IsTransactionWallet = 1 },
                  new WalletTypeMaster { CreatedBy = 900, CreatedDate = DateTime.UtcNow, Description = "ARISTO", Status = 1, WalletTypeName = "ARISTO", IsDepositionAllow = 1, IsWithdrawalAllow = 1, IsTransactionWallet = 1 },
                  new WalletTypeMaster { CreatedBy = 900, CreatedDate = DateTime.UtcNow, Description = "BCI", Status = 1, WalletTypeName = "BCI", IsDepositionAllow = 1, IsWithdrawalAllow = 1, IsTransactionWallet = 1 },
                  new WalletTypeMaster { CreatedBy = 900, CreatedDate = DateTime.UtcNow, Description = "BCH", Status = 1, WalletTypeName = "BCH", IsDepositionAllow = 1, IsWithdrawalAllow = 1, IsTransactionWallet = 1 },
                  new WalletTypeMaster { CreatedBy = 900, CreatedDate = DateTime.UtcNow, Description = "BTG", Status = 1, WalletTypeName = "BTG", IsDepositionAllow = 1, IsWithdrawalAllow = 1, IsTransactionWallet = 1 },
                  new WalletTypeMaster { CreatedBy = 900, CreatedDate = DateTime.UtcNow, Description = "CVC", Status = 1, WalletTypeName = "CVC", IsDepositionAllow = 1, IsWithdrawalAllow = 1, IsTransactionWallet = 1 },
                  new WalletTypeMaster { CreatedBy = 900, CreatedDate = DateTime.UtcNow, Description = "REP", Status = 1, WalletTypeName = "REP", IsDepositionAllow = 1, IsWithdrawalAllow = 1, IsTransactionWallet = 1 },
                  new WalletTypeMaster { CreatedBy = 900, CreatedDate = DateTime.UtcNow, Description = "OMG", Status = 1, WalletTypeName = "OMG", IsDepositionAllow = 1, IsWithdrawalAllow = 1, IsTransactionWallet = 1 },
                  new WalletTypeMaster { CreatedBy = 900, CreatedDate = DateTime.UtcNow, Description = "PAY", Status = 1, WalletTypeName = "PAY", IsDepositionAllow = 1, IsWithdrawalAllow = 1, IsTransactionWallet = 1 },
                  new WalletTypeMaster { CreatedBy = 900, CreatedDate = DateTime.UtcNow, Description = "GNT", Status = 1, WalletTypeName = "GNT", IsDepositionAllow = 1, IsWithdrawalAllow = 1, IsTransactionWallet = 1 },
                  new WalletTypeMaster { CreatedBy = 900, CreatedDate = DateTime.UtcNow, Description = "BAT", Status = 1, WalletTypeName = "BAT", IsDepositionAllow = 1, IsWithdrawalAllow = 1, IsTransactionWallet = 1 },
                  new WalletTypeMaster { CreatedBy = 900, CreatedDate = DateTime.UtcNow, Description = "KNC", Status = 1, WalletTypeName = "KNC", IsDepositionAllow = 1, IsWithdrawalAllow = 1, IsTransactionWallet = 1 },
                  new WalletTypeMaster { CreatedBy = 900, CreatedDate = DateTime.UtcNow, Description = "ZRX", Status = 1, WalletTypeName = "ZRX", IsDepositionAllow = 1, IsWithdrawalAllow = 1, IsTransactionWallet = 1 },
                  new WalletTypeMaster { CreatedBy = 900, CreatedDate = DateTime.UtcNow, Description = "FUN", Status = 1, WalletTypeName = "FUN", IsDepositionAllow = 1, IsWithdrawalAllow = 1, IsTransactionWallet = 1 },
                  new WalletTypeMaster { CreatedBy = 900, CreatedDate = DateTime.UtcNow, Description = "EOS", Status = 1, WalletTypeName = "EOS", IsDepositionAllow = 1, IsWithdrawalAllow = 1, IsTransactionWallet = 1 },
                  new WalletTypeMaster { CreatedBy = 900, CreatedDate = DateTime.UtcNow, Description = "QRL", Status = 1, WalletTypeName = "QRL", IsDepositionAllow = 1, IsWithdrawalAllow = 1, IsTransactionWallet = 1 },
                  new WalletTypeMaster { CreatedBy = 900, CreatedDate = DateTime.UtcNow, Description = "NMR", Status = 1, WalletTypeName = "NMR", IsDepositionAllow = 1, IsWithdrawalAllow = 1, IsTransactionWallet = 1 },
                  new WalletTypeMaster { CreatedBy = 900, CreatedDate = DateTime.UtcNow, Description = "LMX", Status = 1, WalletTypeName = "LMX", IsDepositionAllow = 1, IsWithdrawalAllow = 1, IsTransactionWallet = 1 }

               );
                _context.SaveChanges();
            }
        }

        private void AddAppType()
        {
            if (!_context.AppType.Any())
            {
                _context.AppType.AddRange(
                    new Core.Entities.Configuration.AppType { CreatedBy = 900, CreatedDate = DateTime.Now, AppTypeName = "WebSocket", Status = 1, UpdatedDate = DateTime.Now },
                    new Core.Entities.Configuration.AppType { CreatedBy = 900, CreatedDate = DateTime.Now, AppTypeName = "JsonRPC", Status = 1, UpdatedDate = DateTime.Now },
                    new Core.Entities.Configuration.AppType { CreatedBy = 900, CreatedDate = DateTime.Now, AppTypeName = "TCPSocket", Status = 1, UpdatedDate = DateTime.Now },
                    new Core.Entities.Configuration.AppType { CreatedBy = 900, CreatedDate = DateTime.Now, AppTypeName = "RestAPI", Status = 1, UpdatedDate = DateTime.Now },
                    new Core.Entities.Configuration.AppType { CreatedBy = 900, CreatedDate = DateTime.Now, AppTypeName = "HttpApi", Status = 1, UpdatedDate = DateTime.Now },
                    new Core.Entities.Configuration.AppType { CreatedBy = 900, CreatedDate = DateTime.Now, AppTypeName = "SocketApi", Status = 1, UpdatedDate = DateTime.Now },
                    new Core.Entities.Configuration.AppType { CreatedBy = 900, CreatedDate = DateTime.Now, AppTypeName = "BitcoinDeamon", Status = 1, UpdatedDate = DateTime.Now }

                    );
            }
        }

        private void AddProviderType()
        {
            if (!_context.ServiceProviderType.Any())
            {
                _context.ServiceProviderType.AddRange(
                    new Core.Entities.Configuration.ServiceProviderType { CreatedBy = 900, CreatedDate = DateTime.Now, ServiveProTypeName = "TransactionAPI", Status = 1, UpdatedDate = DateTime.Now },
                    new Core.Entities.Configuration.ServiceProviderType { CreatedBy = 900, CreatedDate = DateTime.Now, ServiveProTypeName = "CommunicationAPI", Status = 1, UpdatedDate = DateTime.Now },
                    new Core.Entities.Configuration.ServiceProviderType { CreatedBy = 900, CreatedDate = DateTime.Now, ServiveProTypeName = "LiquidityProvider", Status = 1, UpdatedDate = DateTime.Now },
                    new Core.Entities.Configuration.ServiceProviderType { CreatedBy = 900, CreatedDate = DateTime.Now, ServiveProTypeName = "PaymentGateway", Status = 1, UpdatedDate = DateTime.Now },
                    new Core.Entities.Configuration.ServiceProviderType { CreatedBy = 900, CreatedDate = DateTime.Now, ServiveProTypeName = "MarketData", Status = 1, UpdatedDate = DateTime.Now }
                    );
            }
        }

        private void AddTrnType()
        {
            if (!_context.TrnTypeMaster.Any())
            {
                _context.TrnTypeMaster.AddRange(
                    new Core.Entities.TrnTypeMaster { CreatedBy = 900, CreatedDate = DateTime.Now, TrnTypeId = Core.Enums.enTrnType.Transaction, TrnTypeName = "Transaction", Status = 1, UpdatedDate = DateTime.Now },
                    new Core.Entities.TrnTypeMaster { CreatedBy = 900, CreatedDate = DateTime.Now, TrnTypeId = Core.Enums.enTrnType.Buy_Trade, TrnTypeName = "Buy_Trade", Status = 1, UpdatedDate = DateTime.Now },
                    new Core.Entities.TrnTypeMaster { CreatedBy = 900, CreatedDate = DateTime.Now, TrnTypeId = Core.Enums.enTrnType.Sell_Trade, TrnTypeName = "Sell_Trade", Status = 1, UpdatedDate = DateTime.Now },
                    new Core.Entities.TrnTypeMaster { CreatedBy = 900, CreatedDate = DateTime.Now, TrnTypeId = Core.Enums.enTrnType.Withdraw, TrnTypeName = "Withdraw", Status = 1, UpdatedDate = DateTime.Now },
                    new Core.Entities.TrnTypeMaster { CreatedBy = 900, CreatedDate = DateTime.Now, TrnTypeId = Core.Enums.enTrnType.Shoping_Cart, TrnTypeName = "Shoping_Cart", Status = 1, UpdatedDate = DateTime.Now },
                    new Core.Entities.TrnTypeMaster { CreatedBy = 900, CreatedDate = DateTime.Now, TrnTypeId = Core.Enums.enTrnType.Deposit, TrnTypeName = "Deposit", Status = 1, UpdatedDate = DateTime.Now },
                    new Core.Entities.TrnTypeMaster { CreatedBy = 900, CreatedDate = DateTime.Now, TrnTypeId = Core.Enums.enTrnType.Generate_Address, TrnTypeName = "Generate_Address", Status = 1, UpdatedDate = DateTime.Now },
                     new Core.Entities.TrnTypeMaster { CreatedBy = 900, CreatedDate = DateTime.Now, TrnTypeId = Core.Enums.enTrnType.Topup, TrnTypeName = "Topup", Status = 1, UpdatedDate = DateTime.Now },
                      new Core.Entities.TrnTypeMaster { CreatedBy = 900, CreatedDate = DateTime.Now, TrnTypeId = Core.Enums.enTrnType.Charge, TrnTypeName = "Charge", Status = 1, UpdatedDate = DateTime.Now },
                       new Core.Entities.TrnTypeMaster { CreatedBy = 900, CreatedDate = DateTime.Now, TrnTypeId = Core.Enums.enTrnType.Commission, TrnTypeName = "Commission", Status = 1, UpdatedDate = DateTime.Now }
                    );
                _context.SaveChanges();
            }
        }

        private void AddCurrencyType()
        {
            if (!_context.CurrencyTypeMaster.Any())
            {
                _context.CurrencyTypeMaster.AddRange(
                new Core.Entities.NewWallet.CurrencyTypeMaster { CreatedBy = 900, CreatedDate = DateTime.Now, CurrencyTypeId = Convert.ToInt64(Core.Enums.EnCurrencyType.Crypto), CurrencyTypeName = "Crypto", Status = 1, UpdatedDate = DateTime.Now },

             new Core.Entities.NewWallet.CurrencyTypeMaster { CreatedBy = 900, CreatedDate = DateTime.Now, CurrencyTypeId = Convert.ToInt64(Core.Enums.EnCurrencyType.Fiat), CurrencyTypeName = "Fiat", Status = 1, UpdatedDate = DateTime.Now });

                _context.SaveChanges();
            }
        }

        private void AddAllowedChannels()
        {
            if (!_context.AllowedChannels.Any())
            {
                _context.AllowedChannels.AddRange(
                new Core.Entities.NewWallet.AllowedChannels { CreatedBy = 900, CreatedDate = DateTime.Now, ChannelID = Convert.ToInt64(Core.Enums.EnAllowedChannels.Web), ChannelName = "Web", Status = 1, UpdatedDate = DateTime.Now },

                 new Core.Entities.NewWallet.AllowedChannels
                 {
                     CreatedBy = 900,
                     CreatedDate = DateTime.Now,
                     ChannelID = Convert.ToInt64(Core.Enums.EnAllowedChannels.App
 ),
                     ChannelName = "App",
                     Status = 1,
                     UpdatedDate = DateTime.Now
                 });

                _context.SaveChanges();
            }
        }

        private void AddChargeTypeMaster()
        {
            if (!_context.ChargeTypeMaster.Any())
            {
               _context.ChargeTypeMaster.AddRange(
               new Core.Entities.NewWallet.ChargeTypeMaster { CreatedBy = 900, CreatedDate = DateTime.Now, Id = Convert.ToInt64(Core.Enums.EnChargeType.Default), ChargeName = "Default ", Status = 1, UpdatedDate = DateTime.Now });

                _context.SaveChanges();
            }
        }

        private void AddCommissionTypeMaster()
        {
            if (!_context.CommissionTypeMaster.Any())
            {
               _context.CommissionTypeMaster.AddRange(
               new Core.Entities.NewWallet.CommissionTypeMaster { CreatedBy = 900, CreatedDate = DateTime.Now, Id = Convert.ToInt64(Core.Enums.EnCommissionType.Default), TypeName = "Default ", Status = 1, UpdatedDate = DateTime.Now });

                _context.SaveChanges();
            }
        }

        private void AddUserTypeMaster()
        {
            if (!_context.UserTypeMaster.Any())
              {
                  _context.UserTypeMaster.AddRange(
                new Core.Entities.NewWallet.UserTypeMaster { CreatedBy = 900, CreatedDate = DateTime.Now, UserTypeId = Convert.ToInt64(Core.Enums.enUserType.Admin), UserType = "Admin", Status = 1, UpdatedDate = DateTime.Now },
                new Core.Entities.NewWallet.UserTypeMaster { CreatedBy = 900, CreatedDate = DateTime.Now, UserTypeId = Convert.ToInt64(Core.Enums.enUserType.User), UserType = "User", Status = 1, UpdatedDate = DateTime.Now },
                new Core.Entities.NewWallet.UserTypeMaster { CreatedBy = 900, CreatedDate = DateTime.Now, UserTypeId = Convert.ToInt64(Core.Enums.enUserType.OrganizationAdmin), UserType = "User", Status = 1, UpdatedDate = DateTime.Now });

                _context.SaveChanges();
            }
        }

        //private void AddWTrnTypeMaster()
        //{
        //    if (!_context.WTrnTypeMaster.Any())
        //    {
        //        _context.WTrnTypeMaster.AddRange(
        //        new Core.Entities.NewWallet.WTrnTypeMaster { CreatedBy = 900, Discription = "Initial", CreatedDate = DateTime.Now, TrnTypeId = Convert.ToInt64(Core.Enums.enWalletTrnType.Cr_Topup), TrnTypeName = "Cr_Topup", Status = 1, UpdatedDate = DateTime.Now },
        //        new Core.Entities.NewWallet.WTrnTypeMaster { CreatedBy = 900, Discription = "Initial", CreatedDate = DateTime.Now, TrnTypeId = Convert.ToInt64(Core.Enums.enWalletTrnType.cr_Deposit), TrnTypeName = "cr_Deposit", Status = 1, UpdatedDate = DateTime.Now },
        //        new Core.Entities.NewWallet.WTrnTypeMaster { CreatedBy = 900, Discription = "Initial", CreatedDate = DateTime.Now, TrnTypeId = Convert.ToInt64(Core.Enums.enWalletTrnType.Cr_Trade), TrnTypeName = "Cr_Buy_Trade", Status = 1, UpdatedDate = DateTime.Now },
        //        new Core.Entities.NewWallet.WTrnTypeMaster { CreatedBy = 900, Discription = "Initial", CreatedDate = DateTime.Now, TrnTypeId = Convert.ToInt64(Core.Enums.enWalletTrnType.Cr_Refund), TrnTypeName = "Cr_Refund", Status = 1, UpdatedDate = DateTime.Now },
        //        new Core.Entities.NewWallet.WTrnTypeMaster { CreatedBy = 900, Discription = "Initial", CreatedDate = DateTime.Now, TrnTypeId = Convert.ToInt64(Core.Enums.enWalletTrnType.Cr_Commission), TrnTypeName = "Cr_Commission", Status = 1, UpdatedDate = DateTime.Now },
        //        new Core.Entities.NewWallet.WTrnTypeMaster { CreatedBy = 900, Discription = "Initial", CreatedDate = DateTime.Now, TrnTypeId = Convert.ToInt64(Core.Enums.enWalletTrnType.Cr_Partial_Cancel), TrnTypeName = "Cr_Partial_Cancel", Status = 1, UpdatedDate = DateTime.Now },
        //        new Core.Entities.NewWallet.WTrnTypeMaster { CreatedBy = 900, Discription = "Initial", CreatedDate = DateTime.Now, TrnTypeId = Convert.ToInt64(Core.Enums.enWalletTrnType.Cr_Trans_IN), TrnTypeName = "Cr_Trans_IN", Status = 1, UpdatedDate = DateTime.Now },
        //        new Core.Entities.NewWallet.WTrnTypeMaster { CreatedBy = 900, Discription = "Initial", CreatedDate = DateTime.Now, TrnTypeId = Convert.ToInt64(Core.Enums.enWalletTrnType.Dr_Trade), TrnTypeName = "Dr_Sell_Trade", Status = 1, UpdatedDate = DateTime.Now },
        //        new Core.Entities.NewWallet.WTrnTypeMaster { CreatedBy = 900, Discription = "Initial", CreatedDate = DateTime.Now, TrnTypeId = Convert.ToInt64(Core.Enums.enWalletTrnType.Dr_Withdrawal), TrnTypeName = "Dr_Withdrawal", Status = 1, UpdatedDate = DateTime.Now },
        //        new Core.Entities.NewWallet.WTrnTypeMaster { CreatedBy = 900, Discription = "Initial", CreatedDate = DateTime.Now, TrnTypeId = Convert.ToInt64(Core.Enums.enWalletTrnType.Dr_Ecommerce), TrnTypeName = "Dr_Ecommerce", Status = 1, UpdatedDate = DateTime.Now },
        //        new Core.Entities.NewWallet.WTrnTypeMaster { CreatedBy = 900, Discription = "Initial", CreatedDate = DateTime.Now, TrnTypeId = Convert.ToInt64(Core.Enums.enWalletTrnType.Dr_Charges), TrnTypeName = "Dr_Charges", Status = 1, UpdatedDate = DateTime.Now },
        //        new Core.Entities.NewWallet.WTrnTypeMaster { CreatedBy = 900, Discription = "Initial", CreatedDate = DateTime.Now, TrnTypeId = Convert.ToInt64(Core.Enums.enWalletTrnType.Dr_Trans_OUT), TrnTypeName = "Dr_Trans_OUT ", Status = 1, UpdatedDate = DateTime.Now },
        //        new Core.Entities.NewWallet.WTrnTypeMaster { CreatedBy = 900, Discription = "Initial", CreatedDate = DateTime.Now, TrnTypeId = Convert.ToInt64(Core.Enums.enWalletTrnType.Dr_Freeze), TrnTypeName = "Dr_Freeze", Status = 1, UpdatedDate = DateTime.Now },

        //        new Core.Entities.NewWallet.WTrnTypeMaster { CreatedBy = 900, Discription = "Initial", CreatedDate = DateTime.Now, TrnTypeId = Convert.ToInt64(Core.Enums.enWalletTrnType.Cr_Bonus), TrnTypeName = "Cr_Bonus ", Status = 1, UpdatedDate = DateTime.Now },
        //        new Core.Entities.NewWallet.WTrnTypeMaster { CreatedBy = 900, Discription = "Initial", CreatedDate = DateTime.Now, TrnTypeId = Convert.ToInt64(Core.Enums.enWalletTrnType.Dr_ESCROW), TrnTypeName = "Dr_ESCROW", Status = 1, UpdatedDate = DateTime.Now },
        //        new Core.Entities.NewWallet.WTrnTypeMaster { CreatedBy = 900, Discription = "Initial", CreatedDate = DateTime.Now, TrnTypeId = Convert.ToInt64(Core.Enums.enWalletTrnType.Dr_Deposit), TrnTypeName = "Dr_Debit", Status = 1, UpdatedDate = DateTime.Now });

        //        _context.SaveChanges();
        //    }
        //}

        private void AddWalletUsageType()
        {
            if (!_context.WalletUsageType.Any())
            {
                _context.WalletUsageType.AddRange(
                    new Core.Entities.NewWallet.WalletUsageType{ CreatedBy = 900, CreatedDate = DateTime.Now, WalletUsageTypeName = "Trading Wallet", Status = 1, UpdatedDate = DateTime.Now ,Id=0},
                    new Core.Entities.NewWallet.WalletUsageType { CreatedBy = 900, CreatedDate = DateTime.Now, WalletUsageTypeName = "Market Wallet", Status = 1, UpdatedDate = DateTime.Now ,Id=1},
                    new Core.Entities.NewWallet.WalletUsageType { CreatedBy = 900, CreatedDate = DateTime.Now, WalletUsageTypeName = "Cold Wallet", Status = 1, UpdatedDate = DateTime.Now,Id=2 }
                    );
                _context.SaveChanges();
            }
        }
    }
}
