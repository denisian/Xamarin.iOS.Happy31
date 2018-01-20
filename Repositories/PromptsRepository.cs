//
//  PromptsRepository.cs
//  Happy31.iOSApp
//
//  Copyright © 2017 Denis Klyucherov. All rights reserved.
//

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SQLite;

namespace Happy31
{
    /// <summary>
    /// Creating of the table "Prompts" acccording to the PromptsModel. Actions with the table.
    /// </summary>
    public class PromptsRepository
    {
        SQLiteAsyncConnection database;

        public string Message { get; private set; }

        public PromptsRepository()
        {
            string databasePath = CreateSQLiteDatabase.GetDatabasePath();
            database = new SQLiteAsyncConnection(databasePath);
            database.CreateTableAsync<PromptsModel>(); // Create table if it not exists
        }

        // Get Table Prompts from remote database and creating new one in local database
        public async Task GetTablePrompts()
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
                    Message = rs.Message;
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
            catch (SQLiteException ex)
            {
                Console.WriteLine(ex.Message);
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