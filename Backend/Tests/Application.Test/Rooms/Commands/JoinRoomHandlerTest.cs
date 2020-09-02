using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Rooms.Commands.JoinRoom;
using Application.Rooms.RoomService;
using Application.Test.Common;
using Domain.Entities;
using FluentAssertions;
using Moq;
using Xunit;

namespace Application.Test.Rooms.Commands
{
    public class JoinRoomHandlerTest : TestBase
    {
        private readonly Mock<ILock> _lock = new Mock<ILock>();
        private readonly Mock<IMemoryStore> _store = new Mock<IMemoryStore>();
        private readonly Room _room = BogusData.Room.Generate();
        private readonly Mock<IRoomNotifier> _roomService = new Mock<IRoomNotifier>();

        public JoinRoomHandlerTest()
        {
            _lock.Setup(l => l.IsAcquired).Returns(true);
            _store.Setup(s => s.CreateLock(_room.Id)).ReturnsAsync(_lock.Object);
        }

        [Fact]
        public async Task Handle_JoinRoom_Existing()
        {
            var user = BogusData.User.Generate();

            _store.Setup(s => s.ObjectGet<Room>(_room.Id)).ReturnsAsync(_room);

            var joinRoomHandler = new JoinRoom.JoinRoomHandler(_store.Object, _roomService.Object, Mapper);

            var result = await joinRoomHandler.Handle(new JoinRoom(_room.Id, user.ConnectionId, user.NickName), CancellationToken.None);

            result.Should().NotBeNull();
            result.Id.Should().Be(_room.Id);
            
            result.Users.Where(u => u.ConnectionId == user.ConnectionId && u.NickName == user.NickName).Should().ContainSingle();
            
            _store.Verify(s => s.ObjectSet(_room.Id, _room));
            _store.Verify(s => s.CreateLock(_room.Id));
            
            _lock.Verify(l => l.IsAcquired);
            _lock.Verify(l => l.Dispose());
            
            _roomService.Verify(r => r.NotifyUserJoined(_room, It.Is<User>(u => u.ConnectionId == user.ConnectionId && u.NickName == user.NickName)));
        }

        [Fact]
        public void Handle_JoinRoom_NonExisting()
        {
            var user = BogusData.User.Generate();

            var joinRoomHandler = new JoinRoom.JoinRoomHandler(_store.Object, _roomService.Object, Mapper);

            Func<Task> act = async () =>
                await joinRoomHandler.Handle(new JoinRoom(_room.Id, user.ConnectionId, user.NickName), CancellationToken.None);

            act.Should().Throw<NotFoundException>();
        }

        [Fact]
        public async Task Handle_Rejoin()
        {
            var user = BogusData.User.Generate();

            _room.Users.Add(user);
            
            _store.Setup(s => s.ObjectGet<Room>(_room.Id)).ReturnsAsync(_room);
            
            var joinRoomHandler = new JoinRoom.JoinRoomHandler(_store.Object, _roomService.Object, Mapper);

            var result = await joinRoomHandler.Handle(new JoinRoom(_room.Id, user.ConnectionId, user.NickName),
                CancellationToken.None);
            
            result.Should().NotBeNull();
            result.Id.Should().Be(_room.Id);
            
            result.Users.Where(u => u.ConnectionId == user.ConnectionId && u.NickName == user.NickName).Should().ContainSingle();
            
            _store.Verify(s => s.ObjectSet(_room.Id, _room), Times.Never());
            _roomService.Verify(s => s.NotifyUserJoined(_room, user));
        }
    }
}