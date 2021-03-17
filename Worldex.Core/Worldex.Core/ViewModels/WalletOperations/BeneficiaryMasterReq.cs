using Worldex.Core.ApiModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Worldex.Core.ViewModels.WalletOperations
{
    public class BeneficiaryMasterReq
    {
        [Required(ErrorMessage = "1,Please Enter Required Parameter,4223")]
        public string AccWalletID { get; set; }
        [Required(ErrorMessage = "1,Please Enter Required Parameter,4224")]
        public string BeneAddress { get; set; }
        public string Name { get; set; }
    }

    public class BeneficiaryUpdateReq
    {
        [Required(ErrorMessage = "1,Please Enter Required Parameter,4224")]
        public long BenefifiaryID { get; set; }
        [Required(ErrorMessage = "1,Please Enter Required Parameter,4231")]
        public string Name { get; set; }
        [Required(ErrorMessage = "1,Please Enter Required Parameter,4228")]
        public int WhitelistingBit { get; set; }
        [Required(ErrorMessage = "1,Please Enter Required Parameter,4232")]
        public short Status { get; set; }
    }
    public class BulkBeneUpdateReq
    {
        [Required(ErrorMessage = "1,Please Enter Required Parameter,4225")]
        public long[] ID { get; set; }
        [Required(ErrorMessage = "1,Please Enter Required Parameter,4226")]
        public short WhitelistingBit { get; set; }
    }
    public class BeneUpdate
    {
        public Int32 AffectedRows { get; set; }
    }
    public class BeneficiaryMasterRes
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public long BeneficiaryID { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string CoinName { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Address { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public short? IsWhiteListed { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public short? Status { get; set; }
    }
    public class BeneficiaryMasterRes1
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public long BeneficiaryID { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Address { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public short? Status { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public short? IsInternalAddress { get; set; }
    }


    public class BeneficiaryResponse
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<BeneficiaryMasterRes> Beneficiaries { get; set; }
        public BizResponseClass BizResponse { get; set; }
    }

    public class BeneficiaryResponse1
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<BeneficiaryMasterRes1> Beneficiaries { get; set; }
        public BizResponseClass BizResponse { get; set; }
    }
}
