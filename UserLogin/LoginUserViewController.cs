//
//  LoginUserViewController.cs
//  Happy31.iOSApp
//
//  Copyright © 2017 Denis Klyucherov. All rights reserved.
//

using Foundation;
using System;
using UIKit;
using Facebook.LoginKit;
using Facebook.CoreKit;
using System.Collections.Generic;
using CoreFoundation;
using System.Threading.Tasks;
using CoreGraphics;

namespace Happy31
{
    /// <summary>
    /// Managing of the user's login in the application. Facebook authentication
    /// </summary>
    public partial class LoginUserViewController : UIViewController
    {
        // Facebook permissions
        List<string> readPermissions = new List<string> { "public_profile", "email" };
        LoginButton loginView;
        UsersRepository userRepository;
        NSData facebookImageData;

        public LoginUserViewController (IntPtr handle) : base (handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            // Perform any additional setup after loading the view, typically from a nib.

            AddBackground();
            SetAppearance();

            // Create Table Users
            userRepository = new UsersRepository();

            userEmailTextField.ShouldReturn = ((textField) => { userPasswordTextField.BecomeFirstResponder(); return true; });
            userPasswordTextField.ShouldReturn = ((textField) =>
            { 
                // Dismiss the Keyboard
                userPasswordTextField.ResignFirstResponder();
                SignInButton_TouchUpInside(signInButton);
                return true;
            });

            facebookImageData = new NSData();
        }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);

            FacebookAuthentication();
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

        // Set appearance of buttons and textfields
        void SetAppearance()
        {
            logInLabel.TextColor = UIColor.DarkGray;

            signInButton.Layer.BorderWidth = 0.5f;
            signInButton.Layer.CornerRadius = 5;
            signInButton.BackgroundColor = UIColor.FromRGB(198, 123, 112);
            signInButton.SetTitleColor(UIColor.White, UIControlState.Normal);

            fbAuthButton.Layer.BorderWidth = 0.5f;
            fbAuthButton.Layer.CornerRadius = 5;
            fbAuthButton.SetTitleColor(UIColor.White, UIControlState.Normal);
            fbAuthButton.Hidden = true;

            userEmailTextField.Layer.BorderWidth = 0.5f;
            userEmailTextField.Layer.CornerRadius = 5;
            userEmailTextField.Layer.BorderColor = UIColor.DarkGray.CGColor;
            userEmailTextField.BackgroundColor = UIColor.Clear;

            userPasswordTextField.Layer.BorderWidth = 0.5f;
            userPasswordTextField.Layer.CornerRadius = 5;
            userPasswordTextField.Layer.BorderColor = UIColor.DarkGray.CGColor;
            userPasswordTextField.BackgroundColor = UIColor.Clear;

            registerButton.TintColor = UIColor.FromRGB(198, 123, 112);
        }

        // Facebook Authentification
        void FacebookAuthentication()
        {
            flag = false;
            // If was send true to Profile.EnableUpdatesOnAccessTokenChange method
            // this notification will be called after the user is logged in and
            // after the AccessToken is gotten
            Profile.Notifications.ObserveDidChange((sender, e) =>
            {
                if (e.NewProfile == null)
                    return;

                var accessToken = AccessToken.CurrentAccessToken.TokenString;
                var request = new GraphRequest("/" + e.NewProfile.UserID, new NSDictionary("fields", "email,first_name,last_name,picture.type(large)"), accessToken, null, "GET");
                request.Start((connection, result, error) =>
                {
                    // Show the loading overlay on the UI
                    LoadingOverlay.ShowOverlay(View);

                    var userInfo = result as NSDictionary;

                    // Get Facebook avatar image from url
                    var avatarUrl = new NSUrl(userInfo["picture"].ValueForKey(new NSString("data")).ValueForKey(new NSString("url")).ToString());
                    var data = NSData.FromUrl(avatarUrl);

                    facebookImageData = data;

                    // string facebookAvatar = data.GetBase64EncodedString(NSDataBase64EncodingOptions.None);
                    // facebookAvatar = new NSString(eefacebookAvatar, NSStringEncoding.UTF8); //Encoding.ASCII.GetString(ToByteArray(data));

                    // Add Facebook user to database
                    var user = new UsersModel()
                    {
                        FirstName = userInfo["first_name"].ToString(),
                        LastName = userInfo["last_name"].ToString(),
                        Email = userInfo["email"].ToString().ToLower(),
                        LoginProvider = "Facebook"
                    };

                    // Call REST service to send Json data
                    RestService rs = new RestService();

                    // Get Json data from server in JsonResponseModel format
                    Task<UsersModel> jsonResponeTask = rs.UserLoginAndRegisterJson(user, "register");

                    // If there was an error in PostJsonDataAsync class, display message
                    if (jsonResponeTask == null)
                    {
                        LoadingOverlay.RemoveOverlay();
                        return;
                    }

                    // Get user id from Json after login or display an error
                    GetUserIdFromJson(jsonResponeTask, user);

                    if (error != null)
                    {
                        LoadingOverlay.RemoveOverlay();
                        Console.WriteLine("Error in request. Start Facebook login");
                        return;
                    }
                });
            });

            // Set the Read and Publish permissions you want to get
            loginView = new LoginButton(fbAuthButton.Frame)
            {
                LoginBehavior = LoginBehavior.Native,
                ReadPermissions = readPermissions.ToArray()
            };

            // Handle actions once the user is logged in
            loginView.Completed += LoginView_Completed;

            // Handle actions once the user is logged out
            loginView.LoggedOut += (sender, e) =>
            {
                // Set logout User Defaults
                var plist = NSUserDefaults.StandardUserDefaults;
                plist.SetBool(false, "isUserLoggedIn"); // set flag that user logged out
                plist.SetString("", "userId");
                plist.Synchronize();
            };

            // Use your System Account of Settings
            //loginButton.LoginBehavior = LoginBehavior.SystemAccount;

            // Add views to main view
            View.AddSubview(loginView);
        }

