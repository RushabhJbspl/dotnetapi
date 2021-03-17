using Worldex.Core.ViewModels.KYC;
using Worldex.Core.ViewModels.KYCConfiguration;
using System;
using System.Collections.Generic;

namespace Worldex.Core.Interfaces.KYCConfiguration
{
    public interface IKYCConfiguration
    {
        Guid Add(KYCIdentityMasterInsertReqViewModel emailMasterViewModel);
        Guid Update(KYCIdentityMasterUpdateReqViewModel kYCIdentityMasterUpdateReqViewModel);
        Guid IsKYCConfigurationExist(string Name);
        Guid AddUserKYCMappingConfiguration(UserKYCConfigurationMappingReqViewModel userKYCConfigurationMapping);
        Guid IsKYCConfigurationmappingExist(UserKYCConfigurationMappingReqViewModel userKYCConfigurationMapping);
        Guid UpdateKYCMappig(UserKYCConfigurationMappingUpdateReqViewModel userKYCConfigurationMappingUpdateReqViewModel);
        List<KYCIndentityMappinglistViewModel> KYCIndentityConfigurationlist(int UserId);
        KYCListFilterationDataListResponseViewModel GetKYCList(DateTime? fromdate, DateTime? todate,int PageIndex = 0, int Page_Size = 0, int Status = 0, string Mobile = null, string EmailAddress = null, string HostURL = null);
        List<KYCIndentitylistViewModel> KYCIdentityGetList();
        KYCInsertDocumentId CheckDocumentFormat(string Name);
        long KYCVerification(KYCUpdateViewModel kYCUpdateViewModel);
    }
}
