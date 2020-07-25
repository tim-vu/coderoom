using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace WebUI.Hubs
{
    public class HubContext : IHubContext
    {
        private readonly IHubContext<RoomHub> _context;

        public HubContext(IHubContext<RoomHub> context)
        {
            _context = context;
        }

        public Task Send(List<string> connectionIds, string methodName, object arg1)
        {
            return _context.Clients.Clients(connectionIds).SendAsync(methodName, arg1);
        }

        public Task Send(List<string> connectionIds, string methodName, object arg1, object arg2)
        {
            return _context.Clients.Clients(connectionIds).SendAsync(methodName, arg1, arg2);
        }

        public Task Send(List<string> connectionIds, string methodName, object arg1, object arg2, object arg3)
        {
            return _context.Clients.Clients(connectionIds).SendAsync(methodName, arg1, arg2, arg3);
        }
    }
}