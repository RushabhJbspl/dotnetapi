using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Worldex.Core.Entities.User
{
    public class IPAddressMaster
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long AutoNo { get; set; }
        [Required]
        public System.Byte[] FromIP { get;set;}
        [Required]
        public System.Byte[] ToIP { get; set; }
        [Required]
        public int Status { get; set; }
        [Required]
        public float Longitude { get; set; }
        [Required]
        public float Lattitude { get; set; }
        [Required]
        [StringLength(5)]
        public string CountryCode { get; set; }
        [Required]
        [StringLength(150)]
        public string CountryName { get; set; }     

    }
}
