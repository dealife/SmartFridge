using RestSharp;
using Newtonsoft.Json;
using SmartFridgeAPI.Models;
using System.Text.RegularExpressions;
using System.Net;

namespace SmartFridgeAPI.Services
{
    public class GeminiService
    {
        private readonly string _apiKey = "AIzaSyD7Ac2S_QZ6w9goW4oh4g4vChuvkUJ9vwg";
        // Используем стабильную версию API v1
        private readonly string _url = "https://generativelanguage.googleapis.com/v1/models/gemini-1.5-flash:generateContent";

        public async Task<AnalyzeResponse> AnalyzeImage(IFormFile photo)
        {
            try
            {
                using var ms = new MemoryStream();
                await photo.CopyToAsync(ms);
                var base64Image = Convert.ToBase64String(ms.ToArray());

                // Настраиваем клиент так, чтобы он автоматически подхватывал VPN на твоем ПК
                var options = new RestClientOptions(_url)
                {
                    // Эта строка говорит программе: "Используй те же настройки интернета, что и браузер"
                    Proxy = HttpClient.DefaultProxy, 
                    ThrowOnAnyError = false
                };

                var client = new RestClient(options);
                var request = new RestRequest("", Method.Post);
                request.AddQueryParameter("key", _apiKey);

                var prompt = "Посмотри на фото продуктов. Что там есть? Предложи 2 рецепта. " +
                             "Ответь строго в формате JSON: " +
                             "{ \"Ingredients\": [\"названия\"], \"Recipes\": [ { \"Title\": \"\", \"Description\": \"\", \"MissingIngredients\": [] } ] }";

                var body = new
                {
                    contents = new[] {
                        new {
                            parts = new object[] {
                                new { text = prompt },
                                new { inline_data = new { mime_type = photo.ContentType, data = base64Image } }
                            }
                        }
                    }
                };

                request.AddJsonBody(body);

                var response = await client.ExecuteAsync(request);

                if (!response.IsSuccessful)
                {
                    // Если ошибка осталась, мы увидим причину в черном окне консоли
                    Console.WriteLine("--- КРИТИЧЕСКАЯ ОШИБКА API ---");
                    Console.WriteLine($"Статус: {response.StatusCode}");
                    Console.WriteLine($"Ответ: {response.Content}");
                    return null;
                }

                var dynamicResponse = JsonConvert.DeserializeObject<dynamic>(response.Content);
                string rawText = dynamicResponse.candidates[0].content.parts[0].text;

                // Чистим ответ от лишнего текста, если AI его добавил
                var match = Regex.Match(rawText, @"\{.*\}", RegexOptions.Singleline);
                if (!match.Success) return null;

                return JsonConvert.DeserializeObject<AnalyzeResponse>(match.Value);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка в коде: {ex.Message}");
                return null;
            }
        }
    }
}