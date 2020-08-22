using System.Text.Json;

namespace WebUI.Integration.Tests.Common
{
    public class Serializer
    {
        private static readonly JsonSerializerOptions Options = new JsonSerializerOptions()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
        
        public static T Deserialize<T>(string content)
        {
            return JsonSerializer.Deserialize<T>(content, Options);
        }
    }
}