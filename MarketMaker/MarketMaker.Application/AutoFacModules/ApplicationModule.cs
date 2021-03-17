using Autofac;
using EventBusRabbitMQ.EventHandlers;
using MarketMaker.Application.IntegrationEvents.Events;
using System.Reflection;
using LoggingNlog;
using MarketMaker.Domain.Events;
using MediatR;

namespace MarketMaker.Application.AutoFacModules
{
    public class ApplicationModule : Autofac.Module
    {

        public ApplicationModule()
        {
        }

        protected override void Load(ContainerBuilder builder)
        {
            //register mediatR event -Sahil 07-10-2019 12:38 PM
            //comment for testing mediator handler call twice -Sahil 09-10-2019 07:02 PM
            //builder.RegisterAssemblyTypes(typeof(MarketMakerAuthTokenChangedDomainEvent).GetTypeInfo().Assembly)
            //    .AsClosedTypesOf(typeof(IRequestHandler<,>));

            //builder.RegisterAssemblyTypes(typeof(TransactionOnHoldCompletedIntegrationEvent).GetTypeInfo().Assembly)
            //    .AsClosedTypesOf(typeof(INotificationHandler<>));

            //builder.RegisterAssemblyTypes(typeof(UserBalanceCheckCompletedIntegrationEvent).GetTypeInfo().Assembly)
            //    .AsClosedTypesOf(typeof(INotificationHandler<>));

            //register all domain event handler (they implement INotificationHandler) -Sahil 09-10-2019 07:28 PM 
            builder.RegisterAssemblyTypes(typeof(TransactionOnHoldCompletedIntegrationEvent).GetTypeInfo().Assembly)
                .AsClosedTypesOf(typeof(INotificationHandler<>));

            //register all event handler implement IRequestHandler -Sahil 09-10-2019 07:34 PM
            builder.RegisterAssemblyTypes(typeof(MarketMakerAuthTokenChangedDomainEvent).GetTypeInfo().Assembly)
                .AsClosedTypesOf(typeof(IRequestHandler<,>));


            //add for mediator configuration -Sahil 09-10-2019 07:21 PM
            builder.Register<ServiceFactory>(context =>
            {
                var componentContext = context.Resolve<IComponentContext>();
                return t => { object o; return componentContext.TryResolve(t, out o) ? o : null; };
            });

            builder.RegisterAssemblyTypes(typeof(TransactionOnHoldCompletedIntegrationEvent).GetTypeInfo().Assembly)
        .AsClosedTypesOf(typeof(IIntegrationEventHandler<>));

        }
    }
}
