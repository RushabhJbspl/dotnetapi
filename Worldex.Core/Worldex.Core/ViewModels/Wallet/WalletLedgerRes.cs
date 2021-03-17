using Worldex.Core.ApiModels;
using System;
using System.Collections.Generic;

namespace Worldex.Core.ViewModels.Wallet
{
    public class WalletLedgerRes
    {
        public long   LedgerId { get; set; }

        public decimal PreBal { get; set; }

        public decimal PostBal { get; set; }

        public decimal CrAmount { get; set; }

        public decimal DrAmount { get; set; }

        public string Remarks { get; set; }

        public decimal Amount { get; set; }

        public DateTime TrnDate { get; set; }

    }
    public class ListWalletLedgerRes: BizResponseClass
    {
        public List<WalletLedgerRes> WalletLedgers { get; set; }
      
        public BizResponseClass BizResponseObj { get; set; }
    }
    public class ListWalletLedgerResv1 : BizResponseClass
    {
        public List<WalletLedgerRes> WalletLedgers { get; set; }
        public long TotalCount { get; set; }
        public long PageSize { get; set; }
        public long PageNo { get; set; }
        // public BizResponseClass BizResponseObj { get; set; }
    }
    public class ListWalletLedgerResponse : BizResponseClass
    {
        public List<WalletLedgerResponse> WalletLedgers { get; set; }
        public long TotalCount { get; set; }
        public long PageSize { get; set; }
        public long PageNo { get; set; }
    }


    public class WalletLedgerResponse
    {
        public long LedgerId { get; set; }

        public decimal PreBal { get; set; }

        public decimal PostBal { get; set; }

        public decimal CrAmount { get; set; }

        public decimal DrAmount { get; set; }

        public string Remarks { get; set; }

        public decimal Amount { get; set; }

        public string TrnDate { get; set; }
    }
}
