using Worldex.Core.ApiModels;
using Worldex.Core.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Worldex.Core.ViewModels.ControlPanel
{
    public class BlockUserReqRes
    {
        public long? ID { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameters,17043")]
        [StringLength(50,ErrorMessage ="1,Invalid Parameter Length,17044")]
        public string Address { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameters,17045")]
        public long WalletTypeID { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameters,17047")]
        [EnumDataType(typeof(UserAddressStatus), ErrorMessage = "1,Invalid Parameter,17048")]
        public UserAddressStatus Status { get; set; }

        [StringLength(150, ErrorMessage = "1,Invalid Parameter Length,17046")]
        [Required(ErrorMessage = "1,Please Enter Required Parameters,17071")]
        public string Remarks { get; set; }
    }

    public class BlockUserRes
    {
        public long Id { get; set; }
        public string Address { get; set; }
        public long BlockedByUserId { get; set; }
        public string BlockedByUserName { get; set; }
        public DateTime CreatedDate { get; set; }
        public long WalletId { get; set; }
        public long WalletTypeId { get; set; }
        public string WalletTypeName { get; set; }
        public short Status { get; set; }
        public string StrStatus { get; set; }
        public string Remarks { get; set; }
    }

    public class ListBlockUserRes : BizResponseClass
    {
        public List<BlockUserRes> Data { get; set; }
    }

    public class DestroyBlackFundReq
    {
        [Required(ErrorMessage = "1,Please Enter Required Parameters,17053")]
        public long Id { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameters,17043")]
        [StringLength(50, ErrorMessage = "1,Invalid Parameter Length,17044")]
        public string Address { get; set; }

        [StringLength(150, ErrorMessage = "1,Invalid Parameter Length,17046")]
        [Required(ErrorMessage = "1,Please Enter Required Parameters,17071")]
        public string Remarks { get; set; }
    }

    public class DestroyBlackFundRes
    {
        public long Id { get; set; }
        public string Address { get; set; }
        public long ActionByUserId { get; set; }
        public string ActionByUserName { get; set; }
        public DateTime ActionDate { get; set; }
        public string TrnHash { get; set; }
        public string Remarks { get; set; }
    }

    public class ListDestroyBlackFundRes : BizResponseClass
    {
        public List<DestroyBlackFundRes> Data { get; set; }
    }

    public class TokenTransferReq
    {
        [Required(ErrorMessage = "1,Please Enter Required Parameters,17045")]
        public long FromWalletTypeID { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameter,17058")]
        [StringLength(50, ErrorMessage = "1,Invalid Parameter Length,17059")]
        public string ToAddress { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameter,17060")]
        [Range(0, 9999999999.999999999999999999), Column(TypeName = "decimal(28, 18)")]
        public decimal Amount { get; set; }

        [StringLength(150, ErrorMessage = "1,Invalid Parameter Length,17046")]
        [Required(ErrorMessage = "1,Please Enter Required Parameters,17071")]
        public string Remarks { get; set; }
    }

    public class TokenTransferRes
    {
        public long Id { get; set; }
        public long ActionByUserId { get; set; }
        public string ActionByUserName { get; set; }
        public string FromAddress { get; set; }
        public string ToAddress { get; set; }
        public decimal Amount { get; set; }
        public string TrnHash { get; set; }
        public DateTime ActionDate { get; set; }
        public string Remarks { get; set; }
    }

    public class ListTokenTransferRes : BizResponseClass
    {
        public List<TokenTransferRes> Data { get; set; }
    }

    public class TokenSupplyReq
    {
        [Required(ErrorMessage = "1,Please Enter Required Parameters,17060")]
        //[Range(0, 9999999999.999999999999999999), Column(TypeName = "decimal(28, 18)")]
        [Range(0, 10000000,ErrorMessage ="1,Invalid Amount,17063")]        
        public int Amount { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameters,17045")]
        public long WalletTypeId { get; set; }

        [StringLength(150, ErrorMessage = "1,Invalid Parameter Length,17046")]
        [Required(ErrorMessage = "1,Please Enter Required Parameters,17071")]
        public string Remarks { get; set; }

    }

    public class TokenSupplyRes
    {
        public long Id { get; set; }
        public long WalletTypeId { get; set; }
        public string WalletTypeName { get; set; }
        public decimal Amount { get; set; }
        public long ActionByUserId { get; set; }
        public string ActionByUserName { get; set; }
        public string TrnHash { get; set; }
        public short ActionType { get; set; }
        public string ActionTypeName { get; set; }
        public DateTime ActionDate { get; set; }
        public string ContractAddress { get; set; }
        public string Remarks { get; set; }
    }

    public class ListTokenSupplyRes : BizResponseClass
    {
        public List<TokenSupplyRes> Data { get; set; }
    }

    public class SetTransferFeeReq
    {
        [Required(ErrorMessage = "1,Please Enter Required Parameters,17045")]
        public long WalletTypeId { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameters,17067")]
        [Range(0, 9, ErrorMessage = "1,Invalid Value,17064")]
        public int BasePoint { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameters,17068")]
        [Range(5, 100, ErrorMessage = "1,Invalid Value,17065")]
        public int Maxfee { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameters,17069")]
        [Range(0, 4, ErrorMessage = "1,Invalid Value,17066")]
        public int Minfee { get; set; }

        [StringLength(150, ErrorMessage = "1,Invalid Parameter Length,17046")]
        [Required(ErrorMessage = "1,Please Enter Required Parameters,17071")]
        public string Remarks { get; set; }
    }

    public class SetTransferFeeRes
    {
        public long Id { get; set; }        
        public long WalletTypeId { get; set; }
        public string WalletTypeName { get; set; }
        public long ActionByUserId { get; set; }
        public string ActionByUserName { get; set; }
        public string TrnHash { get; set; }
        public int BasePoint { get; set; }
        public int Maxfee { get; set; }
        public int Minfee { get; set; }
        public DateTime ActionDate { get; set; }
        public string ContractAddress { get; set; }
        public string Remarks { get; set; }
    }
    public class ListSetTransferFeeRes : BizResponseClass
    {
        public List<SetTransferFeeRes> Data { get; set; }
    }
}
