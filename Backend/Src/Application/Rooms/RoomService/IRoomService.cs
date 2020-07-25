using System.Threading.Tasks;
using Domain.Entities;

namespace Application.Rooms.RoomService
{
    public interface IRoomService
    {
        Task NotifyTextChanged(Room room, string callerConnectionId);

        Task NotifyTypingUserChanged(Room room);

        Task NotifyUserJoined(Room room, User user);

        Task NotifyUserLeft(Room room, string userConnectionId);

        Task NotifyLanguageChanged(Room room, string callerConnectionId);

        Task NotifyCodeExecutionStarted(Room room, string callerConnectionId, int lines);
        
        Task NotifyCodeExecutionCompleted(Room room);

        Task NotifyUserJoinedGroupCall(Room room, string connectionId);
    }
}