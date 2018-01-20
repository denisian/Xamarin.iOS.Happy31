//
//  DefaultSettings.cs
//  Happy31.iOSApp
//
//  Copyright © 2017 Denis Klyucherov. All rights reserved.
//

using System;
using Foundation;

namespace Happy31.UserSettings
{
    /// <summary>
    /// Default user's settings
    /// </summary>
    public static class DefaultSettings
    {
        public static void SetSettings()
        {
            // Set Shared User Defaults
            var plist = NSUserDefaults.StandardUserDefaults;
            plist.SetBool(true, "enableNotifications");
            plist.SetString(DateTime.Parse("09:00 AM").ToString("hh:mm tt", System.Globalization.CultureInfo.InvariantCulture), "promptStartTime");
            plist.SetString(DateTime.Parse("10:00 PM").ToString("hh:mm tt", System.Globalization.CultureInfo.InvariantCulture), "promptEndTime");
            plist.SetString("3", "promptMinInterval");
            plist.Synchronize();
        }
    }
}