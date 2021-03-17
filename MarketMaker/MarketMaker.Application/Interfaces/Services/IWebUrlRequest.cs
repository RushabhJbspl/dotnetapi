using System.Collections.Generic;
using System.Net;

namespace MarketMaker.Application.Interfaces.Services
{
    public interface IWebUrlRequest
    {
        /// <summary>
        /// method usage get data form url or api call 
        /// </summary>
        /// <param name="url">web url</param>
        /// <param name="request"> request data</param>
        /// <param name="methodType"> http method type</param>
        /// <param name="contentType"> request content type</param>
        /// <param name="header"> http request header data</param>
        /// <param name="timeout"> http request timeout</param>
        /// <returns> string of http response</returns>
        /// <remarks>-Sahil 04-10-2019 04:26 PM</remarks>
        string Request(string url, string request, string methodType = "GET", string contentType = "application/json", WebHeaderCollection header = null, int timeout = 9000);

        /// <summary>
        /// Convert data to x-www-form-urlencoded data 
        /// </summary>
        /// <param name="datas">key value pair for http request data to be send</param>
        /// <returns>string formated with x-www-form-urlencoded data </returns>
        /// <remarks>-Sahil 04-10-2019 05:31 PM</remarks>
        string GetFormUrlEncodedRequest(Dictionary<string, string> datas);
    }
}
