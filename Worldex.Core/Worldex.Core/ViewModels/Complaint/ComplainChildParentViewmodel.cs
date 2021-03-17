using System.Collections.Generic;

namespace Worldex.Core.ViewModels.Complaint
{
   public class ComplainChildParentViewmodel
    {
        public List<ComplainMasterDataViewModel> ComplainMasterDataViewModel { get; set; }
        public  List<CompainTrailViewModel> CompainTrailViewModel { get; set; }
    }
}
