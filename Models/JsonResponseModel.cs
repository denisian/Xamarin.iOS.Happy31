using Newtonsoft.Json;

namespace Happy31
{
    // Json model to get messages from server using Json and display them to user
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
