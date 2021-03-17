using MarketMaker.Domain.Entities.Aggregate;
using System;

namespace MarketMaker.Domain.Entities
{
    public sealed class Common : Entity
    {
        public Common()
        {
            GUID = Guid.NewGuid();
        }
    }
}
