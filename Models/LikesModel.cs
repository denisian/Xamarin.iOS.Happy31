using SQLite;
using SQLiteNetExtensions.Attributes;

namespace Happy31
{
    // Create the Plain Old CLR Object (POCO) class
    [Table("likes")]
    public class LikesModel
    {
        [ForeignKey(typeof(UsersModel))]
        public string UserId { get; }

        [ForeignKey(typeof(PostsModel))]
        public string PostId { get; }

        [ForeignKey(typeof(CommentsModel))]
        public string CommentId { get; }
    }
}
