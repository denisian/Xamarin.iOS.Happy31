//
//  JsonResponseModel.cs
//  Happy31.iOSApp
//
//  Copyright © 2017 Denis Klyucherov. All rights reserved.
//

using Newtonsoft.Json;

namespace Happy31
{
    /// <summary>
    /// Model to get messages from the server using Json and display them to the user
    /// </summary>
    public class JsonResponseModel
    {
        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("user_id")]
        public string UserId { get; set; }

        [JsonProperty("prompt_id")]
        public string PromptId { get; set; }
    }
}