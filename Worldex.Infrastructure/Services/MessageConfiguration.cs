using Worldex.Core.Interfaces;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Dapper;
using System;
using System.Data.SqlClient;
using System.Collections.Generic;
using Worldex.Core.ApiModels;
using Worldex.Core.Services.RadisDatabase;
using Worldex.Core.Enums;
using Microsoft.Extensions.Caching.Memory;
//using System.Collections.Generic;
//using System.Data.Entity;

namespace Worldex.Infrastructure.Services
{
    public class MessageConfiguration : IMessageConfiguration
    {
        private readonly WorldexContext _dbContext;
        private SqlConnectionFactory _connectionString;
        private IMemoryCache _cache { get; set; }

        public MessageConfiguration(WorldexContext dbContext, SqlConnectionFactory constr, IMemoryCache Cache)
        {
            _dbContext = dbContext;
            _connectionString = constr;
            _cache=Cache;
        }


        public async Task<IEnumerable<CommunicationProviderList>> GetAPIConfigurationAsyncV1(long ServiceTypeID, long CommServiceTypeID)
        {
            using (var connection = new SqlConnection(_connectionString.Connection()))
            {
                connection.Open();

                var result = await connection.QueryAsync<CommunicationProviderList>(
                   @"select AP.ID as AppType, CASM.SenderID,CASM.SMSSendURL As SendURL,CASM.Priority,CASM.SMSBalURL,CSM.RequestID, RM.RequestFormat,
                        RM.ContentType,RM.MethodType, CSM.ServiceName,CSMP.UserID, CSMP.Password,CSMP.Balance ,CSM.ID,
                        ISNull(CSM.ResponseFailure,'') AS ResponseFailure ,ISNull(CSM.ResponseSuccess,'') AS ResponseSuccess ,ISNULL(TC.StatusRegex,'') AS StatusRegex,
                        ISNull(TC.StatusMsgRegex,'') AS StatusMsgRegex,ISNull(TC.BalanceRegex,'') AS BalanceRegex,ISNull(TC.ErrorCodeRegex,'') AS ErrorCodeRegex,
                        ISNull(TC.OprTrnRefNoRegex,'') AS OprTrnRefNoRegex,ISNull(TC.TrnRefNoRegex,'') AS TrnRefNoRegex,ISNull(TC.ResponseCodeRegex,'') AS ResponseCodeRegex,ISNull(TC.Param1Regex,'') AS Param1Regex,ISNull(TC.Param2Regex,'') AS Param2Regex,ISNull(TC.Param3Regex,'') AS Param3Regex
                        from ServiceTypeMaster SM
                        inner join CommServiceTypeMaster CSTM on SM.ServiceTypeID = CSTM.ServiceTypeID
                        inner join CommServiceproviderMaster CSMP on CSTM.CommServiceTypeID = CSMP.CommServiceTypeID
                        inner join CommServiceMaster CSM on CSMP.ID = CSM.CommSerproID
                        inner join CommAPIServiceMaster CASM on CSM.ID = CASM.CommServiceID
                        inner join RequestFormatMaster RM on CSM.RequestID = RM.ID
                        left join  Apptype AP on AP.id = RM.RequestType
                        left join ThirdPartyAPIResponseConfiguration TC on TC.Id = CSM.ParsingDataID
                        where SM.ServiceTypeID = @ServiceTypeID and CSTM.CommServiceTypeID = @CommServiceTypeID and 
                        CSM.Status = 1 and CSMP.Status = 1 and CASM.Status = 1"
                        , new { ServiceTypeID, CommServiceTypeID }
                    );

                //if (result.AsList().Count == 0)
                //    throw new KeyNotFoundException();

                return result;
            }
        }


