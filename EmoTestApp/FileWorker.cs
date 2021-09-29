
using System;
using System.Collections.Generic;
using System.IO;
****sample;
using WaveFileLibWrap;

namespace EmoTestApp
{
    public class FileWorker
    {
        private string FileName { get; set; }
        private List<Emotion> emotions { get; set; } = new List<Emotion>();
        private List<Emotion> emotions2 { get; set; } = new List<Emotion>();
        public FileWorker(string _fileName)
        {
            FileName = _fileName;
        }
        public void Proccess(EmoFileProcessor _emoFileProcessor)
        {
            int PeriodInSeconds = 13;
            FileName = @"****\test.wav";
            try
            {
                using (var reader = new InSoundFile(FileName))
                {
                    int SamplesPerSec = reader.SampleRate; 
                    int ChannelsCount = reader.ChannelCount;
                    int SamplesByPeriod = SamplesPerSec * PeriodInSeconds;
                    int SamplesByPeriodAllChannels = SamplesPerSec * PeriodInSeconds * ChannelsCount;
                    short[] samplesBuffer = new short[SamplesByPeriodAllChannels];
                    byte[] bytesBuffer = new byte[SamplesByPeriodAllChannels * sizeof(short)];

                    short[] leftBuffer = new short[samplesBuffer.Length / ChannelsCount];
                    short[] rightBuffer = new short[samplesBuffer.Length / ChannelsCount];
                    byte[] leftBufferBytes = new byte[leftBuffer.Length * sizeof(short)];
                    byte[] rightBufferBytes = new byte[rightBuffer.Length * sizeof(short)];
                    float offset = 0;

                    for (int i = 0; i <= reader.SampleCount / SamplesByPeriod; i++)
                    {
                        var readed = reader.ReadData(samplesBuffer, SamplesByPeriod);

                        if (ChannelsCount == 1)
                        {
                            if (bytesBuffer.Length != readed * ChannelsCount * sizeof(short))
                                bytesBuffer = new byte[readed * ChannelsCount * sizeof(short)];

                            Buffer.BlockCopy(samplesBuffer, 0, bytesBuffer, 0, bytesBuffer.Length);
                            _emoFileProcessor.AddSample(new Sample { Data = bytesBuffer, SamplingRate = SamplesPerSec }, 0, offset);
                        }

                        if (ChannelsCount == 2)
                        {
                            if (leftBufferBytes.Length != readed * sizeof(short))
                            {
                                leftBufferBytes = new byte[readed * sizeof(short)];
                                rightBufferBytes = new byte[readed * sizeof(short)];
                            }

                            for (int j = 0; j < samplesBuffer.Length / ChannelsCount; j++)
                            {
                                if (j % 2 == 0)
                                {
                                    leftBuffer[j] = samplesBuffer[j * 2]; // for first channel
                                }
                                else
                                {
                                    rightBuffer[j] = samplesBuffer[j * 2 + 1]; // for second chsnnel
                                }
                            }

                            Buffer.BlockCopy(leftBuffer, 0, leftBufferBytes, 0, leftBufferBytes.Length);
                            _emoFileProcessor.AddSample(new Sample { Data = leftBufferBytes, SamplingRate = SamplesPerSec }, 0, offset);

                            Buffer.BlockCopy(rightBuffer, 0, rightBufferBytes, 0, rightBufferBytes.Length);
                            _emoFileProcessor.AddSample(new Sample { Data = rightBufferBytes, SamplingRate = SamplesPerSec }, 1, offset);
                        }

                        offset += readed / SamplesPerSec;
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }

            emotions.AddRange(_emoFileProcessor.Emotions);
            _emoFileProcessor.Emotions.Clear();

            GetSamples("", _emoFileProcessor);
        }


        public List<Sample> GetSamples(string _filename, EmoFileProcessor _emoFileProcessor)
        {
            _filename = @"*****\test.wav";

            using (var reader = new InSoundFile(_filename))
            {
                var data = File.ReadAllBytes(_filename);
                int period = 39;

                var samples = new List<Sample>();
                var currentBytesChannel = 0;
                var BytesPerWavSample = 2;
                var batchSizeForRecognitionSample =
                    BytesPerWavSample *
                    reader.SampleRate * 
                    period; 

                var dataByChannel = new List<List<byte>>(reader.ChannelCount);
                for (int i = 0; i < reader.ChannelCount; i++)
                {
                    dataByChannel.Add(new List<byte>());
                }


                var DataStartPosition = 54;

                for (int i = DataStartPosition; i < data.Length; i += BytesPerWavSample)
                {
                    // read sample data
                    for (int j = 0; j < BytesPerWavSample; j++)
                    {
                        if (i + j < data.Length)
                        {
                            dataByChannel[currentBytesChannel].Add(data[i + j]);
                        }
                    }

                    // if ChannelsCount > 1 then next BytesPerWavSample bytes related to other channel
                    currentBytesChannel++;

                    //if end
                    if (currentBytesChannel >= reader.ChannelCount)
                    {
                        currentBytesChannel = 0;
                    }
                }

                for (int channelIndex = 0; channelIndex < dataByChannel.Count; channelIndex++)
                {
                    var sampleStart = new TimeSpan(0);
                    for (int offset = 0;
                        offset < dataByChannel[channelIndex].Count;
                        offset += batchSizeForRecognitionSample)
                    {
                        var currentBatchSize =
                            offset + batchSizeForRecognitionSample < dataByChannel[channelIndex].Count
                                ? batchSizeForRecognitionSample
                                : dataByChannel[channelIndex].Count - offset;

                        try
                        {
                            var sample = new Sample()
                            {
                                Data = dataByChannel[channelIndex].GetRange(offset, currentBatchSize).ToArray(),
                                SamplingRate = reader.SampleRate
                            };

                            _emoFileProcessor.AddSample(sample, channelIndex, (int) sampleStart.TotalSeconds);

                            sampleStart += TimeSpan.FromSeconds(13);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                            throw;
                        }
                    }
                }
                emotions2 = _emoFileProcessor.Emotions;
                return samples;
            }
        }
    }
}
