using System;
using Newtonsoft.Json;
using SQLite;

namespace Happy31
{
    [Table("users_prompts")]
    public class UsersPromptsModel
    {
        [PrimaryKey]
        [JsonProperty("user_prompt_id")]
        public int UserPromptId { get; set; }

        [JsonProperty("user_id"), NotNull]
        public string UserId { get; set; }

        [JsonProperty("prompt_id"), NotNull]
        public int PromptId { get; set; }

        [JsonProperty("created_at"), NotNull]
        public string CreatedAt { get; set; }

        [JsonProperty("is_sync"), NotNull]
        public string IsSync { get; set; } = false.ToString();
    }
}
