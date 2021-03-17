using Worldex.Core.ApiModels;
using System;
using System.Collections.Generic;

namespace Worldex.Core.ViewModels.ControlPanel
{
    public class ImpExpAddressReq
    {
        public long? ServiceProviderID { get; set; }
        public string FileData { get; set; }
        public string FileExtension { get; set; }
    }

    public class ImpExpAddressRes
    {
        public string AddressLable { get; set; }
        public string Address { get; set; }
        public string WalletTypeName { get; set; }
        public byte IsDefaultAddress { get; set; }
        public string ServiceProviderName { get; set; }
        public string Email { get; set; }
    }

    public class ListImpExpAddressRes : BizResponseClass
    {
        public List<ImpExpAddressRes> Details { get; set; }
    }
    public class ImpExpAddressRes3
    {
        public string AddressLable { get; set; }
        public string Address { get; set; }
        public string WalletTypeName { get; set; }
        public long WalletTypeId { get; set; }
        public long UserId { get; set; }
        public long WalletId { get; set; }
        public byte IsDefaultAddress { get; set; }
        public string ServiceProviderName { get; set; }
        public long SerProId { get; set; }
        public string Email { get; set; }
    }
    public class ListImpExpAddressRes3 : BizResponseClass
    {
        public List<ImpExpAddressRes3> Details { get; set; }
    }
    public class AddressRes
    {
        public string AddressLable { get; set; }
        public string Address { get; set; }
        public string WalletTypeName { get; set; }
        public byte IsDefaultAddress { get; set; }
        public string ServiceProviderName { get; set; }
        public string Email { get; set; }
    }
    public class ListAddressRes : BizResponseClass
    {
        public List<AddressRes> Data { get; set; }
        public long TotalCount { get; set; }
        public long PageSize { get; set; }
        public long PageNo { get; set; }
    }
    public class ImpExpAddressRes2
    {
        public string AddressLable { get; set; }
        public string Address { get; set; }
        public string WalletTypeName { get; set; }
        public long WalletTypeID { get; set; }
        public byte IsDefaultAddress { get; set; }
        public string ServiceProviderName { get; set; }
        public long SerProID { get; set; }
        public string Email { get; set; }
        public long UserID { get; set; }
    }
    public class ListImpExpAddressRes2 : BizResponseClass
    {
        public List<ImpExpAddressRes2> Details { get; set; }
    }

    public class ExpAddress_EmailLinkTokenViewModel
    {
        public long Id { get; set; }

        public string Username { get; set; }

        public string Email { get; set; }

        public DateTime CurrentTime { get; set; }

        public DateTime Expirytime { get; set; }

        public string DownloadLink { get; set; }
    }

}
