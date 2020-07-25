using System;
using System.Linq;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Domain.Entities;
using Domain.Enums;

namespace Application.Rooms.RoomService
{
    public class RoomService : IRoomService
    {
        private readonly IHubContext _hubContext;

        public RoomService(IHubContext hubContext)
        {
            _hubContext = hubContext;
        }

        public Task NotifyTextChanged(Room room, string callerConnectionId)
        {
            var recipients = room.Users.Select(u => u.ConnectionId).Where(id => id != callerConnectionId).ToList();
            return _hubContext.Send(recipients, "OnTextChanged", room.Text);
        }

        public Task NotifyTypingUserChanged(Room room)
        {
            var recipients = room.Users.Select(u => u.ConnectionId).ToList();
            return _hubContext.Send(recipients, "OnTypingUserChanged", room.TypingUserConnectionId);
        }

        public Task NotifyUserJoined(Room room, User user)
        {
            var recipients = room.Users.Select(u => u.ConnectionId).Where(id => id != user.ConnectionId).ToList();
            return _hubContext.Send(recipients, "OnUserJoined", user);
        }

        public Task NotifyUserLeft(Room room, string userConnectionId)
        {
            var recipients = room.Users.Select(u => u.ConnectionId).ToList();
            return _hubContext.Send(recipients, "OnUserLeft", userConnectionId);
        }

        public Task NotifyLanguageChanged(Room room, string callerConnectionId)
        {
            var recipients = room.Users.Select(u => u.ConnectionId).Where(id => id != callerConnectionId).ToList();
            return _hubContext.Send(recipients, "OnLanguageChanged", room.Language);
        }

        public Task NotifyCodeExecutionStarted(Room room, string nickName, int lines)
        {
            var message = $"{nickName} is execution {lines} lines of {room.Language.GetName()}";
            return _hubContext.Send(room.Users.Select(u => u.ConnectionId).ToList(), "OnCodeExecutionStarted", message);
        }

        public Task NotifyCodeExecutionCompleted(Room room)
        {
            var recipients = room.Users.Select(u => u.ConnectionId).ToList();
            return _hubContext.Send(recipients, "OnCodeExecutionCompleted", room.Output.Last());
        }

        public Task NotifyUserJoinedGroupCall(Room room, string connectionId)
        {
            var recipients = room.Users.Select(u => u.ConnectionId).Where(c => c != connectionId).ToList();
            return _hubContext.Send(recipients, "OnUserJoinedGroupCall", connectionId);
        }
    }
}