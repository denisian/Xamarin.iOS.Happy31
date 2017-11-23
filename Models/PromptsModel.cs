using Newtonsoft.Json;
using SQLite;

namespace Happy31
{
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
