using Worldex.Core.Enums;
namespace Worldex.Core.ApiModels
{
    public class BizResponseClass
    {
        public enResponseCode ReturnCode { get; set; }

        public string ReturnMsg { get; set; }

        public enErrorCode ErrorCode { get; set; }
    }
}
