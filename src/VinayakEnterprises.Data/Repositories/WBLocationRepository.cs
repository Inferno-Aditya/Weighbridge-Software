using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using VinayakEnterprises.Core.Models;
using VinayakEnterprises.Data.Interfaces;

namespace VinayakEnterprises.Data.Repositories;

public class WBLocationRepository : IWBLocationRepository
{
    private readonly AppDbContext _context;

    public WBLocationRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<WBLocation?> GetByIdAsync(int id)
    {
        using var connection = _context.CreateConnection();
        return await connection.QueryFirstOrDefaultAsync<WBLocation>(
            "SELECT * FROM WBLocations WHERE Id = @Id AND IsDeleted = 0", new { Id = id });
    }

    public async Task<IEnumerable<WBLocation>> GetAllAsync()
    {
        using var connection = _context.CreateConnection();
        return await connection.QueryAsync<WBLocation>(
            "SELECT * FROM WBLocations WHERE IsDeleted = 0");
    }

    public async Task<int> AddAsync(WBLocation entity)
    {
        using var connection = _context.CreateConnection();
        var sql = "INSERT INTO WBLocations (LocationCode, LocationName) VALUES (@LocationCode, @LocationName); SELECT last_insert_rowid();";
        var id = await connection.ExecuteScalarAsync<int>(sql, entity);
        entity.Id = id;
        return id;
    }

    public async Task<int> UpdateAsync(WBLocation entity)
    {
        using var connection = _context.CreateConnection();
        var sql = "UPDATE WBLocations SET LocationCode = @LocationCode, LocationName = @LocationName WHERE Id = @Id";
        return await connection.ExecuteAsync(sql, entity);
    }

    public async Task<int> DeleteAsync(int id)
    {
        using var connection = _context.CreateConnection();
        var sql = "UPDATE WBLocations SET IsDeleted = 1 WHERE Id = @Id";
        return await connection.ExecuteAsync(sql, new { Id = id });
    }
}
