using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Common.Interfaces
{
    public interface IHubContext
    {
        Task Send(List<string> connectionIds, string methodName, object arg1);
        Task Send(List<string> connectionIds, string methodName, object arg1, object arg2);
        Task Send(List<string> connectionIds, string methodName, object arg1, object arg2, object arg3);
        
    }
}