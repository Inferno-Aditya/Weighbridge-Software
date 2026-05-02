using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using VinayakEnterprises.Core.Models;
using VinayakEnterprises.Data.Interfaces;

namespace VinayakEnterprises.Data.Repositories;

public class SystemSettingsRepository : ISystemSettingsRepository
{
    private readonly AppDbContext _context;

    public SystemSettingsRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<SystemSettings?> GetByIdAsync(int id)
    {
        using var connection = _context.CreateConnection();
        return await connection.QueryFirstOrDefaultAsync<SystemSettings>(
            "SELECT * FROM SystemSettings WHERE Id = @Id", new { Id = id });
    }

    public async Task<IEnumerable<SystemSettings>> GetAllAsync()
    {
        using var connection = _context.CreateConnection();
        return await connection.QueryAsync<SystemSettings>(
            "SELECT * FROM SystemSettings");
    }

    public async Task<int> AddAsync(SystemSettings entity)
    {
        using var connection = _context.CreateConnection();
        var sql = "INSERT INTO SystemSettings (Theme, Language, CameraIndex, DefaultPrinter, SessionTimeout, StableWeightThreshold) VALUES (@Theme, @Language, @CameraIndex, @DefaultPrinter, @SessionTimeout, @StableWeightThreshold); SELECT last_insert_rowid();";
        var id = await connection.ExecuteScalarAsync<int>(sql, entity);
        entity.Id = id;
        return id;
    }

    public async Task<int> UpdateAsync(SystemSettings entity)
    {
        using var connection = _context.CreateConnection();
        var sql = "UPDATE SystemSettings SET Theme = @Theme, Language = @Language, CameraIndex = @CameraIndex, DefaultPrinter = @DefaultPrinter, SessionTimeout = @SessionTimeout, StableWeightThreshold = @StableWeightThreshold WHERE Id = @Id";
        return await connection.ExecuteAsync(sql, entity);
    }

    public async Task<int> DeleteAsync(int id)
    {
        using var connection = _context.CreateConnection();
        var sql = "DELETE FROM SystemSettings WHERE Id = @Id";
        return await connection.ExecuteAsync(sql, new { Id = id });
    }
}
