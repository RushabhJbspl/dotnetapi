using Worldex.Core.SharedKernel;

namespace Worldex.Core.Events
{
    class WalletDrEvent<T> : BaseDomainEvent
    {
        public T WalletStatus;
        public WalletDrEvent(T walletStatus)
        {
            WalletStatus = walletStatus;
        }
    }

    class WalletCrEvent<T> : BaseDomainEvent
    {
        public T WalletStatus;
        public WalletCrEvent(T walletStatus)
        {
            WalletStatus = walletStatus;            
        }
    }
    class WalletStatusDisable<T> : BaseDomainEvent
    {
        public T WalletStatus;
        public WalletStatusDisable(T walletStatus)
        {
            WalletStatus = walletStatus;
        }
    }
    class WalletPublicAddress<T> : BaseDomainEvent
    {
        public T PublicAddress;
        public WalletPublicAddress(T publicAddress)
        {
            PublicAddress = publicAddress;
        }
    }
}
