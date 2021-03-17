using Worldex.Core.ApiModels;
using Worldex.Core.ApiModels.Chat;
using Worldex.Core.Enums;
using System.Threading.Tasks;

namespace Worldex.Core.Interfaces
{
    public interface IMessageService
    {
        Task<string> SendEmailAsync(string Recepient, string Subject, string BCC,string CC, string Body, string Url, string UserID, string Password,string Port);
        Task<string> SendSMSAsync(long Mobile, string Message, string Url, string SerderID, string UserID, string Password,long AppType);
        Task<string> SendNotificationAsync(string DeviceID, string tickerText, string contentTitle, string Message, string Url, string Request, string APIKey, string MethodType, string ContentType);
        Task<TemplateMasterData> ReplaceTemplateMasterData(EnTemplateType TemplateType, CommunicationParamater MessageParameter, enCommunicationServiceType CommType);
    }
}
