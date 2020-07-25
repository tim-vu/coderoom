using System;
using System.Linq;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Application.Rooms.RoomService;
using Domain.Entities;

namespace Application.Rooms.RoomTextLock
{
    public class RoomTextLock : IRoomTextLock
    {
        private static readonly TimeSpan LockDuration = TimeSpan.FromSeconds(1);
        
        private readonly IMemoryStore _memoryStore;
        private readonly IRoomService _roomService;
        private readonly IDateTime _dateTime;
        private readonly ITaskRunner _taskRunner;

        public RoomTextLock(IMemoryStore memoryStore, IRoomService roomService, IDateTime dateTime, ITaskRunner taskRunner)
        {
            _memoryStore = memoryStore;
            _roomService = roomService;
            _dateTime = dateTime;
            _taskRunner = taskRunner;
        }

        public bool CanWrite(Room room, string connectionId)
        {
            return _dateTime.UtcNow.Subtract(room.LastEdit) > LockDuration ||
                   room.TypingUserConnectionId == connectionId;
        }

        public void LockRoom(Room room, string connectionId)
        {
            room.TypingUserConnectionId = connectionId;
        }

        public Task ExpireLock(string roomId, DateTime now)
        {
            return _taskRunner.Delay(LockDuration).ContinueWith(async t =>
            {
                using var @lock = await _memoryStore.CreateLock(roomId);

                if (!@lock.IsAcquired)
                    return;
                
                var room = await _memoryStore.ObjectGet<Room>(roomId);

                if (room == default || room.LastEdit != now)
                    return;

                room.TypingUserConnectionId = null;

                await _memoryStore.ObjectSet(room.Id, room);

                _ = _roomService.NotifyTypingUserChanged(room);
            });
        }
    }
}