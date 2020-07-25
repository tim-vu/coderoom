using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Rooms.RoomService;
using Domain.Entities;
using Domain.Enums;
using MediatR;

namespace Application.Rooms.Commands.ChangeLanguage
{
    public class ChangeLanguage : IRequest
    {
        public string RoomId { get; }
        
        public string ConnectionId { get; }

        public Language Language { get; }

        public ChangeLanguage(string roomId, string connectionId, Language language)
        {
            RoomId = roomId;
            ConnectionId = connectionId;
            Language = language;
        }
        
        public class ChangeLanguageHandler : IRequestHandler<ChangeLanguage, Unit>
        {
            private readonly IMemoryStore _store;
            private readonly IRoomService _roomService;

            public ChangeLanguageHandler(IMemoryStore store, IRoomService roomService)
            {
                _store = store;
                _roomService = roomService;
            }

            public async Task<Unit> Handle(ChangeLanguage request, CancellationToken cancellationToken)
            {
                Room room;
                using (var @lock = await _store.CreateLock(request.RoomId))
                {
                    if (!@lock.IsAcquired)
                        return Unit.Value;

                    room = await _store.ObjectGet<Room>(request.RoomId);

                    if (room is null)
                    {
                        throw new NotFoundException($"Room {request.RoomId}");
                    }

                    if (room.Users.All(u => u.ConnectionId != request.ConnectionId))
                    {
                        throw new IllegalOperationException("Unable to change language: user not in room");
                    }

                    room.Language = request.Language;

                    await _store.ObjectSet(request.RoomId, room);
                }

                _ = _roomService.NotifyLanguageChanged(room, request.ConnectionId);
                return Unit.Value;
            }
        }
    }
}