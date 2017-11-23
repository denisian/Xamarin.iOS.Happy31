using Foundation;
using System;
using UIKit;
using Facebook.LoginKit;
using Facebook.CoreKit;
using System.Collections.Generic;
using CoreFoundation;
using System.Threading.Tasks;

namespace Happy31
{
    public partial class LoginUserViewController : UIViewController
    {
        UsersRepository userRepository;

        //string userId;

        // Facebook permissions
        List<string> readPermissions = new List<string> { "public_profile", "email" };

        LoginButton loginView;
        Task<JsonResponseModel> jsonResponeTask;

        public LoginUserViewController (IntPtr handle) : base (handle)
        {
           userRepository = new UsersRepository();
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            // Perform any additional setup after loading the view, typically from a nib.

            FacebookAuthentication();

        }

        // Facebook Authentification
        void FacebookAuthentication()
        {
            //var profile = Profile.CurrentProfile;
            //if (profile == null)
            //{
            //    loginButton = new LoginButton(fbAuthButton.Frame)
            //    {
            //        LoginBehavior = LoginBehavior.Native
            //    };

            //    loginButton.Completed += async (sender, e) => { await LoginView_Completed(sender, e); };

            //    fbAuthButton.Hidden = true;
            //    Add(loginButton);
            //}
          

            // Facebook Authentication

            // If was send true to Profile.EnableUpdatesOnAccessTokenChange method
            // this notification will be called after the user is logged in and
            // after the AccessToken is gotten
            Profile.Notifications.ObserveDidChange((sender, e) =>
            {
                Console.WriteLine("start1");
                if (e.NewProfile == null)
                {
                    return;
                }
                Console.WriteLine("start2");
                var accessToken = AccessToken.CurrentAccessToken.TokenString;
                var request = new GraphRequest("/" + e.NewProfile.UserID, new NSDictionary("fields", "email,first_name,last_name"), accessToken, null, "GET");
                request.Start((connection, result, error) =>
                {
                    Console.WriteLine("start3");
                    var userInfo = result as NSDictionary;

                    // Add Facebook user to database
                    var user = new UsersModel()
                    {
                        FirstName = userInfo["first_name"].ToString(),
                        LastName = userInfo["last_name"].ToString(),
                        Email = userInfo["email"].ToString(),
                        LoginProvider = "Facebook"
                    };
                    Console.WriteLine("start3.1");
                    // Call REST service to send Json data
                    RestService rs = new RestService();

                    // Get Json data from server in JsonResponseModel format
                    jsonResponeTask = rs.PostJsonDataAsync(user, "register");

                    // If there was an error in PostJsonDataAsync class, display message
                    if (jsonResponeTask == null)
                    {
                        Console.WriteLine(rs.Message);
                        return;
                    }
                    Console.WriteLine("start3.2");

                    // Get user id from Json after login or display an error
                    GetUserIdFromJson();

                    //userId = jsonResponse.Result.UserId;
                    //Console.WriteLine(userId);
                    Console.WriteLine("start3.3");
                });
            });
            Console.WriteLine("start4");

            // Set the Read and Publish permissions you want to get
            //loginButton = new LoginButton(new CGRect(51, 0, 218, 46))
            loginView = new LoginButton(fbAuthButton.Frame)
            //loginButton = new LoginButton()
            {
                LoginBehavior = LoginBehavior.Native,
                ReadPermissions = readPermissions.ToArray()
            };

            // Handle actions once the user is logged in
            loginView.Completed += LoginView_Completed;

            // Handle actions once the user is logged out
            //loginView.LoggedOut += (sender, e) => { 
            // Handle your logout
            //};
            loginView.LoggedOut += LoginView_LoggedOut;

            // Use your System Account of Settings
            //loginButton.LoginBehavior = LoginBehavior.SystemAccount;

            fbAuthButton.Hidden = true;

            // Add views to main view
            View.AddSubview(loginView);
        }

        // Facebook: Get user id from Json after login or display an error
        async void GetUserIdFromJson()
        {
            // Show the loading overlay on the UI
            LoadingOverlay.ShowOverlay(View);

            // Create instance of JsonResponseModel and pass jsonResponeTask there
            var jsonResponse = await jsonResponeTask;

            if (jsonResponse.Status == "Error")
                DisplayAlertMessage(jsonResponse.Message);
            else
                LoginIsSuccessful(jsonResponse.UserId);
        }

