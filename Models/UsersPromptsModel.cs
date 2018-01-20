//
//  UsersPromptsModel.cs
//  Happy31.iOSApp
//
//  Copyright © 2017 Denis Klyucherov. All rights reserved.
//

using Newtonsoft.Json;
using SQLite;
using SQLiteNetExtensions.Attributes;

namespace Happy31
{
    /// <summary>
    /// Model for the table "UsersPrompts" in the local database
    /// </summary>
    [Table("users_prompts")]
    public class UsersPromptsModel
    {
        [PrimaryKey]
        [JsonProperty("user_prompt_id")]
        public int UserPromptId { get; set; }

        [JsonProperty("user_id"), NotNull]
        public string UserId { get; set; }

        [JsonProperty("prompt_id"), NotNull]
        [ForeignKey(typeof(PromptsModel))]
        public int PromptId { get; set; }

        [JsonProperty("created_at"), NotNull]
        public string CreatedAt { get; set; }

        [JsonProperty("is_done"), NotNull]
        public string IsDone { get; set; }

        [JsonProperty("is_sync"), NotNull]
        public string IsSync { get; set; } = false.ToString();
    }
}