using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.SemanticKernel;
using api_sentiment_analysis.Util;
using Azure.AI.TextAnalytics;
using api_sentiment_analysis.Plugins;


string _apiLanguageKey = Helper.GetEnvironmentVariable("ApiLanguageKey");
string _apiLanguageEndpoint = Helper.GetEnvironmentVariable("ApiLanguageEndpoint");


var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureServices(services =>
    {
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();
        services.AddSingleton<TextAnalyticsClient>(s =>
        {
            
            string endpoint = _apiLanguageEndpoint;
            string apiKey = _apiLanguageKey;

            return new TextAnalyticsClient(new Uri(endpoint), new Azure.AzureKeyCredential(apiKey));
        });

        services.AddTransient<Kernel>(s =>
        {
            var builder = Kernel.CreateBuilder();
            return builder.Build();
        });

    })
    .Build();


host.Run();
