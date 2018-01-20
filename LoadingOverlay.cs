//
//  LoadingOverlay.cs
//  Happy31.iOSApp
//
//  Copyright © 2017 Denis Klyucherov. All rights reserved.
//

using CoreFoundation;
using UIKit;

namespace Happy31
{
    /// <summary>
    /// Display an Activity Indicator when loading
    /// </summary>
    // 
    public static class LoadingOverlay
    {
        // Create Activity Indicator
        static UIActivityIndicatorView activityIndicator = new UIActivityIndicatorView(UIActivityIndicatorViewStyle.WhiteLarge);

        public static void ShowOverlay(UIView view)
        {
            DispatchQueue.MainQueue.DispatchAsync(() =>
            {
                // Position Activity Spinner in the center of the main view
                activityIndicator.Center = view.Center;

                activityIndicator.Color = UIColor.FromRGB(214, 93, 98);
                // activityIndicator.Frame = new CGRect(view.Center.X, view.Center.Y, 80, 80);

                // Preventing Activity Spinner from hiding when StopAnimating() is called
                activityIndicator.HidesWhenStopped = false;

                // Start Activity Spinner
                activityIndicator.StartAnimating();

                view.AddSubview(activityIndicator);
            });
        }

        // Remove Activity Indicator
        public static void RemoveOverlay()
        {
            DispatchQueue.MainQueue.DispatchAsync(() =>
            {
                activityIndicator.StopAnimating();
                activityIndicator.RemoveFromSuperview();
            });
        }
    }
}