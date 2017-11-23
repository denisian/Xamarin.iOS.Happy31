using System.Text.RegularExpressions;

namespace Happy31
{
    public static class Validation
    {
        static string emailPattern = @"^([a-z0-9_\.-]+)@([\da-z\.-]+)\.([a-z\.]{2,6})$";
        static string passwordPattern = @"^[a-zA-Z0-9_-]{4,50}$";

        static readonly Regex emailRegex = new Regex(emailPattern, RegexOptions.IgnoreCase);
        static readonly Regex passwordRegex = new Regex(passwordPattern);

        static string _message;
        public static string Message => _message;

        public static bool ValidationResult(string email, string password)
        {
            Match emailMatch = emailRegex.Match(email);
            Match passwordMatch = passwordRegex.Match(password);

            if (!emailMatch.Success && !passwordMatch.Success)
            {
                _message = "Email and Password are not correct";
                return false;
            }

            else if (!emailMatch.Success)
            {
                _message = "Email is missing or incorrect";
                return false;
            }

            else if (!passwordMatch.Success)
            {
                _message = "Password is missing or does not meet security requirements (minimum 4 characters)";
                return false;
            }

            else
                return true;
        }
    }
}
