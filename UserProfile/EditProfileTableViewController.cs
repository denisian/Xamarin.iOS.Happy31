//
//  EditProfileTableViewController.cs
//  Happy31.iOSApp
//
//  Copyright © 2017 Denis Klyucherov. All rights reserved.
//

using System;
using UIKit;
using CoreGraphics;
using System.Threading.Tasks;
using AVFoundation;
using MobileCoreServices;
using CoreFoundation;

namespace Happy31.UserProfile
{
    /// <summary>
    /// Edit user profile Table View Controller
    /// </summary>
    public partial class EditProfileTableViewController : UITableViewController
    {
        UIImagePickerController imagePicker;
        UIBarButtonItem saveButton;

        public UsersModel User { get; set; }
        public UIImage SharedImage { get; set; }

        public EditProfileTableViewController (IntPtr handle) : base (handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            userEditInfoTableView.RowHeight = 120;
            userEditInfoTableView.ContentInset = new UIEdgeInsets(100, 0, 0, 0);
            userEditInfoTableView.TableFooterView = new UIView(CGRect.Empty); // Remove empty cells
            GetTableViewData(User, SharedImage, this);

            AddBackground();

            saveButton = new UIBarButtonItem()
            {
                Title = "Save"
            };

            saveButton.Clicked += async (sender, e) =>
            {
                LoadingOverlay.ShowOverlay(this.View);

                var listUserEditedInfo = EditProfileSharedData.EditedUserInfo;

                string userFirstName = listUserEditedInfo[0];
                string userLastName = listUserEditedInfo[1];
                string userEmail = listUserEditedInfo[2];
                string userPassword = listUserEditedInfo[3];

                if (string.IsNullOrEmpty(userFirstName) || string.IsNullOrEmpty(userLastName))
                {
                    LoadingOverlay.RemoveOverlay();
                    DisplayAlertMessage("All fields are required to fill in");
                    return;
                }

                // If email or password do not meet requirements
                if (string.IsNullOrWhiteSpace(userEmail))
                {
                    LoadingOverlay.RemoveOverlay();
                    DisplayAlertMessage("Email is required to fill in");
                    return;
                }

                if (!Validation.ValidationResult(userEmail, "email"))
                {
                    LoadingOverlay.RemoveOverlay();
                    DisplayAlertMessage(Validation.Message);
                    return;
                }
            
                if (!string.IsNullOrEmpty(userPassword))
                    if (!Validation.ValidationResult(userPassword, "password"))
                    {
                        LoadingOverlay.RemoveOverlay();
                        DisplayAlertMessage(Validation.Message);
                        return;
                    }

                saveButton.Enabled = false;

                var userRepository = new UsersRepository();
                var updatedUser = new UsersModel()
                {
                    Id = User.Id,
                    FirstName = userFirstName,
                    LastName = userLastName,
                    Email = userEmail.ToLower(),
                    Password = userPassword,
                    Avatar = await ConvertImage.ConvertImageToBinary(SharedImage),
                    LoginProvider = User.LoginProvider,
                    CreatedAt = User.CreatedAt
                };

                // Call REST service to send Json data
                RestService rs = new RestService();

                // Get Json data from server in JsonResponseModel format
                Task<UsersModel> jsonResponeTask = rs.UserLoginAndRegisterJson(updatedUser, "update");

                // If there was an error in PostJsonDataAsync class, display message
                if (jsonResponeTask == null)
                {
                    LoadingOverlay.RemoveOverlay();
                    DisplayAlertMessage(rs.Message);
                    return;
                }

                // Get user id from Json after login or display an error
                // Create instance of JsonResponseModel and pass jsonResponeTask there
                var jsonResponse = await jsonResponeTask;

                // Get user id from Json after update or display an error
                if (jsonResponse.Status == "Error")
                {
                    LoadingOverlay.RemoveOverlay();
                    DisplayAlertMessage(jsonResponse.Message);
                    return;
                }
                else
                    Console.WriteLine(jsonResponse.Message);

                await userRepository.UpdateUser(updatedUser); // Update in local database

                User = updatedUser; // Send back to UserProfileViewController to update data there

                LoadingOverlay.RemoveOverlay();
                saveButton.Enabled = true;
                this.NavigationController.PopViewController(true);
            };

            this.NavigationItem.SetRightBarButtonItem(saveButton, true);
        }

