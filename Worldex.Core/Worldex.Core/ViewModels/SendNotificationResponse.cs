using MediatR;

namespace Worldex.Core.ViewModels
{
    public class SendNotificationResponse : IRequest
    {
        public int ResponseCode { get; set; }
        public string ResponseMessage { get; set; }
    }
}
