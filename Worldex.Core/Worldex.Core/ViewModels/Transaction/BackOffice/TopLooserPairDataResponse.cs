using Worldex.Core.ApiModels;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Worldex.Core.ViewModels.Transaction.BackOffice
{
    public class TopLooserGainerPairData
    {
        public long PairId { get; set; }
        public string PairName { get; set; }
        [Range(0, 9999999999.999999999999999999), Column(TypeName = "decimal(28, 18)")]
        public decimal ChangePer { get; set; }
        [Range(0, 9999999999.999999999999999999), Column(TypeName = "decimal(28, 18)")]
        public decimal Volume { get; set; }
        [Range(0, 9999999999.999999999999999999), Column(TypeName = "decimal(28, 18)")]
        public decimal LTP { get; set; }
        [Range(0, 9999999999.999999999999999999), Column(TypeName = "decimal(28, 18)")]
        public decimal ChangeValue { get; set; }
        public decimal High { get; set; }
        public decimal Low { get; set; }
        public short UpDownBit { get; set; }
    }

    public class TopLooserGainerPairDataResponse : BizResponseClass
    {
       public List<TopLooserGainerPairData> Response { get; set; }
    }
}
