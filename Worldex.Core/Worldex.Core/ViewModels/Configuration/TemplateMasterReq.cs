using Worldex.Core.ApiModels;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Worldex.Core.ViewModels.Configuration
{
    public class TemplateMasterReq
    {
        public long ID { get; set; }

        // khushali 09-01-2019 for template configuration changes - category wise
        [Required(ErrorMessage = "1,Enter Required Parameter Value,4708")] // chnage error code
        public long TemplateID { get; set; }

        [Required(ErrorMessage = "1,Enter Required Parameter Value,4702")]
        public long CommServiceTypeID { get; set; }

        [Required(ErrorMessage = "1,Enter Required Parameter Value,4703")]
        [StringLength(50, ErrorMessage = "1,Invalid Parameter Value,4704")]
        public string TemplateName { get; set; }

        [Required(ErrorMessage = "1,Enter Required Parameter Value,4705")]
        public string Content { get; set; }

        [Required(ErrorMessage = "1,Enter Required Parameter Value,4706")]
        [StringLength(200, ErrorMessage = "1,Invalid Parameter Value,4707")]
        public string AdditionalInfo { get; set; }
    }

    public class TemplateMasterRes : BizResponseClass
    {
        public TemplateResponse TemplateMasterObj { get; set; }
    }

    public class TemplateCategoryMasterRes : BizResponseClass
    {
        public List<TemplateResponse> TemplateMasterObj { get; set; }
        public short IsOnOff { get; set; }
        public long TemplateID { get; set; }
    }

    public class ListTemplateMasterRes : BizResponseClass
    {
        public List<TemplateResponse> TemplateMasterObj { get; set; }
        public List<KeyValuePair<string, int>> Template { get; set; }
    }

    public class TemplateRes : BizResponseClass
    {
        public List<Template> Result { get; set; }
    }

    public class Template
    {
        public string Value { get; set; }
        public long Key { get; set; }
        public int IsOnOff { get; set; }
        public long TemplateID { get; set; }
        public short ServiceType { get; set; }
    }

    public class ParameterInfo
    {
        public string Name { get; set; }
        public string Aliasname { get; set; }
    }

    public class ListTemplateParameterInfoRes : BizResponseClass
    {
        public List<TemplateParameterInfoRes> templateParameterInfoList { get; set; }
    }

    public class TemplateParameterInfoRes
    {
        public List<ParameterInfo> ParameterInfo { get; set; }
        public short IsOnOff { get; set; }
        public long TemplateType { get; set; }
        public long TemplateID { get; set; }
    }

    public class TemplateResponse
    {
        public long ID { get; set; }
        public long TemplateID { get; set; }
        public string TemplateType { get; set; }
        public long CommServiceTypeID { get; set; }
        public string CommServiceType { get; set; }
        public string TemplateName { get; set; }
        public string Content { get; set; }
        public string AdditionalInfo { get; set; }
        public short Status { get; set; }
        public List<ParameterInfo> ParameterInfo { get; set; }
    }
}
