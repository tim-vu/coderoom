using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Application.Rooms;
using Microsoft.AspNetCore.SignalR.Client;
using Xunit;

namespace WebUI.Integration.Tests.Common
{
    public class RoomTestBase : IClassFixture<AppFixture>, IAsyncLifetime
    {
        protected readonly AppFixture AppFixture;

        protected HubConnection Connection1;
        protected HubConnection Connection2;
        
        public RoomTestBase(AppFixture appFixture)
        {
            AppFixture = appFixture;
        }
        protected async Task<HubConnection> StartConnection(string hubUrl)
        {
            var connection = new HubConnectionBuilder()
                .WithUrl(hubUrl)
                .Build();

            await connection.StartAsync();

            return connection;
        }


        public async Task InitializeAsync()
        {
            var roomHubUrl = AppFixture.GetCompleteServerUrl("/hubs/room");
            Connection1 = await StartConnection(roomHubUrl);
            Connection2 = await StartConnection(roomHubUrl);
        }

        public Task DisposeAsync()
        {
            return Task.WhenAll(Connection1.DisposeAsync(), Connection2.DisposeAsync());
        }
    }
}