using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Worldex.Core.Services;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NetEscapades.AspNetCore.SecurityHeaders;
using NetEscapades.AspNetCore.SecurityHeaders.Infrastructure;
using Newtonsoft.Json;
using BackofficeWorldex.Web.Filters;

namespace BackofficeWorldex.Web.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        // https://github.com/andrewlock/NetEscapades.AspNetCore.SecurityHeaders
        public static IApplicationBuilder AddCustomSecurityHeaders(this IApplicationBuilder app)
        {
            var env = app.ApplicationServices.GetRequiredService<IHostingEnvironment>();

            var policyCollection = new HeaderPolicyCollection()
                   .AddFrameOptionsDeny()
                   .AddXssProtectionBlock()
                   .AddContentTypeOptionsNoSniff()
                   .AddStrictTransportSecurityMaxAge(maxAgeInSeconds: 60 * 60 * 24 * 365) // maxage = one year in seconds
                   .AddReferrerPolicyOriginWhenCrossOrigin()
                   .RemoveServerHeader()
                   .AddContentSecurityPolicy(builder =>
                   {
                       if (env.IsProduction())
                       {
                           builder.AddUpgradeInsecureRequests(); // upgrade-insecure-requests
                       }

                       // builder.AddReportUri() // report-uri: https://report-uri.com
                       //     .To("https://report-uri.com");

                       builder.AddDefaultSrc()
                           .Self();

                       // Allow AJAX, WebSocket and EventSource connections to:
                       var socketUrl = Startup.Configuration["HostUrl"].ToString().Replace("http://", "ws://", StringComparison.OrdinalIgnoreCase).Replace("https://", "wss://", StringComparison.OrdinalIgnoreCase);

                       builder.AddConnectSrc()
                           .Self()
                           .From(socketUrl);

                       builder.AddFontSrc() // font-src 'self'
                           .Self()
                           .Data();

                       builder.AddObjectSrc() // object-src 'none'
                           .None();

                       builder.AddFormAction() // form-action 'self'
                           .Self();

                       builder.AddImgSrc() // img-src https:
                           .Self()
                           .Data();

                       // builder.AddScriptSrc() // script-src 'self'
                       //     .Self();

                       // builder.AddStyleSrc() // style-src 'self'
                       //     .Self();

                       builder.AddUpgradeInsecureRequests(); // upgrade-insecure-requests
                       builder.AddCustomDirective("script-src", "'self' 'unsafe-inline' 'unsafe-eval'");
                       builder.AddCustomDirective("style-src", "'self' 'unsafe-inline' 'unsafe-eval'");

                       builder.AddMediaSrc()
                           .Self();

                       builder.AddFrameAncestors() // frame-ancestors 'none'
                           .None();

                       builder.AddFrameSource()
                           .None();

                       // You can also add arbitrary extra directives: plugin-types application/x-shockwave-flash"
                       // builder.AddCustomDirective("plugin-types", "application/x-shockwave-flash");

                   });

            app.UseSecurityHeaders(policyCollection);
            return app;
        }

        public static IApplicationBuilder ApiTokenRespons(this IApplicationBuilder app)
        {
            app.Use(async (context, nextMiddleware) =>
            {
                using (var memory = new MemoryStream())
                {
                    var originalStream = context.Response.Body;
                    context.Response.Body = memory;

                    await nextMiddleware();

                    memory.Seek(0, SeekOrigin.Begin);
                    var content = new StreamReader(memory).ReadToEnd();
                    memory.Seek(0, SeekOrigin.Begin);

                    var TokenData = JsonConvert.DeserializeObject<tokanreponsmodel>(content);

                    //var RedisDataStore = app.ApplicationServices.GetRequiredService<IUserSessionService>();
                    ////var usedata =  loggerFactory.GetUserAsync(context.User);
                    //RedisDataStore.SetSessionToken(content);

                    string message = context.Session.GetString("Token");
                    if (message == null)
                    { //context.User.ToString()
                        context.Session.Remove("Token");
                    }

                    context.Session.SetString("Token", TokenData.access_token);

                    // Apply logic here for deciding which headers to add
                    context.Response.Headers.Add("Body", content);



                    await memory.CopyToAsync(originalStream);
                    context.Response.Body = originalStream;
                }
            });

            return app;
        }


        public static IApplicationBuilder UseCustomSwaggerApi(this IApplicationBuilder app, Microsoft.Extensions.Configuration.IConfiguration configuration)
        {
            // Enable middleware to serve generated Swagger as a JSON endpoint
            app.UseSwagger();
            // Enable middleware to serve swagger-ui assets (HTML, JS, CSS etc.)
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint(configuration["SwaggerPath"], "Worldex Api V1");               
                //c.SwaggerEndpoint(configuration["SwaggerPath2"], "Worldex Api V2");
                // add search options in swagger ui by nirav savariya 9-29-2018  
                c.EnableFilter("");
                c.DocExpansion(DocExpansion.None);
            });

            return app;
        }
        public static IApplicationBuilder AddDevMiddlewares(this IApplicationBuilder app)
        {
            app.UseCors("CorsPolicy");

            var env = app.ApplicationServices.GetRequiredService<IHostingEnvironment>();
            var loggerFactory = app.ApplicationServices.GetRequiredService<ILoggerFactory>();

            loggerFactory.AddConsole(Startup.Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();
            app.UseDeveloperExceptionPage();
            app.UseDatabaseErrorPage();
            // added by nirav savariya for Req. and Res. using write log data on 31-10-2018
            app.UseMiddleware<RequestResponseLoggingMiddleware>();
            app.UseMiddleware<ResponseRewindMiddleware>();
            //app.UseMiddleware<AuthenticationMiddleware>();
            // NOTE: For SPA swagger needs adding before MVC
            //app.UseCustomSwaggerApi();

            //app.UseSession();

            ////// app.ApiTokenRespons();

            //// Call Method only this url.
            //app.UseWhen(context => context.Request.Path.StartsWithSegments("/connect/token"), appBuilder =>
            //{
            //    appBuilder.ApiTokenRespons();
            //});

            return app;
        }

        public static IApplicationBuilder AddCustomLocalization(this IApplicationBuilder app)
        {
            var options = app.ApplicationServices.GetService<IOptions<RequestLocalizationOptions>>();
            app.UseRequestLocalization(options.Value);

            return app;
        }
    }

    //komal 1-1-2018 for controller grouping, divide controller
    //public class ApiExplorerGetsOnlyConvention : IControllerModelConvention 
    //{
    //    Microsoft.Extensions.Configuration.IConfiguration configuration;

    //    public ApiExplorerGetsOnlyConvention(Microsoft.Extensions.Configuration.IConfiguration _configuration)
    //    {
    //        configuration = _configuration;
    //    }
    //    public void Apply(ControllerModel controller)
    //    {
    //        string namelist = configuration["DisableController"];
    //        if (namelist.Contains(controller.ControllerName))
    //        {
    //            controller.ApiExplorer.GroupName = "v2";
    //            //action.ApiExplorer.IsVisible = true;
    //        }
    #region AppSettingKey
    //komal 1-1-2018 for controller grouping, divide controller
    //"DisableController": "Values,Chat,Socket,ToDoItems,DeviceRegistration,GlobalNotification,Transaction,TransactionBackOffice,TransactionBackOfficeCount,TransactionConfiguration,Wallet,WalletOperations,WalletOpnAdvanced,MasterConfiguration,WalletConfiguration,WalletControlPanel"//for MyAccount
    //"DisableController": "Values,Chat,Socket,ToDoItems,DeviceRegistration,GlobalNotification,Wallet,WalletOperations,WalletOpnAdvanced,MasterConfiguration,WalletConfiguration,WalletControlPanel,Authorization,Manage,Profile,Signin,SignUp,TwoFASetting,BackOfficeComplain,BackOfficeOrganization,BackOffice,KYC,KYCConfiguration,SocialProfile,Complaint,SignUpReport,BackOfficeActivityLog" //for Transaction
    //"DisableController": "Values,Chat,Socket,ToDoItems,DeviceRegistration,GlobalNotification,Transaction,TransactionBackOffice,TransactionBackOfficeCount,TransactionConfiguration,Authorization,Manage,Profile,Signin,SignUp,TwoFASetting,BackOfficeComplain,BackOfficeOrganization,BackOffice,KYC,KYCConfiguration,SocialProfile,Complaint,SignUpReport,BackOfficeActivityLog", //for Wallet
    //"DisableController": "Values,Transaction,TransactionBackOffice,TransactionBackOfficeCount,TransactionConfiguration,Authorization,Manage,Profile,Signin,SignUp,TwoFASetting,BackOfficeComplain,BackOfficeOrganization,BackOffice,KYC,KYCConfiguration,SocialProfile,Complaint,SignUpReport,BackOfficeActivityLog,Wallet,WalletOperations,WalletOpnAdvanced,MasterConfiguration,WalletConfiguration,WalletControlPanel" //for Communication
    //"DisableController": ""
    #endregion
    //    }
    //}
}
