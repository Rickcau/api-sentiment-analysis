using Azure.AI.TextAnalytics;
using Azure;
using Microsoft.SemanticKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azure.AI;
using System.ComponentModel;
using api_sentiment_analysis.Models;

namespace api_sentiment_analysis.Plugins
{
    public class TextAnalyticsPlugin
    {
        private readonly TextAnalyticsClient _client;
        public TextAnalyticsPlugin(TextAnalyticsClient client)
        {
            _client = client;
        }
        [KernelFunction, Description("Sends document to AI Text Services for Sentiment Analysis")]
        public async Task<SentimentResponseDto> GetSentimentAnalysis(
              [Description("The document to Analyze")] string document)
        {
            Response<DocumentSentiment> response = await _client.AnalyzeSentimentAsync(document);
            SentimentResponseDto sentimentResponseDto = new SentimentResponseDto();
            sentimentResponseDto.StatusMessage = "Ok";
            sentimentResponseDto.StatusCode = 200;
            // Response<DocumentSentiment> response = await _client.AnalyzeSentimentAsync(document);
   
            DocumentSentiment docSentiment = response.Value;
            var sentiment = $"Document sentiment is {docSentiment.Sentiment} with: \n" +
                $"  Positive confidence score: {docSentiment.ConfidenceScores.Positive} \n" +
                $"  Neutral confidence score: {docSentiment.ConfidenceScores.Neutral} \n" +
                $"  Negative confidence score: {docSentiment.ConfidenceScores.Negative}";
            sentimentResponseDto.Result = sentiment;
            sentimentResponseDto.Sentiment = docSentiment.Sentiment.ToString();
            sentimentResponseDto.ConfidenceScores_Positive = docSentiment.ConfidenceScores.Positive;
            sentimentResponseDto.ConfidenceScores_Neutral = docSentiment.ConfidenceScores.Neutral;
            sentimentResponseDto.ConfidenceScores_Negative = docSentiment.ConfidenceScores.Negative;

           return sentimentResponseDto;
        }
    }
}
