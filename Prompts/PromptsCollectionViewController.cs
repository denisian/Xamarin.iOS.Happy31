//
//  PromptsCollectionViewController.cs
//  Happy31.iOSApp
//
//  Copyright © 2017 Denis Klyucherov. All rights reserved.
//

using Foundation;
using System;
using System.Collections.Generic;
using UIKit;
using UserNotifications;
using System.Linq;
using CoreGraphics;

namespace Happy31.Prompts
{
    /// <summary>
    /// Prompts collection View Controller 
    /// </summary>
    public partial class PromptsCollectionViewController : UICollectionViewController
    {
        List<DisplayedPromptsModel> collectionUsersPrompts;

        PromptsRepository promptRepository;
        UsersPromptsRepository userPromptRepository;

        UIRefreshControl refreshControl;

        NSUserDefaults plist;
        string userId;

        public PromptsCollectionViewController(IntPtr handle) : base(handle)
        {
            collectionUsersPrompts = new List<DisplayedPromptsModel>();

            promptRepository = new PromptsRepository();
            userPromptRepository = new UsersPromptsRepository();

            plist = NSUserDefaults.StandardUserDefaults;
            userId = plist.StringForKey("userId");
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            this.NavigationItem.LeftBarButtonItem = null;

            AddBackground();

            // Set Navigation bar background
            //this.NavigationController.NavigationBar.SetBackgroundImage(new UIImage(), UIBarMetrics.Default);
            //this.NavigationController.NavigationBar.ShadowImage = new UIImage();
            //this.NavigationController.NavigationBar.Translucent = true;

            // Request notification permissions from the user
            var notifOptions = UNAuthorizationOptions.Alert | UNAuthorizationOptions.Badge | UNAuthorizationOptions.Sound;
            UNUserNotificationCenter.Current.RequestAuthorization(notifOptions, (approved, err) =>
            {
            });

            // Watch for notifications while the app is active
            UNUserNotificationCenter.Current.Delegate = new UserNotificationCenterDelegate();

            // Set edge for the controller
            UIEdgeInsets insets = new UIEdgeInsets(30, 0, 70, 0);
            CollectionView.ContentInset = insets;
            CollectionView.UserInteractionEnabled = true;
            CollectionView.BackgroundColor = UIColor.Clear;

            AddRefreshControl();

            // To use a Supplementary View, it first needs to be registered in the ViewDidLoad method
            CollectionView.RegisterClassForCell(typeof(PromptsCollectionCell), PromptsCollectionCell.cellId);
        }

        void AddRefreshControl()
        {
            refreshControl = new UIRefreshControl()
            {
                TintColor = UIColor.FromRGB(214, 93, 98)
                //AttributedTitle = new NSAttributedString("Fetching new prompt")
            };

            if (UIDevice.CurrentDevice.CheckSystemVersion(10, 0))
                CollectionView.RefreshControl = refreshControl;
            else
                CollectionView.AddSubview(refreshControl);
            
            refreshControl.ValueChanged += (sender, e) =>
            {
                CollectionView.Bounces = false;
                CollectionView.AlwaysBounceVertical = false;

                GenerateNewPrompt();
                ReloadPromptsCollection();
                refreshControl.EndRefreshing();

                CollectionView.Bounces = true;
                CollectionView.AlwaysBounceVertical = true;
            };
        }

        void AddBackground()
        {
            // Get screen width and height
            var width = UIScreen.MainScreen.Bounds.Size.Width;
            var height = UIScreen.MainScreen.Bounds.Size.Height;

            var imageViewBackground = new UIImageView() { Frame = new CGRect(0, 0, width, height) };
            imageViewBackground.Image = UIImage.FromBundle("CommonBackground");

            imageViewBackground.ContentMode = UIViewContentMode.ScaleAspectFill;

            View.AddSubview(imageViewBackground);
            View.SendSubviewToBack(imageViewBackground);
        }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);

            ReloadPromptsCollection();

            // Set app badge to 0 after we open app
            //UIApplication.SharedApplication.ApplicationIconBadgeNumber = 0;
        }

        void ReloadPromptsCollection()
        {
            //CollectionView.RegisterClassForSupplementaryView(typeof(PromptsCollectionCell), UICollectionElementKindSection.Header, headers[0]);

            collectionUsersPrompts = userPromptRepository.DisplayUserPrompts(userId);
            if (collectionUsersPrompts.Any())
            {
                var promtsForDay = GetPromtsIndexesForDay(collectionUsersPrompts.Count);
             
                CollectionView.Source = new PromptsCollectionSource(collectionUsersPrompts, promtsForDay, userId, this);
                CollectionView.ReloadData();
            }
            else
            {
                CollectionView.Source = new PromptsCollectionSource(new List<DisplayedPromptsModel>() { new DisplayedPromptsModel() { Task = "" }}, null, userId, this);
                CollectionView.ReloadData();
            }
        }

        // Generating user's prompts
        async void GenerateNewPrompt()
        {
            var newPrompt = userPromptRepository.GenerateNewPrompt(userId);
            // If prompt succesfully generated, display it in collection view and show notification
            if (newPrompt.Any())
            {
                ReloadPromptsCollection();
                await userPromptRepository.SyncCurrentUserPrompts(userId);

                AppNotifications.Display(newPrompt.First());
            }
            else
                AppNotifications.Display(new DisplayedPromptsModel() { Category = "", Task = "There are no prompts to display" });
        }

        // Get title "Day #" for every third prompt
        List<int> GetPromtsIndexesForDay(int usersPromtsCount)
        {
            var listPrompts = new List<int>();
            for (int i = 0; i < usersPromtsCount; i++)
                listPrompts.Add(i);

            listPrompts = listPrompts.OrderByDescending(i => i).ToList();

            var items = new List<int>();
            for (int i = 0; i < listPrompts.Count; i += 3)
                items.Add(listPrompts.Skip(i).Take(3).TakeLast(1).First());

            return items; //.OrderByDescending(i => i).ToList();
        }

        async partial void SyncAllPromptsButton_Activated(UIBarButtonItem sender)
        {
            SyncAllPromptsButton.Enabled = false;
            string message = await userPromptRepository.SyncAllTableUserPrompts(userId);
            ReloadPromptsCollection();
            AppNotifications.Display(new DisplayedPromptsModel() { Category = "", Task = message });
            SyncAllPromptsButton.Enabled = true;
        }
    }
}