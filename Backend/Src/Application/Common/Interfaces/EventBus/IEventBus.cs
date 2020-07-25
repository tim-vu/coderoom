using Google.Protobuf;

namespace Application.Common.Interfaces.EventBus
{
    public interface IEventBus
    {
        void Publish<T>(IMessage<T> message) where T : IMessage<T>;

        void Subscribe<T, TH>() where T : IMessage<T> where TH : IEventHandler<T>;
    }
}