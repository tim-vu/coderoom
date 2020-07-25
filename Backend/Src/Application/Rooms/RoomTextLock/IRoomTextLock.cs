using System;
using System.Threading.Tasks;
using Domain.Entities;

namespace Application.Rooms.RoomTextLock
{
    public interface IRoomTextLock
    {
        bool CanWrite(Room room, string connectionId);
        
        void LockRoom(Room room, string connectionId);
        
        Task ExpireLock(string roomId, DateTime now);
    }
}