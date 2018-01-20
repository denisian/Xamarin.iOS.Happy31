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

namespace Happy31.UserProgress
{
    [Register ("UserProgressTableViewController")]
    partial class UserProgressTableViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITableView progressTableView { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (progressTableView != null) {
                progressTableView.Dispose ();
                progressTableView = null;
            }
        }
    }
}