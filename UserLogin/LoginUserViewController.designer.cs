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

namespace Happy31
{
    [Register ("LoginUserViewController")]
    partial class LoginUserViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton fbAuthButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel logInLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton registerButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton signInButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField userEmailTextField { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField userPasswordTextField { get; set; }

        [Action ("FbAuthButton_TouchUpInside:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void FbAuthButton_TouchUpInside (UIKit.UIButton sender);

        [Action ("RegisterButton_TouchUpInside:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void RegisterButton_TouchUpInside (UIKit.UIButton sender);

        [Action ("SignInButton_TouchUpInside:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void SignInButton_TouchUpInside (UIKit.UIButton sender);

        void ReleaseDesignerOutlets ()
        {
            if (fbAuthButton != null) {
                fbAuthButton.Dispose ();
                fbAuthButton = null;
            }

            if (logInLabel != null) {
                logInLabel.Dispose ();
                logInLabel = null;
            }

            if (registerButton != null) {
                registerButton.Dispose ();
                registerButton = null;
            }

            if (signInButton != null) {
                signInButton.Dispose ();
                signInButton = null;
            }

            if (userEmailTextField != null) {
                userEmailTextField.Dispose ();
                userEmailTextField = null;
            }

            if (userPasswordTextField != null) {
                userPasswordTextField.Dispose ();
                userPasswordTextField = null;
            }
        }
    }
}