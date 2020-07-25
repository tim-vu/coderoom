using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Application.Rooms.Commands.CreateRoom;
using Application.Test.Common;
using Domain.Entities;
using FluentAssertions;
using Moq;
using Xunit;

namespace Application.Test.Rooms.Commands
{
    public class CreateRoomHandlerTest : TestBase
    {
        [Fact]
        public async Task Handle_CreateRoom()
        {
            const string roomId = "room123";

            var idGenerator = new Mock<IIdGenerator>();
            idGenerator.Setup(i => i.GenerateId()).Returns(roomId);

            var store = new Mock<IMemoryStore>();

            var createRoomHandler =
                new CreateRoom.CreateRoomHandler(store.Object, idGenerator.Object, Mapper);

            var result = await createRoomHandler.Handle(new CreateRoom(), CancellationToken.None);

            idGenerator.Verify(i => i.GenerateId());
            store.Verify(s => s.ObjectSet(roomId, It.IsAny<Room>()));
            
            result.Should().NotBeNull();
            result.Id.Should().Be(roomId);
            result.Text.Should().BeEmpty();
            result.Users.Should().BeEmpty();
            result.LastEdit.Should().Be(DateTime.MinValue);
            result.TypingUserConnectionId.Should().BeNull();
        }
    }
}