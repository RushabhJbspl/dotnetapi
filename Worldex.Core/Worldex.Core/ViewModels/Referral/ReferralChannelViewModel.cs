using Worldex.Core.ApiModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Worldex.Core.Enums;

namespace Worldex.Core.ViewModels.Referral
{
    public class ReferralChannelViewModel
    {
        [Required(ErrorMessage = "1,Please Enter Required Parameter,4901")]
        public int ReferralChannelTypeId { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameter,4901")]
        public int ReferralChannelServiceId { get; set; }

        [StringLength(1000)]
        public string ReferralReceiverAddress { get; set; }
    }

    public class ReferralChannelResponse : BizResponseClass
    {

    }
  
    public class ReferralChannelUpdateViewModel
    {
        public long Id { get; set; }
        public int UserId { get; set; }
        public int ReferralChannelTypeId { get; set; }
        public int ReferralChannelServiceId { get; set; }
        public string ReferralReceiverAddress { get; set; }       
    }    

    public class ReferralChannelListViewModel
    {
        public long Id { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public long ReferralPayTypeId { get; set; }
        public string PayTypeName { get; set; }
        public int ReferralChannelTypeId { get; set; }
        public string ChannelTypeName { get; set; }
        public int ReferralChannelServiceId { get; set; }
        public string Description { get; set; }
        public string ReferralReceiverAddress { get; set; }
        public DateTime CreatedDate { get; set; }
        public int Status { get; set; }
    }
    public class ReferralChannelAllCountViewModel
    {
        public int TotalParticipants { get; set; }
        public int Invite { get; set; }
        public int Clicks { get; set; }
        public int Converts { get; set; }

        public int EmailInvite { get; set; }
        public int SMSInvite { get; set; }

        public int FacebookShare { get; set; }
        public int TwitterShare { get; set; }        
        public int LinkedIn { get; set; }
        public int GoogleShare { get; set; }
        public int InstaShare { get; set; }
        public int Pinterest { get; set; }
        public int Telegram { get; set; }

        public int WhatsApp { get; set; }
        public int Messenger { get; set; }
        public int QRCode { get; set; }
    }
    public class ReferralChannelStatusViewModel
    {
        public long Id { get; set; }
    }
    public class ReferralChannelTypeIdViewModel
    {
        public long ChannelTypeId { get; set; }
    }

    public class ReferralChannelListResponse : BizResponseClass
    {
        public List<ReferralChannelListViewModel> ReferralChannelList { get; set; }
        public int TotalCount { get; set; }
    }

    public class ReferralChannelObjResponse : BizResponseClass
    {
        public ReferralChannelUpdateViewModel ReferralChannelObj { get; set; }
    }
    public class ReferralChannelAdminCountObjResponse : BizResponseClass
    {
        public ReferralChannelAllCountViewModel ReferralChannelAdminCount { get; set; }
    }
    public class ReferralChannelUserCountObjResponse : BizResponseClass
    {
        public ReferralChannelAllCountViewModel ReferralChannelUserCount { get; set; }
    }

    public class ReferralChannelUpdateObjResponse : BizResponseClass
    {
        public ReferralChannelUpdateViewModel ReferralChannelUpdate { get; set; }
    }

    public class ReferralChannelExistResponse : BizResponseClass
    {
        public bool ReferralChannelExist { get; set; }
    }

    public class ReferralChannelStatusResponse : BizResponseClass
    {
        public bool ReferralChannelStatus { get; set; }
    }
    public class SendReferralSMSRequest
    {
        public string MobileNumber { get; set; }
        public string SMSShareURL { get; set; }
        public int ReferralChannelTypeId { get; set; }
    }
    public class SendReferralEmailRequest
    {
        public string EmailAddress{ get; set; }
        public string EmailShareURL { get; set; }
        public int ReferralChannelTypeId { get; set; }
    }

    public class AddReferralClickRequest
    {
        //[Required(ErrorMessage = "1,IPAddress Not Found,4019")]
        //[StringLength(15, ErrorMessage = "1,Invalid IPAddress,4020")]
        public string IPAddress { get; set; }

        public string PassURL { get; set; }
    }


    public class ReferralChannelShareViewModel
    {        
        public string ReferralCode { get; set; }
        public long ChannelTypeId { get; set; }
        public long ServiceId { get; set; }       
    }
    public class ReferralShareURLResponse : BizResponseClass
    {
        public ReferralChannelShareURLViewModel ShareURL { get; set; }       
    }