        public Task<IQueryable> GetAPIConfigurationAsync(long ServiceTypeID, long CommServiceTypeID)
        {
            //IQueryable result = from SM in _dbContext.ServiceTypeMaster
            //                    join CSTM in _dbContext.CommServiceTypeMaster on SM.ServiceTypeID equals CSTM.ServiceTypeID
            //                    join CSMP in _dbContext.CommServiceproviderMaster on CSTM.CommServiceTypeID equals CSMP.CommServiceTypeID
            //                    join CSM in _dbContext.CommServiceMaster on CSMP.CommSerproID equals CSM.CommSerproID
            //                    join CASM in _dbContext.CommAPIServiceMaster on CSM.CommServiceID equals CASM.CommServiceID
            //                    join RM in _dbContext.RequestFormatMaster on CSM.RequestID equals RM.RequestID
            //                    where SM.ServiceTypeID == ServiceTypeID && CSTM.CommServiceTypeID == CommServiceTypeID
            //                    select new CommunicationProviderList
            //                    {
            //                        SenderID = CASM.SenderID,
            //                        SMSSendURL = CASM.SMSSendURL,
            //                        Priority = CASM.Priority,
            //                        SMSBalURL = CASM.SMSBalURL,
            //                        RequestID = CSM.RequestID,
            //                        RequestFormat = RM.RequestFormat,
            //                        contentType = RM.contentType,
            //                        MethodType = RM.MethodType,
            //                        ServiceName = CSM.ServiceName,
            //                        UserID = CSMP.UserID,
            //                        Password = CSMP.Password,
            //                        Balance = CSMP.Balance
            //                  };
            // return Task.FromResult(result);

            IQueryable Result = _dbContext.CommunicationProviderList.FromSql(
                    @"select ISNull(AP.ID , 0) as AppType,  CASM.SenderID,CASM.SMSSendURL As SendURL,CASM.Priority,CASM.SMSBalURL,CSM.RequestID, RM.RequestFormat,
                        RM.ContentType,RM.MethodType, CSM.ServiceName,CSMP.UserID, CSMP.Password,CSMP.Balance ,CSM.ID,
                        ISNull(CSM.ResponseFailure,'') AS ResponseFailure ,ISNull(CSM.ResponseSuccess,'') AS ResponseSuccess ,ISNULL(TC.StatusRegex,'') AS StatusRegex,
                        ISNull(TC.StatusMsgRegex,'') AS StatusMsgRegex,ISNull(TC.BalanceRegex,'') AS BalanceRegex,ISNull(TC.ErrorCodeRegex,'') AS ErrorCodeRegex,
                        ISNull(TC.OprTrnRefNoRegex,'') AS OprTrnRefNoRegex,ISNull(TC.TrnRefNoRegex,'') AS TrnRefNoRegex,ISNull(TC.ResponseCodeRegex,'') AS ResponseCodeRegex,ISNull(TC.Param1Regex,'') AS Param1Regex,ISNull(TC.Param2Regex,'') AS Param2Regex,ISNull(TC.Param3Regex,'') AS Param3Regex
                        from ServiceTypeMaster SM
                        inner join CommServiceTypeMaster CSTM on SM.ServiceTypeID = CSTM.ServiceTypeID
                        inner join CommServiceproviderMaster CSMP on CSTM.CommServiceTypeID = CSMP.CommServiceTypeID
                        inner join CommServiceMaster CSM on CSMP.ID = CSM.CommSerproID
                        inner join CommAPIServiceMaster CASM on CSM.ID = CASM.CommServiceID
                        inner join RequestFormatMaster RM on CSM.RequestID = RM.ID
                        left join  Apptype AP on AP.id = RM.RequestType
                        left join ThirdPartyAPIResponseConfiguration TC on TC.Id = CSM.ParsingDataID
                        where SM.ServiceTypeID = {0} and CSTM.CommServiceTypeID = {1} and CSM.Status = 1 and CSMP.Status = 1 
                        and CASM.Status = 1", ServiceTypeID, CommServiceTypeID);
            return Task.FromResult(Result);
        }

        //enCommunicationServiceType == ServiceTypeID
        //EnTemplateType === TemplateID
        // currently not used CommServiceID
        public Task<IQueryable> GetTemplateConfigurationAsync(long ServiceTypeID, int TemplateID, long CommServiceID = 0)
        {
            IQueryable Result = _dbContext.TemplateMasterData.FromSql(
                    @"select Top 1  TM.TemplateID,ST.CommServiceTypeID AS ServiceTypeID,Content,AdditionalInfo,TCM.IsOnOff 
                        from TemplateMaster TM inner join CommServiceTypeMaster ST on ST.CommServiceTypeID = TM.CommServiceTypeID 
                        inner join TemplateCategoryMaster TCM on TCM.ID = TM.TemplateID where TM.TemplateID = {0} 
                        and ST.CommServiceTypeID = {1} and TM.status = 1 and TCM.IsOnOff  = 1", TemplateID, ServiceTypeID);
            return Task.FromResult(Result);
        }

        // khushali 08-12-2018 For SMS and EMail not working parrallay - error Dbcontext Second operation start 
        //public IQueryable GetTemplateConfigurationAsyncV1(long ServiceTypeID, int TemplateID, enCommunicationServiceType EmailSMS)
        //{
        //    IQueryable Result = null;
        //    if (EmailSMS == enCommunicationServiceType.Email)
        //    {
        //         Result = _dbContext.TemplateMasterData.FromSql(
        //           @"select Top 1 Content,AdditionalInfo,IsOnOff from TemplateMaster TM inner join CommServiceTypeMaster ST on ST.CommServiceTypeID = TM.CommServiceTypeID where TemplateID = {0} and ST.CommServiceTypeID = {1} and TM.status = 1", TemplateID, ServiceTypeID);
        //    }
        //    else if (EmailSMS == enCommunicationServiceType.SMS)
        //    {
        //         Result = _dbContext.TemplateMasterData.FromSql(
        //                @"select Top 1 Content,AdditionalInfo,IsOnOff from TemplateMaster TM inner join CommServiceTypeMaster ST on ST.CommServiceTypeID = TM.CommServiceTypeID where TemplateID = {0} and ST.CommServiceTypeID = {1}", TemplateID, ServiceTypeID);
        //    }

        //    return Result;
        //}


        public IList<TemplateMasterData> GetTemplateConfigurationAsyncV1()
        {
            IQueryable Result = null;           
            Result = _dbContext.TemplateMasterData.FromSql(
              @"select TM.TemplateID,ST.CommServiceTypeID AS ServiceTypeID,Content,AdditionalInfo,TCM.IsOnOff 
                    from TemplateMaster TM inner join CommServiceTypeMaster ST on ST.CommServiceTypeID = TM.CommServiceTypeID 
                    inner join TemplateCategoryMaster TCM on TCM.ID = TM.TemplateID where  TM.status = 1");
            return Result.Cast<TemplateMasterData>().ToList();
        }

        public void ReloadTEmplateMaster()
        {
            IList<TemplateMasterData> Result = GetTemplateConfigurationAsyncV1();
            List<TemplateMasterData> ConfigurationList = Result.ToList();
            _cache.Set("TemplateConfiguration", ConfigurationList);
           // List<TemplateMasterData> Result1 = _cache.Get<List<TemplateMasterData>>("TemplateConfiguration");
        }
    }
}
