// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;

namespace Happy31.Prompts
{
    [Register ("PromptsCollectionViewController")]
    partial class PromptsCollectionViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIBarButtonItem SyncAllPromptsButton { get; set; }

        [Action ("SyncAllPromptsButton_Activated:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void SyncAllPromptsButton_Activated (UIKit.UIBarButtonItem sender);

        void ReleaseDesignerOutlets ()
        {
            if (SyncAllPromptsButton != null) {
                SyncAllPromptsButton.Dispose ();
                SyncAllPromptsButton = null;
            }
        }
    }
}