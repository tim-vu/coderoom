using System.Threading.Tasks;
using Application.CodeExecution;
using Application.Common.Interfaces;
using Application.Common.Interfaces.MemoryStore;
using Application.Common.Protos;
using Application.Rooms.RoomNotifier;
using Application.Rooms.RoomService;
using Application.Test.Common;
using Domain.Entities;
using FluentAssertions;
using Moq;
using Xunit;

namespace Application.Test.CodeExecution
{
    public class ExecutionJobCompletedEventHandlerTest
    {
        private readonly Mock<IMemoryStore> _store = new Mock<IMemoryStore>();
        private readonly Mock<ILock> _lock = new Mock<ILock>();
        private readonly Mock<IRoomNotifier> _roomService = new Mock<IRoomNotifier>();
        private readonly Room _room = BogusData.Room.Generate();
        private readonly ExecutionJobResult _event;
        private readonly ExecutionJobCompletedEventHandler _handler;

        public ExecutionJobCompletedEventHandlerTest()
        {
            _lock.Setup(l => l.IsAcquired).Returns(true);
            _store.Setup(s => s.ObjectGet<Room>(_room.Id)).ReturnsAsync(_room);
            _store.Setup(s => s.CreateLock(_room.Id)).ReturnsAsync(_lock.Object);
            
            _event = new ExecutionJobResult
            {
                JobId = "job123",
                RoomId = _room.Id,
                Error = false,
                Output = "Hello World!",
                ExecutionTime = 0.5f
            };
            
            _handler = new ExecutionJobCompletedEventHandler(_store.Object, _roomService.Object);
        }
        
        [Fact]
        public async Task Handle()
        {
            _room.CurrentExecutionJobId = _event.JobId;

            await _handler.Handle(_event);
            
            _room.CurrentExecutionJobId.Should().BeNull();
            _room.Output.Should().Contain(_event.Output);

            _store.Verify(s => s.ObjectSet(_room.Id, _room));
            
            _roomService.Verify(r => r.NotifyCodeExecutionCompleted(_room));
        }

        [Fact]
        public async Task Handle_JobNotActive()
        {
            await _handler.Handle(_event);

            _room.Output.Should().BeEmpty();
            
            _roomService.Verify(r => r.NotifyCodeExecutionCompleted(It.IsAny<Room>()), Times.Never());
        }
    }
}