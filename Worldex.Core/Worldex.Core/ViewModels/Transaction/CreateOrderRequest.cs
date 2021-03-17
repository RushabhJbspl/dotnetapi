using Worldex.Core.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Worldex.Core.ViewModels.Transaction
{
    public class CreateTransactionRequest
    {
        [Required(ErrorMessage = "1,Please Enter Required Parameters,4601")]
        public long CurrencyPairID { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameters,4602")]
        public string DebitWalletID { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameters,4603")]
        public string CreditWalletID { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameters,4604")]
        [Range(0, 100)]
        public decimal FeePer { get; set; }
        [Required(ErrorMessage = "1,Please Enter Required Parameters,4605")]
        public decimal Fee { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameters,4606")]
        public short TrnMode { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameters,4607")]
        [Range(0, 9999999999.999999999999999999)]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameters,4608")]
        [Range(0, 9999999999.999999999999999999)]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameters,4609")]
        [Range(0, 9999999999.999999999999999999)]
        public decimal Total { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameters,4610")]
        [EnumDataType(typeof(enTransactionMarketType))]
        public enTransactionMarketType OrderType { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameters,4611")]
        [EnumDataType(typeof(enTrnType),ErrorMessage = "1,Invalid Parameter Value,4612")]
        [Range(4, 5, ErrorMessage = "1,Invalid Parameter Value,4612")]
        public enTrnType OrderSide { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameters,4613")]
        public long Nonce { get; set; }//Timestamp
        
        [Range(0, 9999999999.999999999999999999)]
        public decimal StopPrice { get; set; }

        //Rita 4-6-19 for Arbitrage Trading
        // khushali 10-06-2019 Change Data type decimal to long
        public long RouteID { get; set; } = 0;
        //khushali 11-06-19 Specific LP Order for Arbitrage Trading
        public short LPType { get; set; } = 0;
 
    }
    //Rita 11-6-19 for Arbitrage bulk transaction
    public class CreateTransactionRequestBulk
    {
        public List<CreateTransactionRequest> MultipleOrderList { get; set; }
    }

    public class WithdrawalRequest
    {
        [Required(ErrorMessage = "1,Please Enter Required Parameters,4614")]
        public string asset { get; set; }//Timestamp

        [Required(ErrorMessage = "1,Please Enter Required Parameters,4615")]
        public string address { get; set; }//Timestamp

        [Required(ErrorMessage = "1,Please Enter Required Parameters,4602")]
        public string DebitWalletID { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameters,4606")]
        public short TrnMode { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameters,4608")]
        [Range(0, 9999999999.999999999999999999)]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameters,4616")]
        public enWhiteListingBit WhitelistingBit { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameters,4617")]
        public string AddressLabel { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameters,4613")]
        public long Nonce { get; set; }//Timestamp
    }
    public class WithdrawalConfirmationRequest
    {
        [Required(ErrorMessage = "1,Please Enter Required Parameters,11013")]
        public string RefNo { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameters,11014")]
        public int TransactionBit { get; set; }
    }
    //Rita 9-4-18 for margin trading
    public class RespnseToWallet
    {
        public enResponseCodeService ReturnCode { get; set; }
        public enErrorCode ErrorCode { get; set; }
        public string ReturnMsg { get; set; }
        public Guid Guid { get; set; }
        public long TrnNo { get; set; }
    }

    public class CloseOpenPostionRequestMargin
    {
        public long PairID { get; set; }
    }

    public class ConfirmationMailLinkToken
    {
        public long UserId { get; set; }
        public string RefNo { get; set; }
    }
}
