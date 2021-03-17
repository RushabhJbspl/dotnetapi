using System.ComponentModel.DataAnnotations;
using Worldex.Core.Entities.Culture;

namespace Worldex.Core.Entities.Resource
{
    public partial class Resources
    {
        [Key]
        public int Id { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }        

        public Cultures Culture { get; set; }
    }
}
