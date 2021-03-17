using Worldex.Core.ApiModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Worldex.Core.ViewModels.Configuration
{
    public class AppTypeResponse : BizResponseClass
    {
        public IEnumerable <AppTypeViewModel> Response { get; set; }
    }
    public class AppTypeResponseData : BizResponseClass
    {
        public AppTypeViewModel Response { get; set; }
    }
    public class AppTypeRequest
    {
        [Required]
        public long Id { get; set; }

        [Required]
        [StringLength(20)]
        public String AppTypeName { get; set; }
    }
}
