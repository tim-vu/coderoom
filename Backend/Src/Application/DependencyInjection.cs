using System.Reflection;
using Application.CodeExecution;
using Application.CodeExecution.ExecutionJobService;
using Application.Common.Interfaces.EventBus;
using Application.Common.Middleware;
using Application.Common.Protos;
using Application.Rooms.RoomService;
using Application.Rooms.RoomTextLock;
using Application.Rooms.TemplateService;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Application
{
    public static class DependencyInjection
    {
        public static void AddApplication(this IServiceCollection services)
        {
            services.AddAutoMapper(Assembly.GetExecutingAssembly());
            services.AddMediatR(Assembly.GetExecutingAssembly());
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestValidationMiddleware<,>));

            services.AddTransient<IRoomTextLock, RoomTextLock>();
            services.AddTransient<IRoomNotifier, RoomNotifier>();
            services.AddSingleton<ITemplateService, TemplateService>();

            services.AddTransient<IExecutionJobService, ExecutionJobService>();
            services.AddTransient<IEventHandler<ExecutionJobResult>, ExecutionJobCompletedEventHandler>();
        }

        public static void RegisterEventHandlers(this IEventBus eventBus)
        {
            eventBus.Subscribe<ExecutionJobResult, ExecutionJobCompletedEventHandler>();
        }
    }
}