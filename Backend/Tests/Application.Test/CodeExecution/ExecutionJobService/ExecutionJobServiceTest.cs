using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Application.Common.Interfaces.Compiler;
using Application.Common.Interfaces.EventBus;
using Application.Common.Protos;
using Application.Test.Common;
using Domain.Entities;
using Domain.Enums;
using FluentAssertions;
using Moq;
using Xunit;

namespace Application.Test.CodeExecution.ExecutionJobService
{
    public class ExecutionJobServiceTest
    {
        private readonly Mock<IMultiLanguageCompiler> _compiler = new Mock<IMultiLanguageCompiler>();
        private readonly Mock<IEventBus> _eventBus = new Mock<IEventBus>();
        private readonly Mock<IEventHandler<ExecutionJobResult>> _executionJobCompletedHandler = new Mock<IEventHandler<ExecutionJobResult>>();

        [Fact]
        public async Task StartJob()
        {
            const string roomId = "task123";
            const string jobId = "job123";
            const Language language = Language.Java;

            var taskRunner = new Mock<ITaskRunner>();
            
            var sourceFiles = new List<SourceFile>();
            
            var compilationResult = new CompilationResult(BogusData.File.Generate(2));

            _compiler.Setup(c => c.Compile(language, sourceFiles)).ReturnsAsync(compilationResult);
            
            var executionJobService = new Application.Rooms.ExecutionJobService.ExecutionJobService(_compiler.Object, _eventBus.Object, _executionJobCompletedHandler.Object, taskRunner.Object);

            await executionJobService.StartJob(roomId, jobId, language, sourceFiles);
            
            _compiler.Verify(c => c.Compile(language, sourceFiles));
            
           _eventBus.Verify(e => e.Publish(It.Is<ExecutionJob>(j => 
               j.JobId == jobId && 
               j.RoomId == roomId && 
               j.Language == (int)language
               )));
        }

        [Fact]
        public async Task StartJob_CompilationFailed()
        {
            const string roomId = "room123";
            const string jobId = "job123";
            const Language language = Language.Java;

            var taskRunner = new Mock<ITaskRunner>();

            var sourceFiles = new List<SourceFile>();
            
            var compilationResult = new CompilationResult("Unable to compile");

            _compiler.Setup(c => c.Compile(language, sourceFiles)).ReturnsAsync(compilationResult);

            var executionJobService = new Application.Rooms.ExecutionJobService.ExecutionJobService(_compiler.Object, _eventBus.Object, _executionJobCompletedHandler.Object, taskRunner.Object);

            await executionJobService.StartJob(roomId, jobId, language, sourceFiles);
            
            _compiler.Verify(c => c.Compile(language, sourceFiles));

            _executionJobCompletedHandler.Verify(e => e.Handle(It.Is<ExecutionJobResult>(r => 
                r.JobId == jobId &&
                r.RoomId == roomId &&
                r.Error &&
                r.ErrorMessage == compilationResult.Output)));
        }

        [Fact]
        private async Task TimeoutJob()
        {
            const string roomId = "room123";
            const string jobId = "job123";

            var taskRunner = new Mock<ITaskRunner>();
            taskRunner.Setup(t => t.Delay(It.IsAny<TimeSpan>())).Returns(Task.CompletedTask);
            
            var executionJobService = new Application.Rooms.ExecutionJobService.ExecutionJobService(_compiler.Object, _eventBus.Object, _executionJobCompletedHandler.Object, taskRunner.Object);

            await executionJobService.TimeoutJob(roomId, jobId);
            
            _executionJobCompletedHandler.Verify(e => e.Handle(It.Is<ExecutionJobResult>(r => 
                r.RoomId == roomId &&
                r.JobId == jobId &&
                r.ExecutionTime == 0 &&
                r.Error)));
        }
    }
}