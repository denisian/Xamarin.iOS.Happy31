//
//  UsersRepository.cs
//  Happy31.iOSApp
//
//  Copyright © 2017 Denis Klyucherov. All rights reserved.
//

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SQLite;

namespace Happy31
{
    /// <summary>
    /// Creating of the table "Users" acccording to the UsersModel. Actions with the table.
    /// </summary>
    public class UsersRepository
    {
        SQLiteAsyncConnection database;

        public string Message { get; private set; }

        public UsersRepository()
        {
            string databasePath = CreateSQLiteDatabase.GetDatabasePath();
            database = new SQLiteAsyncConnection(databasePath);
            database.CreateTableAsync<UsersModel>(); // Create table if it not exists
        }

        // Adding user into local database after successful login
        public async Task<int> AddUser(UsersModel user)
        {
            try
            {
                var checkExistedEmail = database.Table<UsersModel>().Where(table => table.Email == user.Email);

                // Check if email already exists
                if (checkExistedEmail.ToListAsync().Result.Any())
                {
                    Message = "Email already exists!";
                    return 0;
                }

                var result = await database.InsertAsync(user);
                return result;
            }
            catch (SQLiteException ex)
            {
                Message = ex.Message;
                return 0;
            }
        }

        // Get a user's row by its id
        public IEnumerable<UsersModel> GetUser(string id)
        {
            try
            {
                return database.Table<UsersModel>().Where(table => table.Id == id).ToListAsync().Result;

            }
            catch (SQLiteException ex)
            {
                Console.WriteLine(ex.Message);
                return Enumerable.Empty<UsersModel>();
            }
        }

        // Update a user
        public async Task UpdateUser(UsersModel user)
        {
            try
            {
                await database.UpdateAsync(user);
            }
            catch (SQLiteException ex)
            {
                Message = ex.Message;
            }
        }

        // Get the list of all users from the database
        public IEnumerable<UsersModel> GetAllUsers()
        {
            try
            {
                return database.Table<UsersModel>().ToListAsync().Result;
            }
            catch (SQLiteException ex)
            {
                Message = ex.Message;
                return Enumerable.Empty<UsersModel>();
            }
        }

        // Get a user password by its email
        public string GetUserPassword(string email)
        {
            try
            {
                var userRow = database.Table<UsersModel>().Where(table => table.Email == email);
                var userPassword = userRow.ToListAsync().Result.Select(table => table.Password);
                if (userPassword.Count() > 0)
                    return userPassword.First();
                else
                {
                    Message = "User is not found";
                    return "";
                }
            }
            catch (Exception ex)
            {
                Message = ex.Message;
                return "";
            }
        }
    }
}
