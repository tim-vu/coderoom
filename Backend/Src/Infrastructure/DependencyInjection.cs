using System;
using System.Collections.Generic;
using Application.Common.Interfaces;
using Application.Common.Interfaces.Compiler;
using Application.Common.Interfaces.EventBus;
using Infrastructure.Compiler;
using Infrastructure.Compiler.Compilers.CSharp;
using Infrastructure.Compiler.Compilers.Java;
using Infrastructure.RabbitMq;
using Infrastructure.RabbitMq.SubscriptionManager;
using Infrastructure.Redis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.ObjectPool;
using RabbitMQ.Client;
using RedLockNet;
using RedLockNet.SERedis;
using RedLockNet.SERedis.Configuration;
using StackExchange.Redis;

namespace Infrastructure
{
    public static class DependencyInjection
    {
        public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<IMemoryStore, RedisMemoryStore>();
            services.AddSingleton<IConnectionMultiplexer>(o =>
                ConnectionMultiplexer.Connect(configuration.GetValue<string>("RedisConnection")));
            services.AddSingleton<IDistributedLockFactory, RedLockFactory>(o => RedLockFactory.Create(new List<RedLockMultiplexer> {new RedLockMultiplexer(o.GetService<IConnectionMultiplexer>())}));
            
            services.AddSingleton<IIdGenerator, IdGenerator.IdGenerator>();
            services.AddTransient<IDateTime, MachineDateTime.MachineDateTime>();
            services.AddTransient<ITaskRunner, TaskRunner.TaskRunnerRunner>();

            services.AddTransient<IMultiLanguageCompiler, MultiLanguageCompiler>();

            services.AddSingleton<ISubscriptionManager, DefaultSubscriptionManager>();
            
            services.AddSingleton<IRabbitMQPersistentConnection>(sp =>
            {
                var logger = sp.GetRequiredService<ILogger<DefaultRabbitMQPersistentConnection>>();
                
                var factory = new ConnectionFactory
                {
                    DispatchConsumersAsync = true,
                    HostName = configuration["EventBusConnection"],
                    Endpoint = new AmqpTcpEndpoint(new Uri("amqp://" + configuration["EventBusConnection"]))
                };

                if (!string.IsNullOrEmpty(configuration["EventBusUsername"]))
                {
                    factory.UserName = configuration["EventBusUsername"];
                }

                if (!string.IsNullOrEmpty(configuration["EventBusPassword"]))
                {
                    factory.Password = configuration["EventBusPassword"];
                }
                
                return new DefaultRabbitMQPersistentConnection(factory, logger);
            });

            services.AddSingleton<IEventBus>(sp =>
            {
                var persistentConnection = sp.GetRequiredService<IRabbitMQPersistentConnection>();
                var logger = sp.GetRequiredService<ILogger<RabbitMqEventBus>>();
                var subsManager = sp.GetRequiredService<ISubscriptionManager>();
                var queueName = configuration["EventBusQueueName"];
                
                return new RabbitMqEventBus(persistentConnection, subsManager, sp, logger, queueName);
            });
        }
    }
}