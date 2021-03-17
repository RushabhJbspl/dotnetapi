using Worldex.Core.ApiModels;
using Worldex.Core.Entities.Configuration;
using Worldex.Core.Entities.User;
using Worldex.Core.Enums;
using Worldex.Core.Helpers;
using Worldex.Core.Interfaces.Repository;
using Worldex.Core.Interfaces.User;
using Worldex.Core.ViewModels.AccountViewModels.IpWiseDataViewModel;
using Worldex.Core.ViewModels.AccountViewModels.Login;
using Worldex.Core.ViewModels.AccountViewModels.SignUp;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using PhoneNumbers;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Xml;
using IpData;

namespace Worldex.Infrastructure.Services.User
{
    public class UserService : IUserService
    {
        private readonly WorldexContext _dbContext;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<UserService> _log;
        private readonly IConfiguration _configuration;

        public UserService(WorldexContext dbContext, ILogger<UserService> log, UserManager<ApplicationUser> userManager, IConfiguration configuration)
        {
            _dbContext = dbContext;
            _log = log;
            _userManager = userManager;
            _configuration = configuration;
        }

        /// <summary>
        /// Get User data
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>

        public async Task<ApplicationUser> FindUserDataByUserNameEmailMobile(string UserName)
        {
            string numeric = string.Empty;
            foreach (char str in UserName)
            {
                if (char.IsDigit(str))
                {      //if (numeric.Length < 10)
                    numeric += str.ToString();
                }
            }
            if (numeric.Length == UserName.Length)
            {
                var userdata = _dbContext.Users.Where(i => i.Mobile == UserName).AsNoTracking().FirstOrDefault();
                if (userdata != null)
                {
                    return userdata;
                }
            }
            else
            {
                var userdata = await _userManager.FindByEmailAsync(UserName);
                if (userdata != null)
                {
                    return userdata;
                }
            }
            return null;
        }

        public bool GetMobileNumber(string MobileNumber)
        {
            var userdata = _dbContext.Users.Where(i => i.Mobile == MobileNumber).FirstOrDefault();
            if (userdata?.Mobile == MobileNumber && (userdata.PhoneNumberConfirmed || userdata.EmailConfirmed))
                return false;
            else
                return true;
        }

        /// <summary>
        /// Get User Data
        /// </summary>
        /// <param name="MobileNumber"></param>
        /// <returns></returns>

        public async Task<ApplicationUser> FindByMobileNumber(string MobileNumber)
        {
            var userdata = _dbContext.Users.Where(i => i.Mobile == MobileNumber).FirstOrDefault();
            if (userdata != null)
            {
                //GetLoginWithMobileViewModel model = new GetLoginWithMobileViewModel();
                //model.Mobile = userdata.Mobile;
                //model.Id = userdata.Id;
                //model.IsEnabled = userdata.IsEnabled;
                //model.PhoneNumberConfirmed = userdata.PhoneNumberConfirmed;
                //return model;

                return userdata;
            }
            else
                return null;
        }

        /// <summary>
        /// Get User Data
        /// </summary>
        /// <param name="Email"></param>
        /// <returns></returns>
        public async Task<TempUserRegister> FindByEmail(string Email)
        {
            var userdata = _dbContext.Users.Where(i => i.Email == Email).FirstOrDefault();
            if (userdata != null)
            {
                TempUserRegister model = new TempUserRegister();
                model.Email = userdata.Email;
                model.Id = userdata.Id;
                return model;
            }
            else
                return null;
        }

        /// <summary>
        /// added by nirav savariya for random generate otp on 9/26/2018
        /// </summary>
        /// <returns></returns>

        public long GenerateRandomOTP()
        {
            try
            {
                string[] saAllowedCharacters = { "1", "2", "3", "4", "5", "6", "7", "8", "9", "0" };
                string sOTP = String.Empty;
                long sTempChars;
                Random rand = new Random();
                int iOTPLength = 6;
                try
                {
                    for (int i = 0; i < iOTPLength; i++)
                    {
                        int p = rand.Next(0, saAllowedCharacters.Length);
                        sTempChars = Convert.ToInt64(saAllowedCharacters[rand.Next(0, saAllowedCharacters.Length)]);
                        sOTP += sTempChars;
                    }
                }
                catch (Exception ex)
                {
                    return GenerateRandomOTP();
                    throw ex;
                }
                string firstcharacter = sOTP.Substring(0, 1);
                if (sOTP.Length == 6 && firstcharacter != "0")
                    return Convert.ToInt64(sOTP);
                else
                    return GenerateRandomOTP();
                //Random generator = new Random();
                //String sOTP = generator.Next(1, 999999).ToString("D6");
                //return Convert.ToInt64(sOTP);

            }
            catch (Exception ex)
            {
                _log.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                throw ex;
            }
        }

        /// <summary>
        /// added by nirav savariya for random generate otp with password on 10/15/2018
        /// </summary>
        /// <returns></returns>

