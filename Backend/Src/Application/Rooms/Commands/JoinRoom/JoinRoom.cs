using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Rooms.RoomNotifier;
using Application.Rooms.RoomService;
using AutoMapper;
using Domain.Entities;
using MediatR;

namespace Application.Rooms.Commands.JoinRoom
{
    public class JoinRoom : IRequest<RoomVm>
    {
        public string RoomId { get; }
        
        public string ConnectionId { get; }
        
        public string NickName { get; }

        public JoinRoom(string roomId, string connectionId, string nickName)
        {
            RoomId = roomId;
            ConnectionId = connectionId;
            NickName = nickName;
        }
        
        public class JoinRoomHandler : IRequestHandler<JoinRoom, RoomVm>
        {
            private readonly IMemoryStore _memoryStore;
            private readonly IRoomNotifier _roomNotifier ;
            private readonly IMapper _mapper;

            public JoinRoomHandler(IMemoryStore memoryStore, IRoomNotifier roomNotifier, IMapper mapper)
            {
                _memoryStore = memoryStore;
                _roomNotifier = roomNotifier;
                _mapper = mapper;
            }

            public async Task<RoomVm> Handle(JoinRoom request, CancellationToken cancellationToken)
            {
                using var @lock = await _memoryStore.CreateLock(request.RoomId);
                
                if (!@lock.IsAcquired)
                    return default;

                var room = await _memoryStore.ObjectGet<Room>(request.RoomId);

                if (room == default)
                {
                    throw new NotFoundException($"Room {request.RoomId}");
                }

                var existingUser = room.Users.FirstOrDefault(u => u.ConnectionId == request.ConnectionId);

                if (existingUser != null)
                {
                    _ = _roomNotifier.NotifyUserJoined(room, existingUser);
                    return _mapper.Map<RoomVm>(room);
                }

                var newUser = new User {ConnectionId = request.ConnectionId, NickName = request.NickName};
                room.Users.Add(newUser);

                await _memoryStore.ObjectSet(request.RoomId, room);

                _ = _roomNotifier.NotifyUserJoined(room, newUser);
                
                return _mapper.Map<RoomVm>(room);
            }
        }
    }
}