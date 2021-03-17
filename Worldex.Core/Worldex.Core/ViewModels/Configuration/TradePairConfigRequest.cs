using Worldex.Core.ApiModels;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Worldex.Core.ViewModels.Configuration
{
    public class TradePairConfigRequest
    {
        public long Id { get; set; }
        public string MarketName { get; set; }
        public string PairName { get; set; }
        [Required(ErrorMessage = "1,Please Enter Required Parameters,4531")]
        public long SecondaryCurrencyId { get; set; }
        [Required(ErrorMessage = "1,Please Enter Required Parameters,4533")]
        public long BaseCurrencyId { get; set; }
        [Required(ErrorMessage = "1,Please Enter Required Parameters,4534")]
        [Range(0, 9999999999.999999999999999999, ErrorMessage = "1,Please enter a valid parameters,4549"), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        [JsonProperty(PropertyName = "CurrentRate")]
        public decimal Currentrate { get; set; }
        [Required(ErrorMessage = "1,Please Enter Required Parameters,4535")]
        [Range(0, 9999999999.999999999999999999, ErrorMessage = "1,Please enter a valid parameters,4550"), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        public decimal BuyMinQty { get; set; }
        [Required(ErrorMessage = "1,Please Enter Required Parameters,4536")]
        [Range(0, 9999999999.999999999999999999, ErrorMessage = "1,Please enter a valid parameters,4551"), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        public decimal BuyMaxQty { get; set; }
        [Required(ErrorMessage = "1,Please Enter Required Parameters,4537")]
        [Range(0, 9999999999.999999999999999999, ErrorMessage = "1,Please enter a valid parameters,4552"), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        public decimal SellMinQty { get; set; }
        [Required(ErrorMessage = "1,Please Enter Required Parameters,4538")]
        [Range(0, 9999999999.999999999999999999, ErrorMessage = "1,Please enter a valid parameters,4553"), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        public decimal SellMaxQty { get; set; }
        
        [Required(ErrorMessage = "1,Please Enter Required Parameters,4541")]
        [Range(0, 9999999999.999999999999999999, ErrorMessage = "1,Please enter a valid parameters,4556"), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        public decimal CurrencyPrice { get; set; }
        [Required(ErrorMessage = "1,Please Enter Required Parameters,4542")]
        [Range(0, 9999999999.999999999999999999, ErrorMessage = "1,Please enter a valid parameters,4557"), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        public decimal Volume { get; set; }
        [Required(ErrorMessage = "1,Please Enter Required Parameters,4543")]
        [Range(0, 9999999999.999999999999999999, ErrorMessage = "1,Please enter a valid parameters,4558"), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        public decimal SellPrice { get; set; }
        [Required(ErrorMessage = "1,Please Enter Required Parameters,4544")]
        [Range(0, 9999999999.999999999999999999, ErrorMessage = "1,Please enter a valid parameters,4559"), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        public decimal BuyPrice { get; set; }
        [Required(ErrorMessage = "1,Please Enter Required Parameters,4545")]
        [Range(0, 9999999999.999999999999999999, ErrorMessage = "1,Please enter a valid parameters,4560"), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        public decimal BuyMinPrice { get; set; }
        [Required(ErrorMessage = "1,Please Enter Required Parameters,4546")]
        [Range(0, 9999999999.999999999999999999, ErrorMessage = "1,Please enter a valid parameters,4561"), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        public decimal BuyMaxPrice { get; set; }
        [Required(ErrorMessage = "1,Please Enter Required Parameters,4547")]
        [Range(0, 9999999999.999999999999999999, ErrorMessage = "1,Please enter a valid parameters,4562"), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        public decimal SellMinPrice { get; set; }
        [Required(ErrorMessage = "1,Please Enter Required Parameters,4548")]
        [Range(0, 9999999999.999999999999999999, ErrorMessage = "1,Please enter a valid parameters,4563"), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        public decimal SellMaxPrice { get; set; }
        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        public decimal BuyFees { get; set; }
        [Required]
        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        public decimal SellFees { get; set; }
        [Required]
        public string FeesCurrency { get; set; }

        //komal 12-10-2018 as pr front requierment
        [Required]
        public short Status { get; set; }
        public string StatusText { get; set; }
        public short? ChargeType { get; set; }
        public long? OpenOrderExpiration { get; set; }
        public short QtyLength { get; set; }
        public short PriceLength { get; set; }
        public short AmtLength { get; set; }

        //Rushabh 06-01-2020 Added PairPercentage Parameter As Per Front Requirement.
        [Required]
        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        public decimal PairPercentage { get; set; }

        [NotMapped]
        public short IsMargin { get; set; } = 0;//Rita 12-3-19 for Margin Trading

}
    //-----------------------------------------
    public class TradePairConfigResponse : BizResponseClass
    {
        public TradePairConfigInfo Response { get; set; }
    }
    public class TradePairConfigInfo
    {
        public long PairId { get; set; }
    }
    //------------------------------------------------
    public class TradePairConfigGetResponse : BizResponseClass
    {
        public TradePairConfigRequest Response { get; set; }
    }
    public class TradePairConfigGetAllResponse : BizResponseClass
    {
        public List<TradePairConfigRequest> Response { get; set; }
    }
    //---------------------------------------------
    public class ListPairResponse : BizResponseClass
    {
        public List<ListPairInfo> Response { get; set; }
    }
    public class ListPairInfo
    {
        public long PairId { get; set; }
        public string PairName { get; set; }
        public string BaseCurrency { get; set; }
        public string ChildCurrency { get; set; }
    }
    public class GetAllTradePairConfigResponse : BizResponseClass
    {
        public List<TradePairConfigRequest> Response { get; set; }
        public long TotalPage { get; set; }
        public long PageSize { get; set; }
        public long Count { get; set; }
    }
}
