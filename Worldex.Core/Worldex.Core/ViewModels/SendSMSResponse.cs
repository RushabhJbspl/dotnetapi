using MediatR;

namespace Worldex.Core.ViewModels
{
    public class SendSMSResponse : IRequest
    {
        public int ResponseCode { get; set; }
        public string ResponseMessage { get; set; }
    }
}
