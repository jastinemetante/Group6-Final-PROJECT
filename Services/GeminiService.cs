using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace EchoesOfThePast.Services
{
    public interface IGeminiService
    {
        Task<string> GetHistoricalInsightAsync(string title, string content);
    }

    public class GeminiService : IGeminiService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;

        public GeminiService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _apiKey = configuration["Gemini:ApiKey"] ?? throw new System.ArgumentException("Gemini API Key is missing");
        }

        public async Task<string> GetHistoricalInsightAsync(string title, string content)
        {
            var prompt = $"Provide educational historical context for the following story/document titled \"{title}\":\n\n{content}\n\nKeep it educational, easy to read, and insightful for students and history enthusiasts. Use markdown for formatting.";
            
            var requestBody = new
            {
                contents = new[]
                {
                    new { parts = new[] { new { text = prompt } } }
                }
            };

            var url = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-1.5-flash:generateContent?key={_apiKey}";
            var response = await _httpClient.PostAsJsonAsync(url, requestBody);
            
            if (!response.IsSuccessStatusCode) return "Failed to generate AI insight.";

            var json = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);
            return doc.RootElement
                .GetProperty("candidates")[0]
                .GetProperty("content")
                .GetProperty("parts")[0]
                .GetProperty("text")
                .GetString() ?? string.Empty;
        }
    }
}
