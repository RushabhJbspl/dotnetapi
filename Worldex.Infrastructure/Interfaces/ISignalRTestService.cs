using System;
using System.Collections.Generic;
using System.Text;

namespace Worldex.Infrastructure.Interfaces
{
    public interface ISignalRTestService
    {
        void MarkTransactionHold(long ID, string Token);
    }
}
