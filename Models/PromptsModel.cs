//
//  PromptsModel.cs
//  Happy31.iOSApp
//
//  Copyright © 2017 Denis Klyucherov. All rights reserved.
//

using Newtonsoft.Json;
using SQLite;

namespace Happy31
{
    /// <summary>
    /// Model for the table "Prompts" in the local database
    /// </summary>
    [Table("prompts")]
    public class PromptsModel
    {
        [JsonProperty("prompt_id"), PrimaryKey]
        public int Id { get; set; }

        [JsonProperty("prompt_category"), NotNull]
        public string Category { get; set; }

        [JsonProperty("prompt_task"), NotNull]
        public string Task { get; set; }
    }
}