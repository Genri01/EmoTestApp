using System;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace EmoTestApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var cryptStr = Cryptographer.Encrypt("password");
            var encryptStr = Cryptographer.Decrypt(cryptStr);
            //var encryptStr2 = Cryptographer.Decrypt(Encoding.ASCII.GetBytes(cryptStr));


            //var str = configuration.GetSection("Host");
            var emoClient = new EmoClient();
            emoClient.ConnectToApi();

            var emoFileProcessor = new EmoFileProcessor(emoClient);

            var fileWorker = new FileWorker("");
            fileWorker.Proccess(emoFileProcessor);


            //fileWorker.GetSamples("", emoFileProcessor);


            //var files = Directory.GetFiles(@"D:\TestWav", "*.wav");

            //foreach (var file in files)
            //{
            //    WorkWithFile(file);
            //}

            //var results = GetEmoResults(samples);
            //var emotionType = GetDetailsOfResults(results);
            Console.ReadKey();
        }

    }
}
