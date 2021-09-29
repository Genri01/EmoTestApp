**** sample;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EmoTestApp
{
    public class EmoFileProcessor
    {
        private EmoClient EmoClient { get; set; }
        public List<Emotion> Emotions { get; set; } = new List<Emotion>();

        public EmoFileProcessor(EmoClient _emoClient)
        {
            EmoClient = _emoClient;
        }

        public void AddSample(Sample _sample, int _channel, float _offset)
        {
            if (!EmoClient.EmoModel.StartBufferTransaction(TimeSpan.FromSeconds(30)))
                throw new Exception("Unable to start buffer transaction");

            if (!EmoClient.AddSample(_sample))
                throw new Exception("Unable to add sample to transaction.");

            var apiResults = EmoClient.GetEmoApiResults();

            var chunks = EmoClient.GetDetailsOfResults(apiResults);

            if (chunks.Any())
            {
                foreach (var chunk in chunks)
                {
                    Emotions.Add(new Emotion
                    {
                        OriginalId = Guid.NewGuid(),
                        EmotionStartTime = (int)(_offset * 1000),
                        EmotionType = (int)EmotionTypeConverter.GetTypeEmotion(chunk.Emotion.Name),
                        Channel = _channel,
                        Duration = chunk.Duration.Seconds,
                        Confidence = (int)chunk.Emotion.Probability
                    });
                }
            }
        }
    }
}
