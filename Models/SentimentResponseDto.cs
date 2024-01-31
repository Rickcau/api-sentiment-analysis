using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace api_sentiment_analysis.Models
{
    public class SentimentResponseDto
    {
        public string? Sentiment { get; set; } = string.Empty;
        public double ConfidenceScores_Positive;
        public double ConfidenceScores_Neutral;
        public double ConfidenceScores_Negative;
        public string? StatusMessage { get; set; } = string.Empty;
        public int StatusCode { get; set; }
        public string? Result { get; set; } = string.Empty;
    }
}
