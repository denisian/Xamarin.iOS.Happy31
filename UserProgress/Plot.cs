//
//  Plot.cs
//  Happy31.iOSApp
//
//  Copyright © 2017 Denis Klyucherov. All rights reserved.
//

using OxyPlot;
using OxyPlot.Series;

namespace Happy31.UserProgress
{
    /// <summary>
    /// Setting for the plot model to display users' progress
    /// </summary>
    public static class Plot
    {
        public static PlotModel CreatePlotModel(double done, double unDone)
        {

            var plotModel = new PlotModel { };

            var series = new PieSeries
            {
                
                LegendFormat = "",
                OutsideLabelFormat = "",
                InsideLabelFormat = "",
                TrackerFormatString = "",
                StrokeThickness = 5,
                //InsideLabelPosition = 0.8,
                //AngleSpan = 360,
                //StartAngle = 0,
                TickHorizontalLength = 0,
                TickRadialLength = 0
            };

            series.Slices.Add(new PieSlice("Done", done) { IsExploded = false, Fill = OxyColor.FromRgb(120, 169, 133) });
            series.Slices.Add(new PieSlice("Undone", unDone) { IsExploded = true, Fill = OxyColor.FromRgb(255, 243, 213) });
   
            plotModel.Series.Add(series);

            return plotModel;
        }
    }
}