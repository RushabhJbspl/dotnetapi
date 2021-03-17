using Worldex.Core.ViewModels.KYCConfiguration;
using System;
using System.Collections.Generic;

namespace Worldex.Core.Interfaces.KYCConfiguration
{
    public interface IDocumentMaster
    {
        Guid AddDocumentMaster(DocumentmasterReqViewMode documentmasterReqViewMode);
        Guid UpdateDocumentMaster(DocumentMasterUpdateReqViewModel documentMasterUpdateReqViewModel);
        Guid IsDocumentExist(string Name);
        List<KYCDocumentConfigurationListDisplayViewmodel> KYCConfigurationUserWiseDocumentList(int Userid);
        List<DocumentMasterListViewModel> KYCDocumentGetList();
        List<KYCDocumentKYCFormatListViewmodel> GetFormatList(string DocumentIddata);
    }
}
