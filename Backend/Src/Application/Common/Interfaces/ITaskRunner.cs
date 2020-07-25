using System;
using System.Threading.Tasks;

namespace Application.Common.Interfaces
{
    public interface ITaskRunner
    {
        Task Delay(TimeSpan timeSpan);
    }
}