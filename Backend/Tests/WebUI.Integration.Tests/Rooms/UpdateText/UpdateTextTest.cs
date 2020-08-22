using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using Moq;
using WebUI.Hubs;
using WebUI.Integration.Tests.Common;
using Xunit;

namespace WebUI.Integration.Tests.Rooms.UpdateText
{
    public class UpdateTextTest : RoomTestBase
    {
        public UpdateTextTest(AppFixture appFixture) : base(appFixture)
        {
        }

        [Fact]
        public async void UpdateText()
        {
            const string text = "Hello World!";

            var roomId = await AppFixture.CreateRoom();

            var onTextChanged = new Mock<Action<string>>();
            var onTypingUserChanged = new Mock<Action<string>>();

            Connection1.On("OnTextChanged", onTextChanged.Object);
            Connection1.On("OnTypingUserChanged", onTypingUserChanged.Object);
            
            await Connection1.JoinRoom(roomId, Data.Nickname1);
            await Connection2.JoinRoom(roomId, Data.Nickname2);

            await Connection2.InvokeAsync(nameof(RoomHub.UpdateText), text);

            await Task.WhenAll(
                onTextChanged.VerifyWithTimeout(c => c.Invoke(It.Is<string>(s => s == text)), Times.Once(), 2000),
                onTypingUserChanged.VerifyWithTimeout(c => c.Invoke(It.Is<string>(s => s == Connection1.ConnectionId)),
                    Times.Once(), 2000),
                onTypingUserChanged.VerifyWithTimeout(c =>
                    c.Invoke(It.Is<string>(s => s == null)), Times.Once(), 5000));
        }

        [Fact]
        public async void UpdateText_Locked()
        {
            const string text1 = "Hello Alice";
            const string text2 = "Hello Bob";
            
            var roomId = await AppFixture.CreateRoom();

            var onTextChanged1 = new Mock<Action<string>>();
            Connection1.On("OnTextChanged", onTextChanged1.Object);

            var onTextChanged2 = new Mock<Action<string>>();
            Connection2.On("OnTextChanged", onTextChanged2.Object);
            
            await Connection1.JoinRoom(roomId, Data.Nickname1);
            await Connection2.JoinRoom(roomId, Data.Nickname2);

            await Connection2.UpdateText(text2);
            await Connection1.UpdateText(text1);

            await Task.WhenAll(
                onTextChanged1.VerifyWithTimeout(c => c.Invoke(text2), Times.Exactly(2), 2000),
                onTextChanged2.VerifyWithTimeout(c => c.Invoke(It.IsAny<string>()), Times.Never(), 2000)
            );
        }
    }
}