        public string GenerateRandomOTPWithPassword(PasswordOptions opts = null)
        {
            try
            {
                if (opts == null) opts = new PasswordOptions()
                {
                    RequiredLength = 64,
                    RequiredUniqueChars = 4,
                    RequireDigit = true,
                    RequireLowercase = true,
                    RequireNonAlphanumeric = true,
                    RequireUppercase = true,

                };

                string[] randomChars = new[] {
        "ABCDEFGHJKLMNOPQRSTUVWXYZ",    // uppercase 
        "abcdefghijkmnopqrstuvwxyz",    // lowercase
        "0123456789",                   // digits
        "!@$^*"                      // non-alphanumeric
    };
                Random rand = new Random(Environment.TickCount);
                List<char> chars = new List<char>();

                if (opts.RequireUppercase)
                    chars.Insert(rand.Next(0, chars.Count),
                        randomChars[0][rand.Next(0, randomChars[0].Length)]);

                if (opts.RequireLowercase)
                    chars.Insert(rand.Next(0, chars.Count),
                        randomChars[1][rand.Next(0, randomChars[1].Length)]);

                if (opts.RequireDigit)
                    chars.Insert(rand.Next(0, chars.Count),
                        randomChars[2][rand.Next(0, randomChars[2].Length)]);

                if (opts.RequireNonAlphanumeric)
                    chars.Insert(rand.Next(0, chars.Count),
                        randomChars[3][rand.Next(0, randomChars[3].Length)]);

                for (int i = chars.Count; i < opts.RequiredLength
                    || chars.Distinct().Count() < opts.RequiredUniqueChars; i++)
                {
                    string rcs = randomChars[rand.Next(0, randomChars.Length)];
                    chars.Insert(rand.Next(0, chars.Count),
                        rcs[rand.Next(0, rcs.Length)]);
                }

                return new string(chars.ToArray());

                //string lowers = "abcdefghijklmnopqrstuvwxyz";
                //string uppers = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
                //string number = "0123456789";
                //string specialChars = "@$?()";
                //Random random = new Random();

                //string generated = "!";
                //for (int i = 1; i <= 18; i++)
                //    generated = generated.Insert(
                //        random.Next(generated.Length),
                //        lowers[random.Next(lowers.Length - 1)].ToString()
                //    );

                //for (int i = 1; i <= 15; i++)
                //    generated = generated.Insert(
                //        random.Next(generated.Length),
                //        uppers[random.Next(uppers.Length - 1)].ToString()
                //    );

                //for (int i = 1; i <= 27; i++)
                //    generated = generated.Insert(
                //        random.Next(generated.Length),
                //        number[random.Next(number.Length - 1)].ToString()
                //    );

                //for (int i = 1; i <= 4; i++)
                //    generated = generated.Insert(
                //        random.Next(generated.Length),
                //        specialChars[random.Next(specialChars.Length - 1)].ToString()
                //    );

                //return generated.Replace("!", string.Empty);
            }
            catch (Exception ex)
            {
                _log.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                throw ex;
            }
        }

        public string GenerateRandomPassword(PasswordOptions opts = null)
        {
            if (opts == null) opts = new PasswordOptions()
            {
                RequiredLength = 6,
                RequiredUniqueChars = 4,
                RequireDigit = true,
                RequireLowercase = true,
                RequireNonAlphanumeric = true,
                RequireUppercase = true,

            };

            string[] randomChars = new[] {
        "ABCDEFGHJKLMNOPQRSTUVWXYZ",    // uppercase 
        "abcdefghijkmnopqrstuvwxyz",    // lowercase
        "0123456789",                   // digits
         "!@$^*"                        // non-alphanumeric
    };
            Random rand = new Random(Environment.TickCount);
            List<char> chars = new List<char>();

            if (opts.RequireUppercase)
                chars.Insert(rand.Next(0, chars.Count),
                    randomChars[0][rand.Next(0, randomChars[0].Length)]);

            if (opts.RequireLowercase)
                chars.Insert(rand.Next(0, chars.Count),
                    randomChars[1][rand.Next(0, randomChars[1].Length)]);

            if (opts.RequireDigit)
                chars.Insert(rand.Next(0, chars.Count),
                    randomChars[2][rand.Next(0, randomChars[2].Length)]);

            if (opts.RequireNonAlphanumeric)
                chars.Insert(rand.Next(0, chars.Count),
                    randomChars[3][rand.Next(0, randomChars[3].Length)]);

            for (int i = chars.Count; i < opts.RequiredLength
                || chars.Distinct().Count() < opts.RequiredUniqueChars; i++)
            {
                string rcs = randomChars[rand.Next(0, randomChars.Length)];
                chars.Insert(rand.Next(0, chars.Count),
                    rcs[rand.Next(0, rcs.Length)]);
            }

            return new string(chars.ToArray());
        }

