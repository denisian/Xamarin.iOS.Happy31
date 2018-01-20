//
//  TutorialContentPageViewController.cs
//  Happy31.iOSApp
//
//  Copyright © 2017 Denis Klyucherov. All rights reserved.
//

using System;
using UIKit;

namespace Happy31.Tutorial
{
    /// <summary>
    /// Tutorial content page View Controller.
    /// </summary>
    public partial class TutorialContentPageViewController : UIViewController
    {
        public int pageIndex;
        public string imageFile;

        public TutorialContentPageViewController (IntPtr handle) : base (handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            imageView.Image = UIImage.FromBundle(imageFile);
        }
    }
}