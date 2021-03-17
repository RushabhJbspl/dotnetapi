using Worldex.Core.ApiModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Worldex.Core.ViewModels.Configuration
{

    public class ServiceConfigurationRequestNew
    {
        [Required(ErrorMessage = "1,Please Enter Required Parameters,11246")]
        public long ServiceProviderId { get; set; }

        public long ServiceId { get; set; }

        [StringLength(30, ErrorMessage = "1,Please enter a valid  parameters,4519")]
        [Required(ErrorMessage = "1,Please Enter Required Parameters,4520")]
        public string Name { get; set; }

        [StringLength(10, ErrorMessage = "1,Please enter a valid  parameters,4521")]
        [Required(ErrorMessage = "1,Please Enter Required Parameters,4522")]
        public string SMSCode { get; set; }

        public long TotalSupply { get; set; }

        public long MaxSupply { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameters,4523")]
        public DateTime IssueDate { get; set; }

        [Range(0, 9999999999.999999999999999999), Column(TypeName = "decimal(18, 8)")]
        public decimal IssuePrice { get; set; }

        public long CirculatingSupply { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameters,4525")]
        public string WebsiteUrl { get; set; }

        public List<ExplorerData> Explorer { get; set; }

        public List<CommunityData> Community { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameters,4527")]
        public string Introduction { get; set; }

        public short IsTransaction { get; set; }
        public short IsWithdraw { get; set; }
        public short IsDeposit { get; set; }
        public short IsBaseCurrency { get; set; }
        public string StatusText { get; set; }
        public short Status { get; set; }
        public long WalletTypeID { get; set; }
        public short IsMargin { get; set; } = 0;//Rita 25-2-19,   1-for Margin trading
        public short IsOnlyIntAmountAllow { get; set; } = 0;//2019-6-7 only int is allow

        [Range(0, 9999999999.999999999999999999, ErrorMessage = "1,Please Enter Valid Parameter,14161"), DataType(DataType.Currency)]        
        public decimal Rate { get; set; }

        //[Required(ErrorMessage = "1,Please Enter Required parameters,4206")]
        public long CurrencyTypeId { get; set; }
    }

    public class ServiceConfigurationRequest
    {
        public long ServiceId { get; set; }
        [StringLength(30, ErrorMessage = "1,Please enter a valid  parameters,4519")]
        [Required(ErrorMessage = "1,Please Enter Required Parameters,4520")]
        public string Name { get; set; }
        [StringLength(10, ErrorMessage = "1,Please enter a valid  parameters,4521")]
        [Required(ErrorMessage = "1,Please Enter Required Parameters,4522")]
        public string SMSCode { get; set; }
        public long TotalSupply { get; set; } 
        public long MaxSupply { get; set; }
        [Required(ErrorMessage = "1,Please Enter Required Parameters,4523")]
        public DateTime IssueDate { get; set; }
        [Range(0, 9999999999.999999999999999999), Column(TypeName = "decimal(18, 8)")]
        public decimal IssuePrice { get; set; }
        public long CirculatingSupply { get; set; }
        [Required(ErrorMessage = "1,Please Enter Required Parameters,4525")]
        public string WebsiteUrl { get; set; }
        public List<ExplorerData> Explorer { get; set; }
        public List<CommunityData> Community { get; set; }
        [Required(ErrorMessage = "1,Please Enter Required Parameters,4527")]
        public string Introduction { get; set; }   
        public short IsTransaction { get; set; }
        public short IsWithdraw { get; set; }
        public short IsDeposit { get; set; }
        public short IsBaseCurrency { get; set; }
        public string StatusText { get; set; }
        public short Status { get; set; }
        public long WalletTypeID { get; set; }
        public short IsMargin { get; set; } = 0;//Rita 25-2-19,   1-for Margin trading
        public short IsOnlyIntAmountAllow { get; set; } = 0;//2019-6-7 only int is allow

        [Range(0, 9999999999.999999999999999999, ErrorMessage = "1,Please Enter Valid Parameter,14161"), DataType(DataType.Currency)]
        //[Required(ErrorMessage = "1,Please Enter Required parameters,4206")]
        public decimal Rate { get; set; }

        //[Required(ErrorMessage = "1,Please Enter Required parameters,4206")]
        public long CurrencyTypeId { get; set; }
    }

    public class ExplorerData
    {
        public string Data { get; set; }
    }
    public class CommunityData
    {
        public string Data { get; set; }
    }
    public class ServiceConfigurationResponse : BizResponseClass
    {
        public ServiceConfigurationInfo Response { get; set; }
    }
    public class ServiceConfigurationInfo
    {
        public long ServiceId { get; set; }
    }
    public class ServiceConfigurationGetResponse : BizResponseClass
    {
        public ServiceConfigurationRequest Response { get; set; }
    }

}
