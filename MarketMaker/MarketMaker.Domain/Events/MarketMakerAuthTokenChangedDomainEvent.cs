using MediatR;

namespace MarketMaker.Domain.Events
{

    /// <summary>
    /// Command fetch market maket token and store to imemorycache if not set, later use for buy and sell transaction
    /// </summary>
    /// Make it domain event -Sahil 07-10-2019 12:03 PM
    /// <remarks>-Sahil 04-10-2019 06:21 PM</remarks>
    public class MarketMakerAuthTokenChangedDomainEvent : INotification
    {
    }
}