        // Facebook: Get user id from Json after login or display an error
        async void GetUserIdFromJson(Task<UsersModel> jsonResponeTask, UsersModel user)
        {
            // Create instance of JsonResponseModel and pass jsonResponeTask there
            var jsonResponse = await jsonResponeTask;

            if (jsonResponse.Status == "Error")
                DisplayAlertMessage(jsonResponse.Message);
            else
            {
                var userToInsert = new UsersModel()
                {
                    Id = jsonResponse.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    LoginProvider = user.LoginProvider,
                    Avatar = await ConvertImage.ConvertImageToBinary(new UIImage(facebookImageData)),
                    CreatedAt = jsonResponse.CreatedAt
                };
                LoginIsSuccessful(userToInsert);
            }
        }

        void LoginView_Completed(object sender, LoginButtonCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                // Handle if there was an error
                LoadingOverlay.RemoveOverlay();
                Console.WriteLine("There was an error. Please, try again later");
                DisplayAlertMessage("There was an error. Please, try again later");
                return;
            }

            if (e.Result.IsCancelled)
            {
                return;
                // Handle if the user cancelled the login request
            }
        }

        async partial void SignInButton_TouchUpInside(UIButton sender)
        {
            string userEmail = userEmailTextField.Text;
            string userPassword = userPasswordTextField.Text;

            // If email or password do not meet requirements
            if (!Validation.ValidationResult(userEmail, "email") || !Validation.ValidationResult(userPassword, "password"))
            {
                DisplayAlertMessage(Validation.Message);
                return;
            }

            // Show the loading overlay on the UI
            LoadingOverlay.ShowOverlay(View);

            // Create instance of table Users
            var user = new UsersModel()
            {
                Email = userEmail.ToLower(),
                Password = userPassword
            };

            // Call REST service to send Json data
            RestService rs = new RestService();

            // Get Json data from server in JsonResponseModel format
            Task<UsersModel> jsonResponeTask = rs.UserLoginAndRegisterJson(user, "login");

            // If there was an error in PostJsonDataAsync class, display message
            if (jsonResponeTask == null)
            {
                LoadingOverlay.RemoveOverlay();
                DisplayAlertMessage(rs.Message);
                return;
            }

            // Get user id from Json after login or display an error
            // Create instance of JsonResponseModel and pass jsonResponeTask there
            var jsonResponse = await jsonResponeTask;

            // Get user id from Json after login or display an error
            if (jsonResponse.Status == "Error")
            {
                LoadingOverlay.RemoveOverlay();
                DisplayAlertMessage(jsonResponse.Message);
            }
            else if (!string.IsNullOrEmpty(jsonResponse.Id))
                LoginIsSuccessful(jsonResponse);
        }

        // Save user id on device to use it later
        async void LoginIsSuccessful(UsersModel user)
        {
            // Add the logged in user into local table
            if (await userRepository.AddUser(user) == 0)
            {
                //DisplayAlertMessage(userRepository.Message);
                return;
            }

            // Set Shared User Defaults
            var plist = NSUserDefaults.StandardUserDefaults;
            plist.SetBool(true, "isUserLoggedIn"); // set flag that user logged in
            plist.SetString(user.Id, "userId");

            // Set user settings
            UserSettings.DefaultSettings.SetSettings();

            // Create Table Prompts and load data from the remote server
            var promptRepository = new PromptsRepository();
            await promptRepository.GetTablePrompts();

            // After login, get full table "user_prompts" from remote database and insert it in local database
            var userPromptRepository = new UsersPromptsRepository();
            await userPromptRepository.SyncAllTableUserPrompts(user.Id);

            // Remove overlay
            LoadingOverlay.RemoveOverlay();

            // Create an instance of AppDelegate and call SetRootViewController method to display MainTabBarController after successfu login
            var appDelegate = UIApplication.SharedApplication.Delegate as AppDelegate;
            appDelegate.SetRootViewController(true);
        }

        // Display MainTabBarController
        void ShowMainTabController()
        {
            // Create an instance of AppDelegate and call SetRootViewController method to display MainTabBarController after successfu login
            var appDelegate = UIApplication.SharedApplication.Delegate as AppDelegate;
            appDelegate.SetRootViewController(true);
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

        partial void RegisterButton_TouchUpInside(UIButton sender)
        {
            // Launch a new instance of RegisterUserViewController
            var registerController = this.Storyboard.InstantiateViewController("EnterNameRegisterUser") as RegisterUserViewController;
            if (registerController != null)
            {
                var navigationController = new UINavigationController(registerController);
                this.PresentViewController(navigationController, true, null);
            }
        }
    }
}