using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.RegularExpressions;

namespace CoolDex.Core
{
    //Create Date Validator by Rushali (29-08-2019)
    public class DateValidator : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            if(value==null)
                return true;

            if (string.IsNullOrEmpty(value.ToString()))
                return true;

            var data = Convert.ToDateTime(value).ToString("yyyy-MM-dd");
            if (Regex.IsMatch(data, @"((17|18|19|20|21))-(0[1-9]|1[0-2])-((0|1)[0-9]|2[0-9]|3[0-1])", RegexOptions.IgnoreCase))
            {
                return true;
            }
            return false;
        }
    }
}