    public class ReferralChannelShareURLViewModel
    {
        public string EmailURL { get; set; }
        public string SMSURL { get; set; }
        public string FacebookURL { get; set; }
        public string TwitterURL { get; set; }
        public string LinkedInURL { get; set; }
        public string GoogleURL { get; set; }
        public string InstaURL { get; set; }
        public string PintrestURL { get; set; }
        public string TelegramURL { get; set; }
        public string WhatsAppURL { get; set; }
        public string MessengerURL { get; set; }
        public string QRCodeURL { get; set; }
    }

    //public class ReferralLinkViewModel
    //{
    //    public long Id { get; set; }

    //    public string Username { get; set; }

    //    public string Email { get; set; }

    //    public string Mobile { get; set; }

    //    public DateTime CurrentTime { get; set; }

    //    public DateTime Expirytime { get; set; }

    //    public string ReferCode { get; set; }

    //    public long PromotionType { get; set; }
    //}


    public class AddReferralSchemeTypeMappingReq
    {
        public long? Id { get; set; }//0 for insert else > 0

        [Required(ErrorMessage = "1,Please Enter Required Parameter,32075")]
        public long PayTypeId { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameter,32076")]
        public long ServiceTypeMstId { get; set; }

        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        public decimal MinimumDepositionRequired { get; set; }

        [StringLength(100, ErrorMessage = "1,Invalid Parameter,32079")]
        public string Description { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameter,32077")]
        public DateTime FromDate { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameter,32078")]
        public DateTime ToDate { get; set; }

        [EnumDataType(typeof(ServiceStatus), ErrorMessage = "1,Invalid Parameter,32080")]
        public ServiceStatus? Status { get; set; }
    }

    public class ReturnCount
    {
        public int TotalCount { get; set; }
    }

    public class GetWithdarwData
    {
        public string TrnId { get; set; }
    }


    public class ReferralChannelCount
    {
        public long Id { get; set; }
        public int ReferralChannelTypeId { get; set; }
    }

    public class ReferralSchemeTypeMappingRes
    {
        public long Id { get; set; }
        
        public long PayTypeId { get; set; }
        
        public string PayTypeName { get; set; }

        public long ServiceTypeMstId { get; set; }
        
        public string ServiceTypeName { get; set; }

        public decimal MinimumDepositionRequired { get; set; }
        
        public string Description { get; set; }
        
        public DateTime FromDate { get; set; }
        
        public DateTime ToDate { get; set; }
        
        public short Status { get; set; }

        public string StrStatus { get; set; }
    }

    public class GetReferralSchemeTypeMappingRes : BizResponseClass
    {
        public ReferralSchemeTypeMappingRes Data { get; set; }
    }

    public class ListReferralSchemeTypeMappingRes : BizResponseClass
    {
        public List<ReferralSchemeTypeMappingRes> Data { get; set; }
    }

    public class AddServiceDetail
    {
        public long? Id { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameter,32083")]
        public long SchemeTypeMappingId { get; set; }

        public long MaximumLevel { get; set; }

        public long MaximumCoin { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameter,32084")]
        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        public decimal MinimumValue { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameter,32085")]
        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        public decimal MaximumValue { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameter,32086")]
        public long CreditWalletTypeId { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameter,32087")]
        public int CommissionType { get; set; } // EnAffiCommissionType

        [Required(ErrorMessage = "1,Please Enter Required Parameter,32088")]
        [Range(0, 9999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        public decimal CommissionValue { get; set; }

        [EnumDataType(typeof(ServiceStatus), ErrorMessage = "1,Invalid Parameter,32080")]
        public ServiceStatus? Status { get; set; }
    }

    public class ReferralServiceDetailRes
    {
        public long Id { get; set; }
        
        public long SchemeTypeMappingId { get; set; }

        public string SchemeTypeMappingName { get; set; }

        public long MaximumLevel { get; set; }

        public long MaximumCoin { get; set; }

        public decimal MinimumValue { get; set; }

        public decimal MaximumValue { get; set; }
        
        public long CreditWalletTypeId { get; set; }

        public string WalletTypeName { get; set; }

        public int CommissionType { get; set; }

        public string CommissionTypeName { get; set; }

        public decimal CommissionValue { get; set; }
        
        public short Status { get; set; }

        public string StrStatus { get; set; }
    }

    public class GetReferralServiceDetailRes : BizResponseClass
    {
        public ReferralServiceDetailRes Data { get; set; }
    }

    public class ListReferralServiceDetailRes : BizResponseClass
    {
        public List<ReferralServiceDetailRes> Data { get; set; }
    }

}
