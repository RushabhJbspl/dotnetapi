using System;

namespace Worldex.Core.ViewModels.AccountViewModels.SignUp
{
    public class LinkTokenViewModel
    {
        public long Id { get; set; }

        public string Username { get; set; }

        public string Firstname { get; set; }

        public string Lastname { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }

        public string Mobile { get; set; }

        public DateTime CurrentTime { get; set; }

        public DateTime Expirytime { get; set; }

        public string CountryCode { get; set; }

        public string Device { get; set; }
        public string DeviceOS { get; set; }
        public string DeviceID { get; set; }
        public string IpAddress { get; set; }

    }

    public class EmailLinkTokenViewModel
    {
        public long Id { get; set; }

        public string Username { get; set; }

        public string Email { get; set; }

        public DateTime CurrentTime { get; set; }

        public DateTime Expirytime { get; set; }

    }

}
