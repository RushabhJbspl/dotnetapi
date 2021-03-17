using System;

namespace Worldex.Core.SharedKernel
{
    public abstract class BaseDomainEvent
    {
        public DateTime UpdatedDate { get; protected set; } = DateTime.UtcNow;
    }
}