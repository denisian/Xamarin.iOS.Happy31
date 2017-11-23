using Foundation;
using System;
using UIKit;

using Facebook.LoginKit;
using Facebook.CoreKit;

namespace Happy31
{
    public partial class UserProfileViewController : UIViewController
    {
        public UserProfileViewController (IntPtr handle) : base (handle)
        {
        }

        partial void SignOutButton_Activated(UIBarButtonItem sender)
        {
            LogoutRequest();
        }

        void LogoutRequest()
        {
            var actionSheet = new UIActionSheet();
            actionSheet.AddButton("Sign Out");
            actionSheet.AddButton("Cancel");
            actionSheet.DestructiveButtonIndex = 0;
            actionSheet.CancelButtonIndex = 1;
            //actionSheet.FirstOtherButtonIndex = 2;

            // Sign out button event
            actionSheet.Clicked += (sender, e) =>
            {
                if (e.ButtonIndex == 1) // Cancell button
                    return;
                
                //actionSheet.Clicked += delegate (object a, UIButtonEventArgs b) {
                // Set logout User Defaults
                var plist = NSUserDefaults.StandardUserDefaults;
                plist.SetBool(false, "isUserLoggedIn"); // set flag that user logged out
                plist.SetString("", "userId");
                plist.Synchronize();

                // Logout from Facebook
                if (AccessToken.CurrentAccessToken != null && Profile.CurrentProfile != null)
                    new LoginManager().LogOut();

                // Create an instance of AppDelegate and call SetRootViewController method to display LOginUserViewController after successfu logout
                var appDelegate = UIApplication.SharedApplication.Delegate as AppDelegate;
                appDelegate.SetRootViewController(false);
            };
            actionSheet.ShowInView(View);
        }
    }
}