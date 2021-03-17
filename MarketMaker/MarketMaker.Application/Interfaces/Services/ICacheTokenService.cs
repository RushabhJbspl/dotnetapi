using System;
using System.Collections.Generic;
using System.Text;

namespace MarketMaker.Application.Interfaces.Services
{
    public interface ICacheTokenService
    {
        /// <summary>
        /// store token into memory cached 
        /// </summary>
        /// <param name="tokenKey">key for memory cache</param>
        /// <param name="tokenValue"> token to be store</param>
        /// <remarks>-Sahil 09-10-2019 01:13 PM</remarks>
        void SetToken(string tokenKey, string tokenValue);

        /// <summary>
        /// Method used for retrieve authentication token for make buy/sell api call  
        /// </summary>
        /// <param name="tokenKey">key for memory cache</param>
        /// <returns> auth token</returns>
        /// <remarks>-Sahil 09-10-2019 12:04 PM</remarks>
        string GetStoreToken(string tokenKey);

        /// <summary>
        /// Method remove store token in cache
        /// </summary>
        /// <param name="tokenKey">key for memory cache</param>
        /// <remarks>-Sahil 09-10-2019 12:05 PM</remarks>
        void RemoveStoreToken(string tokenKey);
    }
}
