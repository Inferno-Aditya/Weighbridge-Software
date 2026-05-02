using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using VinayakEnterprises.Core.Models;
using VinayakEnterprises.Data.Interfaces;

namespace VinayakEnterprises.Data.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;

    public UserRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetByIdAsync(int id)
    {
        using var connection = _context.CreateConnection();
        return await connection.QueryFirstOrDefaultAsync<User>(
            "SELECT * FROM Users WHERE Id = @Id AND IsDeleted = 0", new { Id = id });
    }

    public async Task<IEnumerable<User>> GetAllAsync()
    {
        using var connection = _context.CreateConnection();
        return await connection.QueryAsync<User>(
            "SELECT * FROM Users WHERE IsDeleted = 0");
    }

    public async Task<User?> GetByUsernameAsync(string username)
    {
        using var connection = _context.CreateConnection();
        return await connection.QueryFirstOrDefaultAsync<User>(
            "SELECT * FROM Users WHERE Username = @Username AND IsDeleted = 0", new { Username = username });
    }

    public async Task<int> AddAsync(User entity)
    {
        using var connection = _context.CreateConnection();
        var sql = "INSERT INTO Users (Username, PasswordHash, RoleId, IsActive, ForcePasswordChange, IsLocked, FailedLoginAttempts) VALUES (@Username, @PasswordHash, @RoleId, @IsActive, @ForcePasswordChange, @IsLocked, @FailedLoginAttempts); SELECT last_insert_rowid();";
        var id = await connection.ExecuteScalarAsync<int>(sql, entity);
        entity.Id = id;
        return id;
    }

    public async Task<int> UpdateAsync(User entity)
    {
        using var connection = _context.CreateConnection();
        var sql = "UPDATE Users SET Username = @Username, PasswordHash = @PasswordHash, RoleId = @RoleId, IsActive = @IsActive, ForcePasswordChange = @ForcePasswordChange, IsLocked = @IsLocked, FailedLoginAttempts = @FailedLoginAttempts WHERE Id = @Id";
        return await connection.ExecuteAsync(sql, entity);
    }

    public async Task<int> DeleteAsync(int id)
    {
        using var connection = _context.CreateConnection();
        var sql = "UPDATE Users SET IsDeleted = 1 WHERE Id = @Id";
        return await connection.ExecuteAsync(sql, new { Id = id });
    }
}
