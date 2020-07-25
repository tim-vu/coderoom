using System.Threading.Tasks;
using Application.Common.Interfaces;
using Application.Common.Interfaces.EventBus;
using Application.Common.Protos;
using Application.Rooms.RoomService;
using Domain.Entities;

namespace Application.CodeExecution
{
    public class ExecutionJobCompletedEventHandler : IEventHandler<ExecutionJobResult>
    {
        private readonly IMemoryStore _store;
        private readonly IRoomService _roomService;

        public ExecutionJobCompletedEventHandler(IMemoryStore store, IRoomService roomService)
        {
            _store = store;
            _roomService = roomService;
        }

        public async Task Handle(ExecutionJobResult @event)
        {
            using var @lock = await _store.CreateLock(@event.RoomId);
            
            if (!@lock.IsAcquired)
                return;

            var room = await _store.ObjectGet<Room>(@event.RoomId);

            if (room.CurrentExecutionJobId != @event.JobId)
                return;
                
            room.CurrentExecutionJobId = null;
            room.Output.Add(@event.Error ? @event.ErrorMessage : @event.Output);

            await _store.ObjectSet(room.Id, room);

            _ = _roomService.NotifyCodeExecutionCompleted(room);
        }
    }
}