        public SocialCustomPasswordViewMoel GenerateRandomSocialPassword(string ProvideKey)
        {
            try
            {
                if (!string.IsNullOrEmpty(ProvideKey))
                {
                    string str64 = GenerateRandomOTPWithPassword();
                    string alpha = string.Empty; string numeric = string.Empty, password = string.Empty;
                    foreach (char str in str64)
                    {
                        if (char.IsDigit(str))
                        {
                            if (numeric.Length < 6)
                                numeric += str.ToString();
                            else
                                alpha += str.ToString();
                        }
                        else
                            alpha += str.ToString();
                    }
                    if (ProvideKey.Length > 12)
                    {
                        string _Pass1 = alpha.Substring(0, 20);
                        string _Pass11 = _Pass1 + ProvideKey.Substring(1, 2);
                        string _Pass2 = alpha.Substring(20, 10);
                        string _Pass22 = _Pass2 + ProvideKey.Substring(6, 3);
                        string _Pass3 = alpha.Substring(30, 28);
                        string _Pass33 = _Pass3 + ProvideKey.Substring(10, 1);
                        password = _Pass11 + _Pass22 + _Pass33;
                    }
                    else
                    {
                        string _Pass1 = alpha.Substring(0, 20);   // If the provider key length is less then provide key combination is skip 2 and then add 6 to password.
                        string _Pass11 = _Pass1 + ProvideKey.Substring(1, 6);
                        string _Pass2 = alpha.Substring(20, 10);
                        string _Pass3 = alpha.Substring(30, 28);
                        password = _Pass11 + _Pass2 + _Pass3;
                    }

                    return new SocialCustomPasswordViewMoel()
                    {
                        Password = password,
                        AppKey = alpha
                    };

                }

            }
            catch (Exception ex)
            {
                _log.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);

                throw ex;
            }
            return null;
        }
        public async Task<bool> IsValidPhoneNumber(string Mobilenumber, string CountryCode)
        {
            //try
            //{
            //    PhoneNumberUtil phoneUtil = PhoneNumberUtil.GetInstance();
            //    //string countryCode = "IN";
            //    //string Code = GetCountryByIP(IpAddress);
            //    PhoneNumbers.PhoneNumber phoneNumber = phoneUtil.Parse(Mobilenumber, CountryCode);

            //    return phoneUtil.IsValidNumber(phoneNumber); // returns true for valid number    
            //}
            //catch (Exception ex)
            //{
            //    return false;
            //}

            PhoneNumberUtil phoneUtil = PhoneNumberUtil.GetInstance();
            try
            {
                string telephoneNumber = Mobilenumber;
                string countryCode = CountryCode;
                PhoneNumbers.PhoneNumber phoneNumber = phoneUtil.Parse(telephoneNumber, countryCode);

                bool isMobile = false;
                bool isValidNumber = phoneUtil.IsValidNumber(phoneNumber); // returns true for valid number    

                //// returns trueor false w.r.t phone number region  
                bool isValidRegion = phoneUtil.IsValidNumberForRegion(phoneNumber, countryCode);

                //string region = phoneUtil.GetRegionCodeForNumber(phoneNumber); // GB, US , PK    

                var numberType = phoneUtil.GetNumberType(phoneNumber); // Produces Mobile , FIXED_LINE    

                string phoneNumberType = numberType.ToString();

                if (!string.IsNullOrEmpty(phoneNumberType) && (phoneNumberType == "MOBILE" || phoneNumberType == "FIXED_LINE_OR_MOBILE") && isValidNumber && isValidRegion)
                {
                    isMobile = true;
                }

                return isMobile;
            }
            catch (NumberParseException ex)
            {
                String errorMessage = "NumberParseException was thrown: " + ex.Message.ToString();
                return false;
            }
        }
        //Rita-Nishit 27-2-19 set as default for ip whilte listing gives error
        public async Task<IPWiseDataViewModel> GetIPWiseDataOld(string ipAddress)
        {
            try
            {
                IPWiseDataViewModel iPWiseDataViewModel = new IPWiseDataViewModel();

                return iPWiseDataViewModel;
            }
            catch (NumberParseException ex)
            {
                ex.ToString();
                throw;
            }
        }

