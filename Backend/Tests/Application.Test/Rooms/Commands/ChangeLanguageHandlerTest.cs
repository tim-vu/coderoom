using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Common.Interfaces.MemoryStore;
using Application.Rooms.Commands.ChangeLanguage;
using Application.Rooms.RoomNotifier;
using Application.Rooms.RoomService;
using Application.Test.Common;
using Domain.Entities;
using Domain.Enums;
using FluentAssertions;
using Moq;
using Xunit;

namespace Application.Test.Rooms.Commands
{
    public class ChangeLanguageHandlerTest : TestBase
    {
        private const Language Language = Domain.Enums.Language.CSharp;
        private readonly Mock<ILock> _lock = new Mock<ILock>();
        private readonly Mock<IMemoryStore> _store = new Mock<IMemoryStore>();
        private readonly Room _room = BogusData.Room.Generate();
        private readonly Mock<IRoomNotifier> _roomService = new Mock<IRoomNotifier>();
        private readonly Mock<ITemplateService> _templateService = new Mock<ITemplateService>();

        public ChangeLanguageHandlerTest()
        {
            _lock.Setup(l => l.IsAcquired).Returns(true);
            _store.Setup(s => s.CreateLock(_room.Id)).ReturnsAsync(_lock.Object);
            _store.Setup(s => s.ObjectGet<Room>(_room.Id)).ReturnsAsync(_room);
        }

        [Fact]
        public async void ChangeLanguage()
        {
            var user = BogusData.User.Generate();
            _room.Users.Add(user);

            
            
            var changeLanguageHandler = new ChangeLanguage.ChangeLanguageHandler(_store.Object, _roomService.Object, _templateService.Object);

            await changeLanguageHandler.Handle(new ChangeLanguage(_room.Id, user.ConnectionId, Language, false),
                CancellationToken.None);

            _room.Language.Should().Be(Language);
            
            _store.Verify(s => s.ObjectSet(_room.Id, _room));
            _roomService.Verify(s => s.NotifyLanguageChanged(_room, user.ConnectionId));
        }

        [Fact]
        public async void ChangeLanguage_Reset()
        {
            const string template = "Hello There";
            
            var user = BogusData.User.Generate();
            _room.Users.Add(user);

            _templateService.Setup(t => t.GetLanguageTemplate(Language)).Returns(template);
            
            var changeLanguageHandler = new ChangeLanguage.ChangeLanguageHandler(_store.Object, _roomService.Object, _templateService.Object);

            await changeLanguageHandler.Handle(new ChangeLanguage(_room.Id, user.ConnectionId, Language, true),
                CancellationToken.None);

            _room.Language.Should().Be(Language);
            
            _store.Verify(s => s.ObjectSet(_room.Id, _room));
            _roomService.Verify(s => s.NotifyLanguageChanged(_room, user.ConnectionId));
            _roomService.Verify(s => s.NotifyTextChanged(_room, string.Empty));
        }

        [Fact]
        public void ChangeLanguage_NonExisting()
        {
            var room = BogusData.Room.Generate();
            var user = BogusData.User.Generate();

            _store.Setup(s => s.CreateLock(room.Id)).ReturnsAsync(_lock.Object);
            
            var changeLanguageHandler = new ChangeLanguage.ChangeLanguageHandler(_store.Object, _roomService.Object, _templateService.Object);

            Func<Task> act = async () =>
                await changeLanguageHandler.Handle(new ChangeLanguage(room.Id, user.ConnectionId, Language, false),
                    CancellationToken.None);

            act.Should().Throw<NotFoundException>();
        }

        [Fact]
        public void ChangeLanguage_UserNotInRoom()
        {
            var user = BogusData.User.Generate();
            
            var changeLanguageHandler = new ChangeLanguage.ChangeLanguageHandler(_store.Object, _roomService.Object, _templateService.Object);

            Func<Task> act = async () =>
                await changeLanguageHandler.Handle(new ChangeLanguage(_room.Id, user.ConnectionId, Language, false),
                    CancellationToken.None);

            act.Should().Throw<IllegalOperationException>();
        }
    }
}