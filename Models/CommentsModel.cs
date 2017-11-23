using System;
using System.Collections.Generic;
using SQLite;
using SQLiteNetExtensions.Attributes;

namespace Happy31
{
    // Create the Plain Old CLR Object (POCO) class
    [Table("comments")]
    public class CommentsModel
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [NotNull]
        public string Body { get; set; }

        [NotNull]
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

        public DateTimeOffset UpdatedAt { get; set; }

        [ForeignKey(typeof(PostsModel))]
        public string PostId { get; }

        [OneToMany]
        public List<LikesModel> Likes { get; } // 1 to many relations
    }
}
