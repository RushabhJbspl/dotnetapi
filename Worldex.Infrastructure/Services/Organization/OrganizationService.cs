using Worldex.Core.Entities.Organization;
using Worldex.Core.Entities.User;
using Worldex.Core.Interfaces.Organization;
using Worldex.Core.Interfaces.Repository;
using Worldex.Core.ViewModels.Organization;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Worldex.Infrastructure.Services.Organization
{
    public class OrganizationService : IOrganization
    {
        private readonly ICustomExtendedRepository<Organization_Master> _organizationRepository;
        //private readonly ICustomRepository<SubscriptionMaster> _subscriptionRepository;
        public OrganizationService(ICustomExtendedRepository<Organization_Master> organizationRepository)
        {
            _organizationRepository = organizationRepository;
            //_typemasterRepository = typemasterRepository;
            //_subscriptionRepository = subscriptionRepository;
        }

        public Guid AddDomaim(OrganizationViewModel model)
        {
            try
            {
                var getDeviceId = _organizationRepository.Table.FirstOrDefault(i => i.DomainName == model.DomainName && i.UserId == model.UserId && i.Status == model.Status);
                if (getDeviceId != null)
                {
                    return getDeviceId.Id;
                }

                var organization = new Organization_Master
                {
                    //Id = new Guid(),
                    UserId = model.UserId,
                    DomainName = model.DomainName,
                    CreatedDate = DateTime.UtcNow,
                    CreatedBy = model.UserId,
                    Status = true,
                    AliasName = model.AliasName
                };
                _organizationRepository.Insert(organization);
                //_dbContext.SaveChanges();
                return organization.Id;
            }
            catch (Exception ex)
            {
                ex.ToString();
                throw;
            }
        }

        public bool DisActiveDomain(OrganizationViewModel model)
        {
            try
            {
                var Disactivedomain = _organizationRepository.Table.FirstOrDefault(i => i.Id == model.Id && i.UserId == model.UserId && i.Status == true);
                if (Disactivedomain != null)
                {
                    Disactivedomain.SetDisableOrganizationStatus(Disactivedomain.UserId);
                    _organizationRepository.Update(Disactivedomain);
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

        public bool EnableDomain(OrganizationViewModel model)
        {
            try
            {
                var Activedomain = _organizationRepository.Table.FirstOrDefault(i => i.Id == model.Id && i.UserId == model.UserId && i.Status == false);
                if (Activedomain != null)
                {
                    Activedomain.SetEnableOrganizationStatus(Activedomain.UserId);
                    _organizationRepository.Update(Activedomain);
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

        public GetTotalOrganizationResponse GetTotalActiveDomainList(int userid, string UserName, int pageIndex, int pageSize)
        {
            try
            {
                var DomainActiveList = _organizationRepository.Table.Where(i => i.UserId == userid && i.Status == true).OrderByDescending(i => i.CreatedDate).ToList();
                if (DomainActiveList.Count() == 0)
                {
                    return null;
                }

                var DomainList = new List<GetTotalOrganizationList>();
                foreach (var item in DomainActiveList)
                {
                    GetTotalOrganizationList imodel = new GetTotalOrganizationList();
                    imodel.Id = item.Id;
                    //imodel.UserId = item.UserId;
                    imodel.DomainName = item.DomainName;
                    imodel.CreatedDate = item.CreatedDate;
                    imodel.UpdatedDate = item.UpdatedDate;
                    imodel.Status = item.Status;
                    imodel.UserName = UserName;
                    imodel.AliasName = item.AliasName;
                    DomainList.Add(imodel);
                }

                var total = DomainList.Count();
                if (pageIndex == 0)
                {
                    pageIndex = 1;
                }

                if (pageSize == 0)
                {
                    pageSize = 10;
                }

                var skip = pageSize * (pageIndex - 1);

                GetTotalOrganizationResponse response = new GetTotalOrganizationResponse();
                response.TotalCount = total;
                response.GetTotalDomainList = DomainList.Skip(skip).Take(pageSize).ToList();
                return response;
            }
            catch (Exception ex)
            {
                ex.ToString();
                throw;
            }
        }

        public GetTotalOrganizationResponse GetTotalDisactiveDomainList(int userid, string UserName, int pageIndex, int pageSize)
        {
            try
            {
                var DomainDisactiveList = _organizationRepository.Table.Where(i => i.UserId == userid && i.Status == false).OrderByDescending(i => i.CreatedDate).ToList();
                if (DomainDisactiveList.Count() == 0)
                {
                    return null;
                }

                var DomainList = new List<GetTotalOrganizationList>();
                foreach (var item in DomainDisactiveList)
                {
                    GetTotalOrganizationList imodel = new GetTotalOrganizationList();
                    imodel.Id = item.Id;
                    //imodel.UserId = item.UserId;
                    imodel.DomainName = item.DomainName;
                    imodel.CreatedDate = item.CreatedDate;
                    imodel.UpdatedDate = item.UpdatedDate;
                    imodel.Status = item.Status;
                    imodel.UserName = UserName;
                    imodel.AliasName = item.AliasName;
                    DomainList.Add(imodel);
                }

                var total = DomainList.Count();
                if (pageIndex == 0)
                {
                    pageIndex = 1;
                }

                if (pageSize == 0)
                {
                    pageSize = 10;
                }

                var skip = pageSize * (pageIndex - 1);
                GetTotalOrganizationResponse response = new GetTotalOrganizationResponse();
                response.TotalCount = total;
                response.GetTotalDomainList = DomainList.Skip(skip).Take(pageSize).ToList();
                return response;
            }
            catch (Exception ex)
            {
                ex.ToString();
                throw;
            }
        }

        public GetTotalOrganizationResponse GetTotalDomainList(int userid, string UserName, int pageIndex, int pageSize)
        {
            try
            {
                var DomainListDet = _organizationRepository.Table.Where(i => i.UserId == userid).OrderByDescending(i => i.CreatedDate).ToList();
                if (DomainListDet == null)
                {
                    return null;
                }

                var DomainList = new List<GetTotalOrganizationList>();
                foreach (var item in DomainListDet)
                {
                    GetTotalOrganizationList imodel = new GetTotalOrganizationList();
                    imodel.Id = item.Id;
                    imodel.DomainName = item.DomainName;
                    imodel.CreatedDate = item.CreatedDate;
                    imodel.UpdatedDate = item.UpdatedDate;
                    imodel.Status = item.Status;
                    imodel.UserName = UserName;
                    imodel.AliasName = item.AliasName;
                    DomainList.Add(imodel);
                }

                var total = DomainList.Count();
                if (pageIndex == 0)
                {
                    pageIndex = 1;
                }

                if (pageSize == 0)
                {
                    pageSize = 10;
                }

                var skip = pageSize * (pageIndex - 1);

                GetTotalOrganizationResponse response = new GetTotalOrganizationResponse();
                response.TotalCount = DomainListDet.Count;
                response.GetTotalDomainList = DomainList.Skip(skip).Take(pageSize).ToList();
                return response;
            }
            catch (Exception ex)
            {
                ex.ToString();
                throw;
            }
        }

        public OrganizationTotalDomainCount GetTotalDomainCount(int userid)
        {
            try
            {
                var GetAllDomain = _organizationRepository.Table.Where(m => m.UserId == userid).ToList();
                OrganizationTotalDomainCount model = new OrganizationTotalDomainCount();
                int i = 0, j = 0;

                if (GetAllDomain != null)
                {
                    foreach (var item in GetAllDomain)
                    {
                        if (item.Status)
                            i = i + 1;
                        else
                            j = j + 1;
                    }
                }
                else
                    return null;
                if (GetAllDomain != null)
                {
                    model.TotalDoamin = GetAllDomain.Count;
                    model.TotalActiveDomain = i;
                    model.TotalDisActiveDomain = j;
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

        public bool GetDomainByUserwise(string DomainName, int UserId)
        {
            try
            {
                var GetDomain = _organizationRepository.Table.FirstOrDefault(i => i.DomainName == DomainName && i.UserId == UserId && i.Status);
                if (GetDomain != null)
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

        public List<GetUserWiseDomainData> GetUserWiseDomainData(int UserId)
        {
            try
            {
                var DomainData = _organizationRepository.Table.Where(i => i.UserId == UserId && i.Status).ToList();
                var DomainDataList = new List<GetUserWiseDomainData>();
                if (DomainData != null)
                {
                    foreach (var item in DomainData)
                    {
                        GetUserWiseDomainData imodel = new GetUserWiseDomainData();
                        imodel.Id = item.Id;
                        imodel.DomainName = item.DomainName;
                        DomainDataList.Add(imodel);
                    }
                    return DomainDataList.ToList();
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

        public bool CheckValidDomain(Guid Id,int UserId)
        {
            try
            {
                var GetDomain = _organizationRepository.Table.Where(i => i.Id == Id && i.UserId == UserId && i.Status==true).FirstOrDefault();
                if (GetDomain != null)
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
