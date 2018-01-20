// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;

namespace Happy31.UserProfile
{
    [Register ("UserProfileTableViewController")]
    partial class UserProfileTableViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITableView userInfoTableView { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (userInfoTableView != null) {
                userInfoTableView.Dispose ();
                userInfoTableView = null;
            }
        }
    }
}