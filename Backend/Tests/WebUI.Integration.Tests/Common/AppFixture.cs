using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace WebUI.Integration.Tests.Common
{
    public class AppFixture
    {
        private const string BaseUrl = "http://web-api:5050";

        static AppFixture()
        {
            var webhost = WebHost
                .CreateDefaultBuilder(null)
                .ConfigureAppConfiguration(builder =>
                {
                    builder.AddJsonFile("appsettings.Test.json");
                })
                .UseStartup<Startup>()
                .UseUrls(BaseUrl)
                .Build();

            webhost.Start();
        }

        public string GetCompleteServerUrl(string route)
        {
            route = route?.TrimStart('/');
            return $"{BaseUrl}/{route}";
        }
    }
}