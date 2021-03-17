using Worldex.Core.Interfaces.Log;

namespace Worldex.Core.ViewModels.AccountViewModels.Log
{
   public class CommanLogService
    {
        private readonly IipAddressService _iipAddressService;
        private readonly IDeviceIdService _iDeviceIdService;
        public CommanLogService(IDeviceIdService iDeviceIdService, IipAddressService iipAddressService)
        {
            _iDeviceIdService = iDeviceIdService;
            _iipAddressService = iipAddressService;
        }
        public async void AddLog(object model)
        {
            
        }
    }

}
