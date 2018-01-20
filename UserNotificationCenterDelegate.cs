//
//  UserNotificationCenterDelegate.cs
//  Happy31.iOSApp
//
//  Copyright © 2017 Denis Klyucherov. All rights reserved.
//

using System;
using UserNotifications;

namespace Happy31
{
    public class UserNotificationCenterDelegate : UNUserNotificationCenterDelegate
    {
        public UserNotificationCenterDelegate()
        {
        }

        // Handling Foreground App Notifications (differently when it is in the foreground and a Notification is triggered)
        public override void WillPresentNotification(UNUserNotificationCenter center, UNNotification notification, Action<UNNotificationPresentationOptions> completionHandler)
        {
            // Do something with the notification
            //Console.WriteLine("Active Notification: {0}", notification);

            // Tell system to display the notification anyway or use
            // `None` to say we have handled the display locally.
            completionHandler(UNNotificationPresentationOptions.Alert | UNNotificationPresentationOptions.Badge | UNNotificationPresentationOptions.Sound);
        }
    }
}
