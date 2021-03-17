using System;
using Worldex.Core.Entities.User;
using Worldex.Core.Interfaces.Session;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;

namespace Worldex.Core.Services
{
    public class UserSessionService : IUserSessionService
    {
        private readonly IHttpContextAccessor _context;
        private readonly UserManager<ApplicationUser> _userManager;
        public UserSessionService(IHttpContextAccessor context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public string GetSessionValue(string Key, object obj)
        {
            try
            {
                // var SessionUserdata = HttpContext.Session.GetObjectFromJson<ApplicationUser>(user.UserName);
                var SessionUserdata = _context.HttpContext.Session.GetString(Key);
                if (SessionUserdata == null)
                {
                    return null;
                }

                return SessionUserdata.ToString();
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

        }

        public async void SetSessionValue(string Key, object obj)
        {
            try
            {
                // var SessionUserdata = HttpContext.Session.GetObjectFromJson<ApplicationUser>(user.UserName);
                var SessionUserdata = _context.HttpContext.Session.GetString(Key);
                if (SessionUserdata == null)
                {
                    var str = JsonConvert.SerializeObject(obj);
                    _context.HttpContext.Session.SetString(Key, str);
                    await _context.HttpContext.Session.CommitAsync();
                }
            }
            catch (Exception ex)
            {
              
            }
        }

        public async void SetSessionToken(string value)
        {
            ApplicationUser user =await _userManager.GetUserAsync(_context.HttpContext.User);



            var TokenData = JsonConvert.DeserializeObject<tokanreponsmodel>(value);

            RedisUserModel rModel = new RedisUserModel();
            
            rModel.Id = user.Id;
            rModel.IsEnabled = user.IsEnabled;
            rModel.CreatedDate = user.CreatedDate;
            rModel.FirstName = user.FirstName;
            rModel.LastName = user.LastName;
            rModel.Mobile = user.Mobile;
            rModel.ProfilePhoto = user.ProfilePhoto.Id.ToString();
            rModel.Name = user.Name;
            rModel.UserName = user.UserName;
            rModel.NormalizedUserName = user.NormalizedUserName;
            rModel.Email = user.Email;
            rModel.NormalizedEmail = user.NormalizedEmail;
            rModel.EmailConfirmed = user.EmailConfirmed;
            rModel.PasswordHash = user.PasswordHash;
            rModel.SecurityStamp = user.SecurityStamp;
            rModel.ConcurrencyStamp = user.ConcurrencyStamp;
            rModel.PhoneNumber = user.PhoneNumber;
            rModel.PhoneNumberConfirmed = user.PhoneNumberConfirmed;
            rModel.TwoFactorEnabled = user.TwoFactorEnabled;
            rModel.LockoutEnd = (DateTimeOffset)user.LockoutEnd;
            rModel.LockoutEnabled = user.LockoutEnabled;
            rModel.AccessFailedCount = user.AccessFailedCount;

            rModel.token_type = TokenData.token_type;
            rModel.access_token = TokenData.access_token;
            rModel.refresh_token = TokenData.refresh_token;
            rModel.id_token = TokenData.id_token;

            var SessionUserdata = _context.HttpContext.Session.GetObjectFromJson<ApplicationUser>(user.UserName);
            //var SessionUserdata = _context.HttpContext.Session.GetString(user.UserName);
            if (SessionUserdata == null)
            {
                _context.HttpContext.Session.Remove(user.UserName);             
            }

            var str = JsonConvert.SerializeObject(rModel);
             _context.HttpContext.Session.SetObjectAsJson<RedisUserModel>(user.UserName, rModel);

            await _context.HttpContext.Session.CommitAsync();
        }        
    }

    public class tokanreponsmodel
    {
        public string resource { get; set; }
        public string scope { get; set; }
        public string token_type { get; set; }
        public string access_token { get; set; }
        public string expires_in { get; set; }
        public string refresh_token { get; set; }
        public string id_token { get; set; }       
    }

    public class RedisUserModel
    {
        public bool IsEnabled { get; set; }
        public DateTime CreatedDate { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Mobile { get; set; }
        public string ProfilePhoto { get; set; }
        public string Name { get; set; }
        public int Id { get; set; }
        public string UserName { get; set; }
        public string NormalizedUserName { get; set; }
        public string Email { get; set; }
        public string NormalizedEmail { get; set; }
        public bool EmailConfirmed { get; set; }
        public string PasswordHash { get; set; }
        public string SecurityStamp { get; set; }
        public string ConcurrencyStamp { get; set; }
        public string PhoneNumber { get; set; }
        public bool PhoneNumberConfirmed { get; set; }
        public bool TwoFactorEnabled { get; set; }
        public DateTimeOffset LockoutEnd { get; set; }
        public bool LockoutEnabled { get; set; }
        public int AccessFailedCount { get; set; }
        public string token_type { get; set; }

        public string access_token { get; set; }
        public string refresh_token { get; set; }
        public string id_token { get; set; }
    }
}
