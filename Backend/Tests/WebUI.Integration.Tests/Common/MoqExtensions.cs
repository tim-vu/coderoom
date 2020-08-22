using System;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Moq;

namespace WebUI.Integration.Tests.Common
{
    public static class MoqExtensions
    {
        public static async Task VerifyWithTimeout<T>(this Mock<T> mock, Expression<Action<T>> expression, Times times, int timeout)
            where T : class
        {
            var hasBeenExecuted = false;
            var hasTimedOut = false;
            var stopwatch = new Stopwatch();
            stopwatch.Start();
  
            while (!hasBeenExecuted && !hasTimedOut)
            {
                if (stopwatch.ElapsedMilliseconds > timeout)
                {
                    hasTimedOut = true;
                }
  
                try
                {
                    mock.Verify(expression, times);
                    hasBeenExecuted = true;
                }
                catch (Exception)
                {
                    // ignored
                }

                await Task.Delay(20);
            }
        }
    }
}