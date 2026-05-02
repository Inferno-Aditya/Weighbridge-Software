using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using VinayakEnterprises.Core.Models;
using VinayakEnterprises.Data.Interfaces;

namespace VinayakEnterprises.Data.Repositories;

public class AuditLogRepository : IAuditLogRepository
{
    private readonly AppDbContext _context;

    public AuditLogRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<AuditLog?> GetByIdAsync(int id)
    {
        using var connection = _context.CreateConnection();
        return await connection.QueryFirstOrDefaultAsync<AuditLog>(
            "SELECT * FROM AuditLogs WHERE Id = @Id AND IsDeleted = 0", new { Id = id });
    }

    public async Task<IEnumerable<AuditLog>> GetAllAsync()
    {
        using var connection = _context.CreateConnection();
        return await connection.QueryAsync<AuditLog>(
            "SELECT * FROM AuditLogs WHERE IsDeleted = 0");
    }

    public async Task<int> AddAsync(AuditLog entity)
    {
        using var connection = _context.CreateConnection();
        var sql = "INSERT INTO AuditLogs (Timestamp, UserId, UserName, Action, EntityType, EntityId, OldValue, NewValue, IPAddress, PCName) VALUES (@Timestamp, @UserId, @UserName, @Action, @EntityType, @EntityId, @OldValue, @NewValue, @IPAddress, @PCName); SELECT last_insert_rowid();";
        var id = await connection.ExecuteScalarAsync<int>(sql, entity);
        entity.Id = id;
        return id;
    }

    public async Task<int> UpdateAsync(AuditLog entity)
    {
        using var connection = _context.CreateConnection();
        var sql = "UPDATE AuditLogs SET Timestamp = @Timestamp, UserId = @UserId, UserName = @UserName, Action = @Action, EntityType = @EntityType, EntityId = @EntityId, OldValue = @OldValue, NewValue = @NewValue, IPAddress = @IPAddress, PCName = @PCName WHERE Id = @Id";
        return await connection.ExecuteAsync(sql, entity);
    }

    public async Task<int> DeleteAsync(int id)
    {
        using var connection = _context.CreateConnection();
        var sql = "UPDATE AuditLogs SET IsDeleted = 1 WHERE Id = @Id";
        return await connection.ExecuteAsync(sql, new { Id = id });
    }
}
