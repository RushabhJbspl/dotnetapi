using Worldex.Core.Enums;
using MediatR;
using System.ComponentModel.DataAnnotations;

namespace Worldex.Core.ViewModels
{
    public class SendNotificationRequest : IRequest<CommunicationResponse>
    {
        [StringLength(50)]
        public string Subject { get; set; }

        [Required]
        [StringLength(200)]
        public string Message { get; set; }

        [Required]
        [StringLength(500)]
        public string DeviceID { get; set; }

        [Required]
        [StringLength(200)]
        public string TickerText { get; set; }

        [Required]
        [StringLength(200)]
        public string ContentTitle { get; set; }

        public short NotificationType { get; set; } // 3 for Resend 

        public long NotificationID { get; set; }

    }

    public class PushNotificationRequest
    {
        [StringLength(50)]
        public string Subject { get; set; }

        [Required]
        [StringLength(200)]
        public string Message { get; set; }

        [Required]
        public string[] DeviceID { get; set; }

        [Required]
        [StringLength(200)]
        public string TickerText { get; set; }

        [Required]
        [StringLength(200)]
        public string ContentTitle { get; set; }

    }

    public class DeviceRegistrationRequest : IRequest<CommunicationResponse>
    {
        [Required]
        public long UserID { get; set; }

        [Required]
        public string DeviceID { get; set; }

        [Required]
        public EnDeviceSubsscrptionType SubsscrptionType { get; set; }
    }

    public class DeviceRegistrationDTO
    {
        [Required(ErrorMessage = "1,Please Enter Required Parameters,5201")]
        public string DeviceID { get; set; }
    }
}
