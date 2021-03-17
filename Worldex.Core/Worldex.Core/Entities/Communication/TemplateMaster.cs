using Worldex.Core.Enums;
using Worldex.Core.Events;
using Worldex.Core.SharedKernel;
using System;
using System.ComponentModel.DataAnnotations;

namespace Worldex.Core.Entities.Communication
{
    public  class TemplateMaster : BizBase
    {
        [Required]
        public long TemplateID { get; set; }

        [Required]
        public long CommServiceTypeID { get; set; }

        [Required]
        [StringLength(50)]
        public string TemplateName { get; set; }

        [Required]
        public string Content { get; set; }

        [Required]
        [StringLength(200)]
        public string AdditionalInfo { get; set; }

        public void DisableService()
        {
            Status = Convert.ToInt16(ServiceStatus.Disable);
            Events.Add(new ServiceStatusEvent<TemplateMaster>(this));
        }

        public void EnableService()
        {
            Status = Convert.ToInt16(ServiceStatus.Active);
            Events.Add(new ServiceStatusEvent<TemplateMaster>(this));
        }
    }

    public class TemplateCategoryMaster : BizBase
    {        
        [Key]
        [Required]
        public new long Id { get; set; }
        
        [Required]
        public short IsOnOff { get; set; }

        [StringLength(500)]
        public string ParameterInfo { get; set; }

        [StringLength(500)]
        public string TemplateName { get; set; }

        [Required]
        public long TemplateId { get; set; } // tempalet master table - id column

        [Required]
        public short CommServiceTypeID { get; set; } // tempalet master table - servieType column

        public void DisableService()
        {
            IsOnOff = Convert.ToInt16(ServiceStatus.Disable);
            Events.Add(new ServiceStatusEvent<TemplateCategoryMaster>(this));
        }

        public void EnableService()
        {
            IsOnOff = Convert.ToInt16(ServiceStatus.Active);
            Events.Add(new ServiceStatusEvent<TemplateCategoryMaster>(this));
        }
    }
}
