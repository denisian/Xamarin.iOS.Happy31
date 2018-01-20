//
//  AppNotifications.cs
//  Happy31.iOSApp
//
//  Copyright © 2017 Denis Klyucherov. All rights reserved.
//

using System;
using Foundation;
using UIKit;
using UserNotifications;

namespace Happy31
{
    /// <summary>
    /// Dsiplay notification to the user
    /// </summary>
    public static class AppNotifications
    {
        public static void Display(DisplayedPromptsModel displayedPrompts)
        {
            // Get current notification settings
            //UNUserNotificationCenter.Current.GetNotificationSettings((settings) => {
            //    var alertsAllowed = (settings.AlertSetting == UNNotificationSetting.Enabled);
            //});

            var plist = NSUserDefaults.StandardUserDefaults;
            bool enableNotifications = plist.BoolForKey("enableNotifications");
            if (!enableNotifications)
                return;
            
            var content = new UNMutableNotificationContent();
            content.Title = displayedPrompts.Category;
            //content.Subtitle = displayedPrompts.Category;
            content.Body = displayedPrompts.Task;
            content.Badge = NSNumber.FromInt32(Convert.ToInt32(UIApplication.SharedApplication.ApplicationIconBadgeNumber) + 1);

            var timeInterval = new NSDateComponents();
            timeInterval.Second = 1;

            //var trigger = UNCalendarNotificationTrigger.CreateTrigger(timeInterval, false);
            var trigger = UNTimeIntervalNotificationTrigger.CreateTrigger(1, false);

            var requestID = "PromptAlarm";
            var request = UNNotificationRequest.FromIdentifier(requestID, content, trigger);

            UNUserNotificationCenter.Current.AddNotificationRequest(request, (err) =>
            {
                if (err != null)
                {
                    Console.WriteLine("something went wrong");
                }
            }); 
        }
    }
}