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
    [Register ("EditProfileTableViewController")]
    partial class EditProfileTableViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITableView userEditInfoTableView { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (userEditInfoTableView != null) {
                userEditInfoTableView.Dispose ();
                userEditInfoTableView = null;
            }
        }
    }
}