using Worldex.Core.SharedKernel;

namespace Worldex.Core.Entities.Transaction
{
    public class FavouritePair : BizBase
    {
        public long UserId { get; set; }
        public long PairId { get; set; }
    }

    //Rita 23-2-19 for Margin Trading
    public class FavouritePairMargin : BizBase
    {
        public long UserId { get; set; }
        public long PairId { get; set; }
    }
}
