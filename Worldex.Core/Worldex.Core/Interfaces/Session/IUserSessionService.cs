
namespace Worldex.Core.Interfaces.Session
{
    public interface IUserSessionService
    {
        string GetSessionValue(string Key, object obj);

        void SetSessionValue(string Key, object obj);

        void SetSessionToken(string value);
    }
}
