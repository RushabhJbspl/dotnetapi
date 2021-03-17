using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Worldex.Core.ApiModels
{
    public class MessageAPIConfiguration
    {
        [Required]
        public long ServiceTypeID { get; set; }

        [Required]
        public long CommServiceTypeID {get; set;}

        [Required]
        public long CommSerproID {get; set;}

        //public long CommServiceID {get; set;}

        //public long APIId {get; set;}

        [Required]
        public int priority {get; set;}
    }
}
