using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using EventBusRabbitMQ.EventHandlers;
using EventBusRabbitMQ.Events;
using EventBusRabbitMQ.Interfaces;
using LoggingNlog;
using Microsoft.Azure.ServiceBus;
using Newtonsoft.Json;

namespace EventBusRabbitMQ.Services
{
    public class AzureEventBus : IEventBus, IDisposable
    {
        private readonly IAzureServiceBusPersistentConnection serviceBusPersistentConnection;
        private readonly INLogger<AzureEventBus> nLogger;
        private readonly IEventBusSubscriptionsManager subscriptionsManager;
        private readonly SubscriptionClient subscriptionClient;
        private readonly ILifetimeScope autofacScope;
        private string AUTOFAC_SCOPE_NAME;

        public AzureEventBus(IAzureServiceBusPersistentConnection serviceBusPersistentConnection, INLogger<AzureEventBus> nLogger, string subscriptionClientName, IEventBusSubscriptionsManager subscriptionsManager = null, ILifetimeScope autofacScope = null)
        {
            this.serviceBusPersistentConnection = serviceBusPersistentConnection;
            this.nLogger = nLogger;
            this.subscriptionsManager = subscriptionsManager;
            this.autofacScope = autofacScope;

            subscriptionClient = new SubscriptionClient(serviceBusPersistentConnection.ServiceBusConnectionStringBuilder, subscriptionClientName);

            RemoveDefaultRule();
            RegisterSubscriptionClientMessageHandler();
        }

        public void Publish(IntegrationEvent @event, string brokerName, string routingKey, string typeOfExchange)
        {
            //Create send message -Sahil 10-09-2019
            //Label has event name from which event and it's handler matched
            var jsonMessage = JsonConvert.SerializeObject(@event);
            var body = Encoding.UTF8.GetBytes(jsonMessage);
            var message = new Message
            {
                MessageId = Guid.NewGuid().ToString(),
                Body = body,
                Label = routingKey,
            };

            var topicClient = serviceBusPersistentConnection.CreateModel();

            topicClient.SendAsync(message)
                .GetAwaiter()
                .GetResult();

            //debug logging -Sahil 10-09-2019
            nLogger.WriteInfoLog("Publish", $"Azure ServiceBus published label:{message.Label} message: {message.Body}");
        }

        public void Subscribe<T, TH>(string queueName, string brokerName, string routingKey, string TypeOfExchange) where T : IntegrationEvent where TH : IIntegrationEventHandler<T>
        {
            AUTOFAC_SCOPE_NAME = brokerName;
            var containsKey = subscriptionsManager.HasSubscriptionsForEvent<T>();
            if (!containsKey)
            {
                try
                {
                    subscriptionClient.AddRuleAsync(new RuleDescription
                    {
                        Filter = new CorrelationFilter { Label = routingKey },
                        Name = routingKey
                    }).GetAwaiter().GetResult();
                }
                catch (ServiceBusException ex)
                {
                    nLogger.WriteErrorLog("Subscribe", ex);
                }
            }

            nLogger.WriteInfoLog("Subscribe", $"Subscribing to event {typeof(T).Name} with {typeof(TH).Name}");

            subscriptionsManager.AddSubscription<T, TH>(queueName, brokerName, routingKey);
        }

        public void Unsubscribe<T, TH>() where T : IntegrationEvent where TH : IIntegrationEventHandler<T>
        {

            try
            {
                subscriptionClient
                    .RemoveRuleAsync(typeof(T).Name)
                    .GetAwaiter()
                    .GetResult();
            }
            catch (MessagingEntityNotFoundException ex)
            {
                nLogger.WriteErrorLog("Unsubscribe", ex);
            }

            nLogger.WriteInfoLog("Unsubscribe", $"Unsubscribing from event {typeof(T).Name}");

            subscriptionsManager.RemoveSubscription<T, TH>();
        }

        public void Dispose()
        {
            subscriptionsManager.Clear();
        }

        private void RemoveDefaultRule()
        {
            try
            {
                subscriptionClient
                    .RemoveRuleAsync(RuleDescription.DefaultRuleName)
                    .GetAwaiter()
                    .GetResult();
            }
            catch (MessagingEntityNotFoundException ex)
            {
                nLogger.WriteErrorLog("RemoveDefaultRule", ex);
            }
        }

        private void RegisterSubscriptionClientMessageHandler()
        {
            subscriptionClient.RegisterMessageHandler(
                async (message, token) =>
                {
                    var eventName = message.Label;
                    var messageData = Encoding.UTF8.GetString(message.Body);

                    // Complete the message so that it is not received again.
                    if (await ProcessEvent(eventName, messageData))
                    {
                        await subscriptionClient.CompleteAsync(message.SystemProperties.LockToken);
                    }
                },
                new MessageHandlerOptions(ExceptionReceivedHandler) { MaxConcurrentCalls = 10, AutoComplete = false });
        }

        private Task ExceptionReceivedHandler(ExceptionReceivedEventArgs exceptionReceivedEventArgs)
        {
            var ex = exceptionReceivedEventArgs.Exception;
            var context = exceptionReceivedEventArgs.ExceptionReceivedContext;

            nLogger.WriteErrorLog("ExceptionReceivedHandler", ex);

            return Task.CompletedTask;
        }


        private async Task<bool> ProcessEvent(string eventName, string message)
        {
            bool processed = false;
            if (subscriptionsManager.HasSubscriptionsForEvent(eventName))
            {
                using (var scope = autofacScope.BeginLifetimeScope(AUTOFAC_SCOPE_NAME))
                {
                    var subscriptions = subscriptionsManager.GetHandlersForEvent(eventName);
                    foreach (var subscription in subscriptions)
                    {
                        var handler = scope.ResolveOptional(subscription.HandlerType);
                        if (handler == null) continue;
                        var eventType = subscriptionsManager.GetEventTypeByName(eventName);
                        var integrationEvent = JsonConvert.DeserializeObject(message, eventType);
                        var concreteType = typeof(IIntegrationEventHandler<>).MakeGenericType(eventType);
                        await (Task)concreteType.GetMethod("Handle").Invoke(handler, new object[] { integrationEvent });
                    }
                }
                processed = true;
            }

            return processed;
        }
    }
}
