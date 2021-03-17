using System;
using System.Collections.Generic;
using Worldex.Core.ApiModels;

namespace Worldex.Core.ViewModels.FiatBankIntegration
{
    public class GetUserBankReq
    {
        public string BankID { get; set; }
        public long UserId { get; set; }
        public string UserName { get; set; }
        public string BankName { get; set; }
        public string BankCode { get; set; }
        public string BankAccountNumber { get; set; }
        public string BankAcountHolderName { get; set; }
        public string CurrencyCode { get; set; }
        public string CountryCode { get; set; }
        public DateTime CreatedDate { get; set; }
        public short RequestType { get; set; }
        public string RequestTypeName { get; set; }
        public short Status { get; set; }
        public string StrStatus { get; set; }
        public string Remarks { get; set; }

        public string Email { get; set; }
        public bool EmailConfirmed { get; set; }
        public bool TwoFactorEnabled { get; set; }
        public bool IsEnabled { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Mobile { get; set; }
        public string UserCountryCode { get; set; }
        public int IsKycCompleted { get; set; }
    }

    public class ListUserBankReq : BizResponseClass
    {
        public List<GetUserBankReq> Data { get; set; }
    }

    public class GetBankDetail : BizResponseClass
    {
        public string BankId { get; set; }//its a guid
        public string BankName { get; set; }
        public string BankCode { get; set; }
        public string BankAccountNumber { get; set; }
        public string BankAccountHolderName { get; set; }
        public string CurrencyCode { get; set; }
        public string CountryCode { get; set; }
    }

    public class InsertUpdateCoinRes : BizResponseClass
    {
        public int Index { get; set; }
    }
}
