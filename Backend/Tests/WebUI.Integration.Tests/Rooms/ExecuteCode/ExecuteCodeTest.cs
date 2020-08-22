using System;
using System.Threading.Tasks;
using Domain.Enums;
using Microsoft.AspNetCore.SignalR.Client;
using Moq;
using WebUI.Hubs;
using WebUI.Integration.Tests.Common;
using Xunit;

namespace WebUI.Integration.Tests.Rooms.ExecuteCode
{
    public class ExecuteCodeTest : RoomTestBase
    {
        private const string Output = "Hello World!";
        
        private const string JavaCode = @"
            public class Solution {

                public static void main(String[] args){
                    System.out.println(""Hello World!""); 
                }

            }
        ";

        private const string CSharpCode = @"
            using System;

            public class Solution {

                public static void Main(string[] args){
                    Console.WriteLine(""Hello World!"");
                }

            }
        ";

        private const string Python3Code = @"
            print(""Hello World!"")
        ";
        
        public ExecuteCodeTest(AppFixture appFixture) : base(appFixture)
        {
        }
        
        [Theory]
        [InlineData(JavaCode, Language.Java)]
        [InlineData(CSharpCode, Language.CSharp)]
        [InlineData(Python3Code, Language.Python3)]
        public async void ExecuteCode(string code, Language language)
        {
            var roomId = await AppFixture.CreateRoom();

            var onCodeExecutionStarted = new Mock<Action<string>>();
            var onCodeExecutionCompleted = new Mock<Action<string>>();

            Connection1.On("OnCodeExecutionStarted", onCodeExecutionStarted.Object);
            Connection1.On("OnCodeExecutionCompleted", onCodeExecutionCompleted.Object);
            
            await Connection1.JoinRoom(roomId, Data.Nickname1);

            await Connection1.InvokeAsync(nameof(RoomHub.ChangeLanguage), language, false);

            await Connection1.InvokeAsync(nameof(RoomHub.UpdateText), code);
            await Connection1.InvokeAsync(nameof(RoomHub.StartCodeExecution));

            await Task.WhenAll(
                onCodeExecutionStarted.VerifyWithTimeout(c => c.Invoke(It.IsAny<string>()), Times.Once(), 2000),
                onCodeExecutionCompleted.VerifyWithTimeout(c => c.Invoke(Output),
                    Times.Once(), 10000));
        }
    }
}