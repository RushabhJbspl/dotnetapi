using Worldex.Core.ViewModels.KYC;
using System.Collections.Generic;

namespace Worldex.Core.Interfaces.KYC
{
    public interface IKYCLevelMaster
    {
        List<KYCLevelViewModel> GetKYCLevelData();
        long ADDKYCLevel(KYCLevelInsertReqViewModel kYCLevelInsertReqViewModel);
        long UpdateKYCLevel(KYCLevelUpdateReqViewModel kYCLevelUpdateReqViewModel);
        long IsKYCKYCLevelExist(string Kyclevelname);
        int KYCUserWiseLevelCount(int Level);
        KYCLevelListResponse GetKYCLevelList(int PageIndex = 0, int Page_Size = 0);
        List<KYCLevelList> GetKYCLevelList();
    }
}
