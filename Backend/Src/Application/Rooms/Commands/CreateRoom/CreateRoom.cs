using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Application.Rooms.RoomService;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;
using MediatR;

namespace Application.Rooms.Commands.CreateRoom
{
    public class CreateRoom : IRequest<RoomVm>
    {
        public class CreateRoomHandler : IRequestHandler<CreateRoom, RoomVm>
        {
            private static readonly TimeSpan RoomExpire = TimeSpan.FromHours(12);
            private const Language DefaultLanguage = Language.Java;
            
            private readonly IMemoryStore _store;
            private readonly ITemplateService _templateService;
            private readonly IIdGenerator _generator;
            private readonly IMapper _mapper;

            public CreateRoomHandler(IMemoryStore store, IIdGenerator generator, ITemplateService templateService, IMapper mapper)
            {
                _store = store;
                _generator = generator;
                _mapper = mapper;
                _templateService = templateService;
            }

            public async Task<RoomVm> Handle(CreateRoom request, CancellationToken cancellationToken)
            {
                
                
                var room = new Room
                {
                    Id = _generator.GenerateId(),
                    Language = DefaultLanguage,
                    Text = _templateService.GetLanguageTemplate(DefaultLanguage)
                };

                await _store.ObjectSet(room.Id, room);
                await _store.Expire(room.Id, RoomExpire);
                
                return _mapper.Map<RoomVm>(room);
            }
        }
    }
}