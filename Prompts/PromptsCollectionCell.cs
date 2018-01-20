//
//  PromptsCollectionCell.cs
//  Happy31.iOSApp
//
//  Copyright © 2017 Denis Klyucherov. All rights reserved.
//

using Foundation;
using UIKit;
using CoreGraphics;

namespace Happy31.Prompts
{
    /// <summary>
    /// Prompts collection cell 
    /// </summary>
    public class PromptsCollectionCell : UICollectionViewCell
    {
        public UILabel dateLabelDay;
        public UILabel dateLabelFullDate;
        public UITextView promptTextView;
        public UIView bubbleView;
        public UIImageView promptImageView;
        public UIButton promptIsDoneButton;
        public UIButton shareToFeedButton;
        public UIButton addToCalendarButton;

        UsersPromptsRepository userPromptRepository = new UsersPromptsRepository();

        public static NSString cellId = new NSString("customCollectionCell"); // Static reference to use through app

        int promptTextFontSize = CollectionSizes.promptTextFontSize;

        int promptImageSize = CollectionSizes.promptImageSize;
        int spaceBetweenDateAndPrompt = CollectionSizes.spaceBetweenDateAndPrompt;
        int borderSpace = CollectionSizes.borderSpace;

        int promptIsDoneButtonSize = CollectionSizes.promptIsDoneButtonSize;
        int promptShareToFeedButtonSize = CollectionSizes.promptShareToFeedButtonSize;
        int promptAddToCalendarButtonSize = CollectionSizes.promptAddToCalendarButtonSize;

        [Export("initWithFrame:")]
        public PromptsCollectionCell(CGRect frame) : base(frame)
        {
            dateLabelDay = new UILabel()
            {
                Frame = new CGRect(borderSpace, 0, promptImageSize + 20, 30),
                Font = UIFont.SystemFontOfSize(promptTextFontSize + 2),
                TextColor = UIColor.DarkGray
            };

            dateLabelFullDate = new UILabel()
            {
                Font = UIFont.SystemFontOfSize(promptTextFontSize),
                TextAlignment = UITextAlignment.Right,
                TextColor = UIColor.DarkGray
            };

            promptTextView = new UITextView()
            {
                BackgroundColor = UIColor.Clear,
                Font = UIFont.SystemFontOfSize(promptTextFontSize),
                Editable = false,
                Selectable = false,
                TextAlignment = UITextAlignment.Left,
                TranslatesAutoresizingMaskIntoConstraints = false
            };

            bubbleView = new UIView()
            {
                BackgroundColor = UIColor.FromRGB(255, 243, 213),
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            bubbleView.Layer.CornerRadius = 15;
            bubbleView.Layer.MasksToBounds = true;

            promptImageView = new UIImageView()
            {
                Frame = new CGRect(0, 0, promptImageSize, promptImageSize),
                ContentMode = UIViewContentMode.ScaleAspectFill,
                TintColor = UIColor.FromRGB(198, 123, 112),
                TranslatesAutoresizingMaskIntoConstraints = false
            };

            promptIsDoneButton = new UIButton()
            {
                TintColor = UIColor.Gray
            };

            shareToFeedButton = new UIButton()
            {
                TintColor = UIColor.Gray
            };

            addToCalendarButton = new UIButton()
            {
                TintColor = UIColor.Gray
            };

            ContentView.AddSubviews(dateLabelDay, dateLabelFullDate, bubbleView, promptTextView, promptImageView, promptIsDoneButton, shareToFeedButton, addToCalendarButton);

            // Add constraints
            // Get views being constrained
            var views = new NSMutableDictionary();
            views.Add(new NSString("v0"), promptImageView);

            int constraintPromptImage = promptImageSize;
            ContentView.AddConstraints(NSLayoutConstraint.FromVisualFormat($"H:|-{borderSpace}-[v0({constraintPromptImage})]", 0, null, views));
            ContentView.AddConstraints(NSLayoutConstraint.FromVisualFormat($"V:|-{spaceBetweenDateAndPrompt}-[v0({constraintPromptImage})]|", 0, null, views));
        }

        public void UpdateIsDoneColor(string isDone)
        {
            if (isDone == "True")
            {
                promptIsDoneButton.TintColor = UIColor.FromRGB(198, 123, 112);
                bubbleView.BackgroundColor = UIColor.FromRGB(120, 169, 133);
                promptTextView.TextColor = UIColor.White;
            }
            else
            {
                promptIsDoneButton.TintColor = UIColor.Gray;
                bubbleView.BackgroundColor = UIColor.FromRGB(255, 243, 213);
                promptTextView.TextColor = UIColor.DarkGray;
            }
        }

        // Positioning of the cell (use in PromptsCollectionSource)
        public int UpdateIsDoneStatus(int userPromptId, string isDone)
        {
            return userPromptRepository.UpdateIsDoneUserPrompt(userPromptId, isDone);
        }

        // Sync user prompt after it changed status Done/Not done
        public async void SyncUserPrompt(string userId)
        {
            await userPromptRepository.SyncCurrentUserPrompts(userId);
        }
    }
}