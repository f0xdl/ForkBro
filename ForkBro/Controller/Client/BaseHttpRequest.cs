using ForkBro.Controller.Event;
using ForkBro.Model;
using ForkBro.Model.RefEventModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ForkBro.Controller.Client
{
    public abstract class BaseHttpRequest
    {
        public static BaseHttpRequest Empty => new HttpRequest_template();

        public static BaseHttpRequest GetHttpRequest(Bookmaker bookmaker)
        {
            BaseHttpRequest request;
            switch (bookmaker)
            {
                case Bookmaker._1xbet:
                    request = new HttpRequest_1xbet();
                    break;
                case Bookmaker._favbet:
                    request = new HttpRequest_favbet();
                    break;
                default:
                    request = BaseHttpRequest.Empty;
                    break;
            }
            return request;
        }

        public abstract List<RefEvent> GetListEvent();

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

    public class HttpRequest_template : BaseHttpRequest
    {
        public override List<RefEvent> GetListEvent()
        {
            throw new System.NotImplementedException();
        }
    }
}