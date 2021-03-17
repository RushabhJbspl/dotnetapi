using Worldex.Core.Entities.Organization;
using Worldex.Core.Interfaces.Application;
using Worldex.Core.Interfaces.Organization;
using Worldex.Core.Interfaces.Repository;
using Worldex.Core.ViewModels.Organization;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Worldex.Infrastructure.Services.Application
{
    public class BackOfficeApplicationService : IBackOfficeApplication
    {
        private readonly ICustomExtendedRepository<ApplicationMaster> _applicationRepository;
        private readonly ICustomExtendedRepository<Org_App_Mapping> _orgappRepository;
        private readonly ICustomExtendedRepository<Organization_Master> _Iorgmasterrepo;
        private readonly IActivityMasterConfiguration _IactivityMasterConfiguration;
        public BackOfficeApplicationService(ICustomExtendedRepository<ApplicationMaster> applicationRepository, ICustomExtendedRepository<Org_App_Mapping> orgappRepository,
            ICustomExtendedRepository<Organization_Master> Iorgmasterrepo, IActivityMasterConfiguration IactivityMasterConfiguration)
        {
            _applicationRepository = applicationRepository;
            _orgappRepository = orgappRepository;
            _Iorgmasterrepo = Iorgmasterrepo;
            _IactivityMasterConfiguration = IactivityMasterConfiguration;
        }
        public Guid AddApplication(ApplicationViewModel model)
        {
            try
            {
                var getapplication = _applicationRepository.Table.FirstOrDefault(i => i.ApplicationName == model.ApplicationName && i.Status);
                if (getapplication != null)
                {
                    return getapplication.Id;
                }

                var application = new ApplicationMaster
                {
                    ApplicationName = model.ApplicationName,
                    CreatedDate = DateTime.UtcNow,
                    Status = true,
                    Description = model.Description,
                    CreatedBy = model.CreatedBy
                };
                _applicationRepository.Insert(application);
                return application.Id;
            }
            catch (Exception ex)
            {
                ex.ToString();
                throw;
            }
        }

        public bool DisableApplication(ApplicationEnableDisable model)
        {
            try
            {
                var Activeappication = _applicationRepository.Table.FirstOrDefault(i => i.Id == model.Id && i.Status == true);
                if (Activeappication != null)
                {
                    Activeappication.SetDisableApplicationStatus(Activeappication.CreatedBy);
                    _applicationRepository.Update(Activeappication);
                    return true;
                }
                else
                    return false;
            }
            catch (Exception ex)
            {
                ex.ToString();
                throw;
            }
        }

        public bool EnableApplication(ApplicationEnableDisable model)
        {
            try
            {
                var Activeappication = _applicationRepository.Table.FirstOrDefault(i => i.Id == model.Id && i.Status == false);
                if (Activeappication != null)
                {
                    Activeappication.SetEnableApplicationStatus(Activeappication.CreatedBy);
                    _applicationRepository.Update(Activeappication);
                    return true;
                }
                else
                    return false;
            }
            catch (Exception ex)
            {
                ex.ToString();
                throw;
            }
        }

        public bool GetApplicationData(string ApplicationName)
        {
            try
            {
                var GetApplication = _applicationRepository.Table.FirstOrDefault(i => i.ApplicationName == ApplicationName && i.Status);
                if (GetApplication != null)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                ex.ToString();
                throw;
            }
        }

        public ApplicationTotalCount GetTotalApplicationCount()
        {
            try
            {
                var GetAllApplication = _applicationRepository.Table.ToList();
                ApplicationTotalCount model = new ApplicationTotalCount();
                int i = 0, j = 0;
                if (GetAllApplication != null)
                {
                    foreach (var item in GetAllApplication)
                    {
                        if (item.Status)
                            i = i + 1;
                        else
                            j = j + 1;
                    }
                }
                else
                    return null;
                if (GetAllApplication != null)
                {
                    model.TotalApplication = GetAllApplication.Count;
                    model.TotalActiveApplication = i;
                    model.TotalDisActiveApplication = j;
                    return model;
                }
                else
                    return null;
            }
            catch (Exception ex)
            {
                ex.ToString();
                throw;
            }
        }

        public GetTotalApplicationListResponse GetTotalApplicationList(int pageIndex, int pageSize)
        {
            try
            {
                var ApplicationListDet = _applicationRepository.Table.OrderByDescending(i => i.CreatedDate).ToList();
                if (ApplicationListDet == null)
                {
                    return null;
                }

                var AppList = new List<GetTotalApplicationList>();
                foreach (var item in ApplicationListDet)
                {
                    GetTotalApplicationList imodel = new GetTotalApplicationList();
                    imodel.Id = item.Id;
                    imodel.ApplicationName = item.ApplicationName;
                    imodel.CreatedDate = item.CreatedDate;
                    imodel.Status = item.Status;
                    imodel.Description = item.Description;
                    AppList.Add(imodel);
                }

                var total = AppList.Count();
                if (pageIndex == 0)
                {
                    pageIndex = 1;
                }

                if (pageSize == 0)
                {
                    pageSize = 10;
                }

                var skip = pageSize * (pageIndex - 1);
                GetTotalApplicationListResponse response = new GetTotalApplicationListResponse();
                response.TotalCount = ApplicationListDet.Count;
                response.GetTotalApplicationList = AppList.Skip(skip).Take(pageSize).ToList();
                return response;
            }
            catch (Exception ex)
            {
                ex.ToString();
                throw;
            }
        }

        public GetTotalActiveApplicationResponse GetTotalActiveApplicationList(int pageIndex, int pageSize)
        {
            try
            {
                var ApplicationListDet = _applicationRepository.Table.Where(i => i.Status == true).OrderByDescending(i => i.CreatedDate).ToList();
                if (ApplicationListDet == null)
                {
                    return null;
                }

                var AppList = new List<GetTotalApplicationList>();
                foreach (var item in ApplicationListDet)
                {
                    GetTotalApplicationList imodel = new GetTotalApplicationList();
                    imodel.Id = item.Id;
                    imodel.ApplicationName = item.ApplicationName;
                    imodel.CreatedDate = item.CreatedDate;
                    imodel.Status = item.Status;
                    imodel.Description = item.Description;
                    AppList.Add(imodel);
                }

                var total = AppList.Count();
                if (pageIndex == 0)
                {
                    pageIndex = 1;
                }

                if (pageSize == 0)
                {
                    pageSize = 10;
                }

                var skip = pageSize * (pageIndex - 1);
                GetTotalActiveApplicationResponse response = new GetTotalActiveApplicationResponse();
                response.TotalCount = ApplicationListDet.Count;
                response.GetTotalActiveApplicationList = AppList.Skip(skip).Take(pageSize).ToList();
                return response;
            }
            catch (Exception ex)
            {
                ex.ToString();
                throw;
            }
        }

        public GetTotalActiveApplicationResponse GetTotalActiveApplicationListV1(int pageIndex, int pageSize)
        {
            try
            {
                var ApplicationListCount = _applicationRepository.Table.Where(i => i.Status == true).OrderByDescending(i => i.CreatedDate).Count();
                var total = Convert.ToInt32(ApplicationListCount);
                if (pageIndex == 0)
                {
                    pageIndex = 1;
                }

                if (pageSize == 0)
                {
                    pageSize = 10;
                }

                var skip = pageSize * (pageIndex - 1);
                var ApplicationListDet = _applicationRepository.Table.Where(i => i.Status == true).OrderByDescending(i => i.CreatedDate).Skip(skip).Take(pageSize).ToList();
                if (ApplicationListDet == null)
                {
                    return null;
                }

                var AppList = new List<GetTotalApplicationList>();
                foreach (var item in ApplicationListDet)
                {
                    GetTotalApplicationList imodel = new GetTotalApplicationList();
                    imodel.Id = item.Id;
                    imodel.ApplicationName = item.ApplicationName;
                    imodel.CreatedDate = item.CreatedDate;
                    imodel.Status = item.Status;
                    imodel.Description = item.Description;
                    AppList.Add(imodel);
                }

                GetTotalActiveApplicationResponse response = new GetTotalActiveApplicationResponse();
                response.TotalCount = total;
                response.GetTotalActiveApplicationList = AppList.ToList();
                return response;
            }
            catch (Exception ex)
            {
                ex.ToString();
                throw;
            }
        }

        public GetTotalDisactiveApplicationResponse GetTotalDisActiveApplicationList(int pageIndex, int pageSize)
        {
            try
            {
                var ApplicationListDet = _applicationRepository.Table.Where(i => i.Status == false).OrderByDescending(i => i.CreatedDate).ToList();
                if (ApplicationListDet == null)
                {
                    return null;
                }

                var AppList = new List<GetTotalApplicationList>();
                foreach (var item in ApplicationListDet)
                {
                    GetTotalApplicationList imodel = new GetTotalApplicationList();
                    imodel.Id = item.Id;
                    imodel.ApplicationName = item.ApplicationName;
                    imodel.CreatedDate = item.CreatedDate;
                    imodel.Status = item.Status;
                    imodel.Description = item.Description;
                    AppList.Add(imodel);
                }

                var total = AppList.Count();
                if (pageIndex == 0)
                {
                    pageIndex = 1;
                }

                if (pageSize == 0)
                {
                    pageSize = 10;
                }

                var skip = pageSize * (pageIndex - 1);
                GetTotalDisactiveApplicationResponse response = new GetTotalDisactiveApplicationResponse();
                response.TotalCount = ApplicationListDet.Count;
                response.GetTotalDisactiveApplicationList = AppList.Skip(skip).Take(pageSize).ToList();
                return response;
            }
            catch (Exception ex)
            {
                ex.ToString();
                throw;
            }
        }

        public List<GetApplicationData> GetAllApplicationData()
        {
            try
            {
                var ApplicationData = _applicationRepository.Table.Where(i => i.Status == true).ToList();
                var ApplicationDataList = new List<GetApplicationData>();
                if (ApplicationData != null)
                {
                    foreach (var item in ApplicationData)
                    {
                        GetApplicationData imodel = new GetApplicationData();
                        imodel.Id = item.Id;
                        imodel.ApplicationName = item.ApplicationName;
                        ApplicationDataList.Add(imodel);
                    }
                    return ApplicationDataList.ToList();
                }
                else
                    return null;
            }
            catch (Exception ex)
            {
                ex.ToString();
                throw;
            }
        }

        public bool CheckValidMasterApplication(Guid Id)
        {
            try
            {
                var GetApplication = _applicationRepository.Table.Where(i => i.Id == Id && i.Status==true).FirstOrDefault();
                if (GetApplication != null)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                ex.ToString();
                throw;
            }
        }

        public Guid UserWiseAddApplication(CreateUserWiseApplication model)
        {
            try
            {
                var getapplication = _orgappRepository.Table.FirstOrDefault(i => i.AppName == model.AppName && i.Status && i.OrgId == model.DomainId && i.AppId == model.AppId && i.CreatedBy == model.UserId);
                if (getapplication != null)
                {
                    return getapplication.Id;
                }

                var UserApplication = new Org_App_Mapping
                {
                    OrgId = model.DomainId,
                    AppId = model.AppId,
                    AppName = model.AppName,
                    CreatedDate = DateTime.UtcNow,
                    Status = true,
                    CreatedBy = model.UserId,
                    ClientSecret = Guid.NewGuid()
                };
                _orgappRepository.Insert(UserApplication);
                return UserApplication.Id;
            }
            catch (Exception ex)
            {
                ex.ToString();
                throw;
            }
        }

        public GetUserWiseAppData GetUserWiseAppdataDet(Guid Id)
        {
            try
            {
                var UserData = _orgappRepository.Table.Where(i => i.Id == Id).FirstOrDefault();
                if (UserData != null)
                {
                    GetUserWiseAppData data = new GetUserWiseAppData();
                    data.Id = UserData.Id;
                    data.DomainId = UserData.OrgId;
                    data.DomainName = _Iorgmasterrepo.Table.Where(i => i.Id == UserData.OrgId).Count()>0 ?_Iorgmasterrepo.Table.Where(i => i.Id == UserData.OrgId).FirstOrDefault().DomainName : string.Empty;
                    data.AppId = UserData.AppId;
                    data.ApplicationName = _applicationRepository.Table.Where(i => i.Id == UserData.AppId).Count()>0 ?_applicationRepository.Table.Where(i => i.Id == UserData.AppId).FirstOrDefault().ApplicationName:string.Empty;
                    data.AppName = UserData.AppName;
                    data.ClientID = UserData.Id;
                    data.ClientSecret = UserData.ClientSecret;
                    data.Description = UserData.Description !=null ? UserData.Description : string.Empty;
                    data.ApplicationLogo = UserData.ApplicationLogo !=null ? UserData.ApplicationLogo : string.Empty;
                    data.AllowedCallBackURLS = UserData.AllowedCallBackURLS !=null ? UserData.AllowedCallBackURLS: string.Empty;
                    data.AllowedWebOrigins = UserData.AllowedWebOrigins !=null ? UserData.AllowedWebOrigins:string.Empty;
                    data.AllowedLogoutURLS = UserData.AllowedLogoutURLS !=null ? UserData.AllowedLogoutURLS: string.Empty;
                    data.AllowedOriginsCORS = UserData.AllowedOriginsCORS !=null ? UserData.AllowedOriginsCORS: string.Empty;
                    data.JWTExpiration = UserData.JWTExpiration;
                    return data;
                }
                else
                    return null;
            }
            catch (Exception ex)
            {
                ex.ToString();
                throw;
            }
        }

        public Guid UpdateUserWiseApplication(UpdateUserGUIDWiseAppData model, int UserId)
        {
            try
            {
                var UserData = _orgappRepository.Table.Where(i => i.Id == model.Id).FirstOrDefault();
                if (UserData != null)
                {
                    UserData.AppName = model.AppName;
                    UserData.OrgId = model.DomainId;
                    UserData.ClientSecret = model.ClientSecret;
                    UserData.Description = model.Description;
                    UserData.UpdatedBy = UserId;
                    UserData.UpdatedDate = DateTime.UtcNow;
                    UserData.ApplicationLogo = model.ApplicationLogo;
                    UserData.AppId = model.AppId;
                    UserData.AllowedCallBackURLS = model.AllowedCallBackURLS;
                    UserData.AllowedWebOrigins = model.AllowedWebOrigins;
                    UserData.AllowedLogoutURLS = model.AllowedLogoutURLS;
                    UserData.AllowedOriginsCORS = model.AllowedOriginsCORS;
                    UserData.JWTExpiration = model.JWTExpiration;
                    _orgappRepository.Update(UserData);
                    return UserData.Id;
                }
                else
                    return Guid.Empty;
            }
            catch (Exception ex)
            {
                ex.ToString();
                throw;
            }
        }

        public GetTotalUserApplicationListResponse GetUserApplicationList(int pageIndex, int pageSize, int UserId)
        {
            try
            {
                var UserAppListDet = _orgappRepository.Table.Where(i => i.CreatedBy == UserId).OrderByDescending(i => i.CreatedDate).ToList();
                if (UserAppListDet == null)
                {
                    return null;
                }

                var UserAppList = new List<GetTotalUserApplicationList>();
                foreach (var item in UserAppListDet)
                {
                    GetTotalUserApplicationList imodel = new GetTotalUserApplicationList();
                    imodel.Id = item.Id;
                    imodel.ApplicationName = item.AppName;
                    imodel.Description = item.Description !=null ? item.Description:string.Empty;
                    imodel.CreatedDate = item.CreatedDate;
                    imodel.Status = item.Status;
                    imodel.ClientSecret = item.ClientSecret;
                    imodel.MasterApplicationName = _IactivityMasterConfiguration.GetMasterApplicationData().Where(i => i.Id == item.AppId).Count() > 0 ? _IactivityMasterConfiguration.GetMasterApplicationData().Where(i => i.Id == item.AppId).FirstOrDefault().ApplicationName : string.Empty;
                    UserAppList.Add(imodel);
                }

                var total = UserAppList.Count();
                if (pageIndex == 0)
                {
                    pageIndex = 1;
                }

                if (pageSize == 0)
                {
                    pageSize = 10;
                }

                var skip = pageSize * (pageIndex - 1);

                GetTotalUserApplicationListResponse response = new GetTotalUserApplicationListResponse();
                response.TotalCount = UserAppListDet.Count;
                response.TotalUserApplicationList = UserAppList.Skip(skip).Take(pageSize).ToList();
                return response;
            }
            catch (Exception ex)
            {
                ex.ToString();
                throw;
            }
        }

        public bool CheckUserWiseAppData(Guid DomainId, Guid AppId, string appName, long UserId)
        {
            try
            {
                var IsExitsApp = _orgappRepository.Table.Where(i => i.CreatedBy == UserId && i.AppId == AppId && i.AppName==appName && i.OrgId==DomainId && i.Status).FirstOrDefault();
                if (IsExitsApp == null)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                ex.ToString();
                throw;
            }
        }
    }
}
