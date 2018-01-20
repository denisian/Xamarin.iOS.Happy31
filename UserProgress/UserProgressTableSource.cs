//
//  UserProgressTableSource.cs
//  Happy31.iOSApp
//
//  Copyright © 2017 Denis Klyucherov. All rights reserved.
//

using System;
using System.Collections.Generic;
using CoreGraphics;
using OxyPlot.Xamarin.iOS;
using UIKit;

namespace Happy31.UserProgress
{
    /// <summary>
    /// User progress Table Source
    /// </summary>
    public class UserProgressTableSource : UITableViewSource
    {
        List<ProgressModel> usersProgress;

        string cellIdentifier = "TableCell";

        public UserProgressTableSource(List<ProgressModel> _usersProgress)
        {
            usersProgress = _usersProgress;
        }

        public override nint RowsInSection(UITableView tableview, nint section)
        {
            return usersProgress.Count + 1;
        }

        public override nfloat GetHeightForRow(UITableView tableView, Foundation.NSIndexPath indexPath)
        {
            if (indexPath.Row == 0)
                return 90;
            else
                return tableView.RowHeight;
        }

        public override UITableViewCell GetCell(UITableView tableView, Foundation.NSIndexPath indexPath)
        {
            UITableViewCell cell = tableView.DequeueReusableCell(cellIdentifier);
            if (cell == null)
                cell = new UITableViewCell(UITableViewCellStyle.Default, cellIdentifier);

            cell.BackgroundColor = UIColor.Clear;
            cell.SelectionStyle = UITableViewCellSelectionStyle.None;

            if (indexPath.Row == 0)
            {
                cell.TextLabel.Text = "Progress";
                cell.TextLabel.Font = UIFont.SystemFontOfSize(28, UIFontWeight.Bold);
                cell.TextLabel.TextColor = UIColor.FromRGB(198, 123, 112);
                cell.TextLabel.TextAlignment = UITextAlignment.Center;
            }
            else
            {
                string promptCategory = usersProgress[indexPath.Row - 1].Category;
                int doneAmount = usersProgress[indexPath.Row - 1].DoneAmount;
                int undoneAmount = usersProgress[indexPath.Row - 1].UndoneAmount;

                if (promptCategory == "")
                {
                    cell.TextLabel.Text = "There are no any data yet";
                    cell.TextLabel.Font = UIFont.SystemFontOfSize(20, UIFontWeight.Regular);
                    cell.TextLabel.TextColor = UIColor.Gray;
                    cell.TextLabel.TextAlignment = UITextAlignment.Center;
                    return cell;
                }
                else
                    cell.TextLabel.Text = "";

                int promptImageSize = 60;
                var plotSize = 150;

                var promptImageView = new UIImageView()
                {
                    //Frame = new CGRect(20, (tableView.RowHeight - plotSize) / 2 + 5, promptImageSize, promptImageSize),
                    Frame = new CGRect((UIScreen.MainScreen.Bounds.Size.Width - promptImageSize) / 4.5, (tableView.RowHeight - promptImageSize) / 2 + 5, promptImageSize, promptImageSize),
                    ContentMode = UIViewContentMode.ScaleAspectFill,
                    TintColor = UIColor.FromRGB(198, 123, 112),
                    Image = UIImage.FromBundle(promptCategory).ImageWithRenderingMode(UIImageRenderingMode.AlwaysTemplate)
                };

                // Create an unvisible button which covers plotView - to solve problem with scrolling page (OxyPlot's bug)
                var plotButton = new UIButton()
                {
                    //Frame = new CGRect((UIScreen.MainScreen.Bounds.Size.Width - plotSize) / 2, (tableView.RowHeight - plotSize) / 2, plotSize, plotSize),
                    Frame = new CGRect((UIScreen.MainScreen.Bounds.Size.Width - plotSize) * 3 / 4, (tableView.RowHeight - plotSize) / 2, plotSize, plotSize),
                    BackgroundColor = UIColor.Clear
                };

                var plotView = new PlotView
                {
                    BackgroundColor = UIColor.Clear,
                    Model = Plot.CreatePlotModel(doneAmount, undoneAmount),
                    //Frame = new CGRect((UIScreen.MainScreen.Bounds.Size.Width - plotSize) / 2, (tableView.RowHeight - plotSize) / 2, plotSize, plotSize)
                    Frame = new CGRect((UIScreen.MainScreen.Bounds.Size.Width - plotSize)* 3 / 4, (tableView.RowHeight - plotSize) / 2, plotSize, plotSize)
                };

                //var promptProgressLabel = new UILabel()
                //{
                //    Frame = new CGRect(plotView.Frame.GetMaxX() + (UIScreen.MainScreen.Bounds.Size.Width - plotSize) / 4, tableView.RowHeight / 2, 40, 20),
                //    Font = UIFont.SystemFontOfSize(20),
                //    Text = doneAmount + "/" + undoneAmount,
                //    TextColor = UIColor.DarkGray,
                //    TextAlignment = UITextAlignment.Left
                //};

                cell.ContentView.AddSubviews(promptImageView, plotView, plotButton);
            }

            return cell;
        }
    }
}
