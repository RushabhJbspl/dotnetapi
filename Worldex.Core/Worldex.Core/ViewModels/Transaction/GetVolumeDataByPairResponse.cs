using Worldex.Core.ApiModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace Worldex.Core.ViewModels.Transaction
{
    public class GetVolumeDataByPairResponse : BizResponseClass
    {
        public VolumeDataRespose response { get; set; }
    }
}
