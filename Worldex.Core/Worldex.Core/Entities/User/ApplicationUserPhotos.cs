using System.ComponentModel.DataAnnotations;

namespace Worldex.Core.Entities.User
{
    public partial class ApplicationUserPhotos : IEntityBase
    {
        [Key]
        public int Id { get; set; }
        public string ContentType { get; set; }
        public byte[] Content { get; set; }
        public int ApplicationUserId { get; set; }

        public ApplicationUser ApplicationUser { get; set; }
    }
}
