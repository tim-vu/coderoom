using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using AutoMapper;
using Domain.Entities;
using MediatR;

namespace Application.Rooms.Commands.CreateRoom
{
    public class CreateRoom : IRequest<RoomVm>
    {
        public class CreateRoomHandler : IRequestHandler<CreateRoom, RoomVm>
        {
            private static readonly TimeSpan RoomExpire = TimeSpan.FromHours(12);
            
            private readonly IMemoryStore _store;
            private readonly IIdGenerator _generator;
            private readonly IMapper _mapper;

            public CreateRoomHandler(IMemoryStore store, IIdGenerator generator, IMapper mapper)
            {
                _store = store;
                _generator = generator;
                _mapper = mapper;
            }

            public async Task<RoomVm> Handle(CreateRoom request, CancellationToken cancellationToken)
            {
                var room = new Room
                {
                    Id = _generator.GenerateId(),
                    Text = string.Empty
                };

                await _store.ObjectSet(room.Id, room);
                await _store.Expire(room.Id, RoomExpire);
                
                return _mapper.Map<RoomVm>(room);
            }
        }
    }
}