using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Application.Common.Interfaces.Compiler;
using Application.Common.Interfaces.EventBus;
using Application.Common.Protos;
using Domain.Entities;
using Domain.Enums;
using Google.Protobuf;
using ExecutionJob = Application.Common.Protos.ExecutionJob;

namespace Application.Rooms.ExecutionJobService
{
    public class ExecutionJobService : IExecutionJobService
    {
        private readonly TimeSpan _jobTimeout;
        
        private readonly IMultiLanguageCompiler _compiler;
        private readonly IEventBus _eventBus;
        private readonly IEventHandler<ExecutionJobResult> _executionJobCompletedHandler;
        private readonly ITaskRunner _taskRunner;
        
        public ExecutionJobService(TimeSpan jobTimeout, IMultiLanguageCompiler compiler, IEventBus eventBus, IEventHandler<ExecutionJobResult> executionJobCompletedHandler, ITaskRunner taskRunner)
        {
            _jobTimeout = jobTimeout;
            _compiler = compiler;
            _eventBus = eventBus;
            _executionJobCompletedHandler = executionJobCompletedHandler;
            _taskRunner = taskRunner;
        }

        public Task StartJob(string roomId, string id, Language language, List<SourceFile> sourceFiles)
        {
            return Task.Run(async () =>
            {
                var compilationResult = await _compiler.Compile(language, sourceFiles);

                if (!compilationResult.Success)
                {
                    var taskResult = new ExecutionJobResult
                    {
                        JobId = id,
                        RoomId = roomId,
                        Error = true,
                        ErrorMessage = compilationResult.Output
                    };

                    await _executionJobCompletedHandler.Handle(taskResult);

                    return;
                }
                
                var task = new ExecutionJob
                {
                    Language = (int)language,
                    RoomId = roomId,
                    JobId = id
                };

                foreach (var file in compilationResult.Files)
                {
                    task.Files.Add(new File
                    {
                        Filename = file.Name,
                        Content = ByteString.CopyFrom(file.Content)
                    });
                }
                
                _eventBus.Publish(task);
            });
        }

        public Task TimeoutJob(string roomId, string id)
        {
            return _taskRunner.Delay(_jobTimeout).ContinueWith(_ =>
            {
                var jobResult = new ExecutionJobResult
                {
                    RoomId = roomId,
                    JobId = id,
                    ExecutionTime = 0,
                    Error = true,
                    ErrorMessage = "Unable to execute the code"
                };

                _executionJobCompletedHandler.Handle(jobResult);
            });
        }
    }
}