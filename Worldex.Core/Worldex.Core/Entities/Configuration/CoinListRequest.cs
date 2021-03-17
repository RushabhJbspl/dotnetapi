using Worldex.Core.SharedKernel;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Worldex.Core.Entities.Configuration
{
    public class CoinListRequest : BizBase
    {
        public string IconUrl { get; set; }
        [Required]
        public short CoinType { get; set; }
        [StringLength(30)]
        [Required]
        public string CoinName { get; set; }
        [StringLength(6)]
        [Required]
        public string CoinAbbreviationCode { get; set; }
        [Required]
        public string Introduction { get; set; }
        public string CoinTokenAddress {get;set;}
        public string APIDocumentPath { get; set; }
        [Required]
        public string GithubLink { get; set; }
        [Required]
        
        public long DecimalPlace { get; set; }
        [Required]
        [Range(0, 9999999999.999999999999999999), Column(TypeName = "decimal(28, 18)")]
        public decimal TotalSupply { get; set; }
        [Required]
        [Range(0, 9999999999.999999999999999999), Column(TypeName = "decimal(28, 18)")]
        public decimal CirculatingSupply { get; set; }
        [Required]
        [Range(0, 9999999999.999999999999999999), Column(TypeName = "decimal(28, 18)")]
        public decimal MaxSupply { get; set; }
        [Required]
        [Range(0, 9999999999.999999999999999999), Column(TypeName = "decimal(28, 18)")]
        public decimal TrnFee { get; set; }
        [Required]
        public DateTime IssueDate { get; set; }
        [Range(0, 9999999999.999999999999999999), Column(TypeName = "decimal(28, 18)")]
        public decimal IssuePrice { get; set; }
        [Required]
        [StringLength(250)]
        public string FirstName { get; set; }
        [StringLength(250)]
        public string LastName { get; set; }
        [Required]
        public string Address { get; set; }
        public string StreetAddress { get; set; }
        public string AddressLine2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
        public string Phone { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string ProjectName { get; set; }
        public string ProjectWebsiteLink { get; set; }

        [Required]
        [Column(TypeName = "text")]
        public string Explorer { get; set; }
        [Column(TypeName = "text")]
        public string Community { get; set; }

        public string HowFundsWereRaised { get; set; }
        [Required]
        public string Premine { get; set; }
        public string CurListOnOtherExng { get; set; }
        
        
        public string WebsiteFAQ { get; set; }
        [Required]
        public string WebsiteUrl { get; set; }
        [Required]
        public string WhitePaper { get; set; }
        
    }

}
