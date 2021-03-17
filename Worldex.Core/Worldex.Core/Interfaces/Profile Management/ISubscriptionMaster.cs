using Worldex.Core.ViewModels.Profile_Management;

namespace Worldex.Core.Interfaces.Profile_Management
{
    public interface ISubscriptionMaster
    {
        long AddSubscription(SubscriptionViewModel model);
        bool GetSubscriptionData(int UserId,long ProfileId);
        long AddMultiSubscription(int UserId,long ProfileId);
        long GetSpcialProfileSubscriptionData(int UserId, int ProfileId);
        long UnsubscribeProfile(int UserId, long ProfileId);
        SubscriptionProfileType GetSpcialProfiletype(int UserId);
        GetActiveSubscriptionData CurrentUserProfileData(int UserId);
        UserProfileHistoryDataResponse GetUserProfileHistoryData(int Userid, string AddProfile, int PageIndex = 0, int Page_Size = 0);
    }
}
