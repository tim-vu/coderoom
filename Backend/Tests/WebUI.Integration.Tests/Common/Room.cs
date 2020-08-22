using System.Net.Http;
using System.Threading.Tasks;
using Application.Rooms;
using Microsoft.AspNetCore.SignalR.Client;
using WebUI.Hubs;

namespace WebUI.Integration.Tests.Common
{
    public static class Room
    {
        public static Task JoinRoom(this HubConnection connection, string roomId, string nickname)
        {
            return connection.InvokeAsync(nameof(RoomHub.JoinRoom), roomId, nickname);
        }

        public static Task UpdateText(this HubConnection connection, string text)
        {
            return connection.InvokeAsync(nameof(RoomHub.UpdateText), text);
        }
        
        public static async Task<string> CreateRoom(this AppFixture fixture)
        {
            var url = fixture.GetCompleteServerUrl("/api/room");
            
            using var client = new HttpClient();

            var response = await client.PostAsync(url, null);

            var room = Serializer.Deserialize<RoomVm>(await response.Content.ReadAsStringAsync());

            return room.Id;
        }
    }
}