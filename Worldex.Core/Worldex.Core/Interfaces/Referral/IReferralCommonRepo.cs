using Worldex.Core.ViewModels.Referral;
using System.Collections.Generic;

namespace Worldex.Core.Interfaces.Referral
{
    public interface IReferralCommonRepo
    {
        ReferralSchemeTypeMappingRes GetByIdMappingData(long id);
        List<ReferralSchemeTypeMappingRes> ListMappingData(long? payTypeId, long? serviceTypeMstId, short? status);
        ReferralServiceDetailRes GetByIdReferralServiceDetail(long id);
        List<ReferralServiceDetailRes> ListReferralServiceDetail(long? schemeTypeMappingId, long? creditWalletTypeId, short? status);
    }
}
