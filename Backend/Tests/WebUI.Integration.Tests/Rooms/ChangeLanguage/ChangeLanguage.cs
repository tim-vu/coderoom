using System;
using System.Threading.Tasks;
using Domain.Enums;
using Microsoft.AspNetCore.SignalR.Client;
using Moq;
using WebUI.Hubs;
using WebUI.Integration.Tests.Common;
using Xunit;

namespace WebUI.Integration.Tests.Rooms.ChangeLanguage
{
    public class ChangeLanguageTest : RoomTestBase
    {
        public ChangeLanguageTest(AppFixture appFixture) : base(appFixture)
        {
        }

        [Fact]
        public async void ChangeLanguage()
        {
            const Language language = Language.CSharp;

            var roomId = await AppFixture.CreateRoom();

            var onLanguageChanged = new Mock<Action<Language>>();

            Connection1.On("OnLanguageChanged", onLanguageChanged.Object);

            await Connection1.JoinRoom(roomId, Data.Nickname1);
            await Connection2.JoinRoom(roomId, Data.Nickname2);

            await Connection2.InvokeAsync(nameof(RoomHub.ChangeLanguage), language, false);

            await onLanguageChanged.VerifyWithTimeout(c => c.Invoke(language), Times.Once(), 2000);
        }

        [Fact]
        public async void ChangeLanguage_Reset()
        {
            const Language language = Language.CSharp;

            var roomId = await AppFixture.CreateRoom();

            var onLanguageChanged = new Mock<Action<Language>>();
            var onTextChanged1 = new Mock<Action<string>>();
            
            Connection1.On("OnLanguageChanged", onLanguageChanged.Object);
            Connection1.On("OnTextChanged", onTextChanged1.Object);

            var onTextChanged2 = new Mock<Action<string>>();
            Connection2.On("OnTextChanged", onTextChanged2.Object);
            
            await Connection1.JoinRoom(roomId, Data.Nickname1);
            await Connection2.JoinRoom(roomId, Data.Nickname2);

            await Connection2.InvokeAsync(nameof(RoomHub.ChangeLanguage), language, true);

            await Task.WhenAll(
                    onLanguageChanged.VerifyWithTimeout(c => c.Invoke(language), Times.Once(), 2000),
                    onTextChanged1.VerifyWithTimeout(c => c.Invoke(It.IsAny<string>()), Times.Once(), 2000),
                    onTextChanged2.VerifyWithTimeout(c => c.Invoke(It.IsAny<string>()), Times.Once(), 2000)
            );
        }
    }
}