//
//  CreateSQLiteDatabase.cs
//  Happy31.iOSApp
//
//  Copyright © 2017 Denis Klyucherov. All rights reserved.
//

using System;
using System.IO;

namespace Happy31
{
    /// <summary>
    /// Path settings for the local database
    /// </summary>
    public static class CreateSQLiteDatabase
    {
        const string databaseName = "happy31.db";

        public static string GetDatabasePath()
        {
            string documentsFolder = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            string libraryFolder = Path.Combine(documentsFolder, "..", "Library"); // Library folder

            if (!Directory.Exists(libraryFolder))
                Directory.CreateDirectory(libraryFolder);

            var path = Path.Combine(libraryFolder, databaseName);

            return path;
        }
    }
}
