using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using Moq;
using WebUI.Hubs;
using WebUI.Integration.Tests.Common;
using Xunit;

namespace WebUI.Integration.Tests.Rooms.LeaveRoom
{
    public class LeaveRoomTest : RoomTestBase
    {
        
        public LeaveRoomTest(AppFixture appFixture) : base(appFixture)
        {
        }
        
        [Fact]
        public async void LeaveRoom()
        {
            var roomId = await AppFixture.CreateRoom();

            var onUserLeft = new Mock<Action<string>>();

            Connection1.On("OnUserLeft", onUserLeft.Object);

            await Connection1.JoinRoom(roomId, Data.Nickname1);
            await Connection2.JoinRoom(roomId, Data.Nickname2);

            await Connection2.InvokeAsync(nameof(RoomHub.LeaveRoom));

            await onUserLeft.VerifyWithTimeout(c => c.Invoke(Connection2.ConnectionId), Times.Once(), 2000);
        }

        [Fact]
        public async void LeaveRoom_Typing()
        {
            const string text = "Hello Bob";
            
            var roomId = await AppFixture.CreateRoom();
            
            var onUserLeft = new Mock<Action<string>>();
            var onTypingUserChanged = new Mock<Action<string>>();

            Connection1.On("OnUserLeft", onUserLeft.Object);
            Connection1.On("OnTypingUserChanged", onTypingUserChanged.Object);

            await Connection1.JoinRoom(roomId, Data.Nickname1);
            await Connection2.JoinRoom(roomId, Data.Nickname2);

            await Connection2.InvokeAsync(nameof(RoomHub.UpdateText), text);
            await Connection2.InvokeAsync(nameof(RoomHub.LeaveRoom));

            await Task.WhenAll(
                onUserLeft.VerifyWithTimeout(c => c.Invoke(Connection2.ConnectionId), Times.Once(), 2000),
                onTypingUserChanged.VerifyWithTimeout(c => c.Invoke(null), Times.Once(), 2000)
            );

        }

    }
}