        void GetTableViewData(UsersModel user, UIImage image, EditProfileTableViewController viewController)
        {
            DispatchQueue.MainQueue.DispatchAsync(() =>
            {
                userEditInfoTableView.Source = new EditProfileTableSource(user, image, viewController);
                userEditInfoTableView.ReloadData();
            });
        }

        void AddBackground()
        {
            var imageViewBackground = new UIImageView
            {
                Image = UIImage.FromBundle("CommonBackground"),
                ContentMode = UIViewContentMode.ScaleAspectFill
            };

            userEditInfoTableView.BackgroundView = imageViewBackground;
            userEditInfoTableView.SeparatorColor = UIColor.LightGray;
            View.BackgroundColor = UIColor.FromWhiteAlpha(white: 1, alpha: 0.9f);
        }

        public void EditAvatar()
        {
            imagePicker = new UIImagePickerController();

            var actionSheet = new UIActionSheet();
            actionSheet.AddButton("Take Photo");
            actionSheet.AddButton("Choose Photo");
            actionSheet.AddButton("Cancel");
            //actionSheet.DestructiveButtonIndex = 0;
            actionSheet.CancelButtonIndex = 2;

            // Edit picture button event
            actionSheet.Clicked += async (sender, e) =>
            {
                if (e.ButtonIndex == 0) // Take Photo button
                {
                    await AuthorizeCameraUse();
                    imagePicker.MediaTypes = new string[] { UTType.Image };
                    imagePicker.SourceType = UIImagePickerControllerSourceType.Camera;
                    imagePicker.AllowsEditing = true;
                }
                else if (e.ButtonIndex == 1) // Choose Photo button
                {
                    imagePicker.MediaTypes = new string[] { UTType.Image };
                    // imagePicker.MediaTypes = UIImagePickerController.AvailableMediaTypes(UIImagePickerControllerSourceType.PhotoLibrary);
                    imagePicker.SourceType = UIImagePickerControllerSourceType.PhotoLibrary;
                    imagePicker.AllowsEditing = true;
                }
                else if (e.ButtonIndex == 2) // Cancel button
                    return;

                imagePicker.FinishedPickingMedia += ImagePicker_FinishedPickingMedia;
                imagePicker.Canceled += ImagePicker_Canceled;

                await this.NavigationController.PresentViewControllerAsync(imagePicker, true);
            };

            actionSheet.ShowInView(View);
        }

        private void ImagePicker_FinishedPickingMedia(object sender, UIImagePickerMediaPickedEventArgs e)
        {
            // If it's image (not a video)
            if (e.Info[UIImagePickerController.MediaType].ToString() == "public.image")
            {
                var editedImage = e.Info[UIImagePickerController.EditedImage] as UIImage;
                if (editedImage != null)
                {
                    SharedImage = ConvertImage.MaxResizeImage(editedImage, 400, 400);
                    GetTableViewData(User, SharedImage, this);
                }
            }

            imagePicker.DismissViewControllerAsync(true);
        }

        private void ImagePicker_Canceled(object sender, EventArgs e)
        {
            imagePicker.DismissViewControllerAsync(true);
        }

        async Task AuthorizeCameraUse()
        {
            var authorizationStatus = AVCaptureDevice.GetAuthorizationStatus(AVMediaType.Video);

            if (authorizationStatus != AVAuthorizationStatus.Authorized)
                await AVCaptureDevice.RequestAccessForMediaTypeAsync(AVMediaType.Video);
        }

        void DisplayAlertMessage(string message)
        {
            // Excecutes the given code in the background
            DispatchQueue.MainQueue.DispatchAsync(() =>
            {
                var alert = UIAlertController.Create(title: "", message: message, preferredStyle: UIAlertControllerStyle.Alert);
                var okAction = UIAlertAction.Create("Ok", UIAlertActionStyle.Cancel, null);
                alert.AddAction(okAction);
                this.PresentViewController(alert, true, null);
            });
        }
    }
}