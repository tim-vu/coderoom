using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Rooms.Commands.UpdateText;
using Application.Rooms.RoomService;
using Application.Rooms.RoomTextLock;
using Application.Test.Common;
using Domain.Entities;
using FluentAssertions;
using Moq;
using Xunit;

namespace Application.Test.Rooms.Commands
{
    public class UpdateTextTest : TestBase
    {
        private const string Text = "Hello World!";
        private readonly Mock<ILock> _lock = new Mock<ILock>();
        private readonly Mock<IMemoryStore> _store = new Mock<IMemoryStore>();
        private readonly Room _room = BogusData.Room.Generate();
        private readonly Mock<IRoomNotifier> _roomService = new Mock<IRoomNotifier>();
        private readonly DateTime _now = DateTime.UtcNow;
        private readonly Mock<IDateTime> _dateTime = new Mock<IDateTime>();
        private readonly Mock<IRoomTextLock> _roomTextLock = new Mock<IRoomTextLock>();

        public UpdateTextTest()
        {
            _lock.Setup(l => l.IsAcquired).Returns(true);
            _store.Setup(s => s.CreateLock(_room.Id)).ReturnsAsync(_lock.Object);
            _dateTime.Setup(d => d.UtcNow).Returns(_now);
        }

        [Fact]
        public async void UpdateText()
        {
            const string text = "Hello World!";
            
            var user = BogusData.User.Generate();
            _room.Users.Add(user);

            _store.Setup(s => s.ObjectGet<Room>(_room.Id)).ReturnsAsync(_room);

            var roomTextLock = new Mock<IRoomTextLock>();
            roomTextLock.Setup(l => l.CanWrite(_room, user.ConnectionId)).Returns(true);
            
            var updateTextHandler = new UpdateText.UpdateTextHandler(_store.Object, _roomService.Object, _dateTime.Object, roomTextLock.Object);

            await updateTextHandler.Handle(new UpdateText(_room.Id, user.ConnectionId, text), CancellationToken.None);

            _room.Text.Should().Be(text);
            _room.LastEdit.Should().Be(_now);

            roomTextLock.Verify(l => l.LockRoom(_room, user.ConnectionId));
            roomTextLock.Verify(l => l.ExpireLock(_room.Id, _now));
            
            _roomService.Verify(r => r.NotifyTextChanged(_room, user.ConnectionId));
            _roomService.Verify(r => r.NotifyTypingUserChanged(_room));
            
            _store.Verify(s => s.ObjectSet(_room.Id, _room));
        }

        [Fact]
        public async void UpdateText_Locked()
        {
            var initialText = _room.Text;
            
            var user = BogusData.User.Generate();
            _room.Users.Add(user);

            _store.Setup(s => s.ObjectGet<Room>(_room.Id)).ReturnsAsync(_room);

            _roomTextLock.Setup(l => l.CanWrite(_room, user.ConnectionId)).Returns(false);
            
            var updateTextHandler = new UpdateText.UpdateTextHandler(_store.Object, _roomService.Object, _dateTime.Object, _roomTextLock.Object);

            await updateTextHandler.Handle(new UpdateText(_room.Id, user.ConnectionId, Text), CancellationToken.None);

            _room.Text.Should().Be(initialText);
            
            _roomService.Verify(s => s.NotifySingleUserTextChanged(_room, user.ConnectionId));
        }

        [Fact]
        public void UpdateText_NonExisting()
        {
            const string roomId = "I_DONT_EXIST";

            //a lock can be created on non existing keys    
            _store.Setup(s => s.CreateLock(roomId)).ReturnsAsync(_lock.Object);
            
            var user = BogusData.User.Generate();
            
            var updateTextHandler = new UpdateText.UpdateTextHandler(_store.Object, _roomService.Object, _dateTime.Object, _roomTextLock.Object);

            Func<Task> act = async () =>
                await updateTextHandler.Handle(new UpdateText(roomId, user.ConnectionId, Text), CancellationToken.None);

            act.Should().Throw<NotFoundException>();
        }

        [Fact]
        public void UpdateText_UserNotInRoom()
        {
            var user = BogusData.User.Generate();
            
            _store.Setup(s => s.ObjectGet<Room>(_room.Id)).ReturnsAsync(_room);

            var updateTextHandler = new UpdateText.UpdateTextHandler(_store.Object, _roomService.Object, _dateTime.Object, _roomTextLock.Object);

            Func<Task> act = async () =>
                await updateTextHandler.Handle(new UpdateText(_room.Id, user.ConnectionId, Text),
                    CancellationToken.None);

            act.Should().Throw<IllegalOperationException>();
        }
    }
}