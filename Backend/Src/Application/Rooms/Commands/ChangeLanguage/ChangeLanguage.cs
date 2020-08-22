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
        
        public bool Reset { get; } 

        public ChangeLanguage(string roomId, string connectionId, Language language, bool reset)
        {
            RoomId = roomId;
            ConnectionId = connectionId;
            Language = language;
            Reset = reset;
        }
        
        public class ChangeLanguageHandler : IRequestHandler<ChangeLanguage, Unit>
        {
            private readonly IMemoryStore _store;
            private readonly IRoomNotifier _roomNotifier;
            private readonly ITemplateService _templateService;

            public ChangeLanguageHandler(IMemoryStore store, IRoomNotifier roomNotifier, ITemplateService templateService)
            {
                _store = store;
                _roomNotifier = roomNotifier;
                _templateService = templateService;
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

                    if (request.Reset)
                        room.Text = _templateService.GetLanguageTemplate(request.Language);

                    await _store.ObjectSet(request.RoomId, room);
                }

                if (request.Reset)
                {
                    _ = _roomNotifier.NotifyTextChanged(room, string.Empty);
                }
                
                _ = _roomNotifier.NotifyLanguageChanged(room, request.ConnectionId);
                return Unit.Value;
            }
        }
    }
}