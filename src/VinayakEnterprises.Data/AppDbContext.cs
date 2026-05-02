using System;
using System.Data;
using System.IO;
using Microsoft.Data.Sqlite;

namespace VinayakEnterprises.Data;

public class AppDbContext
{
    private readonly string _connectionString;

    public AppDbContext()
    {
        var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        var appFolder = Path.Combine(appData, "VinayakEnterprises");
        
        if (!Directory.Exists(appFolder))
        {
            Directory.CreateDirectory(appFolder);
        }

        var dbPath = Path.Combine(appFolder, "vinayak.db");
        _connectionString = $"Data Source={dbPath};";
    }

    public IDbConnection CreateConnection()
    {
        return new SqliteConnection(_connectionString);
    }
}
