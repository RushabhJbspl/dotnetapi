using Worldex.Core.ApiModels;
using Worldex.Core.Enums;
using Worldex.Infrastructure.Data.Transaction;
using Worldex.Infrastructure.DTOClasses;
using System;
using System.Text.RegularExpressions;

namespace Worldex.Infrastructure.Services
{
    //Common Parsing method Implement Here
    public class WebApiParseResponse
    {
        readonly TransactionWebAPIConfiguration _txnWebAPIConf;
        GetDataForParsingAPI _txnWebAPIParsingData;
        private readonly WebApiDataRepository _webapiDataRepository;
        public WebAPIParseResponseCls _webapiParseResponse;
        short gIsLower = 1;
        public WebApiParseResponse(WebAPIParseResponseCls webapiParseResponse, GetDataForParsingAPI txnWebAPIParsingData,
            WebApiDataRepository webapiDataRepository, TransactionWebAPIConfiguration txnWebAPIConf)
        {
            _txnWebAPIConf = txnWebAPIConf;
            _webapiDataRepository = webapiDataRepository;
            _webapiParseResponse = webapiParseResponse;
        }
        public WebAPIParseResponseCls TransactionParseResponse(string TransactionResponse, long ThirPartyAPIID, short IsLower = 1)
        {
            try
            {
                _txnWebAPIParsingData = _webapiDataRepository.GetDataForParsingAPI(ThirPartyAPIID);
                WebAPIParseResponseCls _webapiParseResponse = ParseResponseViaRegex(TransactionResponse, _txnWebAPIParsingData, IsLower);

                return _webapiParseResponse;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public WebAPIParseResponseCls ArbitrageTransactionParseResponse(string TransactionResponse, long ThirPartyAPIID, short IsLower = 1)
        {
            try
            {
                //Take Regex for response parsing
                _txnWebAPIParsingData = _webapiDataRepository.ArbitrageGetDataForParsingAPI(ThirPartyAPIID);
                WebAPIParseResponseCls _webapiParseResponse = ParseResponseViaRegex(TransactionResponse, _txnWebAPIParsingData, IsLower);

                return _webapiParseResponse;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public string CheckArrayLengthAndReturnResponse(string StrResponse, string[] strArray)
        {
            string ResponseFromParsing = "";
            try
            {
                if (strArray != null && strArray.Length > 1)
                {
                    if (gIsLower == 1)
                        ResponseFromParsing = ParseResponse(StrResponse, strArray[0], strArray[1]);
                    else
                        ResponseFromParsing = ParseResponseNoLower(StrResponse, strArray[0], strArray[1]);
                }//either Send blank                
            }
            catch (Exception ex)
            {
            }
            return ResponseFromParsing;
        }
        public string ParseResponse(string StrResponse, string regex1, string regex2)
        {
            string MatchRegex = "";
            string MatchRegex2 = "";
            try
            {
                if (regex1 != null && regex2 != null)
                {
                    MatchRegex = Regex.Match(StrResponse.ToLower(), regex1.ToLower(), new RegexOptions()).Value;
                    if ((!string.IsNullOrEmpty(MatchRegex)))
                    {
                        MatchRegex2 = Regex.Replace(MatchRegex, regex2.ToLower(), "");
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return MatchRegex2;
        }
        //Rushabh 25-01-2019 Added for Generate Address Response Issue
        public string CheckArrayLengthAndReturnResponse1(string StrResponse, string[] strArray)
        {
            string ResponseFromParsing = "";
            try
            {
                if (strArray != null && strArray.Length > 1)
                {
                    ResponseFromParsing = ParseResponseNoLower(StrResponse, strArray[0], strArray[1]);
                }//either Send blank                
            }
            catch (Exception ex)
            {
            }
            return ResponseFromParsing;
        }
        //Rushabh 25-01-2019 Added for Generate Address Response Issue
        public string ParseResponseNoLower(string StrResponse, string regex1, string regex2)
        {
            string MatchRegex = "";
            string MatchRegex2 = "";
            try
            {
                if (regex1 != null && regex2 != null)
                {
                    MatchRegex = Regex.Match(StrResponse, regex1, new RegexOptions()).Value;
                    if ((!string.IsNullOrEmpty(MatchRegex)))
                    {
                        MatchRegex2 = Regex.Replace(MatchRegex, regex2, "");
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return MatchRegex2;
        }

        public bool IsContain(string StrResponse, string CommaSepratedString)
        {
            try
            {
                //ntrivedi 12-12-2018 when responsesuccess is null in thirdparty 
                if (string.IsNullOrEmpty(CommaSepratedString))
                {
                    return false;
                }
                foreach (string Check in CommaSepratedString.Split(','))
                {
                    if (StrResponse.ToUpper().Contains(Check))
                    {
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return false;
        }

        public WebAPIParseResponseCls ParseResponseViaRegex(string Response, GetDataForParsingAPI _txnWebAPIParsingData, short IsLower = 1)
        {
            gIsLower = IsLower;//Rita 24-7-19 taken global so no need to send in all methods
            try
            {
                string[] BalanceArray = _txnWebAPIParsingData.BalanceRegex.Split("###");
                string[] StatusArray = _txnWebAPIParsingData.StatusRegex.Split("###");
                string[] StatusMsgArray = _txnWebAPIParsingData.StatusMsgRegex.Split("###");
                string[] ResponseCodeArray = _txnWebAPIParsingData.ResponseCodeRegex.Split("###");
                string[] ErrorCodeArray = _txnWebAPIParsingData.ErrorCodeRegex.Split("###");
                string[] TrnRefNoArray = _txnWebAPIParsingData.TrnRefNoRegex.Split("###");
                string[] OprTrnRefNoArray = _txnWebAPIParsingData.OprTrnRefNoRegex.Split("###");
                string[] Param1Array = _txnWebAPIParsingData.Param1Regex.Split("###");
                string[] Param2Array = _txnWebAPIParsingData.Param2Regex.Split("###");
                string[] Param3Array = _txnWebAPIParsingData.Param3Regex.Split("###");
                string[] Param4Array = _txnWebAPIParsingData.Param4Regex.Split("###");
                string[] Param5Array = _txnWebAPIParsingData.Param5Regex.Split("###");
                string[] Param6Array = _txnWebAPIParsingData.Param6Regex.Split("###");
                string[] Param7Array = _txnWebAPIParsingData.Param7Regex.Split("###");

                string BalanceResp = CheckArrayLengthAndReturnResponse(Response, BalanceArray);
                string StatusResp = CheckArrayLengthAndReturnResponse(Response, StatusArray);
                string StatusMsgResp = CheckArrayLengthAndReturnResponse(Response, StatusMsgArray);
                string TrnRefNoResp = CheckArrayLengthAndReturnResponse1(Response, TrnRefNoArray);
                string OprTrnRefNoResp = CheckArrayLengthAndReturnResponse(Response, OprTrnRefNoArray);
                string ResponseCodeResp = CheckArrayLengthAndReturnResponse(Response, ResponseCodeArray);
                string ErrorCodeResp = CheckArrayLengthAndReturnResponse(Response, ErrorCodeArray);
                string Param1Resp = "";//added to solve address issue

                Param1Resp = CheckArrayLengthAndReturnResponse(Response, Param1Array);//not make global islower
                //if (IsLower == 0)
                //{
                //    Param1Resp = CheckArrayLengthAndReturnResponse1(Response, Param1Array);
                //}
                //else
                //{
                //    Param1Resp = CheckArrayLengthAndReturnResponse(Response, Param1Array);
                //}
                string Param2Resp = CheckArrayLengthAndReturnResponse(Response, Param2Array);
                string Param3Resp = CheckArrayLengthAndReturnResponse(Response, Param3Array);
                string Param4Resp = CheckArrayLengthAndReturnResponse(Response, Param4Array);
                string Param5Resp = CheckArrayLengthAndReturnResponse(Response, Param5Array);
                string Param6Resp = CheckArrayLengthAndReturnResponse(Response, Param6Array);
                string Param7Resp = CheckArrayLengthAndReturnResponse(Response, Param7Array);

                if (IsContain(StatusResp.ToUpper(), _txnWebAPIParsingData.ResponseSuccess.ToUpper()) && !string.IsNullOrEmpty(StatusResp))
                {
                    _webapiParseResponse.Status = enTransactionStatus.Success;
                }
                else if (IsContain(StatusResp.ToUpper(), _txnWebAPIParsingData.ResponseFailure.ToUpper()) && !string.IsNullOrEmpty(StatusResp)) // ntrivedi 13-12-2018 for comma seperated failure response code in bitgo
                {
                    _webapiParseResponse.Status = enTransactionStatus.OperatorFail;
                }
                else
                {
                    _webapiParseResponse.Status = enTransactionStatus.Hold;
                }
                if (!string.IsNullOrEmpty(BalanceResp))
                    _webapiParseResponse.Balance = Convert.ToDecimal(BalanceResp);

                _webapiParseResponse.StatusMsg = StatusResp;
                _webapiParseResponse.ResponseMsg = StatusMsgResp;
                _webapiParseResponse.ResponseCode = ResponseCodeResp;
                _webapiParseResponse.ErrorCode = ErrorCodeResp;
                _webapiParseResponse.TrnRefNo = TrnRefNoResp;
                _webapiParseResponse.OperatorRefNo = OprTrnRefNoResp;
                _webapiParseResponse.Param1 = Param1Resp;
                _webapiParseResponse.Param2 = Param2Resp;
                _webapiParseResponse.Param3 = Param3Resp;
                _webapiParseResponse.Param4 = Param4Resp;
                _webapiParseResponse.Param5 = Param5Resp;
                _webapiParseResponse.Param6 = Param6Resp;
                _webapiParseResponse.Param7 = Param7Resp;
                return _webapiParseResponse;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}

