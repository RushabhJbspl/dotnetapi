using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Worldex.Core.ApiModels.Chat
{
    public class ConnetedClientList
    {
        public string ConnectionId { get; set; }
    }

    public class ConnetedClientToken
    {
        public string Token { get; set; }
    }

    public class ChatHistory
    {
        public string Message { get; set; }
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Time { get; set; }
    }

    public class ActivityNotificationMessage
    {
        public int MsgCode { get; set; }
        public string Param1 { get; set; }
        public string Param2 { get; set; }
        public string Param3 { get; set; }
        public string Param4 { get; set; }
        public string Param5 { get; set; }
        public string Param6 { get; set; }
        public short Type
        {
            get;set;
        }
    }

    public class CommunicationParamater
    {
        public string Param1 { get; set; }
        public string Param2 { get; set; }
        public string Param3 { get; set; }
        public string Param4 { get; set; }
        public string Param5 { get; set; }
        public string Param6 { get; set; }
        public string Param7 { get; set; }
        public string Param8 { get; set; }
        public string Param9 { get; set; }
        public string Param10 { get; set; }
        public string Param11 { get; set; }
        public string Param12 { get; set; }
        public string Param13 { get; set; }
        public string Param14 { get; set; }
        public string Param15 { get; set; }
    }

    public class USerDetail
    {
        public string UserName { get; set; }
        public string Reason { get; set; }
        public string ConnectionID { get; set; }
    }
}
