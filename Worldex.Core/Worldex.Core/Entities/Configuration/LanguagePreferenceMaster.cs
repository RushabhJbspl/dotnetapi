using Worldex.Core.SharedKernel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Worldex.Core.Entities.Configuration
{
    public class LanguagePreferenceMaster : BizBase
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public new long Id { get; set; }

        [Required]
        public string LanguageId { get; set; }

        [Key]
        [Required]
        [StringLength(5)]
        public string PreferedLanguage { get; set; }//Locale

        [Required]
        public string Name { get; set; }

        [Required]
        public string Icon { get; set; }
    }
}
