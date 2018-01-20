//
//  PromptsCollectionSource.cs
//  Happy31.iOSApp
//
//  Copyright © 2017 Denis Klyucherov. All rights reserved.
//

using Foundation;
using System;
using System.Collections.Generic;
using UIKit;
using CoreGraphics;
using System.Linq;

namespace Happy31.Prompts
{
    /// <summary>
    /// Prompts collection source
    /// </summary>
    public class PromptsCollectionSource : UICollectionViewSource
    {
        List<DisplayedPromptsModel> prompts;
        List<int> promtsForDay;
        string userId;
        UIViewController viewController;

        static int promptTextFontSize = CollectionSizes.promptTextFontSize;

        static int promptImageSize = CollectionSizes.promptImageSize;
        static int promptsSpace = CollectionSizes.promptsSpace;
        static int spaceBetweenDateAndPrompt = CollectionSizes.spaceBetweenDateAndPrompt;
        static int spaceBetweenPromptAndButtons = CollectionSizes.spaceBetweenPromptAndButtons;

        static int borderSpace = CollectionSizes.borderSpace;
        static int spaceBetweenPromptImageAndBubbleView = CollectionSizes.spaceBetweenPromptImageAndBubbleView;

        static int spaceBetweenBubbleViewAndText = CollectionSizes.spaceBetweenBubbleViewAndText;

        static int promptIsDoneButtonSize = CollectionSizes.promptIsDoneButtonSize;
        static int promptShareToFeedButtonSize = CollectionSizes.promptShareToFeedButtonSize;
        static int promptAddToCalendarButtonSize = CollectionSizes.promptAddToCalendarButtonSize;

        static int spaceBetweenButtons = CollectionSizes.spaceBetweenButtons;

        static int extraSpacePromptTextView = CollectionSizes.extraSpacePromptTextView;

        static int promptMaxCGSizeWidth = (int)UIScreen.MainScreen.Bounds.Size.Width - 2 * borderSpace - promptImageSize - spaceBetweenPromptImageAndBubbleView;
        static int promptMaxCGSizeHeight = 2000;

        string getFirstDate;

        public PromptsCollectionSource(List<DisplayedPromptsModel> _prompts, List<int> _promtsForDay, string _userId, UIViewController _viewController)
        {
            prompts = _prompts;
            promtsForDay = _promtsForDay;
            userId = _userId;
            viewController = _viewController;

            // Get first prompt's date
            getFirstDate = prompts.OrderBy(date => date.CreatedAt).Select(date => date.CreatedAt).First();
        }

        // Changing size of the cell
        [Export("collectionView:layout:sizeForItemAtIndexPath:")]
        public virtual CGSize GetSizeForItem(UICollectionView collectionView, UICollectionViewLayout layout, NSIndexPath indexPath)
        {
            var promptText = prompts[indexPath.Row].Task;

            if (promptText != null)
            {
                var size = new CGSize(promptMaxCGSizeWidth, promptMaxCGSizeHeight);
                var options = NSStringDrawingOptions.UsesFontLeading | NSStringDrawingOptions.UsesLineFragmentOrigin;
                var attributes = new UIStringAttributes { Font = UIFont.SystemFontOfSize(promptTextFontSize) };
                var estimatedFrame = new NSString(promptText).GetBoundingRect(size, options, attributes, null);

                return new CGSize(collectionView.Frame.Width, estimatedFrame.Height + extraSpacePromptTextView + 2 * spaceBetweenDateAndPrompt + promptIsDoneButtonSize);
            }
            return new CGSize(collectionView.Frame.Width, 1000);
        }

        public override nint NumberOfSections(UICollectionView collectionView)
        {
            return 1;
        }

        public override nint GetItemsCount(UICollectionView collectionView, nint section)
        {
            return prompts.Count;
        }

