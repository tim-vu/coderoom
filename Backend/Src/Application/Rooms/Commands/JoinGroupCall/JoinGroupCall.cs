using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Rooms.RoomService;
using Domain.Entities;
using MediatR;

namespace Application.Rooms.Commands.JoinGroupCall
{
    public class JoinGroupCall : IRequest
    {
        public string RoomId { get; }
        
        public string ConnectionId { get; }

        public JoinGroupCall(string roomId, string connectionId)
        {
            RoomId = roomId;
            ConnectionId = connectionId;
        }

        public class JoinGroupCallHandler : IRequestHandler<JoinGroupCall, Unit>
        {
            private readonly IMemoryStore _store;
            private readonly IRoomService _roomService;

            public JoinGroupCallHandler(IMemoryStore store, IRoomService roomService)
            {
                _store = store;
                _roomService = roomService;
            }

            public async Task<Unit> Handle(JoinGroupCall request, CancellationToken cancellationToken)
            {
                using var @lock = await _store.CreateLock(request.RoomId);

                if (!@lock.IsAcquired)
                    return Unit.Value;

                var room = await _store.ObjectGet<Room>(request.RoomId);

                if (room == default)
                {
                    throw new NotFoundException($"Room {request.RoomId}");
                }

                var user = room.Users.FirstOrDefault(u => u.ConnectionId == request.ConnectionId);
                if (user == default)
                {
                    throw new IllegalOperationException("Cannot join the group chat if the user is not in the room");
                }

                user.InGroupCall = true;

                await _store.ObjectSet(request.RoomId, room);

                _ = _roomService.NotifyUserJoinedGroupCall(room, request.ConnectionId);
                
                return Unit.Value;
            }
        }
    }
}