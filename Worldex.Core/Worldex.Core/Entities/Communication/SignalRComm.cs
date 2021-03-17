namespace Worldex.Core.Entities.Communication
{
    public class SignalRComm<T>
    {
        public T Data;
        public string EventType { get; set; }
        public string EventTime
        {
            get { return Helpers.Helpers.GetUTCTime(); }
            set { EventTime = value; }
        }
        public string Parameter { get; set; }
        public string ParamType { get; set; }
        public string ReturnMethod { get; set; }
        public string Method { get; set; }
        public string Subscription { get; set; }
        public short IsBuyerMaker { get; set; }
        public short IsIgnore { get; set; }
        public short LP { get; set; }
        public short IsMargin { get; set; } = 0;//Rita 20-2-19 for Margin Trading
    }
}