        public async Task<IPWiseDataViewModel> GetIPWiseData(string ipAddress)
        {
            try
            {
                IPWiseDataViewModel Response = new IPWiseDataViewModel();

                //Task.Run(() => HelperForLog.WriteLogIntoFileAsyncDtTm("GetIPWiseData", "UserService", "Start ipAddress : " + ipAddress, Helpers.UTC_To_IST()));

                IPAddress ip;//rita 1-7-19 ip validation
                bool ValidateIP = IPAddress.TryParse(ipAddress, out ip);
                if (!ValidateIP)
                {
                    Task.Run(() => HelperForLog.WriteLogIntoFileAsyncDtTm("GetIPWiseData", "UserService", "Invalid IP return : " + ipAddress, Helpers.UTC_To_IST()));
                    Response.IsValid = false;
                    return Response;
                }


                SqlParameter[] param1 = new SqlParameter[]{
                        new SqlParameter("@YourIP",SqlDbType.VarChar, 50, ParameterDirection.Input,false, 0, 0, String.Empty, DataRowVersion.Default, ipAddress),
                        new SqlParameter("@IsValid",SqlDbType.Bit, 100, ParameterDirection.Output, false, 0, 0, String.Empty, DataRowVersion.Default, Response.IsValid),
                        new SqlParameter("@CountryCode",SqlDbType.VarChar, 100, ParameterDirection.Output, false, 0, 0, String.Empty, DataRowVersion.Default, Response.CountryCode),
                        new SqlParameter("@Location",SqlDbType.VarChar, 100, ParameterDirection.Output, false, 0, 0, String.Empty, DataRowVersion.Default, Response.Location),

                };
                //var res =await Task.FromResult(_dbContext.Database.ExecuteSqlCommand("SP_GetIPAddressData @YourIP ,@IsValid OUTPUT,@CountryCode OUTPUT,@Location OUTPUT", param1));
                var res = _dbContext.Database.ExecuteSqlCommand("SP_GetIPAddressData @YourIP ,@IsValid OUTPUT,@CountryCode OUTPUT,@Location OUTPUT", param1);
                await Task.FromResult(res); //lalji for resolve second operation error
                Response.IsValid = Convert.ToBoolean(@param1[1].Value);
                Response.CountryCode = @param1[2].Value.ToString();
                Response.Location = @param1[3].Value.ToString();
                if (!Response.IsValid)//call api
                {
                    //Response = GetIPWiseDataFromAPI(ipAddress);

                    var APIKey = _configuration["IPDataUrlKey"];
                    Response = await Task.FromResult(IPDataThirdPartyAPI(APIKey, ipAddress)).Result;
                    return Response;
                }

                return Response;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("UserService", "GetIPWiseData--Main", ex);
                return null;
            }
        }
        public IPWiseDataViewModel GetIPWiseDataFromAPI(string ipAddress) // Old Method and replace by new ,change by Pratik 8-3-2019
        {
            try
            {
                IPWiseDataViewModel Response = new IPWiseDataViewModel();
                Response.IsValid = false;

                //var APIUrl = _configuration["IPDataUrl"];
                //var APIKey = _configuration["IPDataUrlKey"];

                string APIUrl = _configuration.GetValue<string>("FreeGeoIpURL");
                if (APIUrl == null)//Rita 10-7-19 added validation for error
                    return Response;
                if (APIUrl == string.Empty)
                    return Response;

                var url = APIUrl + ipAddress;

                Task.Run(() => HelperForLog.WriteLogIntoFileAsyncDtTm("GetIPWiseDataFromAPI ", "UserService", "Request URL " + url, Helpers.UTC_To_IST()));
                string responseFromServer = CallToThirdPartyAPIMethod(url, "", "");

                Task.Run(() => HelperForLog.WriteLogIntoFileAsyncDtTm("GetIPWiseDataFromAPI--FreeIpGeoAPI Response", "UserService", responseFromServer, Helpers.UTC_To_IST()));
                if (responseFromServer != string.Empty)
                {
                    dynamic JsonResult = JObject.Parse(responseFromServer);
                    Response.Location = JsonResult.country_name;
                    Response.CountryCode = JsonResult.country_code;
                    string longitude = JsonResult.longitude;
                    string latitude = JsonResult.latitude;

                    SqlParameter[] param1 = new SqlParameter[]{
                        new SqlParameter("@YourIP",SqlDbType.VarChar, 50, ParameterDirection.Input,false, 0, 0, string.Empty, DataRowVersion.Default, ipAddress),
                        new SqlParameter("@IsValid",SqlDbType.Bit, 100, ParameterDirection.Output, false, 0, 0, string.Empty, DataRowVersion.Default, Response.IsValid),
                        new SqlParameter("@CountryCode",SqlDbType.VarChar, 100, ParameterDirection.InputOutput, false, 0, 0, string.Empty, DataRowVersion.Default, Response.CountryCode),
                        new SqlParameter("@Location",SqlDbType.VarChar, 100, ParameterDirection.InputOutput, false, 0, 0, string.Empty, DataRowVersion.Default, Response.Location),
                        new SqlParameter("@IsInsert",SqlDbType.TinyInt, 100, ParameterDirection.Input, false, 0, 0, string.Empty, DataRowVersion.Default, 1),
                        new SqlParameter("@Longitude",SqlDbType.Real, 100, ParameterDirection.Input, false, 0, 0, string.Empty, DataRowVersion.Default, longitude),
                        new SqlParameter("@Lattitude",SqlDbType.Real, 100, ParameterDirection.Input, false, 0, 0, string.Empty, DataRowVersion.Default, latitude),

                };

                    var res = _dbContext.Database.ExecuteSqlCommand("SP_GetIPAddressData @YourIP ,@IsValid OUTPUT,@CountryCode OUTPUT,@Location OUTPUT,@IsInsert,@Longitude,@Lattitude", param1);
                    Response.IsValid = Convert.ToBoolean(@param1[1].Value);
                }

                return Response;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("GetIPWiseDataFromAPI", this.GetType().Name, ex);
                return null;
            }

        }
        private string CallToThirdPartyAPIMethod(string UrlString, string MethodName, string RequestObj, string MethodType = "GET")
        {

            try
            {
                string responseFromServer = "";
                object ResponseObj = new object();
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(UrlString);
                httpWebRequest.ContentType = "application/json; charset=utf-8";
                httpWebRequest.Method = MethodType;
                httpWebRequest.KeepAlive = false;
                httpWebRequest.Timeout = 180000;
                Task.Run(() => HelperForLog.WriteLogIntoFileAsyncDtTm("CallToThirdPartyAPIMethod", "UserService", "URL : " + UrlString + " ThirdParty Request " + RequestObj, Helpers.UTC_To_IST()));
                
                if (MethodType == "POST")
                {
                    using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                    {
                        string json = RequestObj; //JsonConvert.SerializeObject(RequestObj);

                        streamWriter.Write(json);
                        streamWriter.Flush();
                        streamWriter.Close();
                    }
                }
                HttpWebResponse httpWebResponse = httpWebRequest.GetResponse() as HttpWebResponse;
                using (StreamReader sr = new StreamReader(httpWebResponse.GetResponseStream()))
                {
                    responseFromServer = sr.ReadToEnd();
                    sr.Close();
                    sr.Dispose();

                }
                httpWebResponse.Close();
                //Task.Run(() => HelperForLog.WriteLogIntoFileAsyncDtTm("CallToThirdPartyAPIMethod", "UserService", "URL : " + UrlString + " ThirdParty Response " + responseFromServer, Helpers.UTC_To_IST()));

                return responseFromServer;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("UserService", "GetIPWiseDataFromAPI-Third party Exception", ex);
                return "";
            }
        }
        //public async Task<IPWiseDataViewModel> GetIPWiseDataBack(string ipAddress) // Old Method and replace by new ,change by Pratik 8-3-2019
        //{
        //    try
        //    {
        //        var url = "http://ip-api.com/xml/" + ipAddress;
        //       var request = System.Net.WebRequest.Create(url);
        //        string strReturnVal;
        //        string strLocation;
        //        using (WebResponse wrs = request.GetResponse())
        //        using (Stream stream = wrs.GetResponseStream())
        //        using (StreamReader reader = new StreamReader(stream))
        //        {
        //            string response = reader.ReadToEnd();

