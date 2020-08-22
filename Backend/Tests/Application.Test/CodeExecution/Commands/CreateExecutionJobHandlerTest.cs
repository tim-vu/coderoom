using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Application.CodeExecution.Commands.CreateExecutionJob;
using Application.CodeExecution.ExecutionJobService;
using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Rooms.RoomService;
using Application.Test.Common;
using Domain.Entities;
using Domain.Enums;
using FluentAssertions;
using Moq;
using Xunit;

namespace Application.Test.CodeExecution.Commands
{
    public class CreateExecutionJobHandlerTest
    {
        private readonly Mock<ILock> _lock = new Mock<ILock>();
        private readonly Mock<IMemoryStore> _store = new Mock<IMemoryStore>();
        private readonly Mock<IRoomNotifier> _roomService = new Mock<IRoomNotifier>();
        
        public CreateExecutionJobHandlerTest()
        {
            _lock.Setup(l => l.IsAcquired).Returns(true);
        }
        
        [Fact]
        public async Task Handle_CreateExecutionJob()
        {
            var room = BogusData.Room.Generate();
            var user = BogusData.User.Generate();
            room.Users.Add(user);

            var lineCount = room.Text.Split(Environment.NewLine).Length;

            _store.Setup(s => s.CreateLock(room.Id)).ReturnsAsync(_lock.Object);
            _store.Setup(s => s.ObjectGet<Room>(room.Id)).ReturnsAsync(room);

            const string taskId = "task123";
            
            var idGenerator = new Mock<IIdGenerator>();
            idGenerator.Setup(i => i.GenerateId()).Returns(taskId);

            var executionJobService = new Mock<IExecutionJobService>();
            
            var createExecutionJobHandler = new CreateExecutionJob.CreateExecutionJobHandler(_store.Object, idGenerator.Object, executionJobService.Object, _roomService.Object);

            await createExecutionJobHandler.Handle(new CreateExecutionJob(user.ConnectionId, room.Id), CancellationToken.None);

            _roomService.Verify(r => r.NotifyCodeExecutionStarted(room, user.NickName, lineCount));

            room.CurrentExecutionJobId.Should().Be(taskId);
            
            _store.Verify(s => s.ObjectSet(room.Id, room));
            
            var sourceFilename = "Solution" + "." + room.Language.GetFileExtension();
            executionJobService.Verify(e => e.StartJob(
                room.Id, taskId, 
                room.Language, 
                It.Is<List<SourceFile>>(l => l.Count == 1 && l.Exists(f => f.Name == sourceFilename && f.Content == room.Text))
                ));
        }

        [Fact]
        public void Handle_CreateExecutionJob_NonExistingRoom()
        {
            
            _store.Setup(s => s.CreateLock(It.IsAny<string>())).ReturnsAsync(_lock.Object);
            
            var idGenerator = new Mock<IIdGenerator>();
            var executionJobService = new Mock<IExecutionJobService>();
            
            var createExecutionJobHandler = new CreateExecutionJob.CreateExecutionJobHandler(_store.Object, idGenerator.Object, executionJobService.Object, _roomService.Object);

            const string roomId = "room123";
            const string connectionId = "user123";
            
            Func<Task> act = async () =>
                await createExecutionJobHandler.Handle(new CreateExecutionJob(roomId, connectionId), CancellationToken.None);

            act.Should().Throw<NotFoundException>();
        }
    }
}