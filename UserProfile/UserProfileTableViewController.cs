//
//  UserProfileTableViewController.cs
//  Happy31.iOSApp
//
//  Copyright © 2017 Denis Klyucherov. All rights reserved.
//

using CoreGraphics;
using Foundation;
using System;
using System.Collections.Generic;
using System.Linq;
using UIKit;

namespace Happy31.UserProfile
{
    /// <summary>
    /// User profile Table View Controller
    /// </summary>
    public partial class UserProfileTableViewController : UITableViewController
    {
        List<string> userProfileTableItems;

        UsersRepository userRepository;
        IEnumerable<UsersModel> user;
        UIImage profileImage;
        UIBarButtonItem editProfileButton;

        public UserProfileTableViewController (IntPtr handle) : base (handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            editProfileButton = new UIBarButtonItem()
            {
                Title = "Edit"
            };

            editProfileButton.Clicked += (sender, e) => OpenEditProfileViewController();

            this.NavigationItem.SetRightBarButtonItem(editProfileButton, true);

            profileImage = new UIImage();

            userProfileTableItems = new List<string>() { "Member since", "Name", "Email", "Log out" };
            userInfoTableView.ContentInset = new UIEdgeInsets(50, 0, 0, 0);
            userInfoTableView.RowHeight = 90;
            userInfoTableView.SectionHeaderHeight = 90f;
            userInfoTableView.TableFooterView = new UIView(CGRect.Empty); // Remove empty cells

            AddBackground();
        }

        public void OpenEditProfileViewController()
        {
            var editProfileViewController = this.Storyboard.InstantiateViewController("EditProfileTableViewController") as EditProfileTableViewController;
            if (editProfileViewController != null)
            {
                editProfileViewController.User = user.First();
                editProfileViewController.SharedImage = profileImage; // Send profile image
                this.NavigationController.PushViewController(editProfileViewController, true);
            }
        }

        public override void ViewDidAppear(bool animated)
        {
            LoadingOverlay.ShowOverlay(this.View);
            GetUserInfo();
        }

        void AddBackground()
        {
            var imageViewBackground = new UIImageView
            {
                Image = UIImage.FromBundle("CommonBackground"),
                ContentMode = UIViewContentMode.ScaleAspectFill
            };

            userInfoTableView.BackgroundView = imageViewBackground;
            userInfoTableView.SeparatorColor = UIColor.LightGray;
            View.BackgroundColor = UIColor.FromWhiteAlpha(white: 1, alpha: 0.9f);
        }

        async void GetUserInfo()
        {
            var plist = NSUserDefaults.StandardUserDefaults;
            string userId = plist.StringForKey("userId");

            userRepository = new UsersRepository();
            user = userRepository.GetUser(userId);

            if (!user.Any())
            {
                Console.WriteLine("User does not exist");
                LoadingOverlay.RemoveOverlay();
                return;
            }

            if (user.First().Avatar != null && user.First().Avatar.Length > 0)
                profileImage = await ConvertImage.ConvertBinaryToImage(user.First().Avatar);
            else
                profileImage = null;

            userInfoTableView.Source = new UserProfileTableSource(userProfileTableItems, user.First(), profileImage, this);
            userInfoTableView.ReloadData();

            LoadingOverlay.RemoveOverlay();
        }
    }
}