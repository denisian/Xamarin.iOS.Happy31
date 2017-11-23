using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using SQLite;
using SQLiteNetExtensions.Attributes;

namespace Happy31
{
    // Create the Plain Old CLR Object (POCO) class
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
        [JsonProperty("last_name")]
        public string LastName { get; set; }

        //[NotNull, Unique]
        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("password")]
        public string Password { get; set; }

        //[Unique]
        public string EmailActivationToken { get; set; } // long 41
       
        public bool IsActivated { get; set; } // = false; // Activated account

        public bool IsSuspended { get; set; } // = false; // Suspended account

        public string Avatar { get; set; }

        //[NotNull]
        [JsonProperty("provider")]
        public string LoginProvider { get; set; } // Email/Facebook

        //======= NOT SURE

        //public string ProviderId { get; set; } // ??

        //public string ProviderAccessToken { get; set; }
        //=================

        public DateTimeOffset CreatedAt { get; set; } //= DateTimeOffset.UtcNow.ToUniversalTime();
                       
        [OneToMany]
        public List<PostsModel> Posts { get; } // 1 to many relations

        [OneToMany]
        public List<LikesModel> Likes { get; }  // 1 to many relations

        [OneToMany]
        public List<PermissionsModel> Permissions { get; } // 1 to many relations
    }
}
