using Worldex.Core.ApiModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Worldex.Core.ViewModels.APIConfiguration
{
    public class APIMethodsViewModel
    {
        public long? ID { get; set; }
        [Required(ErrorMessage = "1,Please Enter Required Parameters,11078")]
        [StringLength(50)]
        public string MethodName { get; set; }
        public short IsReadOnly { get; set; }
        public short IsFullAccess { get; set; }
        public short Status { get; set; }
        public List<long> SocketMethods { get; set; }
        public List<long> RestMethods { get; set; }
    }

    public class APIMethodsViewModel2
    {
        public long? ID { get; set; }
        [Required(ErrorMessage = "1,Please Enter Required Parameters,11078")]
        [StringLength(50)]
        public string MethodName { get; set; }
        public short IsReadOnly { get; set; }
        public short IsFullAccess { get; set; }
        public short Status { get; set; }
        public Dictionary<long, String> SocketMethods { get; set; }
        public Dictionary<long, String> RestMethods { get; set; }
    }
    public class APIMethodsViewModel3
    {
        public long? ID { get; set; }
        public short IsReadOnly { get; set; }
        public short IsFullAccess { get; set; }
        public short Status { get; set; }
        public List<long> SocketMethods { get; set; }
        public List<long> RestMethods { get; set; }
    }
    public class APIMethodsRequest : APIMethodsViewModel
    {

    }
    public class APIMethodsRequest2 : APIMethodsViewModel3
    {

    }
    public class APIMethodResponse : BizResponseClass
    {
        public List<APIMethodsViewModel2> Response { get; set; }
    }
    public class APIMethodResponseV2 : BizResponseClass
    {
        public List<APIMethodsInfoV2> Response { get; set; }
    }
    public class APIMethodsInfoV2
    {
        public long ID { get; set; }
        public string MethodName { get; set; }
        public short Status { get; set; }
    }
    public class RestMethodResponse : BizResponseClass
    {
        public List<RestMethodViewModel> Response { get; set; }
    }
    public class RestMethodViewModel
    {
        public long ID { get; set; }
        public string MethodName { get; set; }
        public string Path { get; set; }
    }
    public class APIMethodConfigListQryRes
    {
        public long MethodID { get; set; }
        public long ParentID { get; set; }
        public long MethodType { get; set; }
        public string MethodName { get; set; }
    }
}
