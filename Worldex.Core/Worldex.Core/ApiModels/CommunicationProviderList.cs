namespace Worldex.Core.ApiModels
{
    public class CommunicationProviderList
    {
        public string  SenderID { get; set; }
        public string  SendURL { get; set; }
        public int  Priority { get; set; }
        public string  SMSBalURL { get; set; }
        public long  RequestID { get; set; }
        public string  RequestFormat { get; set; }
        public string  ContentType { get; set; }
        public string  MethodType { get; set; }
        public string  ServiceName { get; set; }
        public string  UserID { get; set; }
        public string  Password { get; set; }
        public decimal  Balance { get; set; }
        public string ResponseSuccess { get; set; } = "";
        public string ResponseFailure { get; set; } = "";
        public string BalanceRegex { get; set; } = "";
        public string StatusRegex { get; set; } = "";
        public string StatusMsgRegex { get; set; } = "";
        public string ResponseCodeRegex { get; set; } = "";
        public string ErrorCodeRegex { get; set; } = "";
        public string TrnRefNoRegex { get; set; } = "";
        public string OprTrnRefNoRegex { get; set; } = "";
        public string Param1Regex { get; set; } = "";
        public string Param2Regex { get; set; } = "";
        public string Param3Regex { get; set; } = "";
        public long Apptype { get; set; } = 0;
    }

    public class TemplateMasterData
    {
        public string Content { get; set; }
        public string AdditionalInfo { get; set; }
        public short IsOnOff { get; set; }
        public long TemplateID { get; set; }
        public long ServiceTypeID { get; set; }
        public object Clone()//for copy object
        {
            return MemberwiseClone();
        }
    }
}
