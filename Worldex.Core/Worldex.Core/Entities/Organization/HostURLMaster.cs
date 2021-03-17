using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Worldex.Core.Entities.Organization
{
    public class HostURLMaster
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [StringLength(500)]
        public string HostURL { get; set; }

        [StringLength(500)]
        public string AliasName { get; set; }

    }
}
