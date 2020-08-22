using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using Moq;
using WebUI.Hubs;
using WebUI.Integration.Tests.Common;
using Xunit;

namespace WebUI.Integration.Tests.Rooms.ChangeLanguage
{
    public class UpdateTextRoomTest : RoomTestBase
    {
        public UpdateTextRoomTest(AppFixture appFixture) : base(appFixture)
        {
        }

        [Fact]
        public async void UpdateText()
        {
            const string text = "Hello World";
            
            var roomId = await AppFixture.CreateRoom();

            var onTextChanged = new Mock<Action<string>>();
            var onTypingUserChanged = new Mock<Action<string>>();

            Connection1.On("OnTextChanged", onTextChanged.Object);
            Connection1.On("OnTypingUserChanged", onTypingUserChanged.Object);
            
            await Connection1.JoinRoom(roomId, Data.Nickname1);
            await Connection2.JoinRoom(roomId, Data.Nickname2);

            await Connection2.InvokeAsync(nameof(RoomHub.UpdateText), text);

        } 
    }
}