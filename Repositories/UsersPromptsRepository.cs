//
//  UsersPromptsRepository.cs
//  Happy31.iOSApp
//
//  Copyright © 2017 Denis Klyucherov. All rights reserved.
//

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Foundation;
using SQLite;

namespace Happy31
{
    /// <summary>
    /// Creating of the table "UsersPrompts" acccording to the UsersPromptsModel. Actions with the table.
    /// </summary>
    public class UsersPromptsRepository
    {
        SQLiteAsyncConnection database;
        RestService restService;

        public string Message { get; private set; }

        public UsersPromptsRepository()
        {
            string databasePath = CreateSQLiteDatabase.GetDatabasePath();
            database = new SQLiteAsyncConnection(databasePath);
            database.CreateTableAsync<UsersPromptsModel>(); // Create table if it not exists
        }

        // Generating a new prompt and inserting into table "users_prompts"
        // Return last generated prompt
        public IEnumerable<DisplayedPromptsModel> GenerateNewPrompt(string userId)
        {
            // Getting random promptId
            int promptId = GetCurrentPromptId(userId);

            if (promptId == 0)
            {
                Console.WriteLine("There are no prompts available");
                return Enumerable.Empty<DisplayedPromptsModel>();
            }

            // Insert current prompt into table "users_prompts"
            int res = InsertCurrentPromptIntoTable(new UsersPromptsModel()
            {
                UserId = userId,
                PromptId = promptId,
                CreatedAt = DateTimeOffset.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                IsDone = "False"
            });

            if (res == 0)
            {
                Console.WriteLine("There was a problem when insert prompt");
                return Enumerable.Empty<DisplayedPromptsModel>();;
            }

            // Get last prompt
            var getLastPrompt = DisplayUserPrompts(userId).OrderByDescending(date => date.CreatedAt);
            return getLastPrompt;
        }

        // After first login, get full table "user_prompts" from remote database and insert it in local database
        public async Task<string> SyncAllTableUserPrompts(string userId)
        {
            IEnumerable<UsersPromptsModel> userPromptsToSync;

            // Firstly, check if there are any data in "users_prompts" table for the current user
            // If there are no any data (could be user just registered or logged in from another device), send request "sync_all" to server to check if there are any saved prompts
            // If server contains any prompts, get them
            var data = DisplayUserPrompts(userId);

            if (data.Any())
                TruncateTableUsersPrompts();
                //return;

            // If local table "user-prompts" is empty, send request "sync_all" to server
            userPromptsToSync = new UsersPromptsModel[] { new UsersPromptsModel() { UserId = userId, IsSync = "sync_all" } };

            // Send Json and get response from remote server
            var jsonResponse = await RestService(userPromptsToSync);

            // If there was an error in PostJsonDataAsync class, display message
            if (jsonResponse == null)
            {
                Message = restService.Message;
                Console.WriteLine(Message);
                return Message;
            }

            // Insert prompts from server into table "users_prompts"
            foreach (var item in jsonResponse)
            {
                if (item.PromptId == 0)
                {
                    Message = "There is no data on the server to sync";
                    Console.WriteLine(Message);
                    return Message;
                }

                int res = InsertCurrentPromptIntoTable(new UsersPromptsModel()
                {
                    UserId = item.UserId,
                    PromptId = item.PromptId,
                    CreatedAt = item.CreatedAt,
                    IsDone = item.IsDone,
                    IsSync = "True" // Change attrubute "is_sync=true" for the new users promts received from the remote server
                });
                if (res == 0)
                {
                    Message = "There was a problem when inserting prompt id=" + item.UserPromptId;
                    Console.WriteLine(Message);
                    return Message;
                }
            }

            return Message = "Prompts have been synchronised successfully";
        }

        // This method starts after user gets prompts and/or there is an internect connection
        // Retrieve prompts from local table "users_prompts" with attrubute "is_sync=False" - that is prompts, which haven't been synchronized with remote server
        // Send these prompts to the remote server, insert them and get response from server for every prompt if they were inserted without errors
        // If for the prompt there was not any error, update the current prompt, changing attribute "is_sync=True"
        public async Task SyncCurrentUserPrompts(string userId)
        {
            IEnumerable<UsersPromptsModel> userPromptsToSync;

            // Retrieve not sync prompts from local database
            userPromptsToSync = RetrieveNotSyncPromptsFromLocalDb(userId);

            if (!userPromptsToSync.Any())
            {
                Console.WriteLine("No data to sync");
                return;
            }

            // Send Json and get response from remote server
            var jsonResponse = await RestService(userPromptsToSync);

            // If there was an error in PostJsonDataAsync class, display message
            if (jsonResponse == null)
            {
                Console.WriteLine(restService.Message);
                return;
            }

            foreach (var status in jsonResponse) // Loop through prompts statuses received from server
            {
                if (status.UserId != userId) // Check if response has been received from the same requested user 
                {
                    Console.WriteLine("Requested and responsed userId are not the same!");
                    return;
                }

                string currentSyncStatusFromServer = status.IsSync;
                if (currentSyncStatusFromServer == "True") // If promt has been successfully inserted in the table on server successfully
                    foreach (var item in userPromptsToSync) // Loop through prompts
                        if (item.UserPromptId == status.UserPromptId) // Check id between prompt and hos status
                            UpdateUserPrompt(new UsersPromptsModel() // Update the current prompt (set "is_sync=True")
                            {
                                UserPromptId = item.UserPromptId,
                                UserId = item.UserId,
                                PromptId = item.PromptId,
                                CreatedAt = item.CreatedAt,
                                IsDone = item.IsDone,
                                IsSync = "True"
                            });
            }
        }

