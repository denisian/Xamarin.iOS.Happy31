//
//  TutorialPageViewController.cs
//  Happy31.iOSApp
//
//  Copyright © 2017 Denis Klyucherov. All rights reserved.
//

using System;
using UIKit;
using System.Collections.Generic;
using System.Linq;
using CoreGraphics;

namespace Happy31.Tutorial
{
    /// <summary>
    /// Displaying Tutorial to the user after first start of the application
    /// </summary>
    public partial class TutorialViewController : UIViewController
    {
        UIPageViewController pageViewController;
        List<string> pageImages;

        public TutorialViewController (IntPtr handle) : base (handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            var backgroundImageView = new UIImageView
            {
                Frame = new CGRect(0, 0, this.View.Frame.Width, this.View.Frame.Height),
                Image = UIImage.FromBundle("TutorialBackground")
            };

            pageImages = new List<string> { "TutorialPage1", "TutorialPage2", "TutorialPage3", "TutorialPage4" };

            pageViewController = this.Storyboard.InstantiateViewController("PageViewController") as UIPageViewController;
            pageViewController.DataSource = new TutorialPageViewControllerDataSource(this, pageImages);

            var startVC = this.ViewControllerAtIndex(0) as TutorialContentPageViewController;
            var viewControllers = new UIViewController[] { startVC };

            pageViewController.SetViewControllers(viewControllers, UIPageViewControllerNavigationDirection.Forward, true, null);

            // Set appearance of buttons and page control on TutorialViewController
            int buttonHeight = 39;
            int buttonSpace = 20;

            var logInTutorialButton = new UIButton()
            {
                Frame = new CGRect(buttonSpace, UIScreen.MainScreen.Bounds.Size.Height - buttonHeight - buttonSpace, UIScreen.MainScreen.Bounds.Size.Width / 2 - buttonSpace - buttonSpace/2, buttonHeight),
            };
            logInTutorialButton.SetTitle("Log In", UIControlState.Normal);
            logInTutorialButton.TitleLabel.Font = UIFont.SystemFontOfSize(16);
            logInTutorialButton.Layer.BorderWidth = 0.5f;
            logInTutorialButton.Layer.CornerRadius = 5;
            logInTutorialButton.Layer.BorderColor = UIColor.DarkGray.CGColor;
            logInTutorialButton.SetTitleColor(UIColor.White, UIControlState.Normal);
            logInTutorialButton.SetTitleColor(UIColor.FromWhiteAlpha(0.8f, 0.2f), UIControlState.Highlighted);

            var signUpTutorialButton = new UIButton()
            {
                Frame = new CGRect(logInTutorialButton.Layer.Frame.GetMaxX() + buttonSpace, UIScreen.MainScreen.Bounds.Size.Height - buttonHeight - buttonSpace, UIScreen.MainScreen.Bounds.Size.Width / 2 - buttonSpace - buttonSpace/2, buttonHeight),
            };
            signUpTutorialButton.SetTitle("Sign Up", UIControlState.Normal);
            signUpTutorialButton.TitleLabel.Font = UIFont.SystemFontOfSize(16);
            signUpTutorialButton.Layer.BorderWidth = 0.5f;
            signUpTutorialButton.Layer.CornerRadius = 5;
            signUpTutorialButton.Layer.BorderColor = UIColor.DarkGray.CGColor;
            signUpTutorialButton.SetTitleColor(UIColor.FromRGB(214, 93, 98), UIControlState.Normal);
            signUpTutorialButton.SetTitleColor(UIColor.FromWhiteAlpha(0.8f, 0.2f), UIControlState.Highlighted);

            int pageControlWidth = 7;
            var tutorialPageControl = new UIPageControl()
            {
                Frame = new CGRect(UIScreen.MainScreen.Bounds.Size.Width / 2 - pageControlWidth/2, UIScreen.MainScreen.Bounds.Size.Height - buttonHeight - buttonSpace - 40, pageControlWidth, 0)
            };
            tutorialPageControl.Pages = pageImages.Count();
            tutorialPageControl.PageIndicatorTintColor = UIColor.White;
            tutorialPageControl.CurrentPageIndicatorTintColor = UIColor.FromRGB(214, 93, 98);
            tutorialPageControl.CurrentPage = 0;

            int nextPageButtonSize = 40;
            var nextPageTutorialButton = new UIButton()
            {
                Frame = new CGRect(UIScreen.MainScreen.Bounds.Size.Width / 2 - nextPageButtonSize / 2, UIScreen.MainScreen.Bounds.Size.Height - buttonHeight - buttonSpace - 40 * 2 - nextPageButtonSize, nextPageButtonSize, nextPageButtonSize)
            };
            nextPageTutorialButton.SetImage(UIImage.FromBundle("StartTutorialButton").ImageWithRenderingMode(UIImageRenderingMode.AlwaysOriginal), UIControlState.Normal);

            AddChildViewController(this.pageViewController);

            View.AddSubviews(backgroundImageView, this.pageViewController.View, nextPageTutorialButton, tutorialPageControl, signUpTutorialButton, logInTutorialButton);

            pageViewController.DidMoveToParentViewController(this);

            // Getting index of the current page to change pageControl
            pageViewController.DidFinishAnimating += delegate (object sender, UIPageViewFinishedAnimationEventArgs e)
            {
                if (e.Completed)
                {
                    var currentVC = pageViewController.ViewControllers.First() as TutorialContentPageViewController;
                    tutorialPageControl.CurrentPage = currentVC.pageIndex;

                    if (currentVC.pageIndex == pageImages.Count() - 1)
                        nextPageTutorialButton.Hidden = true;
                    else
                        nextPageTutorialButton.Hidden = false;
                }
            };

            // Next Page button
            nextPageTutorialButton.TouchUpInside += (sender, e) =>
            {
                var currentVC = pageViewController.ViewControllers.First() as TutorialContentPageViewController;
                var currentIndex = currentVC.pageIndex;

                if (currentIndex < pageImages.Count() - 1)
                {
                    var nextVC = this.ViewControllerAtIndex(currentIndex + 1) as TutorialContentPageViewController;
                    pageViewController.SetViewControllers(new UIViewController[] { nextVC }, UIPageViewControllerNavigationDirection.Forward, true, null);
                    tutorialPageControl.CurrentPage = currentIndex + 1;

                    // Hide button "next" on the last page
                    if (currentIndex == pageImages.Count() - 2)
                        nextPageTutorialButton.Hidden = true;
                }
            };

            // Launch Login View Controller
            logInTutorialButton.TouchUpInside += (sender, e) =>
            {
                var loginController = this.Storyboard.InstantiateViewController("LoginUserViewController") as LoginUserViewController;
                if (loginController != null)
                    this.PresentViewController(loginController, true, null);
            };

            // Launch Register View Controller
            signUpTutorialButton.TouchUpInside += (sender, e) =>
            {
                var registerController = this.Storyboard.InstantiateViewController("EnterNameRegisterUser") as RegisterUserViewController;
                if (registerController != null)
                {
                    var navigationController = new UINavigationController(registerController);
                    this.PresentViewController(navigationController, true, null);
                }
            };
        }

        private void RestartTutorial(object sender, EventArgs e)
        {
            var startVC = this.ViewControllerAtIndex(0) as TutorialContentPageViewController;
            var viewControllers = new UIViewController[] { startVC };
            this.pageViewController.SetViewControllers(viewControllers, UIPageViewControllerNavigationDirection.Forward, true, null);
        }

        public UIViewController ViewControllerAtIndex(int index)
        {
            var viewController = this.Storyboard.InstantiateViewController("TutorialContentPageViewController") as TutorialContentPageViewController;
            viewController.imageFile = pageImages.ElementAt(index);
            viewController.pageIndex = index;

            return viewController;
        }
    }
}