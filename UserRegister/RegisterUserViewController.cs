//
//  RegisterUserViewController.cs
//  Happy31.iOSApp
//
//  Copyright © 2017 Denis Klyucherov. All rights reserved.
//

using System;
using UIKit;
using CoreFoundation;
using System.Threading.Tasks;
using CoreGraphics;

namespace Happy31
{
    /// <summary>
    /// Register user View Controller
    /// </summary>
    public partial class RegisterUserViewController : UIViewController
    {
        UsersModel user;

        public RegisterUserViewController(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            AddBackground();

            // Set Navigation bar background
            this.NavigationController.NavigationBar.SetBackgroundImage(new UIImage(), UIBarMetrics.Default);
            this.NavigationController.NavigationBar.ShadowImage = new UIImage();
            this.NavigationController.NavigationBar.Translucent = true;

            if (this.RestorationIdentifier == "EnterNameRegisterUser")
            {
                enterNameImage.Image = UIImage.FromBundle("RegistrationName").ImageWithRenderingMode(UIImageRenderingMode.AlwaysTemplate);
                enterNameImage.ContentMode = UIViewContentMode.ScaleAspectFit;
                enterNameImage.TintColor = UIColor.FromRGB(198, 123, 112);

                enterNameLabel.TextColor = UIColor.DarkGray;

                backToSignInButton.TintColor = UIColor.FromRGB(198, 123, 112);

                userFirstNameTextField.Layer.BorderWidth = 0.5f;
                userFirstNameTextField.Layer.CornerRadius = 5;
                userFirstNameTextField.Layer.BorderColor = UIColor.DarkGray.CGColor;
                userFirstNameTextField.BackgroundColor = UIColor.Clear;

                userLastNameTextField.Layer.BorderWidth = 0.5f;
                userLastNameTextField.Layer.CornerRadius = 5;
                userLastNameTextField.Layer.BorderColor = UIColor.DarkGray.CGColor;
                userLastNameTextField.BackgroundColor = UIColor.Clear;

                enterNameNextButton.SetTitleColor(UIColor.DarkGray, UIControlState.Normal);

                userFirstNameTextField.ShouldReturn = ((textField) => { userLastNameTextField.BecomeFirstResponder(); return true; });
                userLastNameTextField.ShouldReturn = ((textField) => { EnterNameNextButton_TouchUpInside(enterNameNextButton); return true; });
            }
            else if (this.RestorationIdentifier == "EnterEmailRegisterUser")
            {
                enterEmailImage.Image = UIImage.FromBundle("RegistrationEmail").ImageWithRenderingMode(UIImageRenderingMode.AlwaysTemplate);
                enterEmailImage.ContentMode = UIViewContentMode.ScaleAspectFit;
                enterEmailImage.TintColor = UIColor.FromRGB(198, 123, 112);

                enterEmailLabel.TextColor = UIColor.DarkGray;

                userEmailTextField.Layer.BorderWidth = 0.5f;
                userEmailTextField.Layer.CornerRadius = 5;
                userEmailTextField.BackgroundColor = UIColor.Clear;

                enterEmailNextButton.SetTitleColor(UIColor.DarkGray, UIControlState.Normal);

                userEmailTextField.ShouldReturn = ((textField) => { EnterEmailNextButton_TouchUpInside(enterEmailNextButton); return true; });
            }
            else if (this.RestorationIdentifier == "EnterPasswordRegisterUser")
            {
                enterPasswordImage.Image = UIImage.FromBundle("RegistrationPassword").ImageWithRenderingMode(UIImageRenderingMode.AlwaysTemplate);
                enterPasswordImage.ContentMode = UIViewContentMode.ScaleAspectFit;
                enterPasswordImage.TintColor = UIColor.FromRGB(198, 123, 112);

                enterPasswordLabel.TextColor = UIColor.DarkGray;

                userPasswordTextField.Layer.BorderWidth = 0.5f;
                userPasswordTextField.Layer.CornerRadius = 5;
                userPasswordTextField.BackgroundColor = UIColor.Clear;

                userRepeatPasswordTextField.Layer.BorderWidth = 0.5f;
                userRepeatPasswordTextField.Layer.CornerRadius = 5;
                userRepeatPasswordTextField.BackgroundColor = UIColor.Clear;

                signUpButton.Layer.BorderWidth = 0.5f;
                signUpButton.Layer.CornerRadius = 5;
                signUpButton.BackgroundColor = UIColor.FromRGB(198, 123, 112);
                signUpButton.SetTitleColor(UIColor.White, UIControlState.Normal);

                userPasswordTextField.ShouldReturn = ((textField) => { userRepeatPasswordTextField.BecomeFirstResponder(); return true; });
                userRepeatPasswordTextField.ShouldReturn = ((textField) =>
                {
                    // Dismiss the Keyboard
                    userRepeatPasswordTextField.ResignFirstResponder();
                    SignUpButton_TouchUpInside(signUpButton);
                    return true;
                });
            }
        }

