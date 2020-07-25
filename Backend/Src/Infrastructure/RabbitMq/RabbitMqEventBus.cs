using System;
using System.Diagnostics.CodeAnalysis;
using System.Net.Sockets;
using System.Threading.Tasks;
using Application.Common.Interfaces.EventBus;
using Google.Protobuf;
using Infrastructure.RabbitMq.SubscriptionManager;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.ObjectPool;
using Polly;
using Polly.Retry;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;

namespace Infrastructure.RabbitMq
{
    public class RabbitMqEventBus : IEventBus, IDisposable
    {
        private const string BrokerName = "coderoom_event_bus";
        
        private readonly ILogger<RabbitMqEventBus> _logger;
        private readonly IRabbitMQPersistentConnection _persistentConnection;
        private readonly ISubscriptionManager _subscriptionManager;
        private readonly IServiceProvider _serviceProvider;
        private readonly string _queueName;
        private readonly int _retryCount;

        private IModel _consumerChannel;

        public RabbitMqEventBus(IRabbitMQPersistentConnection persistentConnection,
            ISubscriptionManager subscriptionManager, IServiceProvider serviceProvider,
            ILogger<RabbitMqEventBus> logger, string queueName = null, int retryCount = 5)
        {
            _logger = logger;
            _subscriptionManager = subscriptionManager;
            _serviceProvider = serviceProvider;
            _queueName = queueName;
            _retryCount = retryCount;
            _persistentConnection = persistentConnection;

            _consumerChannel = CreateConsumerChannel();
        }

        [SuppressMessage("ReSharper", "AccessToDisposedClosure")]
        public void Publish<T>(IMessage<T> message) where T : IMessage<T>
        {
            if (!_persistentConnection.IsConnected)
            {
                _persistentConnection.TryConnect();
            }
            
            var eventName = message.GetType().Name;

            var policy = Policy.Handle<BrokerUnreachableException>()
                .Or<SocketException>()
                .WaitAndRetry(_retryCount, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), (ex, time) =>
                {
                    _logger.LogWarning(ex, "Could not publish event: {EventId} after {Timeout}s ({ExceptionMessage})", eventName, $"{time.TotalSeconds:n1}", ex.Message);
                });

            using var channel = _persistentConnection.CreateModel();
            
            channel.ExchangeDeclare(BrokerName, "direct");

            var body = message.ToByteArray();

            policy.Execute(() =>
            {
                var properties = channel.CreateBasicProperties();

                channel.BasicPublish(
                    BrokerName,
                    eventName,
                    true,
                    properties,
                    body);
            });
        }

        public void Subscribe<T, TH>() where T : IMessage<T> where TH : IEventHandler<T>
        {
            var eventName = typeof(T).Name;
            
            if (_subscriptionManager.HasSubscriptionForEvent(eventName))
            {
                _subscriptionManager.AddSubscription<T, TH>();
                return;
            }
            
            _subscriptionManager.AddSubscription<T, TH>();
            
            if (!_persistentConnection.IsConnected)
            {
                _persistentConnection.TryConnect();
            }

            using var channel = _persistentConnection.CreateModel();
            
            channel.QueueBind(_queueName, BrokerName, eventName);
            
            StartBasicConsume();
        }

        private void StartBasicConsume()
        {
            _logger.LogTrace("Starting RabbitMQ basic consume");

            if (_consumerChannel != null)
            {
                var consumer = new AsyncEventingBasicConsumer(_consumerChannel);
                
                consumer.Received += ConsumerOnReceived;

                _consumerChannel.BasicConsume(_queueName, true, consumer);
                return;
            }
            
            _logger.LogError("Cannot start basic consume when _consumerChannel == null");
        }

        private async Task ConsumerOnReceived(object sender, BasicDeliverEventArgs @event)
        {
            try
            {
                var eventName = @event.RoutingKey;

                var type = _subscriptionManager.GetEventTypeByName(eventName);

                if (type == null)
                    return;

                var handlers = _subscriptionManager.GetHandlersForEventName(eventName);

                var message = (IMessage)Activator.CreateInstance(type);
                
                message.MergeFrom(@event.Body.ToArray());

                foreach (var handlerType in handlers)
                {
                    var concreteType = typeof(IEventHandler<>).MakeGenericType(type);
                    var handler = ActivatorUtilities.CreateInstance(_serviceProvider, handlerType);

                    // ReSharper disable once PossibleNullReferenceException
                    await (Task)concreteType.GetMethod("Handle").Invoke(handler, new object[] {message});
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error processing message: " + @event.RoutingKey);
            }
        }

        private IModel CreateConsumerChannel()
        {
            if (!_persistentConnection.IsConnected)
            {
                _persistentConnection.TryConnect();
            }

            _logger.LogTrace("Creating RabbitMQ consumer channel");
            
            var channel = _persistentConnection.CreateModel();
            
            channel.ExchangeDeclare(BrokerName, "direct");

            channel.QueueDeclare(_queueName, false, false, false, null);

            channel.CallbackException += (sender, args) =>
            {
                _logger.LogWarning(args.Exception, "Recreating RabbitMQ consumer channel");
                
                _consumerChannel.Dispose();
                _consumerChannel = CreateConsumerChannel();
                StartBasicConsume();
            };

            return channel;
        }

        public void Dispose()
        {
            _persistentConnection?.Dispose();
            _consumerChannel?.Dispose();
        }
    }
}