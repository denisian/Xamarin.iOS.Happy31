//
//  UserProfileTableSource.cs
//  Happy31.iOSApp
//
//  Copyright © 2017 Denis Klyucherov. All rights reserved.
//

using CoreGraphics;
using Foundation;
using System;
using System.Collections.Generic;
using UIKit;
using Facebook.CoreKit;
using Facebook.LoginKit;

namespace Happy31.UserProfile
{
    /// <summary>
    /// User profile Table Source
    /// </summary>
    public class UserProfileTableSource : UITableViewSource
    {
        List<string> tableItems;
        UIViewController viewController;
        UsersModel user;
        UIImage profileImage;

        string cellIdentifier = "TableCell";

        public UserProfileTableSource(List<string> _items, UsersModel _user, UIImage _profileImage, UIViewController _viewController)
        {
            tableItems = _items;
            user = _user;
            profileImage = _profileImage;
            viewController = _viewController;
        }

        public override nint RowsInSection(UITableView tableview, nint section)
        {
            return tableItems.Count + 1;
        }


        public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
        {
            if (indexPath.Row == 0)
                return tableView.RowHeight + 50;
            else
                return tableView.RowHeight;
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            UITableViewCell cell = tableView.DequeueReusableCell(cellIdentifier);
            if (cell == null)
                cell = new UITableViewCell(UITableViewCellStyle.Value1, cellIdentifier);

            if (indexPath.Row == 0)
            {
                var profileImageView = new UIImageView();
                var picturesize = 150;
                profileImageView.Layer.CornerRadius = 75;
                profileImageView.Layer.BorderWidth = 1;
                profileImageView.Layer.MasksToBounds = true;
                profileImageView.Layer.BorderColor = UIColor.Clear.CGColor;
                profileImageView.ContentMode = UIViewContentMode.ScaleAspectFill;
                profileImageView.TintColor = UIColor.FromRGB(198, 123, 112);
                profileImageView.Frame = new CGRect((UIScreen.MainScreen.Bounds.Size.Width - picturesize) / 2, (tableView.RowHeight - picturesize) / 2, picturesize, picturesize);
                if (profileImage == null)
                    profileImageView.Image = UIImage.FromBundle("ProfileEmptyIcon").ImageWithRenderingMode(UIImageRenderingMode.AlwaysTemplate);
                else
                    profileImageView.Image = profileImage;

                cell.SelectionStyle = UITableViewCellSelectionStyle.None;

                cell.ContentView.AddSubview(profileImageView);
            }
            else if (indexPath.Row > 0)
            {
                cell.TextLabel.Text = tableItems[indexPath.Row - 1];

                if (indexPath.Row == 1) // Member
                {
                    cell.DetailTextLabel.Text = DateTime.Parse(user.CreatedAt).ToString("MMMM yyyy");
                    cell.SelectionStyle = UITableViewCellSelectionStyle.None;
                }
                else if (indexPath.Row == 2) // Name
                {
                    cell.DetailTextLabel.Text = user.FirstName + " " + user.LastName;
                    cell.SelectionStyle = UITableViewCellSelectionStyle.None;
                }
                else if (indexPath.Row == 3) // Email
                {
                    cell.DetailTextLabel.Text = user.Email;
                    cell.SelectionStyle = UITableViewCellSelectionStyle.None;
                }
                else if (indexPath.Row == 4) // Log out button
                    cell.TextLabel.TextColor = UIColor.FromRGB(214, 93, 98);
            }

            cell.BackgroundColor = UIColor.Clear;

            return cell;
        }

        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            if (indexPath.Row > 1)
                if (tableItems[indexPath.Row - 1] == "Log out")
                    LogoutRequest();

            tableView.DeselectRow(indexPath, true);
        }

        void LogoutRequest()
        {
            var actionSheet = new UIActionSheet();
            actionSheet.AddButton("Sign Out");
            actionSheet.AddButton("Cancel");
            //actionSheet.DestructiveButtonIndex = 0;
            actionSheet.CancelButtonIndex = 1;
            //actionSheet.FirstOtherButtonIndex = 2;

            // Sign out button event
            actionSheet.Clicked += (sender, e) =>
            {
                if (e.ButtonIndex == 1) // Cancell button
                    return;

                //actionSheet.Clicked += delegate (object a, UIButtonEventArgs b) {
                // Reset User Defaults
                NSUserDefaults.StandardUserDefaults.RemovePersistentDomain(NSBundle.MainBundle.BundleIdentifier);

                // Logout from Facebook
                if (AccessToken.CurrentAccessToken != null && Profile.CurrentProfile != null)
                    new LoginManager().LogOut();

                // Removing database after logged out
                var deleteTables = new DeleteAllTables();
                deleteTables.DeleteTables();

                // Create an instance of AppDelegate and call SetRootViewController method to display LOginUserViewController after successfu logout
                var appDelegate = UIApplication.SharedApplication.Delegate as AppDelegate;
                appDelegate.SetRootViewController(false);

                // Set app badge to 0 after logged out
                UIApplication.SharedApplication.ApplicationIconBadgeNumber = 0;
            };
            actionSheet.ShowInView(viewController.View);
        }
    }
}