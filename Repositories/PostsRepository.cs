using System;
using SQLite;

namespace Happy31
{
    public class PostsRepository
    {
        SQLiteAsyncConnection database;

        public string Message { get; private set; }

        public PostsRepository()
        {
            string databasePath = SQLiteDatabase.GetDatabasePath();
            database = new SQLiteAsyncConnection(databasePath);
            database.CreateTableAsync<PostsModel>(); // Create table if it not exists
        }

        // Add a post
        public string AddPost(PostsModel post)
        {
            try
            {
                var postId = database.InsertAsync(post).ToString();
                Message = "Registration complete!";
                return postId;
            }
            catch (Exception ex)
            {
                Message = ex.Message;
                return "";
            }
        }
    }
}
