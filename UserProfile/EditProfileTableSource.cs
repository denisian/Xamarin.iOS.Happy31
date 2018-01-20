//
//  EditProfileTableSource.cs
//  Happy31.iOSApp
//
//  Copyright © 2017 Denis Klyucherov. All rights reserved.
//

using CoreGraphics;
using System;
using System.Collections.Generic;
using Foundation;
using UIKit;

namespace Happy31.UserProfile
{
    /// <summary>
    /// Edit user profile Table Source
    /// </summary>
    public class EditProfileTableSource : UITableViewSource
    {
        List<string> userProfileLabels;
        public List<string> userItems { get; private set; }

        UsersModel user;
        UIImage profileImage;
        EditProfileTableViewController viewController;

        string cellIdentifier = "TableCell";

        public EditProfileTableSource(UsersModel _user, UIImage _profileImage, EditProfileTableViewController _viewController)
        {
            user = _user;
            profileImage = _profileImage;
            viewController = _viewController;

            userProfileLabels = new List<string>();
            userItems = new List<string>();
            EditProfileSharedData.EditedUserInfo = new List<string>();

            var userItemsArray = new string[] { user.FirstName, user.LastName, user.Email, null };

            userProfileLabels.AddRange(new string[] { "First name", "Last name", "Email", "Password" });
            userItems.AddRange(userItemsArray); // Convert user to List to access by index in GetCell
            EditProfileSharedData.EditedUserInfo.AddRange(userItemsArray);

            // Hide Email and Password labels and textFields if user logged in via Facebook
            if (user.LoginProvider == "Facebook")
            {
                userProfileLabels.RemoveRange(2, 2); // Remove Email and Password fields
                userItems.RemoveRange(2, 2); // Remove Email and Password fields
            }
        }

        public override nint RowsInSection(UITableView tableview, nint section)
        {
            return userProfileLabels.Count + 1;
        }

        public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
        {
            if (indexPath.Row == 0)
                return tableView.RowHeight + 100;
            else
                return
                    tableView.RowHeight;
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            UITableViewCell cell = tableView.DequeueReusableCell(cellIdentifier);

            if (cell == null)
                cell = new UITableViewCell(UITableViewCellStyle.Default, cellIdentifier);

            var profileImageView = new UIImageView();
            var editProfileImageButton = new UIButton();
            if (indexPath.Row == 0)
            {
                var picturesize = 250;
                profileImageView.Layer.CornerRadius = 125;
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

                editProfileImageButton.Frame = new CGRect(profileImageView.Frame.Location, profileImageView.Frame.Size);
                editProfileImageButton.TouchUpInside += (sender, e) => viewController.EditAvatar();
            }

            var labelLeft = new UILabel();
            var textFieldRight = new UITextField();

            if (indexPath.Row > 0)
            {
                int leftBorder = 15;
                int rightBorder = 25;
                int spaceBetweenLabelAndTextField = 20;
                nfloat height = (tableView.RowHeight - spaceBetweenLabelAndTextField * 2) / 2;

                labelLeft.Frame = new CGRect(leftBorder, spaceBetweenLabelAndTextField, UIScreen.MainScreen.Bounds.Size.Width - rightBorder, height);
                textFieldRight.Frame = new CGRect(leftBorder, labelLeft.Frame.Height + spaceBetweenLabelAndTextField, UIScreen.MainScreen.Bounds.Size.Width - rightBorder, height);

                labelLeft.Font = UIFont.SystemFontOfSize(16, UIFontWeight.Regular);

                textFieldRight.Font = UIFont.SystemFontOfSize(18, UIFontWeight.Light);
                textFieldRight.ClearButtonMode = UITextFieldViewMode.Always;
                textFieldRight.AdjustsFontSizeToFitWidth = true;
                textFieldRight.TextAlignment = UITextAlignment.Left;
                textFieldRight.AttributedPlaceholder = new NSAttributedString("Enter a new " + userProfileLabels[indexPath.Row - 1], foregroundColor: UIColor.Gray, font: UIFont.SystemFontOfSize(18, UIFontWeight.UltraLight));

                labelLeft.Text = userProfileLabels[indexPath.Row - 1];
                textFieldRight.Text = userItems[indexPath.Row - 1];

                if (labelLeft.Text == "Email")
                {
                    textFieldRight.KeyboardType = UIKeyboardType.EmailAddress;
                    textFieldRight.AutocapitalizationType = UITextAutocapitalizationType.None;
                }

                if (labelLeft.Text == "Password")
                    textFieldRight.SecureTextEntry = true;

                textFieldRight.ShouldChangeCharacters = (textField, range, replacementString) =>
                {
                    using (NSString original = new NSString(textField.Text), replace = new NSString(replacementString))
                    {
                        userItems[indexPath.Row - 1] = original.Replace(range, replace);
                        textField.Text = userItems[indexPath.Row - 1];

                        // Sharing data with EditProfileViewController
                        EditProfileSharedData.EditedUserInfo[indexPath.Row - 1] = userItems[indexPath.Row - 1];
                    }
                    return false;
                };

                // Assign empty values after ClearButton was pressed
                textFieldRight.ShouldClear = (textField) =>
                {
                    textField.Text = null;
                    userItems[indexPath.Row - 1] = null;

                        // Sharing data with EditProfileViewController
                        EditProfileSharedData.EditedUserInfo[indexPath.Row - 1] = userItems[indexPath.Row - 1];

                    return true;
                };

                textFieldRight.ReturnKeyType = UIReturnKeyType.Done;
                textFieldRight.ShouldReturn = ((textField) =>
                {
                        // Dismiss the Keyboard
                        textFieldRight.ResignFirstResponder();
                    return true;
                });
            }

            cell.SelectionStyle = UITableViewCellSelectionStyle.None;
            cell.BackgroundColor = UIColor.Clear;

            cell.ContentView.AddSubviews(profileImageView, editProfileImageButton, labelLeft, textFieldRight);
            //}
            return cell;
        }
    }
}