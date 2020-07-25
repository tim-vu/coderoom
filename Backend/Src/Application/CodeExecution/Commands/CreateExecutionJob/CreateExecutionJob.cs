using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.CodeExecution.ExecutionJobService;
using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Rooms.RoomService;
using Domain.Entities;
using Domain.Enums;
using MediatR;

namespace Application.CodeExecution.Commands.CreateExecutionJob
{
    public class CreateExecutionJob : IRequest
    {
        public string RoomId { get; }
        
        public string ConnectionId { get; }

        public CreateExecutionJob(string connectionId, string roomId)
        {
            ConnectionId = connectionId;
            RoomId = roomId;
        }
        
        public class CreateExecutionJobHandler : IRequestHandler<CreateExecutionJob>
        {
            private readonly IMemoryStore _memoryStore;
            private readonly IExecutionJobService _executionJobService;
            private readonly IIdGenerator _idGenerator;
            private readonly IRoomService _roomService;
            
            public CreateExecutionJobHandler(IMemoryStore memoryStore, IIdGenerator idGenerator, IExecutionJobService executionJobService, IRoomService roomService)
            {
                _memoryStore = memoryStore;
                _idGenerator = idGenerator;
                _executionJobService = executionJobService;
                _roomService = roomService;
            }

            public async Task<Unit> Handle(CreateExecutionJob request, CancellationToken cancellationToken)
            {
                using var @lock = await _memoryStore.CreateLock(request.RoomId);

                if (!@lock.IsAcquired)
                    return Unit.Value;

                var room = await _memoryStore.ObjectGet<Room>(request.RoomId);

                if (room == default)
                {
                    throw new NotFoundException($"Room {request.RoomId}");
                }

                if (room.Users.All(u => u.ConnectionId != request.ConnectionId))
                {
                    throw new IllegalOperationException("Cannot execute the code if the user is not in the room");
                }

                // Handle scenario differently?
                if (room.CurrentExecutionJobId != default)
                {
                    return Unit.Value;
                }

                var id = _idGenerator.GenerateId();

                room.CurrentExecutionJobId = id;
                await _memoryStore.ObjectSet(room.Id, room);
                
                var sourceFiles = new List<SourceFile>
                {
                    new SourceFile
                    {
                        Content = room.Text,
                        Name = "Solution" + "." + room.Language.GetFileExtension()
                    }
                };

                await _roomService.NotifyCodeExecutionStarted(room, room.Users.First(u => u.ConnectionId == request.ConnectionId).NickName, room.Text.Split(Environment.NewLine).Length);
                
                _ = _executionJobService.StartJob(room.Id, id, room.Language, sourceFiles);
                _ = _executionJobService.TimeoutJob(room.Id, id);
                
                return Unit.Value;
            }
        }
    }
}