using Worldex.Core.Entities.Backoffice;
using Worldex.Core.Interfaces.BackOffice;
using Worldex.Core.Interfaces.Repository;
using Worldex.Core.ViewModels.BackOffice;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace Worldex.Infrastructure.Services.BackOffice
{
    public class IPRangeServices : IIPRange
    {
        private readonly WorldexContext _dbContext;
        private readonly ICustomExtendedRepository<IPRange> _IprangeextendedRepository;

        public IPRangeServices(WorldexContext dbContext, ICustomExtendedRepository<IPRange> IprangeextendedRepository)
        {
            _dbContext = dbContext;
            _IprangeextendedRepository = IprangeextendedRepository;
        }
        /// <summary>
        /// Create this method for add  iprange data in database created by pankaj
        /// </summary>
        /// <param name="IPRangeAddViewModel"></param>
        /// <returns></returns>
        public Guid AddIPRange(IPRangeAddViewModel IPRangeAddViewModel)
        {
            try
            {
                try
                {
                    IPRange iPRange = new IPRange()
                    {
                        UserId = IPRangeAddViewModel.UserId,
                        StartIp = IPRangeAddViewModel.StartIp,
                        EndIp = IPRangeAddViewModel.EndIp,
                        CreatedDate = DateTime.UtcNow,
                        CreatedBy = IPRangeAddViewModel.UserId,
                        Status = true,
                    };
                    _IprangeextendedRepository.Insert(iPRange);
                    return iPRange.Id;
                }
                catch (Exception ex)
                {

                    throw ex;
                }
            }
            catch (Exception ex)
            {
                ex.ToString();
                throw ex;
            }
        }
        /// <summary>
        /// Check the data allready in database created by pankaj
        /// </summary>
        /// <param name="IPRangeViewModel"></param>
        /// <returns></returns>
        public Guid ISIPRangeExist(IPRangeAddViewModel IPRangeViewModel)
        {
            try
            {
                var ISIPAddressExist = _IprangeextendedRepository.Table.FirstOrDefault(i => i.Status == true && i.UserId==IPRangeViewModel.UserId && (i.StartIp == IPRangeViewModel.StartIp || i.EndIp == IPRangeViewModel.EndIp));
                if (ISIPAddressExist == null)
                {
                    return Guid.Empty;
                }
                else
                {
                    return ISIPAddressExist.Id;
                }
            }
            catch (Exception ex)
            {
                ex.ToString();
                throw ex;
            }
        }
        /// <summary>
        /// Created this by pankaj kathiriya for get the detail for iprange 
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        public List<IPRangeDataviewmodel> GetUserWiseIPRange(int userid)
        {
            try
            {
                IQueryable<IPRangeDataviewmodel> Result;

                string Query = "Select StartIp,EndIp,Status,UserId from IPRange where status=1 and UserId = " + userid;
                Result = _dbContext.IPRangeDataviewmodels.FromSql(Query);
                return Result.ToList();
            }
            catch (Exception ex)
            {
                ex.ToString();
                throw ex;
            }
        }

        public Guid DeleteRange(IPRangeDeleteReqViewModel IPRangeDelete)
        {
            try
            {
                var ISIPAddressDelete = _IprangeextendedRepository.Table.FirstOrDefault(i => i.Id == IPRangeDelete.Id && i.Status == true);
                if (ISIPAddressDelete == null)
                {
                    return Guid.Empty;
                }
                else
                {

                    ISIPAddressDelete.Status = false;
                    ISIPAddressDelete.UpdatedBy = IPRangeDelete.Userid;
                    ISIPAddressDelete.UpdatedDate = DateTime.UtcNow;
                    _IprangeextendedRepository.Update(ISIPAddressDelete);
                    return ISIPAddressDelete.Id;
                }
            }
            catch (Exception ex)
            {
                ex.ToString();
                throw ex;
            }
        }


        public bool IPAddressinrange(string IPAddess, int userid)
        {
            try
            {
                bool flag = false;
                var IPRange = GetUserWiseIPRange(userid);
                if (IPRange.Count > 0)
                {
                    foreach (var item in IPRange)
                    {
                        long ipStart = BitConverter.ToInt32(IPAddress.Parse(item.StartIp).GetAddressBytes().Reverse().ToArray(), 0);
                        long ipEnd = BitConverter.ToInt32(IPAddress.Parse(item.EndIp).GetAddressBytes().Reverse().ToArray(), 0);
                        long ip = BitConverter.ToInt32(IPAddress.Parse(IPAddess).GetAddressBytes().Reverse().ToArray(), 0);

                        if (ip >= ipStart && ip <= ipEnd)  ///Check the ip in range
                        {
                            flag = true;
                            break;
                        }

                    }
                    return flag;
                }
                else
                {
                    return true;  /// Added by pankaj beacause if user does not set the ip range in this case not required to check the ip so.
                }
            }
            catch (Exception ex)
            {
                ex.ToString();
                throw ex;
            }
        }

        public IPRangeGetdataResponse GetIPRange(int PageIndex = 0, int Page_Size = 0)
        {
            try
            {
                IQueryable<IPRangeGetDataViewModel> Result;

                string Query = "Select Id,StartIp,EndIp,Status,CreatedDate from IPRange where  status=1";
                Result = _dbContext.IPRangeGetData.FromSql(Query);
                if (PageIndex == 0)
                {
                    PageIndex = 1;
                }

                if (Page_Size == 0)
                {
                    Page_Size = 10;
                }

                var skip = Page_Size * (PageIndex - 1);
                 Result.Skip(skip).Take(Page_Size).ToList();
                IPRangeGetdataResponse IPRangeGetdata = new IPRangeGetdataResponse()
                {
                    IPRangeGet = Result.Skip(skip).Take(Page_Size).OrderByDescending(r =>r.CreatedDate).ToList(),
                    TotalCount = Result.Count()
                };
                return IPRangeGetdata;

            }
            catch (Exception ex)
            {
                ex.ToString();
                throw ex;
            }
        }
    }
}
