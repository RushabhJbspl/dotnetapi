using Worldex.Core.ApiModels;
using System.Collections.Generic;

namespace Worldex.Core.ViewModels.Configuration
{
    public class DeviceUserResponse
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string UserName { get; set; }

        public string DeviceID { get; set; }

        public long UserID { get; set; }
    }

    public class DeviceUserResponseRes : BizResponseClass
    {
        public List<DeviceUserResponse> Result { get; set; }
    }
}
