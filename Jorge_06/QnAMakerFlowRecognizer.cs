using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.AI.QnA;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Jorge_06
{
    public class QnAMakerFlowRecognizer : IRecognizer
    {
        private const string QNA_TRIGGER = "Q:";
        private readonly QnAMaker _resolver;

        public QnAMakerFlowRecognizer(IConfiguration configuration)
        {
            var qnaIsConfigured = !string.IsNullOrEmpty(configuration["QnAKnowledgebaseId"]) && !string.IsNullOrEmpty(configuration["QnAEndpointKey"]) && !string.IsNullOrEmpty(configuration["QnAEndpointHostName"]);
            if (qnaIsConfigured)
            {
                var endpoint = new QnAMakerEndpoint()
                {
                    KnowledgeBaseId = configuration["QnAKnowledgebaseId"],
                    EndpointKey = configuration["QnAEndpointKey"],
                    Host = configuration["QnAEndpointHostName"]
                };

                _resolver = new QnAMaker(endpoint, new QnAMakerOptions() { Top = 1 });
            }
        }

        public async Task<RecognizerResult> RecognizeAsync(ITurnContext turnContext, CancellationToken cancellationToken)
        {
            var result = new RecognizerResult()
            {
                Intents = new Dictionary<string, IntentScore>(),
                Properties = new Dictionary<string, object>()
            };

            var fullText = turnContext?.Activity?.AsMessageActivity()?.Text ?? "";
            if (fullText.StartsWith(QNA_TRIGGER))
            {
                var query = fullText.Substring(QNA_TRIGGER.Length);
                var qnaResult = await _resolver.GetAnswersAsync(turnContext);
                result.Text = query;
                result.Intents.Add("QNA", new IntentScore() { Score = 1f });
                result.Properties.Add("Result", qnaResult);
            }

            return result;
        }

        public Task<T> RecognizeAsync<T>(ITurnContext turnContext, CancellationToken cancellationToken) where T : IRecognizerConvert, new()
        {
            var result = new T();
            result.Convert(RecognizeAsync(turnContext, cancellationToken));
            return Task.FromResult(result);
        }

        public string GetAnswerFromRecognizerResult(RecognizerResult result)
        {
            if (result.Properties.TryGetValue("Result", out object qnaResultObj))
            {
                var qnaResult = qnaResultObj as QueryResult[];

                return qnaResult[0].Answer;
            }

            return string.Empty;
        }
    }
}
