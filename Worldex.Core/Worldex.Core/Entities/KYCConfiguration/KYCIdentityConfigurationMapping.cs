using Worldex.Core.SharedKernel;
using System;

namespace Worldex.Core.Entities.KYCConfiguration
{
    public class KYCIdentityConfigurationMapping : BizBaseExtended
    {
        public int Userid { get; set; }
        public Guid KYCConfigurationMasterId { get; set; }
        public long LevelId { get; set; }
    }
}
