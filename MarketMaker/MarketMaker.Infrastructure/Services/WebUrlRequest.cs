using System;
using MarketMaker.Application.Interfaces.Services;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Web;
using LoggingNlog;

namespace MarketMaker.Infrastructure.Services
{
    public class WebUrlRequest : IWebUrlRequest
    {
        private readonly INLogger<WebUrlRequest> _logger;

        public WebUrlRequest(INLogger<WebUrlRequest> logger)
        {
            _logger = logger;
        }
        public string GetFormUrlEncodedRequest(Dictionary<string, string> datas)
        {
            string formatedStringData = string.Empty;
            foreach (var data in datas)
            {
                if (formatedStringData.Equals(""))
                {
                    formatedStringData = $"{data.Key}={HttpUtility.UrlEncode(data.Value)}";
                    continue;
                }

                formatedStringData += $"&{data.Key}={HttpUtility.UrlEncode(data.Value)}";
            }

            return formatedStringData;
        }

        public string Request(string url, string request, string methodType = "GET", string contentType = "application/json", WebHeaderCollection header = null, int timeout = 9000)
        {
            _logger.WriteInfoLog("Request", $"request for {url}  request :   {request}");
            string response = string.Empty;
            HttpWebRequest webRequest = WebRequest.Create(url) as HttpWebRequest;
            webRequest.Method = methodType.ToUpper();
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            if (header != null) webRequest.Headers = header;

            webRequest.KeepAlive = false;
            webRequest.Timeout = timeout;
            webRequest.ContentType = contentType;
            webRequest.ContentLength = request.Length;

            if (request != "" && !string.IsNullOrEmpty(request))
            {
                using (StreamWriter streamWriter = new StreamWriter(webRequest.GetRequestStream()))
                {
                    streamWriter.Write(request);
                    streamWriter.Flush();
                    streamWriter.Close();
                }
            }

            try
            {
                HttpWebResponse webResponse = webRequest.GetResponse() as HttpWebResponse;

                using (StreamReader streamReader = new StreamReader(webResponse.GetResponseStream()))
                {
                    response = streamReader.ReadToEnd();
                    streamReader.Close();
                    streamReader.Dispose();
                }

                webResponse.Close();
            }
            catch (WebException e)
            {
                _logger.WriteInfoLog("Request", $"method return null for WebException check error log");
                _logger.WriteErrorLog("Request", e);
                using (WebResponse Exresponse = e.Response)
                {
                    HttpWebResponse httpResponse = (HttpWebResponse)Exresponse;
                    using (Stream data = Exresponse.GetResponseStream())
                    using (var reader = new StreamReader(data))
                    {
                        string responseFromAPI = reader.ReadToEnd();
                        _logger.WriteInfoLog("Exception", $"Response from API : " + responseFromAPI);
                    }
                }
                return null;
            }
            return response;
        }
    }
}
