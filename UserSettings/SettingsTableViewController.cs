//
//  SettingsTableViewController.cs
//  Happy31.iOSApp
//
//  Copyright © 2017 Denis Klyucherov. All rights reserved.
//

using Foundation;
using System;
using UIKit;
using System.Collections.Generic;
using CoreGraphics;

namespace Happy31.UserSettings
{
    /// <summary>
    /// Users' settings table View Controller
    /// </summary>
    public partial class SettingsTableViewController : UITableViewController
    {
        List<string> settingsTableItems;
        List<string> settingsValues;

        public SettingsTableViewController (IntPtr handle) : base (handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            settingsTableItems = new List<string>() { "Settings", "Enable notifications", "Start time", "End time", "Minimal interval between\nprompts (in hours)" };
            settingsValues = new List<string>();

            settingsTableView.ContentInset = new UIEdgeInsets(0, 0, 0, 0);
            settingsTableView.RowHeight = 90;
            settingsTableView.SectionHeaderHeight = 90f;
            settingsTableView.TableFooterView = new UIView(CGRect.Empty); // Remove empty cells

            AddBackground();

            GetUserSettings();
        }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
        }

        void AddBackground()
        {
            var imageViewBackground = new UIImageView
            {
                Image = UIImage.FromBundle("CommonBackground"),
                ContentMode = UIViewContentMode.ScaleAspectFill
            };

            settingsTableView.BackgroundView = imageViewBackground;
            settingsTableView.SeparatorColor = UIColor.LightGray;
            View.BackgroundColor = UIColor.FromWhiteAlpha(white: 1, alpha: 0.9f);
        }

        void GetUserSettings()
        {
            var plist = NSUserDefaults.StandardUserDefaults;
            bool enableNotifications = plist.BoolForKey("enableNotifications");
            string promptStartTime = plist.StringForKey("promptStartTime");
            string promptEndTime = plist.StringForKey("promptEndTime");
            string promptMinInterval = plist.StringForKey("promptMinInterval");

            settingsValues.AddRange(new string[] { null, enableNotifications.ToString(), promptStartTime, promptEndTime, promptMinInterval });

            settingsTableView.Source = new SettingsTableSource(settingsTableItems, settingsValues);
            settingsTableView.ReloadData();
        }
    }
}