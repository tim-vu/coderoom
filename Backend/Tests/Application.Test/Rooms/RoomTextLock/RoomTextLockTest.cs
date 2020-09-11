using System;
using System.Linq;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Application.Common.Interfaces.MemoryStore;
using Application.Rooms.RoomNotifier;
using Application.Rooms.RoomService;
using Application.Test.Common;
using Domain.Entities;
using FluentAssertions;
using Moq;
using Xunit;

namespace Application.Test.Rooms.RoomTextLock
{
    public class RoomTextLockTest
    {
        private readonly Mock<ITaskRunner> _taskMock = new Mock<ITaskRunner>();
        private readonly Mock<IRoomNotifier> _roomService = new Mock<IRoomNotifier>();
        private readonly Mock<ILock> _lock = new Mock<ILock>();
        private readonly Mock<IMemoryStore> _memoryStore = new Mock<IMemoryStore>();
        private readonly Room _room = BogusData.Room.Generate();

        public RoomTextLockTest()
        {
            _taskMock.Setup(t => t.Delay(It.IsAny<TimeSpan>())).Returns(Task.CompletedTask);
            _lock.Setup(l => l.IsAcquired).Returns(true);
            _memoryStore.Setup(m => m.ObjectGet<Room>(_room.Id)).ReturnsAsync(_room);
            _memoryStore.Setup(s => s.CreateLock(_room.Id)).ReturnsAsync(_lock.Object);
        }


        [Fact]
        public async Task ExpireLock()
        {
            _room.TypingUserConnectionId = _room.Users.First().ConnectionId;

            var now = DateTime.UtcNow;
            var dateTime = new Mock<IDateTime>();
            dateTime.Setup(d => d.UtcNow).Returns(now);

            _room.LastEdit = now;
            
            var textLock = new Application.Rooms.RoomTextLock.RoomTextLock(_memoryStore.Object, _roomService.Object, dateTime.Object, _taskMock.Object);

            await textLock.ExpireLock(_room.Id, now);

            _room.TypingUserConnectionId.Should().BeNull();
            
            _memoryStore.Verify(m => m.ObjectSet(_room.Id, _room));
            _roomService.Verify(r => r.NotifyTypingUserChanged(_room));
        }

        [Fact]
        public async Task ExpireLock_UserTyped()
        {
            var typingUser = _room.Users.First();
            _room.TypingUserConnectionId = typingUser.ConnectionId;
            
            var now = DateTime.UtcNow;
            var later = now.AddSeconds(5);

            var dateTime = new Mock<IDateTime>();
            dateTime.Setup(d => d.UtcNow).Returns(now);

            _room.LastEdit = later;
            
            var textLock = new Application.Rooms.RoomTextLock.RoomTextLock(_memoryStore.Object, _roomService.Object, dateTime.Object, _taskMock.Object);

            await textLock.ExpireLock(_room.Id, now);

            _room.TypingUserConnectionId.Should().Be(typingUser.ConnectionId);
        }
    }
}