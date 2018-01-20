//
//  UsersModel.cs
//  Happy31.iOSApp
//
//  Copyright © 2017 Denis Klyucherov. All rights reserved.
//

using Newtonsoft.Json;
using SQLite;

namespace Happy31
{
    /// <summary>
    /// Model for the table "Users" in the local database
    /// </summary>
    [Table("users")]
    public class UsersModel
    {
        [Ignore]
        [JsonProperty("status")]
        public string Status { get; set; }

        [Ignore]
        [JsonProperty("message")]
        public string Message { get; set; }

        [PrimaryKey, Unique]
        [JsonProperty("user_id")]
        public string Id { get; set; }

        [NotNull]
        [JsonProperty("first_name")]
        public string FirstName { get; set; }

        [NotNull]
        [JsonProperty("last_name")]
        public string LastName { get; set; }

        [Unique]
        [JsonProperty("email")]
        public string Email { get; set; }

        [Ignore]
        [JsonProperty("password")]
        public string Password { get; set; }

        [JsonProperty("avatar")]
        public byte[] Avatar { get; set; }

        [JsonProperty("provider")]
        public string LoginProvider { get; set; } // Email/Facebook

        [JsonProperty("created_at"), NotNull]
        public string CreatedAt { get; set; }
    }
}