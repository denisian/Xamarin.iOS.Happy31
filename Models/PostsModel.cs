using System;
using System.Collections.Generic;
using SQLite;
using SQLiteNetExtensions.Attributes;

namespace Happy31
{
    // Create the Plain Old CLR Object (POCO) class
    [Table("posts")]
    public class PostsModel
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [NotNull]
        public string Title { get; set; }

        [NotNull]
        public string Body { get; set; }

        public string Image { get; set; } // ??

        [NotNull]
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

        public DateTimeOffset UpdatedAt { get; set; }

        [ForeignKey(typeof(UsersModel))]
        public string UserId { get; }

        [ForeignKey(typeof(PostsModel))]
        public string PostId { get; }

        [OneToMany]
        public List<CommentsModel> Comments { get; } // 1 to many relations

        [OneToMany]
        public List<LikesModel> Likes { get; } // 1 to many relations
    }
}