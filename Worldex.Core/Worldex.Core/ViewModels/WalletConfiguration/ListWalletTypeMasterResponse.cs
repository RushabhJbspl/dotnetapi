using Worldex.Core.ApiModels;
using Worldex.Core.Entities.MarginEntitiesWallet;
using Worldex.Core.Entities.Wallet;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Worldex.Core.ViewModels.WalletConfiguration
{
    public class ListWalletTypeMasterResponse: BizResponseClass
    {
        public IEnumerable<WalletTypeMaster> walletTypeMasters { get; set; }
    }

    public class ListMarginWalletTypeMasterResponse : BizResponseClass
    {
        public IEnumerable<MarginWalletTypeMaster> walletTypeMasters { get; set; }
    }

    public class ListArbitrageWalletTypeMasterResponse : BizResponseClass
    {
        public IEnumerable<ArbitrageWalletTypeMaster> ArbitrageWalletTypeMasters { get; set; }
    }
    public class ArbitrageWalletTypeMasterRes 
    {        
        public new long Id { get; set; }
      
        public string WalletTypeName { get; set; }
       
        public string Description { get; set; }
      
        public short IsDepositionAllow { get; set; }
       
        public short IsWithdrawalAllow { get; set; }
        
        public short IsTransactionWallet { get; set; }

        public short? IsDefaultWallet { get; set; }        

        public short? IsLocal { get; set; }//add for Call ERC-20 API
    }

    public class ArbitrageWalletMasterRes : BizResponseClass
    {
        public IEnumerable<ArbitrageWalletMaster> ArbitrageWalletMaster { get; set; }
    }
    public class ArbitrageWalletMasterResSub
    {
        public new long Id { get; set; }
       
        public string WalletName { get; set; }
        
        [Range(0, 9999999999.999999999999999999), DataType(System.ComponentModel.DataAnnotations.DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        public decimal Balance { get; set; }  
       
        [StringLength(50)]      
        public string AccWalletID { get; set; } // dynamically generated accountid 
      
        public long UserID { get; set; }
       
        [Range(0, 9999999999.999999999999999999), DataType(System.ComponentModel.DataAnnotations.DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        public decimal OutBoundBalance { get; set; }
       
        [Range(0, 9999999999.999999999999999999), DataType(System.ComponentModel.DataAnnotations.DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        public decimal InBoundBalance { get; set; }
    }
}
