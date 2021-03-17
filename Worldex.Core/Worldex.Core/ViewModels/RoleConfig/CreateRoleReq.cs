using Worldex.Core.ApiModels;
using Worldex.Core.Enums;
using Worldex.Core.ViewModels.BackOffice;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Worldex.Core.ViewModels.RoleConfig
{
    #region Role Management

    public class CreateRoleReq
    {
        public long? RoleId { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameter,4169")]
        [StringLength(250)]
        public string RoleName { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameter,4170")]
        [StringLength(250)]
        public string RoleDescription { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameter,4174")]
        [EnumDataType(typeof(ServiceStatus), ErrorMessage = "1,Fail,4176")]
        public ServiceStatus Status { get; set; }
        //[Required(ErrorMessage = "1,Please Enter Required Parameter,4171")]
        //public long PermissionGroupID { get; set; }
    }
    public class GetRoleId
    {
        public int RoleId { get; set; }
    }

    public class ChangeRoleStatusReq
    {
        [Required(ErrorMessage = "1,Please Enter Required Parameter,4173")]
        public long RoleId { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameter,4174")]
        [EnumDataType(typeof(ServiceStatus), ErrorMessage = "1,Fail,4176")]
        public ServiceStatus Status { get; set; }
    }

    public class GetRoleDetailRes
    {
        public long RoleID { get; set; }
        public string RoleName { get; set; }
        public string RoleDescription { get; set; }
        public short Status { get; set; }
    }

    public class GetRoleDetail22 : BizResponseClass
    {
        public GetRoleDetailRes RoleDetail { get; set; }
    }

    public class GetRoleDetail : BizResponseClass
    {
        public long ID { get; set; }
        public string RoleName { get; set; }
        public string RoleDescription { get; set; }
        //public long PermissionGroupID { get; set; }
        public short Status { get; set; }
        public string StrStatus { get; set; }
    }

    public class GetRoleDetail2
    {
        public long RoleID { get; set; }
        public string RoleName { get; set; }
        public string RoleDescription { get; set; }
        //public long PermissionGroupID { get; set; }
        public short Status { get; set; }
        public string StrStatus { get; set; }
    }

    public class ListRoleDetail : BizResponseClass
    {
        public List<GetRoleDetail2> Details { get; set; }
        public int TotalCount { get; set; }
        public int PageNo { get; set; }
        public int PageSize { get; set; }

    }

    #endregion

    #region Permission Group

    public class CreatePermissionGrpReq
    {
        //[Required(ErrorMessage = "1,Please Enter Required Parameter,4184")]
        public long? PermissionGroupID { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameter,4178")]
        [StringLength(50)]
        public string GroupName { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameter,4182")]
        [StringLength(100)]
        public string Description { get; set; }

        //[Required(ErrorMessage = "1,Please Enter Required Parameter,4179")]
        //public long AccessRightId { get; set; }

        public string IPAddress { get; set; }

        //[Required(ErrorMessage = "1,Please Enter Required Parameter,4180")]
        public long LinkedRoleId { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameter,4174")]
        [EnumDataType(typeof(ServiceStatus), ErrorMessage = "1,Fail,4176")]
        public ServiceStatus Status { get; set; }

        //public UserAccessRightRequest Data { get; set; }
        public UserAccessRightsViewModel Data { get; set; }
    }

    public class GetPermissionGroupRes : BizResponseClass
    {
        //public GetPermissionGroup Data { get; set; }
        public GetPermissionGroup2 Data { get; set; }
    }

    public class GetPermissionGroup2
    {
        public long PermissionGroupID { get; set; }

        public string GroupName { get; set; }

        public string Description { get; set; }

        public string LinkedRole { get; set; }

        public int LinkedRoleId { get; set; }

        public string IPAddress { get; set; }

        public DateTime CreatedDate { get; set; }

        public short Status { get; set; }

        public string UpdatedBy { get; set; }

        public string CreatedBy { get; set; }

    }


    public class GetPermissionGroup
    {
        public long Id { get; set; }

        public string GroupName { get; set; }

        public string GroupDescription { get; set; }

        public string LinkedRole { get; set; }

        public int LinkedRoleID { get; set; }

        public string IPAddress { get; set; }

        public DateTime CreatedDate { get; set; }

        public short Status { get; set; }

        public string UpdatedBy { get; set; }

        public string CreatedBy { get; set; }

    }
    
    public class ListPermissionGrp : BizResponseClass
    {
        public List<GetPermissionGroup> Data { get; set; }
        public int TotalCount { get; set; }
        public int PageNo { get; set; }
        public int PageSize { get; set; }
    }

    public class GetRoleHistoryData
    {
        public long Id { get; set; }
        public DateTime UpdatedDate { get; set; }
        public short Status { get; set; }
        public string ModificationDetail { get; set; }
        public string Module { get; set; }        
        public string IPAddress { get; set; }
        public string UserName { get; set; }
    }

    public class ListRoleHistoryData : BizResponseClass
    {
        public List<GetRoleHistoryData> Data { get; set; }
        public int TotalCount { get; set; }
        public int PageNo { get; set; }
        public int PageSize { get; set; }
    }

    #endregion

    #region User Management

    public class CreateUserReq
    {   

        [Required(ErrorMessage = "1,Please Required Parameter,14020")]
        [Display(Name = "Username")]
        [StringLength(50, ErrorMessage = "1,Invalid Parameter,14021")]
        public string Username { get; set; }

        [Required(ErrorMessage = "1,Please Required Parameter,14022")]
        [StringLength(50, ErrorMessage = "1,Invalid Parameter,14023")]
        [Display(Name = "Firstname")]
        public string Firstname { get; set; }

        [Required(ErrorMessage = "1,Please Required Parameter,14024")]
        [StringLength(50, ErrorMessage = "1,Invalid Parameter,14025")]
        [Display(Name = "Lastname")]
        public string Lastname { get; set; }

        [Required(ErrorMessage = "1,Please Required Parameter,14026")]
        [StringLength(50, ErrorMessage = "1,Invalid Parameter,14027")]
        [RegularExpression(@"^[-a-zA-Z0-9~!$%^&*_=+}{\'?]+(\.[-a-zA-Z0-9~!$%^&*_=+}{\'?]+)*@([a-zA-Z0-9_][-a-zA-Z0-9_]*(\.[-a-zA-Z0-9_]+)*\.(aero|arpa|biz|com|coop|edu|gov|info|int|mil|museum|name|net|org|pro|travel|mobi|[a-zA-Z]{2,3})|([0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}))(:[0-9]{1,5})?$", ErrorMessage = "1,Invalid Format,14028")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "1,Please Enter Password,14036")]
        [DataType(DataType.Password)]
        [StringLength(50, ErrorMessage = "1,The Password must be at least 6 and at max 50 characters long,17326", MinimumLength = 6)]//change by mansi-05-11-2019
       // [StringLength(50, ErrorMessage = "1,The {0} must be at least {2} and at max {1} characters long,4011", MinimumLength = 6)]
        [Display(Name = "Password")]
        [RegularExpression("^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-]).{6,}$", ErrorMessage = "1,Invalid Password,14037")]
        public string Password { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameter,14038")]
        [Display(Name = "Mobile")]
        public string Mobile { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameter,14039")]
        [EnumDataType(typeof(EnMyAccountUserStatus), ErrorMessage = "1,Fail,14040")]
        public EnMyAccountUserStatus Status { get; set; }

        //[Required(ErrorMessage = "1,Please Provide country code,4132")]
        //[Display(Name = "CountryCode")]
        //[StringLength(5, ErrorMessage = "1,Please enter a valid Contry Code,4131")]
        //public string CountryCode { get; set; }
        //[Required(ErrorMessage = "1,Please Provide PreferedLanguage,4185")]
        [StringLength(5, ErrorMessage = "1,Please enter a valid PreferedLanguage,4186")]
        public string PreferedLanguage { get; set; } = "en";

        [Required(ErrorMessage = "1,Please Enter Required Parameter,14041")]
        public int GroupID { get; set; }

        public short? IsAdmin { get; set; }
    }

    public class EditUserReq
    {
        //[Required(ErrorMessage = "1,Please Required Parameter,14020")]
        //[Display(Name = "Username")]
        //[StringLength(50, ErrorMessage = "1,Invalid Parameter,14021")]
        //public string Username { get; set; }

        [Required(ErrorMessage = "1,Please Required Parameter,14042")]
        public int UserId { get; set; }

        [Required(ErrorMessage = "1,Please Required Parameter,14022")]
        [StringLength(50, ErrorMessage = "1,Invalid Parameter,14023")]
        [Display(Name = "Firstname")]
        public string Firstname { get; set; }

        [Required(ErrorMessage = "1,Please Required Parameter,14024")]
        [StringLength(50, ErrorMessage = "1,Invalid Parameter,14025")]
        [Display(Name = "Lastname")]
        public string Lastname { get; set; }

        public long RoleId { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameter,14039")]
        [EnumDataType(typeof(EnMyAccountUserStatus), ErrorMessage = "1,Fail,14040")]
        public EnMyAccountUserStatus Status { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameter,14041")]
        public int GroupID { get; set; }

        public short? IsAdmin { get; set; }


        public short TwoFactorEnabled { get; set; }

        public short IsLockOut { get; set; }
    }
    
    public class GetUserDetail
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Mobile { get; set; }
        public Int32 RoleId { get; set; }
        public string RoleName { get; set; }
        public string PermissionGroup { get; set; }
        public short Status { get; set; }
        public int GroupID { get; set; }
        public short IsAdmin { get; set; }
        public string StrIsAdmin { get; set; }
        public short IsLockOut { get; set; }//add by mansi
        public short TwoFactorEnabled { get; set; }//add by mansi
        public DateTime CreatedDate { get; set; }//add by mansi

    }

    public class GetUserDetailResp : BizResponseClass
    {
        public GetUserDetail Data { get; set; }
    }

    public class ListUserDetails : BizResponseClass
    {
        public List<GetUserDetail> Data { get; set; }
        public int TotalCount { get; set; }
        public int PageNo { get; set; }
        public int PageSize { get; set; }
    }

    public class ListUserDetails2 : BizResponseClass
    {
        public List<GetUserDetail> Data { get; set; }
    }

    #endregion

    public class ViewUserRes
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }        
        public string RoleName { get; set; }
        public DateTime CreatedDate { get; set; }
        public short Status { get; set; }
    }

    public class ViewUnAssignedUserRes
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string RoleName { get; set; }
        public DateTime CreatedDate { get; set; }
        public short Status { get; set; }
    }

    public class ListViewUnAssignedUserRes : BizResponseClass
    {
        public List<ViewUnAssignedUserRes> Result { get; set; }
        public int TotalCount { get; set; }
        public int PageNo { get; set; }
        public int PageSize { get; set; }
    }

    public class ViewUserDetailRes : BizResponseClass
    {
        public List<ViewUserRes> Data { get; set; }
        public long TotalRecords { get; set; }
    }

    public class ChangePermissionGroupStatusReq
    {
        [Required(ErrorMessage = "1,Please Enter Required Parameter,4184")]
        public long PermissionGroupID { get; set; }

        [Required(ErrorMessage = "1,Please Enter Required Parameter,4174")]
        [EnumDataType(typeof(ServiceStatus), ErrorMessage = "1,Fail,4176")]
        public ServiceStatus Status { get; set; }
    }

    public class GetPermissionGroupIDByLinkedRoleRes : BizResponseClass
    {
        public long GroupID { get; set; }
    }
    
}
