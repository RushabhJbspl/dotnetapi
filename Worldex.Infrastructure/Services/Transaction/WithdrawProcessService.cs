using Worldex.Core.Entities;
using Worldex.Core.Entities.Configuration;
using Worldex.Core.Entities.Wallet;
using Worldex.Core.Helpers;
using Worldex.Core.Interfaces;
using Worldex.Core.Interfaces.Repository;
using Worldex.Core.ViewModels.Transaction;
using Worldex.Infrastructure.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace Worldex.Infrastructure.Services.Transaction
{
    public class WithdrawProcessService : IWithdrawProcessService
    {
        private readonly ICommonRepository<WithdrawERCTokenQueue> _withdrawERCTokenQueueRepository;
        private readonly IWithdrawTransactionRepository _withdrawTransactionRepository;
        private readonly IWebApiSendRequest _IWebApiSendRequest;
        private readonly ICommonRepository<ThirdPartyAPIConfiguration> _thirdPartyAPIConfigurationRepository;
        private readonly ICommonRepository<ServiceMaster> _serviceMasterRepository;
        private readonly Microsoft.Extensions.Configuration.IConfiguration _configuration;

        public WithdrawProcessService(ICommonRepository<WithdrawERCTokenQueue> withdrawERCTokenQueueRepository, IWithdrawTransactionRepository withdrawTransactionRepository, IWebApiSendRequest iWebApiSendRequest, ICommonRepository<ThirdPartyAPIConfiguration> thirdPartyAPIConfigurationRepository, ICommonRepository<ServiceMaster> serviceMasterRepository, Microsoft.Extensions.Configuration.IConfiguration configuration)
        {
            _withdrawERCTokenQueueRepository = withdrawERCTokenQueueRepository;
            _withdrawTransactionRepository = withdrawTransactionRepository;
            _IWebApiSendRequest = iWebApiSendRequest;
            _thirdPartyAPIConfigurationRepository = thirdPartyAPIConfigurationRepository;
            _serviceMasterRepository = serviceMasterRepository;
            _serviceMasterRepository = serviceMasterRepository;
            _configuration = configuration;
        }

        public WithdrawERCAdminAddress GetWithdrawERCAdminAddress(long ServiceId,long ApiId)
        {
            WithdrawERCAdminAddress response = new WithdrawERCAdminAddress();
            try
            {
                List<WithdrawERCAdminAddress> ERCAdminAddress = new List<WithdrawERCAdminAddress>();
                //Get the admin address from addressmaster
                var ServiceData = _serviceMasterRepository.GetById(ServiceId);
                if (ServiceData != null)
                {
                    ERCAdminAddress = _withdrawTransactionRepository.GetERCAdminAddress(ServiceData.SMSCode);
                }
                else
                {
                    return null;
                }
                var thirdPartyData = _thirdPartyAPIConfigurationRepository.GetById(ApiId);
                var isCheck = 0;
                if (thirdPartyData != null)
                {
                    if (ERCAdminAddress.Count > 0)
                    {
                        foreach (var AdminAddress in ERCAdminAddress)
                        {   //ntrivedi 11-03-2019 status !=2 for system failed transaction added for atcc failed transaction 
                            var trnObject = _withdrawERCTokenQueueRepository.FindBy(x => x.AddressId == AdminAddress.AddressId && x.AdminAddress == AdminAddress.Address && x.Status != 9 && x.Status != 2).OrderByDescending(x => x.Id).FirstOrDefault();

                            if (trnObject != null)
                            {
                                if ((trnObject.Status == 1 || trnObject.Status == 4) && trnObject.TrnRefNo.Length > 0) //ntrivedi added status 4 also 07-03-2019
                                {
                                    var trnStatus = GetTransactionStatus(trnObject.TrnRefNo, thirdPartyData);
                                    if (trnStatus == 1)
                                    {
                                        response = AdminAddress;
                                        isCheck = 1;
                                        break;
                                    }
                                }
                            }
                            else // There is no transaction done by these address
                            {
                                response = AdminAddress;
                                isCheck = 1;
                                break;
                            }
                        }
                    }
                    else
                    {
                        return null;
                    }

                    if(isCheck == 0)  // In All Address Record has been processing, so send the mail
                    {
                        return null;
                    }
                }
                else
                {
                    return null;
                }

                return response;
            }
            catch(Exception ex)
            {
                HelperForLog.WriteErrorLog("GetWithdrawERCAdminAddress:##APIId " + ApiId, "WithdrawProcessService", ex);
                return null;
            }
        }

        public int GetTransactionStatus(string TxnId,ThirdPartyAPIConfiguration thirdPartyData)
        {
            try
            {
                WithdrwaERCStatusCheck requestData = new WithdrwaERCStatusCheck();              
                var ContentType = "application/json";
                var MethodType = "Post";
                requestData.sitename = _configuration["sitename"].ToString();
                requestData.site_id = _configuration["site_id"].ToString();
                requestData.txnid = TxnId;
                var requestDataString = JsonConvert.SerializeObject(requestData);
                WebHeaderCollection webHeaderCollection = new WebHeaderCollection();
                var statusResponse = _IWebApiSendRequest.SendAPIRequestAsync(thirdPartyData.APIStatusCheckURL, requestDataString, ContentType,30000, webHeaderCollection, MethodType);

                WithdrwaERCStatusCheckResponse responseData = JsonConvert.DeserializeObject<WithdrwaERCStatusCheckResponse>(statusResponse);
                if(responseData.isError == false)
                {
                    if(responseData.transaction != null)
                    {
                        if(responseData.transaction.status == null)
                        {
                            return 0;
                        }
                        if(responseData.transaction.status == 0 || responseData.transaction.status == 1)
                        {
                            return 1;
                        }
                        else
                        {
                            return 0;
                        }
                    }
                }
                return 0;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("GetTransactionStatus:##TxnId " + TxnId, "WithdrawProcessService", ex);
                return 2;
            }
        }
    }
}
