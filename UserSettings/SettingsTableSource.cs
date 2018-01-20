//
//  SettingsTableSource.cs
//  Happy31.iOSApp
//
//  Copyright © 2017 Denis Klyucherov. All rights reserved.
//

using CoreGraphics;
using Foundation;
using System;
using System.Collections.Generic;
using UIKit;
using UserNotifications;

namespace Happy31.UserSettings
{
    /// <summary>
    /// User's settings Table Source
    /// </summary>
    public class SettingsTableSource : UITableViewSource
    {
        List<string> tableItems;
        List<string> settingsValues;
        NSUserDefaults plist = NSUserDefaults.StandardUserDefaults;

        string cellIdentifier = "TableCell";

        public SettingsTableSource(List<string> _items, List<string> _values)
        {
            tableItems = _items;
            settingsValues = _values;
        }

        public override nint RowsInSection(UITableView tableview, nint section)
        {
            return tableItems.Count;
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            UITableViewCell cell = tableView.DequeueReusableCell(cellIdentifier);
            if (cell == null)
                cell = new UITableViewCell(UITableViewCellStyle.Default, cellIdentifier);

            var rightBorder = 35;

            var datePicker = new UIDatePicker();
            datePicker.Mode = UIDatePickerMode.Time;
            datePicker.TimeZone = NSTimeZone.LocalTimeZone;
            datePicker.UserInteractionEnabled = true;

            var textField = new CustomTextField();
            textField.Text = settingsValues[indexPath.Row];
            textField.TextColor = UIColor.Gray;
            textField.ContentMode = UIViewContentMode.Right;

            cell.TextLabel.Text = tableItems[indexPath.Row];
            //cell.TextLabel.Lines = 2;

            if (indexPath.Row == 0)
            {
                cell.TextLabel.Font = UIFont.SystemFontOfSize(28, UIFontWeight.Bold);
                cell.TextLabel.TextColor = UIColor.FromRGB(198, 123, 112);
                cell.TextLabel.TextAlignment = UITextAlignment.Center;
            }
            else if (indexPath.Row == 1)
            {
                var enableNotifSwitch = new UISwitch();
                int buttonSize = 30;
                enableNotifSwitch.Frame = new CGRect(tableView.Frame.Width - buttonSize - rightBorder, (tableView.RowHeight - buttonSize) / 2, buttonSize, tableView.RowHeight);
                enableNotifSwitch.SetState(Convert.ToBoolean(settingsValues[indexPath.Row]), true);
                enableNotifSwitch.OnTintColor = UIColor.FromRGB(198, 123, 112);

                enableNotifSwitch.ValueChanged += (sender, e) =>
                {
                    plist.SetBool(((UISwitch)sender).On, "enableNotifications");

                    if (!((UISwitch)sender).On)
                        UNUserNotificationCenter.Current.RemoveAllPendingNotificationRequests();
                };

                cell.ContentView.AddSubview(enableNotifSwitch);
            }
            else if (indexPath.Row == 2 || indexPath.Row == 3)
            {
                int textFieldSize = 100;
                textField.Frame = new CGRect(UIScreen.MainScreen.Bounds.Size.Width - textFieldSize + 10, (tableView.RowHeight - textFieldSize * 0.9) / 2, textFieldSize, tableView.RowHeight);

                // Set default value for datePicker
                var dateFromString = DateTime.Parse(settingsValues[indexPath.Row]);
                var specifiedDate = DateTime.SpecifyKind(dateFromString, DateTimeKind.Local);
                datePicker.SetDate((NSDate)specifiedDate, true);

                textField.AllEditingEvents += delegate {
                    textField.InputView = datePicker;
                };

                datePicker.ValueChanged += (sender, e) =>
                {
                    textField.Text = DateTime.Parse(datePicker.Date.ToString()).ToString("hh:mm tt", System.Globalization.CultureInfo.InvariantCulture);

                    if (indexPath.Row == 2)
                        plist.SetString(textField.Text, "promptStartTime");
                    else if (indexPath.Row == 3)
                        plist.SetString(textField.Text, "promptEndTime");
                };

                cell.ContentView.AddSubview(textField);
            }
            else if (indexPath.Row == 4)
            {
                int textFieldSize = 50;
                textField.Frame = new CGRect(UIScreen.MainScreen.Bounds.Size.Width - 30, tableView.RowHeight / 2 - 45, textFieldSize, tableView.RowHeight);
                textField.KeyboardType = UIKeyboardType.NumberPad;
                //textField.ReturnKeyType = UIReturnKeyType.Done;

                textField.EditingChanged += (sender, e) => {
                    if (textField.Text.Length > 2)
                        textField.Text = textField.Text.Substring(2, 1);

                    if (textField.Text != "")
                    {
                        if (Convert.ToInt32(textField.Text) > 24)
                            textField.Text = "24";
                        else if (Convert.ToInt32(textField.Text) < 1)
                            textField.Text = "1";
                    }
                    else
                        textField.Text = settingsValues[indexPath.Row];
                        
                };

                textField.EditingDidEnd += delegate {
                    //if (textField.Text == "")
                        //textField.Text = settingsValues[indexPath.Row];
                    plist.SetString(textField.Text, "promptMinInterval");
                    Console.WriteLine(plist.StringForKey("promptMinInterval"));
                };

                //textField.ShouldReturn = ((txtField) =>
                //{
                //    // Dismiss the Keyboard
                //    textField.ResignFirstResponder();

                //    return true;
                //});

                cell.ContentView.AddSubview(textField);
            }

            cell.BackgroundColor = UIColor.Clear;
            cell.SelectionStyle = UITableViewCellSelectionStyle.None;
            return cell;
        }
    }

    public class CustomTextField : UITextField
    {
        // Remove blinking cursor
        public override CGRect GetCaretRectForPosition(UITextPosition position)
        {
            return CGRect.Empty;
        }

        // Remove selection
        public override UITextSelectionRect[] GetSelectionRects(UITextRange range)
        {
            return null;
        }

        // Remove copy-paste actions
        public override bool CanPerform(ObjCRuntime.Selector action, NSObject withSender)
        {
            if (action == new ObjCRuntime.Selector("copy:") || action == new ObjCRuntime.Selector("selectAll:") || action == new ObjCRuntime.Selector("paste:"))
                return false;
            else
                return base.CanPerform(action, withSender);
        }
    }
}