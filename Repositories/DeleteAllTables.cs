//
//  DeleteAllTables.cs
//  Happy31.iOSApp
//
//  Copyright © 2017 Denis Klyucherov. All rights reserved.
//

using System;
using SQLite;

namespace Happy31
{
    /// <summary>
    /// Delete all table in the local database after the user log out
    /// </summary>
    // 
    public class DeleteAllTables
    {
        SQLiteAsyncConnection database;

        public DeleteAllTables()
        {
            string databasePath = CreateSQLiteDatabase.GetDatabasePath();
            database = new SQLiteAsyncConnection(databasePath);
        }

        class DbTables
        {
            [Column("name")]
            public string Table { get; set; }
        }

        public async void DeleteTables()
        {
            try
            {
                string sqlGetAllTables = "SELECT name FROM sqlite_master WHERE type='table'";
                var res = database.QueryAsync<DbTables>(sqlGetAllTables).Result;

                foreach (var row in res)
                   await database.ExecuteAsync($"DROP TABLE IF EXISTS {row.Table}");

                SQLiteAsyncConnection.ResetPool(); // Dispose all of the connections sitting in the pool
            }
            catch (SQLiteException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
