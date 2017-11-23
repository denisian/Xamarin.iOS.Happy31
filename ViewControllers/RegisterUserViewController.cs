using Foundation;
using System;
using UIKit;
using CoreFoundation;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace Happy31
{
    public partial class RegisterUserViewController : UIViewController
    {
        UsersModel user;

        //LoadingOverlay loadOverlay;

        public RegisterUserViewController(IntPtr handle) : base(handle)
        {
        }

        async partial void SignUpButton_TouchUpInside(UIButton sender)
        {
            string userFirstName = userFirstNameTextField.Text;
            string userLastName = userLastNameTextField.Text;
            string userEmail = userEmailTextField.Text;
            string userPassword = userPasswordTextField.Text;
            string userRepeatPassword = userRepeatPasswordTextField.Text;

            // Check for empty fields
            if (string.IsNullOrWhiteSpace(userFirstName) ||
                string.IsNullOrWhiteSpace(userLastName) ||
                string.IsNullOrWhiteSpace(userEmail) ||
                string.IsNullOrWhiteSpace(userPassword) ||
                string.IsNullOrWhiteSpace(userRepeatPassword))
            {
                // Display an alert message
                DisplayAlertMessage("All fields are required to fill in");
                return;
            }
            // If email or password do not meet requirements
            else if (!Validation.ValidationResult(userEmail, userPassword))
            {
                DisplayAlertMessage(Validation.Message);
                return;
            }

            // Check if passwords match
            if (!userPassword.Equals(userRepeatPassword))
            {
                // Display an alert message
                DisplayAlertMessage("Passwords do not match");
                return;
            }

            // Show the loading overlay on the UI
            LoadingOverlay.ShowOverlay(View);

            // Add user
            // Create instance of table Users
            user = new UsersModel()
            {
                FirstName = userFirstName,
                LastName = userLastName,
                Email = userEmail,
                Password = userPassword,
                LoginProvider = "Email"
            };

            // Call REST service to send Json data
            RestService rs = new RestService();

            // Get Json data from server in JsonResponseModel format
            Task<JsonResponseModel> jsonResponeTask = rs.PostJsonDataAsync(user, "register");

            // If there was an error in PostJsonDataAsync class, display message
            if (jsonResponeTask == null)
            {
                DisplayAlertMessage(rs.Message);
                return;
            }

            // Create instance of JsonResponseModel and pass jsonResponeTask there
            var jsonResponse = await jsonResponeTask;

            // Get user status (Success/Error)
            string statusUser = jsonResponse.Status;
            string alertMessage = jsonResponse.Message;

            // Excecutes the given code in the background
            DispatchQueue.MainQueue.DispatchAsync(() =>
            {
                var alert = UIAlertController.Create(title: "Alert", message: alertMessage, preferredStyle: UIAlertControllerStyle.Alert);
                var okAction = UIAlertAction.Create("Ok", UIAlertActionStyle.Cancel, (obj) =>
                {
                    // If user has been added successfully, dismiss view controller
                    if (statusUser == "Success")
                        this.DismissViewController(true, null);
                });
                alert.AddAction(okAction);
                this.PresentViewController(alert, true, null);
            });

            // Remove overlay
            LoadingOverlay.RemoveOverlay();
        }

        partial void BackToSignInButton_TouchUpInside(UIButton sender)
        {
            this.DismissViewController(true, null);
            //string pass = PasswordHash.HashPassword(userPasswordTextField.Text);
            //Console.WriteLine(pass);
            //Console.WriteLine(PasswordHash.ValidatePassword(userPasswordTextField.Text, pass));
        }

        void DisplayAlertMessage(string message)
        {
            // Excecutes the given code in the background
            DispatchQueue.MainQueue.DispatchAsync(() =>
            {
                var alert = UIAlertController.Create(title: "Alert", message: message, preferredStyle: UIAlertControllerStyle.Alert);
                var okAction = UIAlertAction.Create("Ok", UIAlertActionStyle.Cancel, null);
                alert.AddAction(okAction);
                this.PresentViewController(alert, true, null);
            });
        }
    }
}