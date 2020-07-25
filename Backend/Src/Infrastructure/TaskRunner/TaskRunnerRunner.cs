using System;
using System.Threading.Tasks;
using Application.Common.Interfaces;

namespace Infrastructure.TaskRunner
{
    public class TaskRunnerRunner : ITaskRunner
    {
        public Task Run(Action action)
        {
            return Task.Run(action);
        }

        public Task Delay(TimeSpan timeSpan)
        {
            return Task.Delay(timeSpan);
        }
    }
}