using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Worldex.Core.Entities.User
{
    public partial class ApplicationRole : IdentityRole<int>
    {
        [DataType(DataType.DateTime)]
        public DateTime CreatedDate { get; set; }

        public long CreatedBy { get; set; }

        public long? UpdatedBy { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime? UpdatedDate { get; set; }

        [Required]
        public short Status { get; set; }

        [StringLength(250)]
        public string Description { get; set; }
    }
}
