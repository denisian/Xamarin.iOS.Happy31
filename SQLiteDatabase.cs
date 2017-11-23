using System;
using System.IO;

namespace Happy31
{
    public static class SQLiteDatabase
    {
        const string databaseName = "happy31.db";
        //public static UsersRepository _database;
        //public static UsersRepository Database
        //{
        //    get
        //    {
        //        if (_database == null)
        //        {
        //            _database = new UsersRepository(databaseName);
        //        }
        //        return _database;
        //    }
        //}

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
