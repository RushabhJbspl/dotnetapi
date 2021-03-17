using Worldex.Core.ApiModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Worldex.Core.ViewModels.Configuration
{
    public class CoinListRequestViewModel
    {
        public long ID { get; set; }
        public string ImageUrl { get; set; }
        [Required(ErrorMessage = "1,Please Enter Required Parameters,4677")]
        public short CoinType { get; set; }
        [StringLength(30, ErrorMessage = "1,Please enter a valid  parameters,4519")]
        [Required(ErrorMessage = "1,Please Enter Required Parameters,4520")]
        public string CoinName { get; set; }
        [StringLength(6, ErrorMessage = "1,Please enter a valid  parameters,4521")]
        [Required(ErrorMessage = "1,Please Enter Required Parameters,4522")]
        public string CoinAbbreviationCode { get; set; }
        [Required(ErrorMessage = "1,Please Enter Required Parameters,4527")]
        public string Introduction { get; set; }
        public string CoinTokenAddress { get; set; }
        public string APIDocumentPath { get; set; }
        [Required(ErrorMessage = "1,Please Enter Required Parameters,4678")]
        public string GithubLink { get; set; }
        [Required(ErrorMessage = "1,Please Enter Required Parameters,4679")]
        public long DecimalPlace { get; set; }
        [Required(ErrorMessage = "1,Please Enter Required Parameters,4680")]
        [Range(0, 9999999999.999999999999999999), Column(TypeName = "decimal(28, 18)")]
        public decimal TotalSupply { get; set; }
        [Required(ErrorMessage = "1,Please Enter Required Parameters,4681")]
        [Range(0, 9999999999.999999999999999999), Column(TypeName = "decimal(28, 18)")]
        public decimal CirculatingSupply { get; set; }
        [Required(ErrorMessage = "1,Please Enter Required Parameters,4682")]
        [Range(0, 9999999999.999999999999999999), Column(TypeName = "decimal(28, 18)")]
        public decimal MaxSupply { get; set; }
        [Required(ErrorMessage = "1,Please Enter Required Parameters,4683")]
        [Range(0, 9999999999.999999999999999999), Column(TypeName = "decimal(28, 18)")]
        public decimal TrnFee { get; set; }
        [Required(ErrorMessage = "1,Please Enter Required Parameters,4523")]
        public DateTime IssueDate { get; set; }
        [Range(0, 9999999999.999999999999999999), Column(TypeName = "decimal(28, 18)")]
        public decimal IssuePrice { get; set; }
        [Required(ErrorMessage = "1,Please Enter Required Parameters,4684")]
        [StringLength(250)]
        public string FirstName { get; set; }
        [StringLength(250)]
        public string LastName { get; set; }
        [Required(ErrorMessage = "1,Please Enter Required Parameters,4685")]
        public string Address { get; set; }
        public string StreetAddress { get; set; }
        public string AddressLine2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
        public string Phone { get; set; }
        [Required(ErrorMessage = "1,Please Enter Required Parameters,4686")]
        public string Email { get; set; }
        [Required(ErrorMessage = "1,Please Enter Required Parameters,4687")]
        public string ProjectName { get; set; }
        public string ProjectWebsiteLink { get; set; }
        [Required(ErrorMessage = "1,Please Enter Required Parameters,4688")]
        public List<ExplorerData> Explorer { get; set; }
        public List<CommunityData> Community { get; set; }
        public string HowFundsWereRaised { get; set; }
        [Required(ErrorMessage = "1,Please Enter Required Parameters,4689")]
        public string Premine { get; set; }
        public string CurListOnOtherExng { get; set; }
        public string WebsiteFAQ { get; set; }
        [Required(ErrorMessage = "1,Please Enter Required Parameters,4525")]
        public string WebsiteUrl { get; set; }
        [Required(ErrorMessage = "1,Please Enter Required Parameters,4690")]
        public string WhitePaper { get; set; }

        public short Status { get; set; }
    }

    public class CoinListRequestRequest : CoinListRequestViewModel
    {

    }
    public class CoinListRequestResponse : BizResponseClass
    {
        public List<CoinListRequestViewModel> Response { get; set; }
    }
    public class GetCoinRequestListRequest
    {
        public long ID { get; set; }
        public String Status { get; set; }
    }
    public class SetCoinRequestStatusRequest
    {
        [Required(ErrorMessage = "1,Please Enter Required Parameters,4674")]
        public long ID { get; set; }
        [Required(ErrorMessage = "1,Please Enter Required Parameters,4675")]
        public short Status { get; set; }
    }
}
