using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Worldex.Core.Entities.Resource;

namespace Worldex.Core.Entities.Culture
{
    public partial class Cultures
    {       
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }

        public ICollection<Resources> Resources { get; set; }
    }
}
