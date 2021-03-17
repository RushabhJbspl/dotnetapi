using Worldex.Core.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Worldex.Infrastructure.DTOClasses
{
    public class CreateOrderRequest 
    {
        [Required]
        [Range(1, 9999999999.999999999999999999), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(28, 18)")]
        public decimal OrderAmt { get; set; }

        [Required]
        [StringLength(150)]
        public string ORemarks { get; set; }

        [Required]
        public long OWalletMasterID { get; set; }

        [Required]
        public long DWalletMasterID { get; set; }

        [Required]
        public enOrderType OrderType { get; set; }

    }

    public class CreateOrderResponse : BizResponse
    {
        public long OrderID { get; set; }

        public string ORemarks { get; set; }        
    }
}
