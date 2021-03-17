using Worldex.Core.ApiModels;
using System.Collections.Generic;

namespace Worldex.Core.ViewModels.WalletOperations
{
    public class TransferInOutRes
    {
        public List<TransfersRes> Transfers { get; set; }
        public long TotalCount { get; set; }
        public long PageSize { get; set; }
        public long PageNo { get; set; }
        public BizResponseClass BizResponseObj { get; set; }
    }
    public class TransfersRes
    {
        public long AutoNo { get; set; }

        public string TrnID { get; set; }

        public string WalletType { get; set; }

        public string Address { get; set; }

        public string UserName { get; set; }

        public string Email { get; set; }

        public long Confirmations { get; set; }

        public decimal Amount { get; set; }

        public string ConfirmedTime { get; set; }

        public short? ConfirmationCount { get; set; }

        public string ExplorerLink { get; set; }

        public long OrgId { get; set; }

        public string OrganizationName { get; set; }

        public string StrAmount { get; set; }

        public int UserId { get; set; }
    }
}
