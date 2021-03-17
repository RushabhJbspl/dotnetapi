using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Worldex.Core.Entities.Organization
{
    public class ActivityRegisterDetail
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Required]
        public Guid ActivityId { get; set; }

        [StringLength(8000)]
        public string Request { get; set; }

        [StringLength(8000)]
        public string Response { get; set; }
    }
}
