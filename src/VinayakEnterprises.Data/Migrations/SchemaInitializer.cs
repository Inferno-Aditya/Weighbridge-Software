using System;
using System.IO;
using Dapper;

namespace VinayakEnterprises.Data.Migrations;

public class SchemaInitializer
{
    private readonly AppDbContext _context;

    public SchemaInitializer(AppDbContext context)
    {
        _context = context;
    }

    public void Initialize()
    {
        using var connection = _context.CreateConnection();
        connection.Open();

        var sql = @"
            CREATE TABLE IF NOT EXISTS Users (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Username TEXT NOT NULL,
                PasswordHash TEXT NOT NULL,
                RoleId INTEGER NOT NULL,
                IsActive BOOLEAN DEFAULT 1,
                ForcePasswordChange BOOLEAN DEFAULT 1,
                IsLocked BOOLEAN DEFAULT 0,
                FailedLoginAttempts INTEGER DEFAULT 0,
                IsDeleted BOOLEAN DEFAULT 0
            );

            CREATE TABLE IF NOT EXISTS Roles (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Name TEXT NOT NULL,
                IsDeleted BOOLEAN DEFAULT 0
            );

            CREATE TABLE IF NOT EXISTS Customers (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                CodeNo TEXT,
                Name TEXT NOT NULL,
                Address TEXT,
                City TEXT,
                Phone TEXT,
                Mobile TEXT,
                VATNo TEXT,
                Email TEXT,
                W_Charges TEXT,
                RateType TEXT,
                GSTNo TEXT,
                IsBlacklist BOOLEAN DEFAULT 0,
                IsDeleted BOOLEAN DEFAULT 0
            );

            CREATE TABLE IF NOT EXISTS Suppliers (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                CodeNo TEXT,
                Name TEXT NOT NULL,
                Address TEXT,
                City TEXT,
                Phone TEXT,
                Mobile TEXT,
                VATNo TEXT,
                Email TEXT,
                W_Charges TEXT,
                Website TEXT,
                IsBlacklist BOOLEAN DEFAULT 0,
                IsDeleted BOOLEAN DEFAULT 0
            );

            CREATE TABLE IF NOT EXISTS Items (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                CodeNo TEXT,
                Name TEXT NOT NULL,
                Price DECIMAL,
                Weight DECIMAL,
                Unit TEXT,
                IsDeleted BOOLEAN DEFAULT 0
            );

            CREATE TABLE IF NOT EXISTS Vehicles (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                VehicleNo TEXT NOT NULL,
                TareWtKg INTEGER,
                RTOWtKg INTEGER,
                TareDate DATETIME,
                MaxTareAllow INTEGER,
                MinTareAllow INTEGER,
                IsBlacklist BOOLEAN DEFAULT 0,
                IsDeleted BOOLEAN DEFAULT 0
            );

            CREATE TABLE IF NOT EXISTS Field01 (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                CodeNo TEXT,
                Name TEXT NOT NULL,
                Remarks TEXT,
                IsDeleted BOOLEAN DEFAULT 0
            );

            CREATE TABLE IF NOT EXISTS Field02 (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                CodeNo TEXT,
                Name TEXT NOT NULL,
                Remarks TEXT,
                IsDeleted BOOLEAN DEFAULT 0
            );

            CREATE TABLE IF NOT EXISTS Field03 (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                CodeNo TEXT,
                Name TEXT NOT NULL,
                Remarks TEXT,
                IsDeleted BOOLEAN DEFAULT 0
            );

            CREATE TABLE IF NOT EXISTS WBLocations (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                LocationCode TEXT,
                LocationName TEXT NOT NULL,
                IsDeleted BOOLEAN DEFAULT 0
            );

            CREATE TABLE IF NOT EXISTS HelpCodes (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                EntityType TEXT NOT NULL,
                Code TEXT NOT NULL,
                Value TEXT NOT NULL,
                IsDeleted BOOLEAN DEFAULT 0
            );

            CREATE TABLE IF NOT EXISTS GoodsDispatch (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                PartyName TEXT NOT NULL,
                VehicleNo TEXT NOT NULL,
                TicketNo TEXT NOT NULL,
                Gross DECIMAL,
                Tare DECIMAL,
                Net DECIMAL,
                Item TEXT,
                IsDeleted BOOLEAN DEFAULT 0
            );

            CREATE TABLE IF NOT EXISTS AuditLogs (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Timestamp DATETIME NOT NULL,
                UserId INTEGER,
                UserName TEXT,
                Action TEXT NOT NULL,
                EntityType TEXT NOT NULL,
                EntityId TEXT,
                OldValue TEXT,
                NewValue TEXT,
                IPAddress TEXT,
                PCName TEXT,
                IsDeleted BOOLEAN DEFAULT 0
            );

            CREATE TABLE IF NOT EXISTS SlipEntries (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                TicketNo TEXT NOT NULL,
                CustomerId INTEGER,
                SupplierId INTEGER,
                VehicleNo TEXT NOT NULL,
                ItemId INTEGER,
                OperatorId INTEGER,
                WBLocation TEXT,
                GrossWt INTEGER,
                TareWt INTEGER,
                NetWt INTEGER,
                GrossTime DATETIME,
                TareTime DATETIME,
                CameraImagePath TEXT,
                Field01Id INTEGER,
                Field02Id INTEGER,
                Field03Id INTEGER,
                TicketStatus TEXT DEFAULT 'New',
                WeighmentNo INTEGER DEFAULT 1,
                ManualData BOOLEAN DEFAULT 0,
                CreatedAt DATETIME NOT NULL,
                UpdatedAt DATETIME,
                IsDeleted BOOLEAN DEFAULT 0
            );

            CREATE INDEX IF NOT EXISTS IX_SlipEntries_GrossTime ON SlipEntries(GrossTime);
            CREATE INDEX IF NOT EXISTS IX_SlipEntries_VehicleNo ON SlipEntries(VehicleNo);
            CREATE INDEX IF NOT EXISTS IX_SlipEntries_TicketNo ON SlipEntries(TicketNo);

            -- System tables (No IsDeleted)
            CREATE TABLE IF NOT EXISTS CompanyMaster (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Name TEXT NOT NULL,
                Address TEXT,
                LogoPath TEXT,
                GSTNo TEXT,
                Phone TEXT
            );

            CREATE TABLE IF NOT EXISTS SystemSettings (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Theme TEXT DEFAULT 'dark',
                Language TEXT DEFAULT 'EN',
                CameraIndex INTEGER DEFAULT 0,
                DefaultPrinter TEXT,
                SessionTimeout INTEGER DEFAULT 30,
                StableWeightThreshold INTEGER DEFAULT 2
            );

            CREATE TABLE IF NOT EXISTS LicenseInfo (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                MachineId TEXT NOT NULL,
                LicenseType TEXT NOT NULL,
                IssueDate DATETIME NOT NULL,
                ExpiryDate DATETIME NOT NULL,
                ActivatedBy TEXT,
                ActivationDate DATETIME
            );
        ";

        connection.Execute(sql);

        // Seed Roles
        var rolesCount = connection.ExecuteScalar<int>("SELECT COUNT(*) FROM Roles");
        if (rolesCount == 0)
        {
            connection.Execute("INSERT INTO Roles (Name) VALUES ('Operator'), ('Technician'), ('Owner')");
        }

        // Seed Default Admin User
        var usersCount = connection.ExecuteScalar<int>("SELECT COUNT(*) FROM Users");
        if (usersCount == 0)
        {
            var ownerRoleId = connection.ExecuteScalar<int>("SELECT Id FROM Roles WHERE Name = 'Owner'");
            // Hash for 'admin123' using BCrypt
            var admin123Hash = "$2a$11$tHN2BcTu1/FQKa4JQav1TuIofBKtzqG9tQ6E/iW9FJZc1hP00WkzK"; 
            connection.Execute(@"
                INSERT INTO Users (Username, PasswordHash, RoleId, IsActive, ForcePasswordChange, IsLocked, FailedLoginAttempts) 
                VALUES ('admin', @Hash, @RoleId, 1, 1, 0, 0)", 
                new { Hash = admin123Hash, RoleId = ownerRoleId });
        }
    }
}
