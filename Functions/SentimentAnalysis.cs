using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Azure.AI.TextAnalytics;
using Microsoft.SemanticKernel;
using api_sentiment_analysis.Plugins;
using Azure;
using System.Text.Json;
using api_sentiment_analysis.Models;
using Microsoft.Azure.Functions.Worker.Http;

namespace api_sentiment_analysis.Functions
{
    public class SentimentAnalysis
    {
        private readonly ILogger<SentimentAnalysis> _logger;
        private readonly Kernel _kernel;
        private readonly TextAnalyticsClient _client;

        public SentimentAnalysis(ILogger<SentimentAnalysis> logger, Kernel kernel, TextAnalyticsClient client)
        {
            _logger = logger;
            _kernel = kernel;
            _client = client;
            _kernel.ImportPluginFromObject(new TextAnalyticsPlugin(_client));
        }

        [Function("SentimentAnalysis")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequestData req)
        {
            /* HTTP with request body example
            {
              "content" : "I had the best day of my life. I decided to go sky-diving and it made me appreciate my whole life so much more. I developed a deep-connection with my instructor as well, and I feel as if I've made a life-long friend in her."
            }
            */
            _logger.LogInformation("C# HTTP SentimentAnalysis trigger function processed a request.");

            // Default set to 400 and if success it will be set to 200
            SentimentResponseDto sentimentResponseDto = new SentimentResponseDto
            {
                StatusCode = 400,
                StatusMessage = "Error with Service Call",
                Result = ""
            };

            try
            {
                var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                var documentRequest = JsonSerializer.Deserialize<Document>(requestBody);
                if (documentRequest == null || documentRequest.Content == null)
                {
                    throw new ArgumentNullException("Content is null. Please check your request body.");
                }

                var function = _kernel.Plugins.GetFunction("TextAnalyticsPlugin", "GetSentimentAnalysis");

                KernelArguments variables = new KernelArguments()
                {
                    ["document"] = documentRequest.Content
                };

                var result = _kernel.InvokeAsync<SentimentResponseDto>(function, variables);
                if (result.Result != null) 
                {
                    sentimentResponseDto = result.Result;
                }
            }
            catch (RequestFailedException exception) // TextAnalyticsClient will throw this exception if problem with endpoint 
            {
                _logger.LogInformation($"Error Code: {exception.ErrorCode}");
                _logger.LogInformation($"Message: {exception.Message}");
            }
            return new OkObjectResult(sentimentResponseDto);
        }
    }
}