        //            XmlDocument ipInfoXML = new XmlDocument();
        //            ipInfoXML.LoadXml(response);
        //            XmlNodeList responseXML = ipInfoXML.GetElementsByTagName("query");
        //            NameValueCollection dataXML = new NameValueCollection();

        //            dataXML.Add(responseXML.Item(0).ChildNodes[2].InnerText, responseXML.Item(0).ChildNodes[2].Value);
        //            IPWiseDataViewModel iPWiseDataViewModel = new IPWiseDataViewModel();
        //            //strReturnVal = responseXML.Item(0).ChildNodes[1].InnerText.ToString(); // Contry
        //            //strReturnVal += "(" +responseXML.Item(0).ChildNodes[2].InnerText.ToString() + ")";  // Contry Code 
        //            string Status = responseXML.Item(0).ChildNodes[0].InnerText.ToString();
        //            if (!string.IsNullOrEmpty(Status) && Status == "fail")
        //            {
        //                iPWiseDataViewModel.CountryCode = Status;
        //                return iPWiseDataViewModel;
        //            }


        //            strReturnVal = responseXML.Item(0).ChildNodes[2].InnerText.ToString();  // Contry Code 
        //            iPWiseDataViewModel.CountryCode = strReturnVal;

        //            strLocation = responseXML.Item(0).ChildNodes[5].InnerText.ToString();  // City name 
        //            strLocation += "," + responseXML.Item(0).ChildNodes[4].InnerText.ToString();/// State name
        //            strLocation += "," + responseXML.Item(0).ChildNodes[1].InnerText.ToString(); ///  Contry name
        //            iPWiseDataViewModel.Location = strLocation;
        //            return iPWiseDataViewModel;

        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        ex.ToString();
        //        throw;
        //    }

        //}

        public async Task<string> GetCountryByIP(string ipAddress)
        {
            try
            {
                IPWiseDataViewModel Response = new IPWiseDataViewModel();
                string CountryCode = "";
                SqlParameter[] param1 = new SqlParameter[]{
                        new SqlParameter("@YourIP",SqlDbType.VarChar, 50, ParameterDirection.Input,false, 0, 0, String.Empty, DataRowVersion.Default, ipAddress),
                        new SqlParameter("@IsValid",SqlDbType.Bit, 100, ParameterDirection.Output, false, 0, 0, String.Empty, DataRowVersion.Default, Response.IsValid),
                        new SqlParameter("@CountryCode",SqlDbType.VarChar, 100, ParameterDirection.Output, false, 0, 0, String.Empty, DataRowVersion.Default, Response.CountryCode),
                        new SqlParameter("@Location",SqlDbType.VarChar, 100, ParameterDirection.Output, false, 0, 0, String.Empty, DataRowVersion.Default, Response.Location),
                };
                var res = _dbContext.Database.ExecuteSqlCommand("SP_GetIPAddressData @YourIP ,@IsValid OUTPUT,@CountryCode OUTPUT,@Location OUTPUT", param1);
                CountryCode = @param1[2].Value.ToString();
                return CountryCode;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return null;
            }
        }

        //public async Task<string> GetCountryByIP(string ipAddress) // Old Method :: Change by Pratik 8-3-2019
        //{
        //    try
        //    {
        //        var url = "http://ip-api.com/xml/" + ipAddress;
        //        var request = System.Net.WebRequest.Create(url);
        //        string strReturnVal;
        //        using (WebResponse wrs = request.GetResponse())
        //        using (Stream stream = wrs.GetResponseStream())
        //        using (StreamReader reader = new StreamReader(stream))
        //        {
        //            string response = reader.ReadToEnd();

