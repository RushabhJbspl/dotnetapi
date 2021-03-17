using Worldex.Core.ApiModels;
using Worldex.Core.Enums;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Worldex.Core.ViewModels.Configuration
{
    public class RouteConfigurationRequest
    {
        public long Id { get; set; }
        [Required(ErrorMessage = "1,Please Enter Required Parameters,4511")]
        [StringLength(30, ErrorMessage = "1,Please enter a valid  parameters,4512")]
        public string RouteName { get; set; }
        [Required(ErrorMessage = "1,Please Enter Required Parameters,4509")]
        public long ServiceID { get; set; } // spelling mistake ntrivedi 03-10-2018
        [Required(ErrorMessage = "1,Please Enter Required Parameters,4513")]
        public long ServiceProDetailId { get; set; }
        [Required(ErrorMessage = "1,Please Enter Required Parameters,4514")]
        public long ProductID { get; set; }
        [Required(ErrorMessage = "1,Please Enter Required Parameters,4515")]
        public short Priority { get; set; }
        public string StatusCheckUrl { get; set; }
        public string ValidationUrl { get; set; }
        public string TransactionUrl { get; set; }
        public long LimitId { get; set; }

        [StringLength(50, ErrorMessage = "1,Please enter a valid  parameters,4516")]
        public string OpCode { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameters,4517")]
        public enTrnType TrnType { get; set; }

        [DefaultValue(0)]
        public byte IsDelayAddress { get; set; }

        [StringLength(100, ErrorMessage = "1,Please enter a valid  parameters,4518")]
        public string ProviderWalletID { get; set; }

        public string status { get; set; }
    }

    public class RouteConfigurationResponse : BizResponseClass
    {
        public RouteConfigurationInfo Response { get; set; }
    }
    public class RouteConfigurationInfo
    {
        public long RouteId { get; set; }
    }
    public class RouteConfigurationGetAllResponse : BizResponseClass
    {
        public List<RouteConfigurationRequest> Response { get; set; }
    }
    //-----------------------------------------------Withdraw Configuration
    public class WithdrawRouteConfigRequest
    {
        public WithdrawRouteConfig Request { get; set; }
    }
    public class WithdrawRouteConfig
    {
        [Required(ErrorMessage = "1,Please Enter Required Parameters,4509")]
        public long ServiceID { get; set; }
        [StringLength(50, ErrorMessage = "1,Please enter a valid  parameters,4656")]
        public string CurrencyName { get; set; }
        public short status { get; set; }
        [Required(ErrorMessage = "1,Please Enter Required Parameter,11170")]
        [EnumDataType(typeof(enTrnType), ErrorMessage = "1,Invalid Parameter,11171")]
        public enTrnType TrnType { get; set; }
        public List<ProviderRoute> AvailableRoute { get; set; }
    }
    //Rushabh 26-04-2019
    public class GetWithdrawRouteConfig
    {        
        public long ServiceID { get; set; }
        public string CurrencyName { get; set; }
        public short status { get; set; }
        public enTrnType TrnType { get; set; }
        public List<ProviderRoute> AvailableRoute { get; set; }
    }

    public class ProviderRoute
    {
        public long Id { get; set; }
        [Required(ErrorMessage = "1,Please Enter Required Parameters,4515")]
        public short Priority { get; set; }
        [Required(ErrorMessage = "1,Please Enter Required Parameters,4513")]
        public long ServiceProDetailId { get; set; }
        public string RouteName { get; set; }
        public string AssetName { get; set; }
        [StringLength(100, ErrorMessage = "1,Please enter a valid  parameters,4518")]
        public string ProviderWalletID { get; set; }
        public decimal ConvertAmount { get; set; }
        public int ConfirmationCount { get; set; }
        public int? AccountNoLen { get; set; }//Rushabh 23-03-2019 For Address Validation
        [StringLength(30, ErrorMessage = "1,Invalid Length,11168")]
        public string AccNoStartsWith { get; set; } //Rushabh 23-03-2019 For Address Validation
        [StringLength(80, ErrorMessage = "1,Invalid Length,11169")]
        public string AccNoValidationRegex { get; set; } //Rushabh 23-03-2019 For Address Validation
    }
    ///-------------------------------------------------------------------Available route 
    public class AvailableRoute
    {
        public long Id { get; set; }
        public string ProviderName { get; set; }
        public string APIName { get; set; }
        public string APISendURL { get; set; }
    }
    public class AvailableRouteResponse : BizResponseClass
    {
        public List<AvailableRoute> Response { get; set; }
    }

    public class WithdrawConfigResponse : BizResponseClass
    {
        public List<GetWithdrawRouteConfig> Response { get; set; }
    }
    public class WithdrawConfigResponse2 : BizResponseClass
    {
        public WithdrawRouteConfig Response { get; set; }
    }

    public class GetAllWithdrawQueryResponse
    {
        public string SMSCode { get; set; }
        public long Id { get; set; }
        public string RouteName { get; set; }
        public short status { get; set; }
        public short Priority { get; set; }
        public long ServiceID { get; set; }
        public string OpCode { get; set; }
        public string ProviderWalletID { get; set; }
        public decimal ConvertAmount { get; set; }
        public int ConfirmationCount { get; set; }
        public long SerProDetailID { get; set; }
        public int TrnType { get; set; }
        public string route { get; set; }
        public string AccNoStartsWith { get; set; }
        public string AccNoValidationRegex { get; set; }
        public int AccountNoLen { get; set; }
    }

}
