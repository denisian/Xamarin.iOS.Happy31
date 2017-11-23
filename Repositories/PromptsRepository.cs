using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SQLite;

namespace Happy31
{
    public class PromptsRepository
    {
        SQLiteAsyncConnection database;

        public string Message { get; private set; }

        public PromptsRepository()
        {
            string databasePath = SQLiteDatabase.GetDatabasePath();
            database = new SQLiteAsyncConnection(databasePath);
            database.CreateTableAsync<PromptsModel>(); // Create table if it not exists
        }

        // Get Table Prompts from remote database and creating new one in local database
        public async void GetTablePrompts()
        {
            try
            {
                // Call REST service to send Json data
                RestService rs = new RestService();

                // Get table Prompts Json data from server
                Task<List<PromptsModel>> jsonRetrievePrompts = rs.RetrieveTablePromptsJson();

                // If there was an error in PostJsonDataAsync class, display message
                if (jsonRetrievePrompts == null)
                {
                    Message = "Something went wrong:" + rs.Message;
                    return;
                }

                // Create instance of JsonResponseModel and pass jsonResponeTask there
                var jsonResponse = await jsonRetrievePrompts;

                //Task dropTable = Task.Factory.StartNew(() => database.DropTableAsync<PromptsModel>());

                //dropTable.Wait();

                // Insert list of prompts in database
                //if (dropTable.IsCompleted)
                    await database.InsertAllAsync(jsonResponse);

                Console.WriteLine(rs.Message);
            }
            catch (Exception ex)
            {
                Message = ex.Message;
            }
        }

        // Get prompt by id
        public PromptsModel GetPrompt(int id)
        {
            try
            {
                return database.GetAsync<PromptsModel>(id).Result;
            }
            catch (Exception ex)
            {
                Message = ex.Message;
                return null;
            }
        }
    }
}
