using System.Net.Http;
using Application.Rooms;
using Domain.Enums;
using FluentAssertions;
using WebUI.Integration.Tests.Common;
using Xunit;

namespace WebUI.Integration.Tests.Rooms.CreateRoom
{
    public class CreateRoomTest : IClassFixture<AppFixture>
    {
        private readonly AppFixture _fixture;
        
        public CreateRoomTest(AppFixture fixture) 
        {
            _fixture = fixture;
        }

        [Fact]
        public async void CreateRoom()
        {
            var url = _fixture.GetCompleteServerUrl("/api/room");

            using var httpClient = new HttpClient();

            var response = await httpClient.PostAsync(url, null);
            var body = Serializer.Deserialize<RoomVm>(await response.Content.ReadAsStringAsync());

            body.Id.Should().NotBeNullOrEmpty();
            body.Language.Should().Be(Language.Java);
            body.Users.Should().BeEmpty();
        }
    }
}