        //            XmlDocument ipInfoXML = new XmlDocument();
        //            ipInfoXML.LoadXml(response);
        //            XmlNodeList responseXML = ipInfoXML.GetElementsByTagName("query");
        //            NameValueCollection dataXML = new NameValueCollection();

        //            dataXML.Add(responseXML.Item(0).ChildNodes[2].InnerText, responseXML.Item(0).ChildNodes[2].Value);

        //            //strReturnVal = responseXML.Item(0).ChildNodes[1].InnerText.ToString(); // Contry
        //            //strReturnVal += "(" +responseXML.Item(0).ChildNodes[2].InnerText.ToString() + ")";  // Contry Code 
        //            string Status = responseXML.Item(0).ChildNodes[0].InnerText.ToString();
        //            if (!string.IsNullOrEmpty(Status) && Status == "fail")
        //                return Status;

        //            strReturnVal = responseXML.Item(0).ChildNodes[2].InnerText.ToString();  // Contry Code 
        //            if (!string.IsNullOrEmpty(strReturnVal))
        //                return strReturnVal;
        //            else
        //                return strReturnVal = "fail";

        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return ex.ToString();
        //    }

        //}

        public async Task<string> GetLocationByIP(string ipAddress)
        {
            try
            {
                IPWiseDataViewModel Response = new IPWiseDataViewModel();
                string CountryCode = "";
                SqlParameter[] param1 = new SqlParameter[]{
                        new SqlParameter("@YourIP",SqlDbType.VarChar, 50, ParameterDirection.Input,false, 0, 0, String.Empty, DataRowVersion.Default, ipAddress),
                       new SqlParameter("@IsValid",SqlDbType.Bit, 100, ParameterDirection.Output, false, 0, 0, String.Empty, DataRowVersion.Default, Response.IsValid),
                        new SqlParameter("@CountryCode",SqlDbType.VarChar, 100, ParameterDirection.Output, false, 0, 0, String.Empty, DataRowVersion.Default, Response.CountryCode),
                        new SqlParameter("@Location",SqlDbType.VarChar, 100, ParameterDirection.Output, false, 0, 0, String.Empty, DataRowVersion.Default, Response.Location),

                };
                var res = _dbContext.Database.ExecuteSqlCommand("SP_GetIPAddressData @YourIP ,@IsValid OUTPUT,@CountryCode OUTPUT,@Location OUTPUT", param1);

                CountryCode = @param1[2].Value.ToString() + " " + @param1[3].Value.ToString();
                return CountryCode;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return null;
            }
        }

        //public async Task<string> GetLocationByIP(string ipAddress) // Old Method :: Change with new by Pratik 8-3-2019
        //{
        //    try
        //    {
        //        var url = "http://ip-api.com/xml/" + ipAddress;
        //        var request = System.Net.WebRequest.Create(url);
        //        string strReturnVal;
        //        using (WebResponse wrs = request.GetResponse())
        //        using (Stream stream = wrs.GetResponseStream())
        //        using (StreamReader reader = new StreamReader(stream))
        //        {
        //            string response = reader.ReadToEnd();

        //            XmlDocument ipInfoXML = new XmlDocument();
        //            ipInfoXML.LoadXml(response);
        //            XmlNodeList responseXML = ipInfoXML.GetElementsByTagName("query");
        //            NameValueCollection dataXML = new NameValueCollection();

        //            dataXML.Add(responseXML.Item(0).ChildNodes[2].InnerText, responseXML.Item(0).ChildNodes[2].Value);

        //            //strReturnVal = responseXML.Item(0).ChildNodes[1].InnerText.ToString(); // Contry
        //            //strReturnVal += "(" +responseXML.Item(0).ChildNodes[2].InnerText.ToString() + ")";  // Contry Code 
        //            string Status = responseXML.Item(0).ChildNodes[0].InnerText.ToString();
        //            if (!string.IsNullOrEmpty(Status) && Status == "fail")
        //                return Status;

        //            strReturnVal = responseXML.Item(0).ChildNodes[5].InnerText.ToString();  // City name 
        //            strReturnVal += "," + responseXML.Item(0).ChildNodes[4].InnerText.ToString();/// State name
        //            strReturnVal += "," + responseXML.Item(0).ChildNodes[1].InnerText.ToString(); ///  Contry name
        //            return strReturnVal;

        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return ex.ToString();
        //    }

        //}

        public List<GetUserData> GetAllUserData()
        {
            try
            {
                var users = _dbContext.GetUserData.FromSql(" select Id,Case when (FirstName Is not null and LastName Is not null) then FirstName +' '+LastName when email is not null then email else UserName end as UserName From Bizuser").ToList();
                if (users.Count > 0)
                {
                    return users;
                }
                return null;
                //var UserData = _dbContext.Users.ToList();
                //var UserDataList = new List<GetUserData>();
                //if (UserData != null)
                //{
                //    foreach (var item in UserData)
                //    {
                //        GetUserData imodel = new GetUserData();
                //        imodel.Id = item.Id;
                //        if (!string.IsNullOrEmpty(item.FirstName) && !string.IsNullOrEmpty(item.LastName))
                //            imodel.UserName = item.FirstName + " " + item.LastName;
                //        else
                //            imodel.UserName = item.UserName;
                //        UserDataList.Add(imodel);
                //    }
                //    return UserDataList.ToList();
                //}
                //else
                //    return null;
            }
            catch (Exception ex)
            {
                ex.ToString();
                throw;
            }
        }

