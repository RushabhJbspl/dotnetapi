using System.Collections.Generic;

namespace Worldex.Core.ViewModels.AccountViewModels.Login
{
    public class FacebookUserAccessTokenValidationViewModel
    {
        public data data { get; set; }
    }
    public class data
    {
        public string app_id { get; set; }
        public string type { get; set; }
        public string application { get; set; }
        public string data_access_expires_at { get; set; }
        public string expires_at { get; set; }
        public bool is_valid { get; set; }
        public string issued_at { get; set; }
        public List<string> scopes { get; set; }
        public string user_id { get; set; }
    }
    public class scopes
    {
    }
}
