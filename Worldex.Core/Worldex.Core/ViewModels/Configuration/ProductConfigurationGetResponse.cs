using Worldex.Core.ApiModels;
using System.Collections.Generic;

namespace Worldex.Core.ViewModels.Configuration
{
    public class ProductConfigurationGetAllResponse : BizResponseClass
    {
        public List<ProductConfigrationGetInfo> Response { get; set; }
    }
    public class ProductConfigurationGetResponse : BizResponseClass
    {
        public ProductConfigrationGetInfo Response { get; set; }
    }
    public class ProductConfigrationGetInfo
    {
        public long Id { get; set; }
        public string ProductName { get; set; }
        public long ServiceID { get; set; }
        public string ServiceName { get; set; }
        public long CountryID { get; set; }
        public string CountryName { get; set; }
    }
    public class ProductConfigurationResponse : BizResponseClass
    {
        public ProductConfigurationInfo Response { get; set; }
    }
    public class ProductConfigurationInfo
    {
        public long ProductId { get; set; }
    }
}
