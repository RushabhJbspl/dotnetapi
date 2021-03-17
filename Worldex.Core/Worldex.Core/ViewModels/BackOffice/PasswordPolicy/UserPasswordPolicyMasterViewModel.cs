using Worldex.Core.ApiModels;
using Worldex.Core.ViewModels.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Worldex.Core.ViewModels.BackOffice.PasswordPolicy
{
    public class UserPasswordPolicyMasterViewModel : TrackerViewModel
    {
        [Required(ErrorMessage = "1,Please enter the Password expiretime ,8048")]
        public int PwdExpiretime { get; set; }
        [Required(ErrorMessage = "1,Please enter the forgot Password expiretime in day ,8049")]
        public int MaxfppwdDay { get; set; }
        [Required(ErrorMessage = "1,Please enter the forgot Password expiretime in month ,8050")]
        public int MaxfppwdMonth { get; set; }
        [Required(ErrorMessage = "1,Please enter the link expiry time,8051")]
        public int LinkExpiryTime { get; set; }
        [Required(ErrorMessage = "1,Please enter the otp expiry time,8052")]
        public int OTPExpiryTime { get; set; }
    }
    public class UserPasswordPolicyMasterReqViewModel
    {
        public int UserId { get; set; }
        public int PwdExpiretime { get; set; }
        public int MaxfppwdDay { get; set; }
        public int MaxfppwdMonth { get; set; }
        public int LinkExpiryTime { get; set; }
        public int OTPExpiryTime { get; set; }
    }
    public class UserPasswordPolicyMasterupdateViewModel : TrackerViewModel
    {
        public long Id { get; set; }
        public int PwdExpiretime { get; set; }
        public int MaxfppwdDay { get; set; }
        public int MaxfppwdMonth { get; set; }
        public int LinkExpiryTime { get; set; }
        public int OTPExpiryTime { get; set; }
    }
    public class UserPasswordPolicyMasterupdatereqViewModel
    {
        public long Id { get; set; }
        public int UserId { get; set; }
        public int PwdExpiretime { get; set; }
        public int MaxfppwdDay { get; set; }
        public int MaxfppwdMonth { get; set; }
        public int LinkExpiryTime { get; set; }
        public int OTPExpiryTime { get; set; }
    }
    public class UserPasswordPolicyMasterDeleteViewModel : TrackerViewModel
    {
        public long Id { get; set; }
    }
    public class UserPasswordPolicyMasterDeletereqViewModel
    {
        public long Id { get; set; }
        public int UserId { get; set; }
    }
    public class UserPasswordPolicyMasterresponseViewModel : BizResponseClass
    {
    }
    public class UserPasswordPolicyMasterGetdataViewModel
    {
        public long Id { get; set; }
        public int UserId { get; set; }
        public int PwdExpiretime { get; set; }
        public int MaxfppwdDay { get; set; }
        public int MaxfppwdMonth { get; set; }
        public int LinkExpiryTime { get; set; }
        public int OTPExpiryTime { get; set; }
        public DateTime CreatedDate { get; set; }
    }
    public class UserPasswordPolicyMasterresponseListViewModel : BizResponseClass
    {
        public int TotalCount { get; set; }
        public List<UserPasswordPolicyMasterGetdataViewModel> UserPasswordPolicyMaster { get; set; }
    }
}
