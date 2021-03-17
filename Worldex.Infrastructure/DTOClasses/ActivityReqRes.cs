using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Worldex.Infrastructure.DTOClasses
{
    public class ActivityReqRes : IRequest
    {
        public Guid Id { get; set; }
        
        public string Remark { get; set; }

        public string Connection { get; set; }

        public Guid ApplicationId { get; set; }

        //[StringLength(500)]
        //public string EndPoint { get; set; }

        public long StatusCode { get; set; }

        
        public string Channel { get; set; }

        //[StringLength(4000)]
        //public string Request { get; set; }

        //[StringLength(4000)]
        //public string Response { get; set; }

        
        public string DeviceId { get; set; }

        
        public string IPAddress { get; set; }

        public long ReturnCode { get; set; }

        
        public string ReturnMsg { get; set; }

        public long ErrorCode { get; set; }

        public string ActivityType { get; set; }

       
        public string Session { get; set; }

        
        public string AccessToken { get; set; }

        
        public string AliasName { get; set; }

        public string ModuleTypeName { get; set; }

        public string HostURLName { get; set; }


        public Guid ActivityDetId { get; set; }

        public Guid ActivityId { get; set; }

        public string Request { get; set; }

        public string Response { get; set; }
    }

    public class ActivityRes : IRequest
    {
        public Guid Id { get; set; }

        public string Remark { get; set; }

        public string Connection { get; set; }

        public Guid ApplicationId { get; set; }

        //[StringLength(500)]
        //public string EndPoint { get; set; }

        public long StatusCode { get; set; }


        public string Channel { get; set; }

        //[StringLength(4000)]
        //public string Request { get; set; }

        //[StringLength(4000)]
        //public string Response { get; set; }


        public string DeviceId { get; set; }


        public string IPAddress { get; set; }

        public long ReturnCode { get; set; }


        public string ReturnMsg { get; set; }

        public long ErrorCode { get; set; }

        public string ActivityType { get; set; }


        public string Session { get; set; }


        public string AccessToken { get; set; }


        public string AliasName { get; set; }

        public string ModuleTypeName { get; set; }

        public string HostURLName { get; set; }


        public Guid ActivityDetId { get; set; }

        public Guid ActivityId { get; set; }

        public string Request { get; set; }

        public string Response { get; set; }

        public long CreatedBy { get; set; }

        public long? UpdatedBy { get; set; }
    }
}
