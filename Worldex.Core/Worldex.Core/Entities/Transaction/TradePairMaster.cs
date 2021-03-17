using Worldex.Core.Enums;
using Worldex.Core.Events;
using Worldex.Core.SharedKernel;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Worldex.Core.Entities.Transaction
{
    public class TradePairMaster : BizBase
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public new long Id
        {
            get => Convert.ToInt64(SecondaryCurrencyId.ToString() + BaseCurrencyId.ToString());
            set => Convert.ToInt64(SecondaryCurrencyId.ToString() + BaseCurrencyId.ToString());
        }
        [Required]
        public string PairName { get; set; }

        [Required]
        public long SecondaryCurrencyId { get; set; }

        [Required]
        public long WalletMasterID { get; set; }

        [Required]
        public long BaseCurrencyId { get; set; }

        public short Priority { get; set; }//rita 01-5-19 for manage list

        public void MakePairActive()
        {
            Status = Convert.ToInt16(ServiceStatus.Active);
            AddValueChangeEvent();
        }
        public void MakePairInActive()
        {
            Status = Convert.ToInt16(ServiceStatus.InActive);
            AddValueChangeEvent();
        }
        public void AddValueChangeEvent()
        {
            Events.Add(new ServiceStatusEvent<TradePairMaster>(this));
        }
    }

    public class TradePairMasterMargin : BizBase
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public new long Id
        {
            get => Convert.ToInt64(SecondaryCurrencyId.ToString() + BaseCurrencyId.ToString());
            set => Convert.ToInt64(SecondaryCurrencyId.ToString() + BaseCurrencyId.ToString());
        }
        [Required]
        public string PairName { get; set; }

        [Required]
        public long SecondaryCurrencyId { get; set; }       

        [Required]
        public long WalletMasterID { get; set; }

        [Required]
        public long BaseCurrencyId { get; set; }

        public short Priority { get; set; }//rita 01-5-19 for manage list

        public void MakePairActive()
        {
            Status = Convert.ToInt16(ServiceStatus.Active);
            AddValueChangeEvent();
        }
        public void MakePairInActive()
        {
            Status = Convert.ToInt16(ServiceStatus.InActive);
            AddValueChangeEvent();
        }
        public void AddValueChangeEvent()
        {
            Events.Add(new ServiceStatusEvent<TradePairMasterMargin>(this));
        }
    }

    public class TradePairMasterArbitrage : BizBase
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public new long Id
        {
            get => Convert.ToInt64(SecondaryCurrencyId.ToString() + BaseCurrencyId.ToString());
            set => Convert.ToInt64(SecondaryCurrencyId.ToString() + BaseCurrencyId.ToString());
        }
        [Required]
        public string PairName { get; set; }

        [Required]
        public long SecondaryCurrencyId { get; set; }

        [Required]
        public long WalletMasterID { get; set; }

        [Required]
        public long BaseCurrencyId { get; set; }

        public short Priority { get; set; }

        public void MakePairActive()
        {
            Status = Convert.ToInt16(ServiceStatus.Active);
            AddValueChangeEvent();
        }
        public void MakePairInActive()
        {
            Status = Convert.ToInt16(ServiceStatus.InActive);
            AddValueChangeEvent();
        }
        public void AddValueChangeEvent()
        {
            Events.Add(new ServiceStatusEvent<TradePairMasterArbitrage>(this));
        }
    }
}
