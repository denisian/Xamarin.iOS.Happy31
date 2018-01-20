//
//  DisplayedPromptsModel.cs
//  Happy31.iOSApp
//
//  Copyright © 2017 Denis Klyucherov. All rights reserved.
//

namespace Happy31
{
    /// <summary>
    /// Model for the displayed user prompts in UI
    /// </summary>
    public class DisplayedPromptsModel
    {
        public int UserPromptId { get; set; }
        public string CreatedAt { get; set; }
        public string Category { get; set; }
        public string Task { get; set; }
        public string IsDone { get; set; }
    }
}