using Autofac;
using Autofac.Extensions.DependencyInjection;
using EventBusRabbitMQ.Interfaces;
using EventBusRabbitMQ.Services;
using LoggingNlog;
using MarketMaker.Application.IntegrationEvents.EventHandling;
using MarketMaker.Application.IntegrationEvents.Events;
using MarketMaker.Application.Interfaces.Queries;
using MarketMaker.Application.Interfaces.Services;
using MarketMaker.Application.Interfaces.Services.Redis;
using MarketMaker.Infrastructure.Contexts;
using MarketMaker.Infrastructure.Queries;
using MarketMaker.Infrastructure.Services;
using MarketMaker.Infrastructure.Services.Redis;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Globalization;
using MarketMaker.Application.AutoFacModules;
using MarketMaker.Application.ViewModels.Config;
using Microsoft.AspNetCore.Localization;

namespace MarketMaker.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            services.AddLogging()
                .AddRedisServices()
                .AddRabbitMQServices(Configuration)
                .AddSqlServices(Configuration)
                .AddMemoryCache(); //for store marketmaker auth token data in cache -Sahil 04-10-2019 06:11 PM

            services.AddScoped<IMarketMakerQueries, MarketMakerQueries>();
            services.AddSingleton<IWebUrlRequest, WebUrlRequest>(); //add for get marketmaker token from api -Sahil 04-10-2019 04:06 PM
            services.AddMediatR(typeof(Startup).Assembly); // add for domain event call -Sahil 07-10-2019 12:24 PM
            services.AddSingleton<ICacheTokenService, CacheTokenService>(); // use for token store related operation -Sahil 09-10-2019 03:01 PM

            //register class for get config -Sahil 10-10-2019 05:53 PM
            services.Configure<OrderApiConfigs>(Configuration)
                .Configure<TokenApiConfigs>(Configuration)
                .Configure<MarketMakerConfigs>(Configuration)
                .Configure<MarketMakerOrderBookTradeConfig>(Configuration.GetSection("marketMakerOrderBookTrade"));

            //configure autofac
            var container = new ContainerBuilder();
            container.RegisterModule(new ApplicationModule());
            container.Populate(services);

            return new AutofacServiceProvider(container.Build());
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseRequestLocalization(new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture("en-US"),
                SupportedCultures = new List<CultureInfo>()
                {
                    new CultureInfo("en-US")
                }
            });
            app.UseMvc();

            ConfigureSubscribeEventBus(app);

        }

        private void ConfigureSubscribeEventBus(IApplicationBuilder app)
        {
            var eventBus = app.ApplicationServices.GetRequiredService<IEventBus>();

            eventBus.Subscribe<TransactionOnHoldCompletedIntegrationEvent, TransactionOnHoldCompletedIntegrationEventHandler>(
                Configuration.GetValue<string>("TransactionRabbitTrnEventConfig:QueueName"),
                Configuration.GetValue<string>("TransactionRabbitTrnEventConfig:BrokerName"),
                Configuration.GetValue<string>("TransactionRabbitTrnEventConfig:RoutingKey"),
                Configuration.GetValue<string>("TransactionRabbitTrnEventConfig:TypeOfExchange")
            );

            //subscribe event for notify when LTP change for binance -Sahil 15-11-2019 01:29 PM
            eventBus.Subscribe<TickerDataChangeIntegrationEvent, TickerDataChangeIntegrationEventHandler>(
                Configuration.GetValue<string>("TickerDataRabbitEventConfig:QueueName"),
                Configuration.GetValue<string>("TickerDataRabbitEventConfig:BrokerName"),
                Configuration.GetValue<string>("TickerDataRabbitEventConfig:RoutingKey"),
                Configuration.GetValue<string>("TickerDataRabbitEventConfig:TypeOfExchange")
            );

            eventBus.Subscribe<OrderbookDataChangeIntegrationEvent, OrderbookDataChangeIntegrationEventHandler>(
                Configuration.GetValue<string>("OrderBookRabbitEventConfig:QueueName"),
                Configuration.GetValue<string>("OrderBookRabbitEventConfig:BrokerName"),
                Configuration.GetValue<string>("OrderBookRabbitEventConfig:RoutingKey"),
                Configuration.GetValue<string>("OrderBookRabbitEventConfig:TypeOfExchange")
            );

            //eventBus.Subscribe<MarketMakerTransactionSettledIntegrationEvent, MarketMakerTransactionSettledIntegrationEventHandler>(
            //    Configuration.GetValue<string>("TransactionRabbitSettleEventConfig:QueueName"),
            //    Configuration.GetValue<string>("TransactionRabbitSettleEventConfig:BrokerName"),
            //    Configuration.GetValue<string>("TransactionRabbitSettleEventConfig:RoutingKey"),
            //    Configuration.GetValue<string>("TransactionRabbitSettleEventConfig:TypeOfExchange")
            //);
        }
    }

    public static class ServiceExtensions
    {
        /// <summary>
        /// Perform DI for Infrastructure Layer's Redis cache service classes.
        /// </summary>
        /// <param name="services"> object of IServiceCollection class</param>
        /// <remarks>-Sahil 28-09-2019</remarks>
        public static IServiceCollection AddRedisServices(this IServiceCollection services)
        {
            services.AddSingleton<IRedisConnectionFactory, RedisConnectionFactory>();
            services.AddSingleton(typeof(IRedisServices<>), typeof(RedisServices<>));
            services.AddSingleton<IRedisTradingManagement, RedisTradingManagement>();

            return services;
        }

        /// <summary>
        /// Add service for EventBusRabbitMQ library 
        /// </summary>
        /// <remarks>-Sahil 02-10-2019</remarks>
        public static IServiceCollection AddRabbitMQServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IEventBusSubscriptionsManager, InMemoryEventBusSubscriptionsManager>();

            //configure rabbitmq connection
            services.AddSingleton<IRabbitMQPersistentConnection>(sp =>
            {
                var logger = sp.GetRequiredService<INLogger<DefaultRabbitMQPersistentConnection>>();
                var factory = new ConnectionFactory()
                {
                    HostName = configuration["EventBusConnection"],
                    Protocol = Protocols.DefaultProtocol,
                    RequestedHeartbeat = Convert.ToUInt16(configuration["EventBusHeartbeat"])
                };

                if (!string.IsNullOrEmpty(configuration["EventBusUserName"]))
                {
                    factory.UserName = configuration["EventBusUserName"];
                }

                if (!string.IsNullOrEmpty(configuration["EventBusPassword"]))
                {
                    factory.Password = configuration["EventBusPassword"];
                }

                if (!string.IsNullOrEmpty(configuration["EventBusVirtualHost"]))
                {
                    factory.VirtualHost = configuration["EventBusVirtualHost"];
                }
                //for recover connection
                factory.AutomaticRecoveryEnabled = true;
                factory.NetworkRecoveryInterval = TimeSpan.FromSeconds(5);
                return new DefaultRabbitMQPersistentConnection(factory, logger);
            });

            //Configure rabbitmq queue and channel
            services.AddSingleton<IEventBus>(sp =>
            {
                var connection = sp.GetRequiredService<IRabbitMQPersistentConnection>();
                var logger = sp.GetRequiredService<INLogger<RabbitMQEventBus>>();
                var subManager = sp.GetRequiredService<IEventBusSubscriptionsManager>();
                var iLifeTimeScope = sp.GetRequiredService<ILifetimeScope>();

                return new RabbitMQEventBus(connection, logger, subManager, iLifeTimeScope);
            });

            return services;
        }

        /// <summary>
        /// Add LoggingNLog library service for logging  
        /// </summary>
        /// <remarks>-Sahil 02-10-2019</remarks>
        public static IServiceCollection AddLogging(this IServiceCollection services)
        {
            services.AddSingleton(typeof(INLogger<>), typeof(NLogger<>));

            return services;
        }

        /// <summary>
        /// Add mssql connection and initialize dbContext from infrastructure layer.
        /// </summary>
        /// <remarks>-Sahil 02-10-2019</remarks>
        public static IServiceCollection AddSqlServices(this IServiceCollection services, IConfiguration configuration)
        {
            var connection = configuration.GetConnectionString("SqlServerConnectionString");
            services.AddDbContext<MarketMakerContext>(options =>
            {
                options.UseSqlServer(connection);
            });

            return services;
        }
    }
}
