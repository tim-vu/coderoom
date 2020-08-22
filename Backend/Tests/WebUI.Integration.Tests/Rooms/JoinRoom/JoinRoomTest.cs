using System;
using System.Threading.Tasks;
using Application.Rooms;
using Domain.Entities;
using FluentAssertions;
using Microsoft.AspNetCore.SignalR.Client;
using Moq;
using WebUI.Hubs;
using WebUI.Integration.Tests.Common;
using Xunit;

namespace WebUI.Integration.Tests.Rooms.JoinRoom
{
    public class JoinRoomTest : RoomTestBase
    {
        
        public JoinRoomTest(AppFixture appFixture) : base(appFixture)
        {
            
        }
        
        [Fact]
        public async void JoinRoom()
        {
            var roomId = await AppFixture.CreateRoom();

            var result = await Connection1.InvokeAsync<RoomVm>(nameof(RoomHub.JoinRoom), roomId, Data.Nickname1);

            result.Should().NotBeNull();
            result.Id.Should().Be(roomId);
            result.Users.Should().HaveCount(1);
            result.Users.Should()
                .ContainSingle(u => u.ConnectionId == Connection1.ConnectionId && u.NickName == Data.Nickname1);
        }

        [Fact]
        public async void JoinRoom_WithUsers()
        {
            var roomId = await AppFixture.CreateRoom();

            var onUserJoined = new Mock<Action<User>>();

            Connection1.On("OnUserJoined", onUserJoined.Object);
            
            await Connection1.JoinRoom(roomId, Data.Nickname1);

            var result = await Connection2.InvokeAsync<RoomVm>(nameof(RoomHub.JoinRoom), roomId, Data.Nickname2);

            result.Should().NotBeNull();
            result.Id.Should().Be(roomId);
            result.Users.Should().HaveCount(2);
            result.Users.Should()
                .ContainSingle(u => u.ConnectionId == Connection1.ConnectionId && u.NickName == Data.Nickname1);
            result.Users.Should()
                .ContainSingle(u => u.ConnectionId == Connection2.ConnectionId && u.NickName == Data.Nickname2);

            await onUserJoined.VerifyWithTimeout(c =>
                c.Invoke(It.Is<User>(u => u.ConnectionId == Connection2.ConnectionId && u.NickName == Data.Nickname2)), Times.Once(), 2000);
        }

    }
}