using Worldex.Core.ApiModels;
using Worldex.Core.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Worldex.Core.ViewModels.Transaction
{
    public class TradingConfigurationViewModel
    {
        public string Name { get; set; }

        public long Id { get; set; }

        public DateTime CreatedDate { get; set; }

        public short Status { get; set; }
    }

    public class TradingConfigurationList : BizResponseClass
    {
        public List<TradingConfigurationViewModel> Data { get; set; }
    }

    public class TradingConfigurationReq
    {
        [Required(ErrorMessage = "1,Please Enter Required Parameters,11176")]
        public long Id { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameters,11177")]
        public ServiceStatus Status { get; set; }
    }
}
