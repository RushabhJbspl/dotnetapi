namespace MarketMaker.Domain.Enum
{
    public enum Status
    {
        Pending = 0,
        Active = 1,
        Deleted = 9,
        Locked = 2
    }

    /// <summary>
    /// MarketMaker user buy and sell range selection preference
    /// <remarks>-Sahil 25-09-2019</remarks>
    /// </summary>
    public enum RangeType
    {
        Percentage = 1,
        Fix = 2
    }

    /// <summary>
    /// MarketMaker transaction type
    /// <remarks>-Sahil 28-09-2019</remarks>
    /// </summary>
    public enum TransactionType
    {
        Buy = 4,
        Sell = 5
    }
}