        // Call REST service to send Json data
        async Task<List<UsersPromptsModel>> RestService(IEnumerable<UsersPromptsModel> userPromptsToSync)
        {
            restService = new RestService();

            // Get Json data from server in JsonResponseModel format
            Task<List<UsersPromptsModel>> jsonResponeTask = restService.SyncUserPromptsJson(userPromptsToSync);

            Console.WriteLine(restService.Message);

            // Create instance of jsonResponeTask and pass jsonResponeTask there
            return await jsonResponeTask;
        }

        // Getting random prompt from each category by user_id
        // Creating temporary table to keep there prompts id which haven't been showed to user for the last 24 hours (from different categories)
        public int GetCurrentPromptId(string userId)
        {
            try
            {
                // Get user's settings
                var plist = NSUserDefaults.StandardUserDefaults;
                string promptStartTime = plist.StringForKey("promptStartTime");
                string promptEndTime = plist.StringForKey("promptEndTime");
                string promptMinInterval = plist.StringForKey("promptMinInterval");  // Interval in hours between prompts

                string sqlTmpTable = "tmp_displayed_prompts";
                int promptEveryDayPeriodTime = 24; // Three prompts from different categories will be shown within 24 hours

                string promptStartTimeToDisplay = DateTime.Parse(promptStartTime).ToString("HHmm"); // Start time of the interval to dsiplay a prompt
                string promptEndTimeToDisplay = DateTime.Parse(promptEndTime).ToString("HHmm"); // End time of the interval to dsiplay a prompt

                // Convert 12:00 AM to 24:00 to correct calculation 
                if (promptStartTimeToDisplay == "0000" )
                    promptStartTimeToDisplay = "2400";

                if (promptEndTimeToDisplay == "0000")
                    promptEndTimeToDisplay = "2400";

                string sqlCreateTempTableQuery =
                    $"CREATE TEMP TABLE IF NOT EXISTS {sqlTmpTable} AS " +
                        $"SELECT id FROM prompts WHERE id NOT IN (SELECT PromptId from users_prompts) " +
                        $"AND CAST(strftime('%H%M', 'now', 'localtime') as integer) BETWEEN {promptStartTimeToDisplay} AND {promptEndTimeToDisplay} " +
                            "AND Category NOT IN " +
                                "(SELECT Category from prompts AS t1, users_prompts AS t2 " +
                    $"WHERE t1.id = t2.PromptId AND CAST((julianday(datetime('now'), 'localtime') - julianday(CreatedAt)) * 24 AS integer) < {promptEveryDayPeriodTime} AND t2.UserId = '{userId}');";

                string sqlSelectRandomPromptQuery =
                    $"SELECT id FROM {sqlTmpTable} WHERE id IN (SELECT id FROM {sqlTmpTable} ORDER BY RANDOM() LIMIT 1) " +
                    $"AND NOT EXISTS (SELECT 'true' from users_prompts WHERE CAST((julianday(datetime('now'), 'localtime') - julianday(CreatedAt)) * 24 AS integer) < {promptMinInterval})";

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
        public List<DisplayedPromptsModel> DisplayUserPrompts(string userId)
        {
            try
            {
                string sqlGetUsersPrompts = $"SELECT t2.UserPromptId, t2.CreatedAt, t1.Category, t1.Task, t2.IsDone from prompts AS t1, users_prompts AS t2 WHERE t2.UserId = '{userId}' AND t1.Id = t2.PromptId ORDER BY CreatedAt DESC";
                return database.QueryAsync<DisplayedPromptsModel>(sqlGetUsersPrompts).Result;
            }
            catch (SQLiteException ex)
            {
                Message = ex.Message;
                return null;
            }
        }

        // Display user's progress
        public List<ProgressModel> DisplayUserProgress(string userId)
        {
            try
            {
                string sqlGetUserProgress = $"SELECT t1.Category, count(CASE t2.IsDone WHEN 'True' THEN t2.IsDone END) AS 'DoneAmount', count(CASE t2.IsDone WHEN 'False' THEN t2.IsDone END) AS 'UndoneAmount' from prompts AS t1, users_prompts AS t2 WHERE t2.UserId = '{userId}' AND t1.Id = t2.PromptId group by t1.Category";
                return database.QueryAsync<ProgressModel>(sqlGetUserProgress).Result;
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
                return Enumerable.Empty<UsersPromptsModel>();
            }
        }

        // Update user prompt
        public void UpdateUserPrompt(UsersPromptsModel userPrompt)
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

        // Update IsDone field in users_prompt table
        // After updating isDone field update sync status to sync with server
        public int UpdateIsDoneUserPrompt(int userPromptId, string isDone)
        {
            try
            {
                string sqlUpdateIsDoneUsersPrompts = $"UPDATE users_prompts SET IsDone = '{isDone}', IsSync = 'False' WHERE UserPromptId = {userPromptId}";
                return database.ExecuteScalarAsync<int>(sqlUpdateIsDoneUsersPrompts).Result;
            }
            catch (SQLiteException ex)
            {
                Message = ex.Message;
                return -1;
            }
        }

        public void TruncateTableUsersPrompts()
        {
            try
            {
                string sqlTruncateTableUsersPrompts = $"DELETE FROM users_prompts";
                database.ExecuteScalarAsync<int>(sqlTruncateTableUsersPrompts);
            }
            catch (SQLiteException ex)
            {
                Message = ex.Message;
            }
        }
    }
}