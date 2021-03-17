using Worldex.Core.ApiModels;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace Worldex.Core.Interfaces
{
    public interface IProviderDataList<in TRequest, TResponse>
    {
        IEnumerable<TResponse> GetProviderDataList(TRequest Request);
    }

    public interface IWebApiData
    {
        WebApiConfigurationResponse GetAPIConfiguration(long ThirPartyAPIID);
    }

    public interface IWebApiSendRequest
    {
        String SendAPIRequestAsync(string Url, string Request, string ContentType, int Timeout, WebHeaderCollection headerDictionary ,string MethodType = "POST",long TrnNo = 0);
        Task<string> SendRequestAsync(string Url, string Request = "", string MethodType = "GET", string ContentType = "application/json", WebHeaderCollection Headers = null, int Timeout = 9000, bool IsWrite = true);
        Task<string> SendRequestAsyncLPArbitrage(string Url, ref short IsAPIProceed, string Request = "", string MethodType = "GET", string ContentType = "application/json", WebHeaderCollection Headers = null, int Timeout = 9000, long TrnNo = 0, bool IsWrite = true);
        Task<string> SendTCPSocketRequestAsync(string HostName, string Port, string request);
        string SendJsonRpcAPIRequestAsync(string Url, string RequestStr,WebHeaderCollection headerDictionary = null);
        String SendAPIRequestAsyncWallet(string Url, string Request, string ContentType, int Timeout, WebHeaderCollection headerDictionary, string MethodType = "POST");

    }

    public interface IWebApiParseResponse<TResponse>
    {
    }
   
}