        [Export("collectionView:layout:minimumLineSpacingForSectionAtIndex:")]
        public nfloat GetMinimumLineSpacingForSection(UICollectionView collectionView, UICollectionViewLayout layout, nint section)
        {
            return promptsSpace;
        }

        public override bool ShouldHighlightItem(UICollectionView collectionView, NSIndexPath indexPath)
        {
            return true;
        }

        // Getting date depending on promt's indexPath (if it's first promt or 4/7/10..etc (index mod 3 = 1))
        string GetDate(int indexPath, string sourceDate, string format)
        {
            if (format == "day")
            {
                if (promtsForDay.Contains(indexPath))
                {
                    int getListIndex = promtsForDay.IndexOf(indexPath) + 1;
                    //int getDay = (DateTime.Parse(sourceDate).Day - DateTime.Parse(getFirstDate).Day) + 1;
                    return "Day " + getListIndex;
                }
                else
                    return "";
            }
            else
                return DateTime.Parse(sourceDate).ToString("m");
        }

        public override UICollectionViewCell GetCell(UICollectionView collectionView, NSIndexPath indexPath)
        {
            var cell = (PromptsCollectionCell)collectionView.DequeueReusableCell(PromptsCollectionCell.cellId, indexPath);

            var promptText = prompts[indexPath.Row].Task;

            if (promptText == "")
            {
                cell.promptTextView.Frame = new CGRect(20, UIScreen.MainScreen.Bounds.Size.Height / 2 - 60 * 2, UIScreen.MainScreen.Bounds.Size.Width - 20 * 2, 60);
                cell.promptTextView.Text = "Pull down to refresh or wait for the prompt to appear";
                cell.promptTextView.TextColor = UIColor.Gray;
                cell.promptTextView.TextAlignment = UITextAlignment.Center;
                return cell;
            }

            cell.promptTextView.Text = promptText;
            cell.promptTextView.TextAlignment = UITextAlignment.Left;
            cell.promptTextView.ScrollEnabled = false;
            cell.promptTextView.ScrollEnabled = true;

            var userPromptId = prompts[indexPath.Row].UserPromptId;
            var promptIsDone = prompts[indexPath.Row].IsDone;

            var promptIcon = prompts[indexPath.Row].Category;

            cell.Tag = userPromptId; // Save tag of every cell to avoid reusing same cells

            if (cell.Tag == userPromptId)
            {
                var promptDay = GetDate(indexPath.Row, prompts[indexPath.Row].CreatedAt, "day");
                var promptFullDate = GetDate(indexPath.Row, prompts[indexPath.Row].CreatedAt, "fulldate");

                cell.dateLabelDay.Text = promptDay;
                cell.dateLabelFullDate.Text = promptFullDate;
            }

            if (promptIcon != null)
                cell.promptImageView.Image = UIImage.FromBundle(promptIcon).ImageWithRenderingMode(UIImageRenderingMode.AlwaysTemplate);

            // Buttons
            cell.promptIsDoneButton.SetImage(UIImage.FromBundle("DoneIcon").ImageWithRenderingMode(UIImageRenderingMode.AlwaysTemplate), UIControlState.Normal);
            //cell.shareToFeedButton.SetImage(UIImage.FromBundle("ShareToFeedIcon").ImageWithRenderingMode(UIImageRenderingMode.AlwaysTemplate), UIControlState.Normal);
            cell.addToCalendarButton.SetImage(UIImage.FromBundle("AddToCalendarIcon").ImageWithRenderingMode(UIImageRenderingMode.AlwaysTemplate), UIControlState.Normal);

            var size = new CGSize(promptMaxCGSizeWidth, promptMaxCGSizeHeight);
            var options = NSStringDrawingOptions.UsesFontLeading | NSStringDrawingOptions.UsesLineFragmentOrigin;
            var attributes = new UIStringAttributes { Font = UIFont.SystemFontOfSize(promptTextFontSize) };
            var estimatedFrame = new NSString(cell.promptTextView.Text).GetBoundingRect(size, options, attributes, null);


            nfloat minimalestimatedWidth = promptIsDoneButtonSize * 4 + spaceBetweenButtons * 4;
            if (estimatedFrame.Width < minimalestimatedWidth)
                estimatedFrame.Width = minimalestimatedWidth;

            cell.promptTextView.Frame = new CGRect(borderSpace + promptImageSize + spaceBetweenPromptImageAndBubbleView + spaceBetweenBubbleViewAndText, spaceBetweenDateAndPrompt, estimatedFrame.Width - 2 * spaceBetweenBubbleViewAndText, estimatedFrame.Height + extraSpacePromptTextView); // 48px to move textView right for image
            cell.bubbleView.Frame = new CGRect(borderSpace + promptImageSize + spaceBetweenPromptImageAndBubbleView, spaceBetweenDateAndPrompt, estimatedFrame.Width, estimatedFrame.Height + extraSpacePromptTextView + spaceBetweenPromptAndButtons);
            cell.dateLabelFullDate.Frame = new CGRect(cell.bubbleView.Frame.GetMaxX() - 150, 0, 150, 30);

            // Buttons
            cell.addToCalendarButton.Frame = new CGRect(cell.bubbleView.Frame.GetMaxX() - promptAddToCalendarButtonSize - 2 * spaceBetweenBubbleViewAndText, estimatedFrame.Height + extraSpacePromptTextView + spaceBetweenPromptAndButtons - spaceBetweenPromptImageAndBubbleView, promptAddToCalendarButtonSize, promptAddToCalendarButtonSize);
            cell.promptIsDoneButton.Frame = new CGRect(cell.addToCalendarButton.Frame.GetMinX() - promptShareToFeedButtonSize - spaceBetweenBubbleViewAndText - spaceBetweenButtons, estimatedFrame.Height + extraSpacePromptTextView + spaceBetweenPromptAndButtons - spaceBetweenPromptImageAndBubbleView, promptShareToFeedButtonSize, promptShareToFeedButtonSize);
            //cell.promptIsDoneButton.Frame = new CGRect(cell.shareToFeedButton.Frame.GetMinX() - promptIsDoneButtonSize - spaceBetweenBubbleViewAndText - spaceBetweenButtons, estimatedFrame.Height + extraSpacePromptTextView - spaceBetweenPromptImageAndBubbleView, promptIsDoneButtonSize, promptIsDoneButtonSize);

            cell.UpdateIsDoneColor(promptIsDone);

            cell.promptIsDoneButton.TouchUpInside += ((sender, e) =>
            {
                if (cell.Tag == userPromptId)
                {
                    if (promptIsDone == "True")
                        promptIsDone = "False";
                    else
                        promptIsDone = "True";

                    if (cell.UpdateIsDoneStatus(userPromptId, promptIsDone) >= 0)
                    {
                        cell.UpdateIsDoneColor(promptIsDone);
                        cell.SyncUserPrompt(userId);
                        prompts[indexPath.Row].IsDone = promptIsDone;

                        // Notification
                        string message;
                        if (promptIsDone == "True")
                            message = "Congratulations! You have successfully done this task.";
                        else
                            message = "No worries! Just complete this task next time.";
                        AppNotifications.Display(new DisplayedPromptsModel() { Category = promptIcon + " prompt", Task = message });
                    }

                    Console.WriteLine($"Status of prompt {userPromptId} successfuly changed to {promptIsDone}");
                    // collectionView.ReloadItems(collectionView.IndexPathsForVisibleItems);
                }
            });

            cell.addToCalendarButton.TouchUpInside += async (sender, e) =>
            {
                if (cell.Tag == userPromptId)
                {
                    var calendar = new AppCalendar();
                    await calendar.AddToCalendar(DateTime.Now, DateTime.Now, promptIcon + " task to be completed", promptText, true, viewController);
                }
            };
                                                      
            return cell;
        }
    }
}