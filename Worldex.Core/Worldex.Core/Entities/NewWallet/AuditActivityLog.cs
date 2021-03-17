using Worldex.Core.SharedKernel;
using System.ComponentModel.DataAnnotations;

namespace Worldex.Core.Entities.NewWallet
{
    public class AuditActivityLog : BizBase
    {
        [Required]
        public string EntityType { get; set; }
        [Required]
        public string ColumnName { get; set; }
        [Required]
        public string OldValue { get; set; }
        [Required]
        public string NewValue { get; set; }
        public string Remarks { get; set; }
    }
}
