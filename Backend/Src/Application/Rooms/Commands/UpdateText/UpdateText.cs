using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Rooms.RoomService;
using Application.Rooms.RoomTextLock;
using Domain.Entities;
using MediatR;

namespace Application.Rooms.Commands.UpdateText
{
    public class UpdateText : IRequest
    {
        public string RoomId { get; }
        
        public string ConnectionId { get;  }
        
        public string Text { get; }

        public UpdateText(string roomId, string connectionId, string text)
        {
            RoomId = roomId;
            ConnectionId = connectionId;
            Text = text;
        }
        
        public class UpdateTextHandler : IRequestHandler<UpdateText>
        {
            private readonly IMemoryStore _memoryStore;
            private readonly IRoomService _roomService;
            private readonly IRoomTextLock _roomTextLock;
            private readonly IDateTime _dateTime;

            public UpdateTextHandler(IMemoryStore memoryStore, IRoomService roomService, IDateTime dateTime, IRoomTextLock roomTextLock)
            {
                _memoryStore = memoryStore;
                _dateTime = dateTime;
                _roomTextLock = roomTextLock;
                _roomService = roomService;
            }

            public async Task<Unit> Handle(UpdateText request, CancellationToken cancellationToken)
            {
                using var @lock = await _memoryStore.CreateLock(request.RoomId);
                
                if (!@lock.IsAcquired)
                    return Unit.Value;
                    
                var room = await _memoryStore.ObjectGet<Room>(request.RoomId);

                if (room is null)
                {
                    throw new NotFoundException($"Room {request.RoomId}");
                }
                
                if (room.Users.All(u => u.ConnectionId != request.ConnectionId))
                {
                    throw new IllegalOperationException("Unable to update text: user not in room");
                }
                
                //TODO: send actual text back to user to prevent inconsistencies
                if (!_roomTextLock.CanWrite(room, request.ConnectionId))
                {
                    return Unit.Value;
                }

                var now = _dateTime.UtcNow;
                
                room.Text = request.Text;
                room.LastEdit = now;

                var typingUserChanged = room.TypingUserConnectionId != request.ConnectionId;
                
                _roomTextLock.LockRoom(room, request.ConnectionId);
                _ = _roomTextLock.ExpireLock(room.Id, now);
                
                await _memoryStore.ObjectSet(request.RoomId, room);
                    
                _ = _roomService.NotifyTextChanged(room, request.ConnectionId);
                
                if(typingUserChanged)
                    _ = _roomService.NotifyTypingUserChanged(room);
                
                return Unit.Value;
            }
        }
    }
}