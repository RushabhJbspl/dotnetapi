using Worldex.Core.Entities.Complaint;
using Worldex.Core.Entities.Organization;
using Worldex.Core.Entities.Profile_Management;
using Worldex.Core.Entities.User;
using Worldex.Core.Interfaces;
using Worldex.Core.Interfaces.Organization;
using Worldex.Core.Interfaces.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Worldex.Infrastructure.Services.Organization
{
    public class ActivityMasterConfigurationService : IActivityMasterConfiguration
    {
        private List<Typemaster> servicetypeMasterList;
        private List<HostURLMaster> servicehosturlMasterList;
        private List<ActivityType_Master> serviceactivitytypeMasterList;
        private List<ApplicationUser> serviceuserDatalist;
        private List<ComplainStatusTypeMaster> ServicecomplainDataLtst;
        private List<ApplicationMaster> servicemasterapplicationdaralist;
        private List<ProfileMaster> servicemasterprofilemasterlist;
        private List<ProfileLevelMaster> servicemasterprofilelevellist;
        private List<SubscriptionMaster> servicemastersubscriptionlist;
        private readonly ICommonRepository<Typemaster> _serviceMasterRepo;
        //private readonly ICustomExtendedRepository<HostURLMaster> _serviceProviderMasterRepo;
        private readonly WorldexContext _dbContext;
        private readonly ICustomExtendedRepository<ActivityType_Master> _serviceProviderDetailRepo;
        private readonly ICustomRepository<ComplainStatusTypeMaster> _ComplainStatusRepository;
        private readonly ICustomExtendedRepository<ApplicationMaster> _serviceApplicationRepo;
        private readonly ICustomRepository<ProfileMaster> _ProfilemasterRepository;
        private readonly ICustomRepository<ProfileLevelMaster> _ProfileLevelMasterRepository;
        private readonly ICustomRepository<SubscriptionMaster> _SubscriptionMasterRepository;

        public ActivityMasterConfigurationService(ICommonRepository<Typemaster> serviceMasterRepo,
            WorldexContext dbContext, ICustomExtendedRepository<ActivityType_Master> serviceProviderDetailRepo,
            ICustomRepository<ComplainStatusTypeMaster> ComplainStatusRepository, ICustomExtendedRepository<ApplicationMaster> serviceApplicationRepo,
            ICustomRepository<ProfileMaster> ProfilemasterRepository,
            ICustomRepository<ProfileLevelMaster> ProfileLevelMasterRepository,
            ICustomRepository<SubscriptionMaster> SubscriptionMasterRepository)
        {

            _serviceMasterRepo = serviceMasterRepo;
            _dbContext = dbContext;
            _serviceProviderDetailRepo = serviceProviderDetailRepo;
            _ComplainStatusRepository = ComplainStatusRepository;
            _serviceApplicationRepo = serviceApplicationRepo;
            _ProfilemasterRepository = ProfilemasterRepository;
            _ProfileLevelMasterRepository = ProfileLevelMasterRepository;
            _SubscriptionMasterRepository = SubscriptionMasterRepository;

            servicetypeMasterList = _serviceMasterRepo.GetAllList();
            servicehosturlMasterList = _dbContext.HostURLMaster.ToList();
            serviceactivitytypeMasterList = _serviceProviderDetailRepo.GetAllList();
            serviceuserDatalist = _dbContext.Users.ToList();
            ServicecomplainDataLtst = _ComplainStatusRepository.Table.ToList();
            servicemasterapplicationdaralist = _serviceApplicationRepo.GetAllList();

            servicemasterprofilemasterlist = _ProfilemasterRepository.Table.ToList();
            servicemasterprofilelevellist = _ProfileLevelMasterRepository.Table.ToList();
            servicemastersubscriptionlist = _SubscriptionMasterRepository.Table.ToList();
        }

        public List<ActivityType_Master> GetActivityTypeData()
        {
            try
            {
                return serviceactivitytypeMasterList;
            }
            catch (Exception ex)
            {
                ex.ToString();
                throw;
            }
        }

        public List<HostURLMaster> GetHostURLData()
        {
            try
            {
                return servicehosturlMasterList;
            }
            catch (Exception ex)
            {
                ex.ToString();
                throw;
            }
        }

        public List<Typemaster> GetTypeMasterData()
        {
            try
            {
                return servicetypeMasterList;
            }
            catch (Exception ex)
            {
                ex.ToString();
                throw;
            }
        }

        public List<ApplicationUser> GetAlluserData()
        {
            try
            {
                return serviceuserDatalist;
            }
            catch (Exception ex)
            {
                ex.ToString();
                throw;
            }
        }

        public List<ComplainStatusTypeMaster> GetComplainStatus()
        {
            try
            {
                return ServicecomplainDataLtst;
            }
            catch (Exception ex)
            {
                ex.ToString();
                throw;
            }
        }

        public List<ApplicationMaster> GetMasterApplicationData()
        {
            try
            {
                return servicemasterapplicationdaralist;
            }
            catch (Exception ex)
            {
                ex.ToString();
                throw;
            }
        }

        public List<ProfileMaster> GetMasterProfileData()
        {
            try
            {
                return servicemasterprofilemasterlist;
            }
            catch (Exception ex)
            {
                ex.ToString();
                throw;
            }
        }

        public List<ProfileLevelMaster> GetMasterProfileLevelData()
        {
            try
            {
                return servicemasterprofilelevellist;
            }
            catch (Exception ex)
            {
                ex.ToString();
                throw;
            }
        }

        public List<SubscriptionMaster> GetUserSubscription()
        {
            try
            {
                return servicemastersubscriptionlist;
            }
            catch (Exception ex)
            {
                ex.ToString();
                throw;
            }
        }
    }
}
