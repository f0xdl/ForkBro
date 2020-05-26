using ForkBro.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using ForkBro.Scanner.EventLinks;
using ForkBro.BookmakerModel;
using System.Collections.Concurrent;
using ForkBro.BookmakerModel.BaseEvents;

namespace ForkBro.Common.BookmakerClient
{
    public abstract class BaseRequest
    {
        public Bookmaker bookmaker { get; protected set; }

        public static BaseRequest GetInstance(Bookmaker bookmaker)
            => bookmaker switch
            {
                Bookmaker._1xbet => new HttpRequest_1xbet(),
                Bookmaker._favbet => new HttpRequest_favbet(),
                _ => throw new Exception("Клиента для букмекера " + bookmaker.ToString() + " не существует")
            };

        public abstract IGameList GetEventsList();
        public abstract T GetBetOdds<T>(long eventId);

        //Async Get/Post request
        protected static async Task<string> GetAsync(string url, string data)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url + "?" + data);
            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            using(HttpWebResponse response = (HttpWebResponse)await request.GetResponseAsync())
            using(Stream stream = response.GetResponseStream())
            using(StreamReader reader = new StreamReader(stream!))
                return await reader.ReadToEndAsync();
        }

        protected static async Task<string> PostAsync(string url, string data, string contentType)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            request.ContentType = contentType;
            byte[] sentData = Encoding.UTF8.GetBytes(data);
            request.ContentLength = sentData.Length;
            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            request.UserAgent = "Mozilla/5.0 (compatible; MSIE 9.0; Windows Phone OS 7.5; Trident/5.0; IEMobile/9.0)";
            Stream sendStream = request.GetRequestStream();
            sendStream.Write(sentData, 0, sentData.Length);
            sendStream.Close();

            using HttpWebResponse response = (HttpWebResponse)await request.GetResponseAsync();
            using Stream stream = response.GetResponseStream();
            using StreamReader reader = new StreamReader(stream!);
            return await reader.ReadToEndAsync();
        }

        public abstract bool TestConnection();
    }
}