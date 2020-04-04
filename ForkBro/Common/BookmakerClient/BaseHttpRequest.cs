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
    public abstract class BaseHttpRequest
    {
        public Bookmaker BM { get; protected internal set; }

        public static BaseHttpRequest GetHttpRequest(Bookmaker bookmaker)
        {
            BaseHttpRequest request;
            switch (bookmaker)
            {
                case Bookmaker._1xbet:
                    request = HttpRequest_1xbet.CreateInstance();
                    break;
                case Bookmaker._favbet:
                    request = new HttpRequest_favbet();
                    break;
                default:
                    throw new Exception("Клиента для букмекера " + bookmaker.ToString() + " не существует");
            }
            request.BM = bookmaker;
            return request;
        }

        public abstract ConcurrentDictionary<ushort, double[,]> GetDictionaryOdds(long eventId, Sport sport);

        public abstract IGameList GetEventsList();
        public abstract string GetBetOdds(long eventId);

        //Async Get/Post request
        public async Task<string> GetAsync(string Url, string Data)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url + "?" + Data);
            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            using (HttpWebResponse response = (HttpWebResponse)await request.GetResponseAsync())
            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream))
            {
                return await reader.ReadToEndAsync();
            }
        }
        public async Task<string> PostAsync(string Url, string Data, string ContentType)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url);
            request.Method = "POST";
            request.ContentType = ContentType;
            byte[] sentData = Encoding.UTF8.GetBytes(Data);
            request.ContentLength = sentData.Length;
            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            request.UserAgent = "Mozilla/5.0 (compatible; MSIE 9.0; Windows Phone OS 7.5; Trident/5.0; IEMobile/9.0)";
            Stream sendStream = request.GetRequestStream();
            sendStream.Write(sentData, 0, sentData.Length);
            sendStream.Close();

            using (HttpWebResponse response = (HttpWebResponse)await request.GetResponseAsync())
            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream))
                return await reader.ReadToEndAsync();
        }

    }
}