        void AddBackground()
        {
            // Get screen width and height
            var width = UIScreen.MainScreen.Bounds.Size.Width;
            var height = UIScreen.MainScreen.Bounds.Size.Height;

            var imageViewBackground = new UIImageView() { Frame = new CGRect(0, 0, width, height) };
            imageViewBackground.Image = UIImage.FromBundle("CommonBackground");

            imageViewBackground.ContentMode = UIViewContentMode.ScaleAspectFill;

            View.AddSubview(imageViewBackground);
            View.SendSubviewToBack(imageViewBackground);
        }

        partial void EnterNameNextButton_TouchUpInside(UIButton sender)
        {
            string userFirstName = userFirstNameTextField.Text;
            string userLastName = userLastNameTextField.Text;

            if (string.IsNullOrWhiteSpace(userFirstName) || string.IsNullOrWhiteSpace(userLastName))
            {
                DisplayAlertMessage("First and last name are required to fill in");
                return;
            }
            else
            {
                RegisterUserSharedData.UserFirstName = userFirstName;
                RegisterUserSharedData.UserLastName = userLastName;

                var registerController = this.Storyboard.InstantiateViewController("EnterEmailRegisterUser") as RegisterUserViewController;
                if (registerController != null)
                    this.NavigationController.PushViewController(registerController, true);
            }
        }

        partial void EnterEmailNextButton_TouchUpInside(UIButton sender)
        {
            string userEmail = userEmailTextField.Text;

            if (string.IsNullOrWhiteSpace(userEmail))
            {
                DisplayAlertMessage("Email is required to fill in");
                return;
            }
            // If email do not meet requirements
            else if (!Validation.ValidationResult(userEmail, "email"))
            {
                DisplayAlertMessage(Validation.Message);
                return;
            }
            else
            {
                RegisterUserSharedData.UserEmail = userEmail;

                var registerController = this.Storyboard.InstantiateViewController("EnterPasswordRegisterUser") as RegisterUserViewController;
                if (registerController != null)
                    this.NavigationController.PushViewController(registerController, true);
            }
        }

        async partial void SignUpButton_TouchUpInside(UIButton sender)
        {
            string userPassword = userPasswordTextField.Text;
            string userRepeatPassword = userRepeatPasswordTextField.Text;

            // Check for empty fields
            if (string.IsNullOrWhiteSpace(userPassword) || string.IsNullOrWhiteSpace(userRepeatPassword))
            {
                // Display an alert message
                DisplayAlertMessage("All fields are required to fill in");
                return;
            }
            // If password do not meet requirements
            else if (!Validation.ValidationResult(userPassword, "password"))
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
                FirstName = RegisterUserSharedData.UserFirstName,
                LastName = RegisterUserSharedData.UserLastName,
                Email = RegisterUserSharedData.UserEmail.ToLower(),
                Password = userPassword,
                LoginProvider = "Email"
            };

            // Call REST service to send Json data
            RestService rs = new RestService();

            // Get Json data from server in JsonResponseModel format
            Task<UsersModel> jsonResponeTask = rs.UserLoginAndRegisterJson(user, "register");

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
            var alert = UIAlertController.Create(title: "", message: alertMessage, preferredStyle: UIAlertControllerStyle.Alert);
            var okAction = UIAlertAction.Create("Ok", UIAlertActionStyle.Cancel, (obj) =>
            {
                // If user has been added successfully, dismiss view controller
                if (statusUser == "Success")
                {
                    var loginController = this.Storyboard.InstantiateViewController("LoginUserViewController") as LoginUserViewController;
                    if (loginController != null)
                        this.PresentViewController(loginController, true, null);
                }
                else if (statusUser == "Error" && alertMessage == "Email already exists")
                {
                    //var registerController = this.Storyboard.InstantiateViewController("EnterEmailRegisterUser") as RegisterUserViewController;
                    //if (registerController != null)
                    this.NavigationController.PopViewController(true);
                }
            });
            alert.AddAction(okAction);
            this.PresentViewController(alert, true, null);

            // Remove overlay
            LoadingOverlay.RemoveOverlay();
        }

        partial void BackToSignInButton_TouchUpInside(UIButton sender)
        {
            var loginController = this.Storyboard.InstantiateViewController("LoginUserViewController") as LoginUserViewController;
            if (loginController != null)
                this.PresentViewController(loginController, true, null);
        }

        void DisplayAlertMessage(string message)
        {
            // Excecutes the given code in the background
            DispatchQueue.MainQueue.DispatchAsync(() =>
            {
                var alert = UIAlertController.Create(title: "", message: message, preferredStyle: UIAlertControllerStyle.Alert);
                var okAction = UIAlertAction.Create("Ok", UIAlertActionStyle.Cancel, null);
                alert.AddAction(okAction);
                this.PresentViewController(alert, true, null);
            });
        }
    }
}