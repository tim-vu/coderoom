using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Rooms.Commands.LeaveRoom;
using Application.Rooms.RoomService;
using Application.Test.Common;
using Domain.Entities;
using FluentAssertions;
using Moq;
using Xunit;

namespace Application.Test.Rooms.Commands
{
    public class LeaveRoomHandlerTest
    {
        private static readonly User User = BogusData.User.Generate();

        [Fact]
        public async Task Handle_LeaveRoom_Existing()
        {
            var room = BogusData.Room.Generate();
            room.Users.Add(User);

            var userCount = room.Users.Count;

            var @lock = new Mock<ILock>();
            @lock.Setup(l => l.IsAcquired).Returns(true);
            
            var store = new Mock<IMemoryStore>();
            store.Setup(s => s.ObjectGet<Room>(room.Id)).ReturnsAsync(room);
            store.Setup(s => s.CreateLock(room.Id)).ReturnsAsync(@lock.Object);

            var roomService = new Mock<IRoomService>();
            
            var leaveRoomHandler = new LeaveRoom.LeaveRoomHandler(store.Object, roomService.Object);

            await leaveRoomHandler.Handle(new LeaveRoom(room.Id, User.ConnectionId), CancellationToken.None);
            
            store.Verify(s => s.ObjectGet<Room>(room.Id));
            store.Verify(s => s.CreateLock(room.Id));
            store.Verify(s => s.ObjectSet(room.Id, It.Is<Room>(r => r.Id == room.Id && r.Users.Count == userCount - 1)));
            
            @lock.Verify(l => l.IsAcquired);
            @lock.Verify(l => l.Dispose());
            
            roomService.Verify(r => r.NotifyUserLeft(room, User.ConnectionId));
        }

        [Fact]
        public async Task Handle_LeaveRoom_Existing_UserTyping()
        {
            var room = BogusData.Room.Generate();
            room.Users.Add(User);
            room.TypingUserConnectionId = User.ConnectionId;

            var userCount = room.Users.Count;

            var @lock = new Mock<ILock>();
            @lock.Setup(l => l.IsAcquired).Returns(true);
            
            var store = new Mock<IMemoryStore>();
            store.Setup(s => s.ObjectGet<Room>(room.Id)).ReturnsAsync(room);
            store.Setup(s => s.CreateLock(room.Id)).ReturnsAsync(@lock.Object);

            var roomService = new Mock<IRoomService>();
            
            var leaveRoomHandler = new LeaveRoom.LeaveRoomHandler(store.Object, roomService.Object);

            await leaveRoomHandler.Handle(new LeaveRoom(room.Id, User.ConnectionId), CancellationToken.None);
            
            store.Verify(s => s.ObjectGet<Room>(room.Id));
            store.Verify(s => s.CreateLock(room.Id));
            store.Verify(s => s.ObjectSet(room.Id, 
                It.Is<Room>(r => 
                    r.Id == room.Id && 
                    r.Users.Count == userCount - 1 &&
                    r.TypingUserConnectionId == null)));
            
            @lock.Verify(l => l.IsAcquired);
            @lock.Verify(l => l.Dispose());
            
            roomService.Verify(r => r.NotifyUserLeft(room, User.ConnectionId));
            roomService.Verify(r => r.NotifyTypingUserChanged(room));
        }

        [Fact]
        public void Handle_LeaveRoom_NonExisting()
        {
            var @lock = new Mock<ILock>();
            @lock.Setup(l => l.IsAcquired).Returns(true);
            
            var store = new Mock<IMemoryStore>();
            store.Setup(s => s.CreateLock(It.IsAny<string>())).ReturnsAsync(@lock.Object);

            var roomService = new Mock<IRoomService>();
            
            var leaveRoomHandler = new LeaveRoom.LeaveRoomHandler(store.Object, roomService.Object);

            Func<Task> act = async () =>
                await leaveRoomHandler.Handle(new LeaveRoom("room123", User.ConnectionId), CancellationToken.None);

            act.Should().Throw<NotFoundException>();
        }
    }
}