        void LoginView_Completed(object sender, LoginButtonCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                // Handle if there was an error
            }

            if (e.Result.IsCancelled)
            {
                // Handle if the user cancelled the login request
            }

            // Handle your successful login

            //var accessToken = AccessToken.CurrentAccessToken.TokenString;

            Console.WriteLine("LoginView_Completed");
           // DisplayAlertMessage(CommonClass.value);
            //Create access token for user
            //var accessToken = new JObject(); 
            //accessToken["access_token"] = tokenString;

            //Send the token to Azure App Service to auth with backend 
            //return await EasyMobileServiceClient.Current.MobileService.LoginAsync(MobileServiceAuthenticationProvider.Facebook, accessToken);
            //return  accessToken;
        }

        private void LoginView_LoggedOut(object sender, EventArgs e)
        {
            // Set logout User Defaults
            var plist = NSUserDefaults.StandardUserDefaults;
            plist.SetBool(false, "isUserLoggedIn"); // set flag that user logged out
            plist.SetString("", "userId");
            plist.Synchronize();
        }

        async partial void SignInButton_TouchUpInside(UIButton sender)
        {
            string userEmail = userEmailTextField.Text;
            string userPassword = userPasswordTextField.Text;

            // If email or password do not meet requirements
            if (!Validation.ValidationResult(userEmail, userPassword))
            {
                DisplayAlertMessage(Validation.Message);
                return;
            }

            // Show the loading overlay on the UI
            LoadingOverlay.ShowOverlay(View);

            // Create instance of table Users
            var user = new UsersModel()
            {
                Email = userEmail,
                Password = userPassword,
            };

            // Call REST service to send Json data
            RestService rs = new RestService();

            // Get Json data from server in JsonResponseModel format
            Task<JsonResponseModel> jsonResponeTask = rs.PostJsonDataAsync(user, "login");

            // If there was an error in PostJsonDataAsync class, display message
            if (jsonResponeTask == null)
            {
                DisplayAlertMessage(rs.Message);
                return;
            }

            // Get user id from Json after login or display an error
            //await System.Threading.Tasks.Task.Run(() =>GetUserIdFromJson());

            // Create instance of JsonResponseModel and pass jsonResponeTask there
            var jsonResponse = await jsonResponeTask;

            // Get user id from Json after login or display an error
            if (jsonResponse.Status == "Error")
                DisplayAlertMessage(jsonResponse.Message);
            else
                LoginIsSuccessful(jsonResponse.UserId);
        }

        // Save user id on device to use it later
        void LoginIsSuccessful(string userId)
        {
            // Set Shared User Defaults
            var plist = NSUserDefaults.StandardUserDefaults;
            plist.SetBool(true, "isUserLoggedIn"); // set flag that user logged in
            plist.SetString(userId, "userId");
            plist.Synchronize(); // Apple does not suggest using it!

            Console.WriteLine("step 5");

            // Create an instance of AppDelegate and call SetRootViewController method to display MainTabBarController after successfu login
            var appDelegate = UIApplication.SharedApplication.Delegate as AppDelegate;
            appDelegate.SetRootViewController(true);

            // Remove overlay
            LoadingOverlay.RemoveOverlay();

            Console.WriteLine(plist.StringForKey("userId"));
        }

        // Display MainTabBarController
        void ShowMainTabController()
        {
            // Create an instance of AppDelegate and call SetRootViewController method to display MainTabBarController after successfu login
            var appDelegate = UIApplication.SharedApplication.Delegate as AppDelegate;
            appDelegate.SetRootViewController(true);

            //var mainTabController = this.Storyboard.InstantiateViewController("MainTabBarController") as MainTabBarController;
            //appDelegate.Window.RootViewController = mainTabController;

            // Launches a new instance of RegisterUserViewController
            //DispatchQueue.MainQueue.DispatchAsync(() =>
            //{
            //    var appDelegate = UIApplication.SharedApplication.Delegate as AppDelegate;
            //    appDelegate.SetRootViewController(true);
            //});
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

        partial void RegisterButton_TouchUpInside(UIButton sender)
        {
            // Launches a new instance of RegisterUserViewController
            RegisterUserViewController registerController = this.Storyboard.InstantiateViewController("RegisterUserViewController") as RegisterUserViewController;
            if (registerController != null)
            {
                this.NavigationController.PresentViewController(registerController, true, null);
            }
        }
    }
}