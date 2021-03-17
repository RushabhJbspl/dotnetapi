using Worldex.Core.Helpers;
using Worldex.Core.Interfaces.LiquidityProvider;
using Worldex.Core.ViewModels.LiquidityProvider;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ExmoApi;
using Newtonsoft.Json;

namespace Worldex.Infrastructure.LiquidityProvider.EXMO
{
    public class ExmoLPService : IEXMOLPService
    {
        public static string ControllerName = "ExmoLPService";
        ExmoPublicApi Public_Client = new ExmoApi.ExmoPublicApi();
        ExmoAuthenticatedApi Private_Client = new ExmoApi.ExmoAuthenticatedApi(EXMOGlobalSettings.API_Key, EXMOGlobalSettings.Secret);

        public async Task<EXMOOrderbookResponse> GetOrederBook(string Market)
        {
            try
            {                          
                var APIResponse = await Public_Client.OrderBookAsync(Market);
                if(APIResponse != null)
                {                    
                    string data = APIResponse.ToString();
                    //HelperForLog.WriteLogIntoFile("GetOrderbook", ControllerName, Helpers.JsonSerialize(APIResponse));
                    data = data.Replace(Market, "Data");
                    var Resp = JsonConvert.DeserializeObject<EXMOOrderbookResponse>(data);
                    return Resp;
                }
                return null;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("GetOrederBook", ControllerName, ex);
                return null;
            }            
        }

        public async Task<EXMOTradeHistoryResponse> GetTradeHistory(string Market)
        {
            try
            {                
                var APIResponse = await Public_Client.TradesAsync(Market);
                if (APIResponse != null)
                {
                    string data = APIResponse.ToString();
                    //HelperForLog.WriteLogIntoFile("GetTradeHistory", ControllerName, Helpers.JsonSerialize(APIResponse));
                    data = data.Replace(Market, "Data");
                    var Resp = JsonConvert.DeserializeObject<EXMOTradeHistoryResponse>(data);
                    return Resp;
                }
                return null;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("GetTradeHistory", ControllerName, ex);
                return null;
            }
        }

        public async Task<EXMOBalanceResponse> GetBalance(string Currency)
        {
            try
            {
                Private_Client.Secret = EXMOGlobalSettings.Secret;
                Private_Client.Key = EXMOGlobalSettings.API_Key;
                var APIResponse = await Private_Client.UserInfoAsync();
                //var APIResponse = "{  \"uid\": 1689684,  \"server_date\": 1563259525,  \"balances\": {    \"BTC\": \"777.70707585856\",    \"LTC\": \"0\"  },  \"reserved\": {    \"BTC\": \"7574.565485657\",    \"LTC\": \"0\"  }}";
                if (APIResponse != null)
                {
                    string data = APIResponse.ToString();
                    data = data.Replace(Currency, "Currency");
                    //HelperForLog.WriteLogIntoFile("GetBalance", ControllerName, Helpers.JsonSerialize(APIResponse));                    
                    var Resp = JsonConvert.DeserializeObject<EXMOBalanceResponse>(data);
                    return Resp;
                }
                return null;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("GetBalance", ControllerName, ex);
                return null;
            }
        }

        public async Task<EXMOPlaceOrderResponse> PlaceOrder(OrderSide side, string market, decimal quantity, decimal rate)
        {
            try
            {
                Private_Client.Secret = EXMOGlobalSettings.Secret;
                Private_Client.Key = EXMOGlobalSettings.API_Key;
                var APIResponse = await Private_Client.OrderCreateAsync(market,Convert.ToDouble(quantity),Convert.ToDouble(rate),side.ToString().ToLower());
                //var APIResponse = "{  \"result\": true,  \"error\": \"\",  \"order_id\": 123456}";
                if (APIResponse != null)
                {
                    string data = APIResponse.ToString();
                    //HelperForLog.WriteLogIntoFile("PlaceOrder", ControllerName, Helpers.JsonSerialize(APIResponse));                    
                    var Resp = JsonConvert.DeserializeObject<EXMOPlaceOrderResponse>(data);
                    return Resp;
                }
                return null;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("PlaceOrder", ControllerName, ex);
                return null;
            }
        }

