 [Table("users")]
    public class UsersModel
    {
        //[PrimaryKey, Unique, JsonProperty("id")]
        [JsonProperty("user_id")]
        public string Id { get; set; } //= Guid.NewGuid().ToString();

        //[NotNull]
        [JsonProperty("first_name")]
        public string FirstName { get; set; }

        //[NotNull]