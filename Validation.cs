//
//  Validation.cs
//  Happy31.iOSApp
//
//  Copyright © 2017 Denis Klyucherov. All rights reserved.
//

using System.Text.RegularExpressions;

namespace Happy31
{
    /// <summary>
    /// Validation to check users' email and password
    /// </summary>
    public static class Validation
    {
        static string emailPattern = @"^([a-z0-9_\.-]+)@([\da-z\.-]+)\.([a-z\.]{2,6})$";
        static string passwordPattern = @"^[a-zA-Z0-9_-]{4,50}$";

        static readonly Regex emailRegex = new Regex(emailPattern, RegexOptions.IgnoreCase);
        static readonly Regex passwordRegex = new Regex(passwordPattern);

        static string _message;
        public static string Message => _message;

        public static bool ValidationResult(string value, string fieldName)
        {
            Match fieldToMatch;
            if (fieldName == "email")
                fieldToMatch = emailRegex.Match(value);
            else
                fieldToMatch = passwordRegex.Match(value);

            if (!fieldToMatch.Success)
            {
                if (fieldName == "email")
                    _message = "Email is not correct";
                else
                    _message = "Password does not meet security requirements (minimum 4 characters)";
                return false;
            }
            else
                return true;
        }
    }
}