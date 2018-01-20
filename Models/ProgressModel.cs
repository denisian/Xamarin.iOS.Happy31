//
//  ProgressModel.cs
//  Happy31.iOSApp
//
//  Copyright © 2017 Denis Klyucherov. All rights reserved.
//

namespace Happy31
{
    /// <summary>
    /// Model to display progress to the user
    /// </summary>
    public class ProgressModel
    {
        public string Category { get; set; }
        public int DoneAmount { get; set; }
        public int UndoneAmount { get; set; }
    }
}