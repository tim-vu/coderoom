using System.Threading.Tasks;
using Google.Protobuf;

namespace Application.Common.Interfaces.EventBus
{
    public interface IEventHandler<in T> where T : IMessage<T>
    {
        Task Handle(T @event);
    }
}