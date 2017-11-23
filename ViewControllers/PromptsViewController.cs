using Foundation;
using System;
using UIKit;

namespace Happy31
{
    public partial class PromptsViewController : UICollectionViewController
    {
        public PromptsViewController (IntPtr handle) : base (handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            // Perform any additional setup after loading the view, typically from a nib.

            GenerateTextFields();

            //myTextView.BackgroundColor = UIColor.Green; //.FromRGB(red: 39/255, green: 53/255, blue: 182/255);
            //myTextView.Editable = false;

            //myTextView.Selectable = true;
            //myTextView.DataDetectorTypes = UIDataDetectorType.CalendarEvent;
            //myTextView.Layer.CornerRadius = 20;
        }

        void GenerateTextFields()
        {
            // UITextView
            var textView = new UITextView();
            textView.Editable = false;
            textView.ScrollEnabled = true;
            textView.Text = "Lorem ipsum";
            textView.Layer.CornerRadius = 30;



        }
    }
}