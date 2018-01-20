//
//  UserProgressTableViewController.cs
//  Happy31.iOSApp
//
//  Copyright © 2017 Denis Klyucherov. All rights reserved.
//

using Foundation;
using System;
using UIKit;
using System.Collections.Generic;
using CoreGraphics;
using System.Linq;

namespace Happy31.UserProgress
{
    /// <summary>
    /// User progress Table View Controller
    /// </summary>
    public partial class UserProgressTableViewController : UITableViewController
    {
        UsersPromptsRepository userPromptRepository;

        public UserProgressTableViewController(IntPtr handle) : base(handle)
        {
            userPromptRepository = new UsersPromptsRepository();
        }

        public override void ViewDidLoad()
        {
            progressTableView.RowHeight = 180;
            progressTableView.TableFooterView = new UIView(CGRect.Empty); // Remove empty cells

            AddBackground();
        }

        public override void ViewDidAppear(bool animated)
        {
            GetUserProgressData();
        }

        void AddBackground()
        {
            var imageViewBackground = new UIImageView
            {
                Image = UIImage.FromBundle("CommonBackground"),
                ContentMode = UIViewContentMode.ScaleAspectFill
            };

            progressTableView.BackgroundView = imageViewBackground;
            progressTableView.SeparatorColor = UIColor.LightGray;
            View.BackgroundColor = UIColor.FromWhiteAlpha(white: 1, alpha: 0.9f);
        }

        void GetUserProgressData()
        {
            var plist = NSUserDefaults.StandardUserDefaults;
            string userId = plist.StringForKey("userId");

            var usersProgress = userPromptRepository.DisplayUserProgress(userId);

            if (usersProgress.Any())
            {
                progressTableView.Source = new UserProgressTableSource(usersProgress);
                progressTableView.ReloadData();
            }
            else
            {
                progressTableView.Source = new UserProgressTableSource(new List<ProgressModel>() { new ProgressModel() { Category = "" } });
                progressTableView.ReloadData();
            }
        }
    }
}