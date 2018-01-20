﻿//
//  AppDelegate.cs
//  Happy31.iOSApp
//
//  Copyright © 2017 Denis Klyucherov. All rights reserved.
//

using Foundation;
using UIKit;
using Facebook.CoreKit;
using System;
using System.Linq;

namespace Happy31
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the
    // User Interface of the application, as well as listening (and optionally responding) to application events from iOS.
    [Register("AppDelegate")]
    public class AppDelegate : UIApplicationDelegate
    {
        string facebookAppId = "139023923418858";
        string facebookAppName = "Happy 31";

        // class-level declarations

        public override UIWindow Window
        {
            get;
            set;
        }

        public override bool FinishedLaunching(UIApplication application, NSDictionary launchOptions)
        {
            // Override point for customization after application launch.

            Xamarin.IQKeyboardManager.SharedManager.Enable = true;

            // Facebook Authentication
            // This is false by default,
            // If you set true, you can handle the user profile info once is logged into FB with the Profile.Notifications.ObserveDidChange notification,
            // If you set false, you need to get the user Profile info by hand with a GraphRequest
            Profile.EnableUpdatesOnAccessTokenChange(true);
            Settings.AppID = facebookAppId;
            Settings.DisplayName = facebookAppName;

            // Check if user logged in (Get flag from LoginUserViewController)
            bool isUserLoggedIn = NSUserDefaults.StandardUserDefaults.BoolForKey("isUserLoggedIn");
            string userId = NSUserDefaults.StandardUserDefaults.StringForKey("userId");
            if (isUserLoggedIn && !string.IsNullOrEmpty(userId))
                SetRootViewController(true);
            else
                SetRootViewController(false);

            // This method verifies if you have been logged into the app before, and keep you logged in after you reopen or kill your app.
            return ApplicationDelegate.SharedInstance.FinishedLaunching(application, launchOptions);
            //return true;
        }

        // Display MainTabBarController after successfu login
        public void SetRootViewController(bool isUserLoggedIn)
        {
            // Create a new window instance based on the screen size
            Window = new UIWindow(UIScreen.MainScreen.Bounds);

            var storyboard = UIStoryboard.FromName("Main", null);
            //isUserLoggedIn = false;
            UIViewController rootViewController;

            // If user logged in, display MainTabBarController
            if (isUserLoggedIn)
            {
                rootViewController = storyboard.InstantiateViewController("MainTabBarController");
                // Set the minimum fetch interval for background mode
                UIApplication.SharedApplication.SetMinimumBackgroundFetchInterval(UIApplication.BackgroundFetchIntervalMinimum);
            }
            else
            {
                rootViewController = storyboard.InstantiateInitialViewController();
                UIApplication.SharedApplication.SetMinimumBackgroundFetchInterval(UIApplication.BackgroundFetchIntervalNever);
            }

            Window.RootViewController = rootViewController;

            // Makes this UIWindow the key window for the application and makes it visible
            Window.MakeKeyAndVisible();

            UIView.TransitionNotifyAsync(this.Window, rootViewController.View, 0.7, UIViewAnimationOptions.TransitionFlipFromLeft);
        }

        public override bool OpenUrl(UIApplication application, NSUrl url, string sourceApplication, NSObject annotation)
        {
            // We need to handle URLs by passing them to their own OpenUrl in order to make the Single sign-on authentication works.
            return ApplicationDelegate.SharedInstance.OpenUrl(application, url, sourceApplication, annotation);
        }

        public override void OnResignActivation(UIApplication application)
        {
            // Invoked when the application is about to move from active to inactive state.
            // This can occur for certain types of temporary interruptions (such as an incoming phone call or SMS message) 
            // or when the user quits the application and it begins the transition to the background state.
            // Games should use this method to pause the game.
        }

        public override void DidEnterBackground(UIApplication application)
        {
            // Use this method to release shared resources, save user data, invalidate timers and store the application state.
            // If your application supports background exection this method is called instead of WillTerminate when the user quits.

            //AppNotifications.Display(new DisplayedPromptsModel() { Category = "", Task = "Application may not work properly if you closed it" });
            // Set app badge to 0 after we open app
            application.ApplicationIconBadgeNumber = 0;
        }

        // Get prompts in background mode
        public async override void PerformFetch(UIApplication application, Action<UIBackgroundFetchResult> completionHandler)
        {
            //Return no new data by default
            var result = UIBackgroundFetchResult.NoData;

            try
            {
                // Check for new data, and display it
                var userPromptRepository = new UsersPromptsRepository();
                string userId = NSUserDefaults.StandardUserDefaults.StringForKey("userId");

                var newPrompt = userPromptRepository.GenerateNewPrompt(userId);
                // If prompt succesfully generated, display it in collection view and show notification
                if (newPrompt.Any())
                {
                    AppNotifications.Display(newPrompt.First());
                    await userPromptRepository.SyncCurrentUserPrompts(userId);
                    // Inform system of fetch results
                    result = UIBackgroundFetchResult.NewData;
                }
                else
                {
                    //AppNotifications.Display(new DisplayedPromptsModel() { Category = "", Task = "There is no any prompt to display" });
                    completionHandler(UIBackgroundFetchResult.NoData);
                }
            }
            catch
            {
                //Indicate a failed fetch if there was an exception
                result = UIBackgroundFetchResult.Failed;
            }
            finally
            {
                completionHandler(result);
            }
        }

        public override void WillEnterForeground(UIApplication application)
        {
            // Called as part of the transiton from background to active state.
            // Here you can undo many of the changes made on entering the background.
        }

        public override void OnActivated(UIApplication application)
        {
            // Restart any tasks that were paused (or not yet started) while the application was inactive. 
            // If the application was previously in the background, optionally refresh the user interface.

            // Set app badge to 0 after we open app
            application.ApplicationIconBadgeNumber = 0;
        }

        public override void WillTerminate(UIApplication application)
        {
            // Called when the application is about to terminate. Save data, if needed. See also DidEnterBackground.
        }
    }
}