        public async Task<EXMOOpenOrderResp> GetOpenOrders()
        {
            try
            {                
                Private_Client.Secret = EXMOGlobalSettings.Secret;
                Private_Client.Key = EXMOGlobalSettings.API_Key;                
                var OpenAPIResponse = await Private_Client.UserOpenOrdersAsync();
                if(OpenAPIResponse != null)
                {
                    string data = OpenAPIResponse.ToString();
                    //data = "{  \"Data\": [    {      \"order_id\": \"12345\",      \"created\": \"1435517311\",      \"type\": \"buy\",      \"pair\": \"BTC_USD\",      \"price\": \"100\",      \"quantity\": \"1\",      \"amount\": \"100\"    }  ]}";
                    var Resp = JsonConvert.DeserializeObject<EXMOOpenOrderResp>(data);
                    return Resp;
                }
                return null;
                #region OldCode
                //string Response = null;
                //EXMOOpenOrderResp OpenOrderResponse = new EXMOOpenOrderResp();
                //EXMOCancelOrderListResp CancelOrderResponse = new EXMOCancelOrderListResp();
                //EXMOOrderTradeResponse TradeResponse = new EXMOOrderTradeResponse();

                //Private_Client.Secret = EXMOGlobalSettings.Secret;
                //Private_Client.Key = EXMOGlobalSettings.API_Key;
                //var OpenAPIResponse = Private_Client.UserOpenOrdersAsync();

                //var CancelAPIResponse = Private_Client.UserCancelledOrders();
                //var TradeAPIResponse  = Private_Client.OrderTradesAsync(Convert.ToInt64(OrderID));

                //var Opendata = await OpenAPIResponse;
                //var Canceldata = await CancelAPIResponse;
                //var Tradedata = await TradeAPIResponse;

                //if (Opendata != null)
                //{
                //    string data = Opendata.ToString();                                   
                //    OpenOrderResponse = JsonConvert.DeserializeObject<EXMOOpenOrderResp>(data);                    
                //}
                //if(Canceldata != null)
                //{
                //    string data = Canceldata.ToString();
                //    CancelOrderResponse = JsonConvert.DeserializeObject<EXMOCancelOrderListResp>(data);
                //}
                //if(Tradedata != null)
                //{
                //    string data = Tradedata.ToString();
                //    TradeResponse = JsonConvert.DeserializeObject<EXMOOrderTradeResponse>(data);
                //}
                //if(OpenOrderResponse != null)
                //{
                //    foreach(var x in OpenOrderResponse.Data)
                //    {
                //        if (x.order_id == OrderID)
                //        {
                //            Response = "Open";
                //        }
                //    }                    
                //}
                //else if (CancelOrderResponse != null)
                //{
                //    if (CancelOrderResponse.order_id == Convert.ToInt32(OrderID))
                //    {
                //        Response = "Cancel";
                //    }
                //}
                //else if(TradeResponse != null)
                //{
                //    if(TradeResponse.trades.Count > 0)
                //    {
                //        Response = "Success";
                //    }
                //}
                //return Response;
                #endregion
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("GetOpenOrders", ControllerName, ex);
                return null;
            }
        }

        public async Task<EXMOOrderTradeResponse> GetTradeByOrderId(string OrderID)
        {
            try
            {                
                Private_Client.Secret = EXMOGlobalSettings.Secret;
                Private_Client.Key = EXMOGlobalSettings.API_Key;
                var APIResponse = await Private_Client.OrderTradesAsync(Convert.ToInt64(OrderID));
                if (APIResponse != null)
                {
                    string data = APIResponse.ToString();
                    //data = "{  \"type\": \"buy\",  \"in_currency\": \"BTC\",  \"in_amount\": \"1\",  \"out_currency\": \"USD\",  \"out_amount\": \"100\",  \"trades\": [    {      \"trade_id\": 12345,      \"date\": 1435488248,      \"type\": \"buy\",      \"pair\": \"BTC_USD\",      \"order_id\": 12345,      \"quantity\": 1,      \"price\": 100,      \"amount\": 100    }  ]}";
                    //HelperForLog.WriteLogIntoFile("GetCancelOrderList", ControllerName, data);
                    var Resp = JsonConvert.DeserializeObject<EXMOOrderTradeResponse>(data);
                    return Resp;
                }
                return null;                
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("GetTradeByOrderId", ControllerName, ex);
                return null;
            }
        }

        public async Task<EXMOAPIReqRes.ExmoPlaceOrderResult> PlaceOrder(string Pair, string OrderType, decimal Qty, decimal price)
        {
            try
            {
                Private_Client.Secret = EXMOGlobalSettings.Secret;
                Private_Client.Key = EXMOGlobalSettings.API_Key;
                EXMOAPIReqRes.ExmoPlaceOrderResult placeOrderResult = new EXMOAPIReqRes.ExmoPlaceOrderResult();


                var res = await Private_Client.OrderCreateAsync(Pair, Convert.ToDouble(Qty), Convert.ToDouble(price), OrderType);
                var data = res.ToString();
                placeOrderResult = JsonConvert.DeserializeObject<EXMOAPIReqRes.ExmoPlaceOrderResult>(data);
                return placeOrderResult;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("PlaceOrder", "EXMOLPService", ex);
                return null;
            }
        }

        public async Task<EXMOAPIReqRes.ExmoCancelOrderResult> CancelOrder(string OrderID)
        {
            try
            {
                Private_Client.Secret = EXMOGlobalSettings.Secret;
                Private_Client.Key = EXMOGlobalSettings.API_Key;
                EXMOAPIReqRes.ExmoCancelOrderResult cancelOrderResult = new EXMOAPIReqRes.ExmoCancelOrderResult();
                var res = await Private_Client.OrderCancelAsync(long.Parse(OrderID));
                var data = res.ToString();
                cancelOrderResult = JsonConvert.DeserializeObject<EXMOAPIReqRes.ExmoCancelOrderResult>(data);
                return cancelOrderResult;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("CancelOrder", "EXMOLPService", ex);
                return null;
            }
        }

        public async Task<EXMOCancelOrderListResponse> GetCancelOrderList()
        {
            try
            {
                Private_Client.Secret = EXMOGlobalSettings.Secret;
                Private_Client.Key = EXMOGlobalSettings.API_Key;                
                var res = await Private_Client.UserCancelledOrders();
                if(res !=null)
                {
                    var data = res.ToString();
                    //data = "{    \"data\": 1435519742,    \"order_id\": 12345,    \"order_type\": \"sell\",    \"pair\": \"BTC_USD\",    \"price\": 100,    \"quantity\": 3,    \"amount\": 300  }";
                    //HelperForLog.WriteLogIntoFile("GetCancelOrderList", ControllerName, data);
                    var Response = JsonConvert.DeserializeObject<EXMOCancelOrderListResponse>(data);                    
                    return Response;
                }                
                return null;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("GetCancelOrderList", ControllerName, ex);
                return null;
            }
        }
    }
}
