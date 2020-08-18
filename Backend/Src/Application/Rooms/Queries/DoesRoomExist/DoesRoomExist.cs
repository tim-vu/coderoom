using System.Threading;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using MediatR;

namespace Application.Rooms.Queries.DoesRoomExist
{
    public class DoesRoomExist : IRequest<bool>
    {
        private string Key { get; }

        public DoesRoomExist(string key)
        {
            Key = key;
        }
        
        public class DoesRoomExistHandler : IRequestHandler<DoesRoomExist, bool>
        {
            private readonly IMemoryStore _memoryStore;

            public DoesRoomExistHandler(IMemoryStore memoryStore)
            {
                _memoryStore = memoryStore;
            }

            public Task<bool> Handle(DoesRoomExist request, CancellationToken cancellationToken)
            {
                return _memoryStore.KeyExists(request.Key);
            }
        }
    }
}