using Worldex.Core.ViewModels.MobileMaster;
using System;
using System.Collections.Generic;

namespace Worldex.Core.Interfaces.PhoneMaster
{
    public interface Iphonemaster
    {
        Guid Add(PhoneMasterReqViewModel phoneMasterViewModel);
        Guid IsPhoneNumberExist(string MobileNumber);
        Guid Update(PhoneMasterUpdateReqViewModel phoneMasterUpdateViewModel);
        List<MobileNumebrListViewModel> GetuserWiseMolibenumberList(int UserId);
        Guid Delete(PhoneMasterDeleteViewModel phoneMasterDeleteViewModel);
    }
}
