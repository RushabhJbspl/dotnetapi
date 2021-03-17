using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace Worldex.Core.Entities.User
{
    public partial class ApplicationUser : IdentityUser<int>
    {       
        public bool IsEnabled { get; set; }
        [DataType(DataType.DateTime)]
        public DateTime CreatedDate { get; set; }
        [StringLength(250)]
        public string FirstName { get; set; }
        [StringLength(250)]
        public string LastName { get; set; }
        [Phone]
        public string Mobile { get; set; }
        [StringLength(5)]
        public string CountryCode { get; set; }
        public int RegTypeId { get; set; }  // Added by pankaj kathiriya for get the user login type

        public ApplicationUserPhotos ProfilePhoto { get; set; }
        public bool IsBlocked { get; set; }

        public bool Thememode { get; set; }

        [StringLength(8)]
        public string ReferralCode { get; set; }

        [DefaultValue(0)]
        public short IsCreatedByAdmin { get; set; }

        public short Status { get; set; }

        // Added column to Map user with Group for Access permissions. -Nishit Jani on A 2019-03-27 9:34 PM
        // Default set to 2 as supposed to remove error.        
        [Required]
        public int GroupID { get; set; } = 2;

        [StringLength(5)]
        public string PreferedLanguage { get; set; } = "en";//Locale

        // Added below entity to get whether User will be login with Passoword or OTP. Further more, with Mail or Mobile. -Nishit Jani 2019-07-05 2:47 PM
        // For type details, Check LoginType enum
        [Required]
        [DefaultValue(101)]
        public int LoginType { get; set; } = 101; //101 means login with Password.

        [DefaultValue(0)]
        public short IsDeviceAuthEnable { get; set; }

        [NotMapped]
        public string Name
        {
            get
            {
                return this.FirstName + " " + this.LastName;
            }
        }
    }
}
