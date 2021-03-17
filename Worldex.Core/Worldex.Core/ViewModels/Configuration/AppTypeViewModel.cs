using System;
using System.ComponentModel.DataAnnotations;

namespace Worldex.Core.ViewModels.Configuration
{
    public class AppTypeViewModel
    {
        public long Id { get; set; }

        [Required]
        [StringLength(20)]
        public string AppTypeName { get; set; }
    }

    public class TrnTypeViewModel
    {
        public long Id { get; set; }

        [Required]
        [StringLength(20)]
        public string TrnTypeName { get; set; }
    }
}
