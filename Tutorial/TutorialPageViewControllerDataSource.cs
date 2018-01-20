//
//  TutorialPageViewControllerDataSource.cs
//  Happy31.iOSApp
//
//  Copyright © 2017 Denis Klyucherov. All rights reserved.
//

using System.Collections.Generic;
using UIKit;

namespace Happy31.Tutorial
{
    /// <summary>
    /// Tutorial page View Controller Data Source.
    /// </summary>
    public class TutorialPageViewControllerDataSource : UIPageViewControllerDataSource
    {
        TutorialViewController _parentViewController;
        List<string> _pageImages;

        public TutorialPageViewControllerDataSource(UIViewController parentViewController, List<string> pageImages)
        {
            _parentViewController = parentViewController as TutorialViewController;
            _pageImages = pageImages;
        }

        public override UIViewController GetPreviousViewController(UIPageViewController pageViewController, UIViewController referenceViewController)
        {
            var viewController = referenceViewController as TutorialContentPageViewController;
            var index = viewController.pageIndex;

            if (index == 0)
                 return null;
                //return _parentViewController.ViewControllerAtIndex(_pageImages.Count - 1);
            else
            {
                index--;
                return _parentViewController.ViewControllerAtIndex(index);
            }
        }

        public override UIViewController GetNextViewController(UIPageViewController pageViewController, UIViewController referenceViewController)
        {
            var viewController = referenceViewController as TutorialContentPageViewController;
            var index = viewController.pageIndex;

            index++;
            if (index == _pageImages.Count)
                  return null;
                //return _parentViewController.ViewControllerAtIndex(0);
            else
                return _parentViewController.ViewControllerAtIndex(index);
        }
    }
}