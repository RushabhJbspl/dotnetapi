using Worldex.Core.ApiModels;
using Worldex.Core.ViewModels.Configuration;

namespace Worldex.Core.ViewModels.ManageViewModels
{
    public class DayNightModeViewModel : TrackerViewModel
    {
        public bool DayNightMode { get; set; }
    }

    public class DayNightModeResponse : BizResponseClass
    {

    }
    public class DeviceAuthPrefViewModel : TrackerViewModel
    {
        public short IsDeviceAuthEnable { get; set; }
    }
}
