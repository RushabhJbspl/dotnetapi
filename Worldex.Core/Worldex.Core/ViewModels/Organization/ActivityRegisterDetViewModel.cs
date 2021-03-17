using System;
using System.ComponentModel.DataAnnotations;

namespace Worldex.Core.ViewModels.Organization
{
    public class ActivityRegisterDetViewModel
    {
        public Guid Id { get; set; }

        public Guid ActivityId { get; set; }

        [StringLength(8000)]
        public string Request { get; set; }

        [StringLength(8000)]
        public string Response { get; set; }
    }
}