        //Uday 21-01-2019 Return Joining date of user for calcualte historical performance
        public DateTime GetUserJoiningDate(long UserId)
        {
            DateTime JoiningDate = new DateTime();
            try
            {
                var userdata = _dbContext.Users.Where(i => i.Id == UserId).FirstOrDefault();
                if (userdata != null)
                {
                    JoiningDate = userdata.CreatedDate;
                    return JoiningDate;
                }
                else
                    return JoiningDate;
            }
            catch (Exception ex)
            {
                JoiningDate = new DateTime();
                return JoiningDate;
            }
        }

        public bool GetUserById(long UserId)
        {
            try
            {
                var userdata = _dbContext.Users.Where(i => i.Id == UserId).FirstOrDefault();
                if (userdata != null)
                {
                    return true;
                }
                else
                    return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public bool CheckMobileNumberExists(string MobileNumber)
        {
            try
            {
                var userdata = _dbContext.Users.Where(i => i.Mobile == MobileNumber).FirstOrDefault();
                if (userdata?.Mobile == MobileNumber)
                    return false;
                else
                    return true;
            }
            catch (Exception ex)
            {
                ex.ToString();
                throw ex;
            }
        }

        //Uday 14-02-2019 Get the user data by id
        public ApplicationUser GetUserDataById(long UserId)
        {
            try
            {
                var userdata = _dbContext.Users.Where(i => i.Id == UserId).FirstOrDefault();
                if (userdata != null)
                {
                    return userdata;
                }
                else
                    return null;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return null;
            }
        }

        //khushali 04-04-2019 Move from Worldex/Web/API/ManageController TO  UserService
        public ApplicationUserPhotos GetUserPhoto(long UserId)
        {
            try
            {
                var profileImage = _dbContext.ApplicationUserPhotos.Include(i => i.ApplicationUser).FirstOrDefault(i => i.ApplicationUser.Id == UserId);
                return profileImage;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return null;
            }
        }

        //khushali 04-04-2019 Move from Worldex/Web/API/ManageController TO  UserService
        public async Task<BizResponseClass> PostUserPhotoAsync(IFormFile file, int UserId)
        {
            BizResponseClass Response = new BizResponseClass();
            try
            {
                if (string.IsNullOrEmpty(file?.ContentType) || (file.Length == 0))
                {
                    Response.ReturnMsg = "Image provided is invalid";
                    Response.ReturnCode = enResponseCode.Fail;
                    return Response;
                }

                var size = file.Length;

                if (size > Convert.ToInt64(_configuration["MaxImageUploadSize"]))
                {
                    Response.ReturnMsg = "Image size greater than allowed size";
                    Response.ReturnCode = enResponseCode.Fail;
                    return Response;
                }

                using (var memoryStream = new MemoryStream())
                {
                    var existingImage = _dbContext.ApplicationUserPhotos.FirstOrDefault(i => i.ApplicationUserId == UserId);

                    await file.CopyToAsync(memoryStream);

                    if (existingImage == null)
                    {
                        var userImage = new ApplicationUserPhotos
                        {
                            ContentType = file.ContentType,
                            Content = memoryStream.ToArray(),
                            ApplicationUserId = UserId
                        };
                        _dbContext.ApplicationUserPhotos.Add(userImage);
                    }
                    else
                    {
                        existingImage.ContentType = file.ContentType;
                        existingImage.Content = memoryStream.ToArray();
                        _dbContext.ApplicationUserPhotos.Update(existingImage);
                    }
                    await _dbContext.SaveChangesAsync();
                }

                Response.ReturnMsg = "UserPhoto upload successfully.";
                Response.ReturnCode = enResponseCode.Success;
                return Response;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return null;
            }
        }
        public LanguagePreferenceMaster GetLanguagePreferenceMaster(string PreferedLanguage)
        {
            try
            {
                var language = (from l in _dbContext.LanguagePreferenceMaster
                                where l.PreferedLanguage == PreferedLanguage
                                select l).FirstOrDefault();

                return language;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return null;
            }
        }

        public async Task<BizResponseClass> UserValidationForWithdraw(long UserId)
        {
            try
            {
                var waitTime = _configuration["WithdrawHoursWait"];
                if (string.IsNullOrEmpty(waitTime.ToString()))
                    waitTime = 6.ToString();
                
                BizResponseClass bizResponse = new BizResponseClass();
                var User = await _userManager.FindByIdAsync(UserId.ToString());

                if (User == null || string.IsNullOrEmpty(User.Id.ToString()))
                {
                    bizResponse.ErrorCode = enErrorCode.Status4037UserNotAvailable;
                    bizResponse.ReturnCode = enResponseCode.Fail;
                    bizResponse.ReturnMsg = EnResponseMessage.UserNotExist;
                }
                else if (!User.EmailConfirmed)
                {
                    bizResponse.ErrorCode = enErrorCode.Status4036VerifyPending;
                    bizResponse.ReturnCode = enResponseCode.Fail;
                    bizResponse.ReturnMsg = EnResponseMessage.SignUpTempUserEmailVerifyPending;
                }
                else if (!User.IsEnabled)
                {
                    bizResponse.ErrorCode = enErrorCode.Status4168UserNotActive;
                    bizResponse.ReturnCode = enResponseCode.Fail;
                    bizResponse.ReturnMsg = EnResponseMessage.UserNotActive;
                }
                else if (User.LockoutEnd >= Helpers.UTC_To_IST())
                {
                    bizResponse.ErrorCode = enErrorCode.Status423Locked;
                    bizResponse.ReturnCode = enResponseCode.Fail;
                    bizResponse.ReturnMsg = EnResponseMessage.StandardLoginLockOut;
                }
                else if ((Helpers.UTC_To_IST() - User.CreatedDate).TotalHours < Convert.ToInt32(waitTime.ToString()))
                {
                    bizResponse.ErrorCode = enErrorCode.Status14214UserCreatedTimeNotValid;
                    bizResponse.ReturnCode = enResponseCode.Fail;
                    bizResponse.ReturnMsg = EnResponseMessage.Status14214UserCreatedTimeNotValid;
                }
                else
                {
                    bizResponse.ErrorCode = enErrorCode.Success;
                    bizResponse.ReturnCode = enResponseCode.Success;
                    bizResponse.ReturnMsg = "Success";
                }
                return bizResponse;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return null;
            }

        }


        /// <summary>
        /// This method use for validate ip via thirtdparty api ipdata -Lalji 16-09-2020
        /// </summary>
        /// <param name="apikey"></param>
        /// <param name="IpAddress"></param>
        /// <returns></returns>
        public async Task<IPWiseDataViewModel> IPDataThirdPartyAPI(string apikey, string IpAddress)
        {

            IPWiseDataViewModel response = new IPWiseDataViewModel();
            try
            {
                response.IsValid = false;
                try
                {
                    var client = new IpDataClient(apikey);

                    // Get IP data from IP
                    response.IpInfo = await client.Lookup(IpAddress);
                    Task.Run(() => HelperForLog.WriteLogIntoFileAsyncDtTm("IPDataThirdPartyAPI", "UserService", "IP Address : " + response.IpInfo.Ip + "countryName:" + response.IpInfo.CountryName + "CountryCode" + response.IpInfo.RegionCode, Helpers.UTC_To_IST()));

                }
                catch (Exception ex)
                {
                    HelperForLog.WriteErrorLog("IPData API-Exception", this.GetType().Name, ex);

                    try
                    {
                        //Call FreeGeoIp Api for get info of ip incase of ipdata fail to respond Address 29-09-2020
                        IPWiseDataViewModel Response = new IPWiseDataViewModel();

                        response = GetIPWiseDataFromAPI(IpAddress);
                        return response;
                    }
                    catch (Exception Ex)
                    {
                        HelperForLog.WriteErrorLog("FreeGeoIP-API", this.GetType().Name, Ex);
                        return null;
                    }
                }
                if (response.IpInfo != null)
                {
                    string Location = response.IpInfo.CountryName;
                    string CountryCode = response.IpInfo.CountryCode;
                    double? longitude = response.IpInfo.Longitude;
                    double? latitude = response.IpInfo.Latitude;

                    SqlParameter[] param1 = new SqlParameter[]{
                        new SqlParameter("@YourIP",SqlDbType.VarChar, 50, ParameterDirection.Input,false, 0, 0, string.Empty, DataRowVersion.Default, IpAddress),
                        new SqlParameter("@IsValid",SqlDbType.Bit, 100, ParameterDirection.Output, false, 0, 0, string.Empty, DataRowVersion.Default, false),
                        new SqlParameter("@CountryCode",SqlDbType.VarChar, 100, ParameterDirection.InputOutput, false, 0, 0, string.Empty, DataRowVersion.Default, CountryCode),
                        new SqlParameter("@Location",SqlDbType.VarChar, 100, ParameterDirection.InputOutput, false, 0, 0, string.Empty, DataRowVersion.Default,Location),
                        new SqlParameter("@IsInsert",SqlDbType.TinyInt, 100, ParameterDirection.Input, false, 0, 0, string.Empty, DataRowVersion.Default, 1),
                        new SqlParameter("@Longitude",SqlDbType.Real, 100, ParameterDirection.Input, false, 0, 0, string.Empty, DataRowVersion.Default, longitude),
                        new SqlParameter("@Lattitude",SqlDbType.Real, 100, ParameterDirection.Input, false, 0, 0, string.Empty, DataRowVersion.Default, latitude),

                };

                    var res = _dbContext.Database.ExecuteSqlCommand("SP_GetIPAddressData @YourIP ,@IsValid OUTPUT,@CountryCode OUTPUT,@Location OUTPUT,@IsInsert,@Longitude,@Lattitude", param1);
                    response.IsValid = Convert.ToBoolean(@param1[1].Value);
                    response.CountryCode = CountryCode;
                    response.Location = Location;
                }
                return await Task.FromResult(response);
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("IPDataThirdPartyAPI", this.GetType().Name, ex);
                response = new IPWiseDataViewModel();
                return response;
            }
        }

    }
}
