using System;
using System.Collections.Generic;
using System.Linq;
using ***** emo.data;
using ***** sample;
using Platform.Api;
using Platform.Api.Emotions;

namespace EmoTestApp
{

    public sealed class EmoClient
    {
        public static EmotionsModelApi EmoModel { get; set; }
        private static readonly string _hostName = "*****";
        private static readonly string _brokerHost = "";
        private static readonly string _brokerUser = "*****";
        private static readonly string _brokerPwd = "*****"; //crypt
        private static readonly string _domainUser = "*****";
        private static readonly string _domainPwd = "*****"; // crypt
        private static Connector Platform { get; set; }
        private static EmotionsApi EmoApi { get; set; }
        private TimeSpan ProcessingTimeout => TimeSpan.FromSeconds(5);
        private static readonly string ManagementQueue = $"EmoClientMQueue_" + Guid.NewGuid();
        private string InputQueue { get; set; }
        public static string RecognitionModel => "FourEmo8kHz";

        public void ConnectToApi()
        {
            try
            {
                InputQueue = $"EmoClientIQueue_{Guid.NewGuid()}";
                var domain = new Domain { Id = 201 };
                var user = new User { Domain = domain, Name = _domainUser, Password = _domainPwd };

                Platform = Connector
                    .Configure()
                    .Queues(InputQueue, ManagementQueue)
                    .Broker(_brokerHost, _brokerUser, _brokerPwd)
                    .Endpoint(_hostName)
                    .Build();

                var _session = Platform.SessionManager.ForUser(user).Build();

                EmoApi = Platform
                    .ApiBuilder
                    .ForSession(_session.Id)
                    .Get<EmotionsApi, ApiContextBase>((a, c) => new EmotionsApi(a, c));

                EmoModel = EmoApi.Models[EmotionModel.Default];
                if (EmoApi.Models?.Available?.Any(x =>
                    x.Name == RecognitionModel) == true)
                {
                    EmoModel = EmoApi.Models[RecognitionModel];
                }
            }
            catch (Exception e)
            {
                throw new Exception("Connect is Failed");
            }
        }

        public bool AddSample(Sample _sample)
        {
            return EmoApi.CurrentTransaction.AddSample(_sample);
        }

        public DetectionResultsList GetEmoApiResults()
        {
            return EmoApi.CurrentTransaction.Close(ProcessingTimeout);
        }

        public List<EmotionDetectionResultChunk> GetDetailsOfResults(DetectionResultsList results)
        {
            var chunks = new List<EmotionDetectionResultChunk>();
            foreach (EmotionMarkingAllChannels detectionResult in results)
            {
                foreach (EmotionMarkingOneChannel channelResult in detectionResult.ChannelResults)
                {
                    foreach (var resultChunk in channelResult.Chunks)
                    {
                        foreach (var emotionRecognition in resultChunk.EmotionPalette)
                        {
                            try
                            {
                                var normProbability = emotionRecognition.Probability;

                                if (normProbability > 20)
                                    chunks.Add(resultChunk);
                            }
                            catch (Exception e) //suppress problems like NaN in raw result
                            {
                                throw;
                            }
                        }
                    }
                }
            }

            return chunks;
        }

        //public EmotionDetectionResultChunk GetDetailsOfResults(DetectionResultsList results)
        //{
        //    foreach (EmotionMarkingAllChannels detectionResult in results)
        //    {
        //        foreach (EmotionMarkingOneChannel channelResult in detectionResult.ChannelResults)
        //        {
        //            foreach (var resultChunk in channelResult.Chunks)
        //            {
        //                var normProbability = 0f;
        //                try
        //                {
        //                    normProbability = resultChunk.Emotion.Probability;

        //                    if (normProbability > 20)
        //                    {
        //                        return resultChunk;
        //                    }
        //                }
        //                catch //suppress problems like NaN in raw result
        //                {
        //                    throw;
        //                }
        //            }
        //        }
        //    }

        //    return null;
        //}
    }
}
