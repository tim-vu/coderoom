using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Entities;
using Domain.Enums;

namespace Application.Rooms.ExecutionJobService
{
    public interface IExecutionJobService
    {
        Task StartJob(string roomId, string id, Language language, List<SourceFile> sourceFiles);

        Task TimeoutJob(string roomId, string id);
    }
}