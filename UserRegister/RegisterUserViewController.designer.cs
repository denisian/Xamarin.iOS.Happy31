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
    [Register ("RegisterUserViewController")]
    partial class RegisterUserViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton backToSignInButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIImageView enterEmailImage { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel enterEmailLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton enterEmailNextButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIImageView enterNameImage { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel enterNameLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton enterNameNextButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIImageView enterPasswordImage { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel enterPasswordLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton signUpButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField userEmailTextField { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField userFirstNameTextField { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField userLastNameTextField { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField userPasswordTextField { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField userRepeatPasswordTextField { get; set; }

        [Action ("BackToSignInButton_TouchUpInside:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void BackToSignInButton_TouchUpInside (UIKit.UIButton sender);

        [Action ("EnterEmailNextButton_TouchUpInside:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void EnterEmailNextButton_TouchUpInside (UIKit.UIButton sender);

        [Action ("EnterNameNextButton_TouchUpInside:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void EnterNameNextButton_TouchUpInside (UIKit.UIButton sender);

        [Action ("SignUpButton_TouchUpInside:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void SignUpButton_TouchUpInside (UIKit.UIButton sender);

        void ReleaseDesignerOutlets ()
        {
            if (backToSignInButton != null) {
                backToSignInButton.Dispose ();
                backToSignInButton = null;
            }

            if (enterEmailImage != null) {
                enterEmailImage.Dispose ();
                enterEmailImage = null;
            }

            if (enterEmailLabel != null) {
                enterEmailLabel.Dispose ();
                enterEmailLabel = null;
            }

            if (enterEmailNextButton != null) {
                enterEmailNextButton.Dispose ();
                enterEmailNextButton = null;
            }

            if (enterNameImage != null) {
                enterNameImage.Dispose ();
                enterNameImage = null;
            }

            if (enterNameLabel != null) {
                enterNameLabel.Dispose ();
                enterNameLabel = null;
            }

            if (enterNameNextButton != null) {
                enterNameNextButton.Dispose ();
                enterNameNextButton = null;
            }

            if (enterPasswordImage != null) {
                enterPasswordImage.Dispose ();
                enterPasswordImage = null;
            }

            if (enterPasswordLabel != null) {
                enterPasswordLabel.Dispose ();
                enterPasswordLabel = null;
            }

            if (signUpButton != null) {
                signUpButton.Dispose ();
                signUpButton = null;
            }

            if (userEmailTextField != null) {
                userEmailTextField.Dispose ();
                userEmailTextField = null;
            }

            if (userFirstNameTextField != null) {
                userFirstNameTextField.Dispose ();
                userFirstNameTextField = null;
            }

            if (userLastNameTextField != null) {
                userLastNameTextField.Dispose ();
                userLastNameTextField = null;
            }

            if (userPasswordTextField != null) {
                userPasswordTextField.Dispose ();
                userPasswordTextField = null;
            }

            if (userRepeatPasswordTextField != null) {
                userRepeatPasswordTextField.Dispose ();
                userRepeatPasswordTextField = null;
            }
        }
    }
}