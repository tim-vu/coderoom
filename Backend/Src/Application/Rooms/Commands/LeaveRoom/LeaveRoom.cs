using System.Threading;
using System.Threading.Tasks;
using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Rooms.RoomNotifier;
using Application.Rooms.RoomService;
using Domain.Entities;
using MediatR;

namespace Application.Rooms.Commands.LeaveRoom
{
    public class LeaveRoom : IRequest
    {
        public string RoomId { get; }
        
        public string ConnectionId { get; }

        public LeaveRoom(string roomId, string connectionId)
        {
            RoomId = roomId;
            ConnectionId = connectionId;
        }
        
        public class LeaveRoomHandler : IRequestHandler<LeaveRoom>
        {
            private readonly IMemoryStore _memoryStore;
            private readonly IRoomNotifier _roomNotifier;

            public LeaveRoomHandler(IMemoryStore memoryStore, IRoomNotifier roomNotifier)
            {
                _memoryStore = memoryStore;
                _roomNotifier = roomNotifier;
            }

            public async Task<Unit> Handle(LeaveRoom request, CancellationToken cancellationToken)
            {
                using var @lock = await _memoryStore.CreateLock(request.RoomId);
                
                if (!@lock.IsAcquired)
                    return default;

                var room = await _memoryStore.ObjectGet<Room>(request.RoomId);

                if (room == default(Room))
                {
                    throw new NotFoundException($"Room {request.RoomId}");
                }

                if(room.Users.RemoveAll(u => u.ConnectionId == request.ConnectionId) == 0)
                    return Unit.Value;

                if (room.TypingUserConnectionId == request.ConnectionId)
                {
                    room.TypingUserConnectionId = null;

                    await _memoryStore.ObjectSet(request.RoomId, room);

                    _ = _roomNotifier.NotifyTypingUserChanged(room);
                    _ = _roomNotifier.NotifyUserLeft(room, request.ConnectionId);
                    return Unit.Value;
                }
                
                await _memoryStore.ObjectSet(request.RoomId, room);

                _ = _roomNotifier.NotifyUserLeft(room, request.ConnectionId);
                
                return Unit.Value;
            }
        }
    }
}