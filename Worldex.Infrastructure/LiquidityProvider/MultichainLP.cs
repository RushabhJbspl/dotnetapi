using LucidOcean.MultiChain.Response;
using LucidOcean.MultiChain.Util;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Worldex.Infrastructure.LiquidityProvider
{

    public class MultichainLP
    {
        JsonRpcClient _Client = null;

        internal MultichainLP(JsonRpcClient client)
        {
            _Client = client;
        }

        public JsonRpcResponse<List<object>> ListAddresses(int PageSize, int PageIndex=0)
        {
            try {
                return _Client.Execute<List<object>>("listaddresses", 0, new object[] { "*", Convert.ToBoolean(0), Convert.ToInt32(PageSize), Convert.ToInt32((PageIndex* PageSize)) });
                
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public JsonRpcResponse<List<object>> ListAddressesTotalCount()
        {
            try
            {
                return _Client.Execute<List<object>>("listaddresses", 0);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public JsonRpcResponse<Dictionary<string, decimal>> GetListAddresses()
        {
            try
            {
                object[] names = { 2, 2 };
                //return _Client.Execute<Dictionary<string, decimal>>("listaddresses",1,"*", false, 2, 2);
                return _Client.Execute<Dictionary<string, decimal>>("listaddresses", 0, new object[] {  "*", Convert.ToBoolean(0), Convert.ToInt32(2),Convert.ToInt32(2) });
               //return _Client.Execute<List<string>>("liststreampublishers", 1, "root", "*", true, 5, 0, false);
            }
            catch(Exception ex) {
                throw ex;
            }
        }

        
        public Task<JsonRpcResponse<Dictionary<string, decimal>>> ListAddressesAsync()
        {
            return _Client.ExecuteAsync<Dictionary<string, decimal>>("listaddresses", 0);
        }

        public Task<JsonRpcResponse<List<string>>> GetAddressesAsync()
        {
            return _Client.ExecuteAsync<List<string>>("getaddresses", 0);
        }

        public JsonRpcResponse<List<string>> GetAddresses()
        {
            return _Client.Execute<List<string>>("getaddresses", 0);
        }
    }
}
