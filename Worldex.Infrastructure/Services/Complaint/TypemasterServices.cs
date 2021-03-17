using Worldex.Core.Entities.Complaint;
using Worldex.Core.Interfaces.Complaint;
using Worldex.Core.Interfaces.Repository;
using Worldex.Core.ViewModels.Complaint;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Worldex.Infrastructure.Services.Complaint
{
    public class TypemasterServices : ITypemaster
    {
        private readonly ICustomRepository<Typemaster> _TypemasterRepository;
        public TypemasterServices(ICustomRepository<Typemaster> TypemasterRepository)
        {
            _TypemasterRepository = TypemasterRepository;
        }
        public List<TypemasterViewModel> GettypeMaster(string Type)
        {
            try
            {
                var IpHistoryList = _TypemasterRepository.Table.Where(i => i.Type == Type).ToList();
                if (IpHistoryList == null)
                {
                    return null;
                }
                var LoginHistory = new List<TypemasterViewModel>();
                foreach (var item in IpHistoryList)
                {
                    TypemasterViewModel loginhistoryViewModel = new TypemasterViewModel()
                    {
                        id=item.Id,
                        Type = item.SubType
                        //EnableStatus =item.EnableStatus,
                        //SubType=item.SubType
                       };
                    LoginHistory.Add(loginhistoryViewModel);
                }
                return LoginHistory;
            }
            catch (Exception ex)
            {
                ex.ToString();
                throw;
            }

           
        }
    }
}
