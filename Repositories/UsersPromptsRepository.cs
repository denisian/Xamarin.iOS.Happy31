using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SQLite;

namespace Happy31
{
    public class UsersPromptsRepository
    {
        SQLiteAsyncConnection database;

        public string Message { get; private set; }

        public UsersPromptsRepository()
        {
            string databasePath = SQLiteDatabase.GetDatabasePath();
            database = new SQLiteAsyncConnection(databasePath);
            database.CreateTableAsync<UsersPromptsModel>(); // Create table if it not exists
        }

        // Getting random prompt from each category by user_id
        // Creating temporary table to keep there prompts id which haven't been showed to user for the last 24 hours (from different categories)
        public int GetCurrentPromptId(string userId)
        {
            try
            {
                string sqlTmpTable = "tmp_displayed_prompts";
                int promptPeriodTime = 1; // Hours

                string sqlCreateTempTableQuery =
                    $"CREATE TEMP TABLE IF NOT EXISTS {sqlTmpTable} AS " +
                        "SELECT id FROM prompts WHERE Category NOT IN " +
                            "(SELECT Category from prompts AS t1, users_prompts AS t2 " +
                    $"WHERE t1.id = t2.PromptId AND CAST((julianday(datetime('now'), 'localtime') - julianday(CreatedAt)) * 24 AS integer) < {promptPeriodTime} AND t2.UserId = '{userId}');";
                
                string sqlSelectRandomPromptQuery = $"SELECT id FROM {sqlTmpTable} WHERE id IN (SELECT id FROM {sqlTmpTable} ORDER BY RANDOM() LIMIT 1);";

                string sqlDropTempTableQuery = $"DROP TABLE IF EXISTS {sqlTmpTable};";

                database.ExecuteAsync(sqlCreateTempTableQuery).Wait();
                int promptId = database.ExecuteScalarAsync<int>(sqlSelectRandomPromptQuery).Result;
                database.ExecuteAsync(sqlDropTempTableQuery);

                return promptId;
            }
            catch (SQLiteException ex)
            {
                Console.WriteLine(ex.Message);
                return 0;
            }
        }

        // Insert current prompt and get number of added rows
        public int InsertCurrentPromptIntoTable(UsersPromptsModel userPrompt)
        {
            try
            {
                string sqlGetLastId = "SELECT * FROM users_prompts ORDER BY userpromptid DESC LIMIT 1";
                int lastId = database.ExecuteScalarAsync<int>(sqlGetLastId).Result;

                if (lastId == 0)
                    userPrompt.UserPromptId = 1;
                else
                    userPrompt.UserPromptId = lastId + 1;

                return database.InsertAsync(userPrompt).Result;
            }
            catch (SQLiteException ex)
            {
                Console.WriteLine(ex.Message);
                return 0;
            }
        }

        // Display user's prompts in View Controller
        public IEnumerable<UsersPromptsModel> DisplayUserPrompts(string userId)
        {
            try
            {
                return database.Table<UsersPromptsModel>().Where(table => table.UserId == userId).ToListAsync().Result;
            }
            catch (SQLiteException ex)
            {
                Message = ex.Message;
                return null;
            }
        }

        // Retrieving not sync prompts from local database
        public IEnumerable<UsersPromptsModel> RetrieveNotSyncPromptsFromLocalDb(string userId)
        {
            try
            {
                return database.Table<UsersPromptsModel>().Where(table => table.UserId == userId && table.IsSync == "False").ToListAsync().Result;
            }
            catch (SQLiteException ex)
            {
                Message = ex.Message;
                return null;
            }
        }

        // Update user prompt status (sync/not sync) after sync with server
        public void UpdateUserPromptSyncStatus(UsersPromptsModel userPrompt)
        {
            try
            {
                database.UpdateAsync(userPrompt);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        // Get Table Prompts from remote database and creating new one in local database
        //public async void RetrieveUserPromptsFromServer(UsersPromptsModel userPrompt)
        //{
        //    try
        //    {
        //        // Call REST service to send Json data
        //        RestService rs = new RestService();

        //        // Get Json data from server in JsonResponseModel format
        //        Task<UsersPromptsModel> jsonResponeTask = rs.SyncUserPromptsJson(userPrompt);

        //        // Create instance of jsonResponeTask and pass jsonResponeTask there
        //        var jsonResponse = await jsonResponeTask;

        //        // If there was an error in PostJsonDataAsync class, display message
        //        if (jsonResponse == null)
        //        {
        //            Message = "Something went wrong:" + rs.Message;
        //            return;
        //        }

        //        jsonResponse.UserId = userPrompt.UserId;

        //        Console.WriteLine(rs.Message);

        //        // Insert current prompt in database if received promptId
        //        //if (!string.IsNullOrEmpty(jsonResponse.PromptId))
        //         //   await database.InsertAsync(jsonResponse);
        //    }
        //    catch (Exception ex)
        //    {
        //        Message = ex.Message;
        //    }
        //}


        public void RemoveTableUsersPrompts()
        {
            try
            {
                database.DropTableAsync<UsersPromptsModel>();
            }
            catch (Exception ex)
            {
                Message = ex.Message;
            }
        }
    }
}
