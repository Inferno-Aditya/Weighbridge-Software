using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using VinayakEnterprises.Core.Models;
using VinayakEnterprises.Data.Interfaces;

namespace VinayakEnterprises.Data.Repositories;

public class HelpCodeRepository : IHelpCodeRepository
{
    private readonly AppDbContext _context;

    public HelpCodeRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<HelpCode?> GetByIdAsync(int id)
    {
        using var connection = _context.CreateConnection();
        return await connection.QueryFirstOrDefaultAsync<HelpCode>(
            "SELECT * FROM HelpCodes WHERE Id = @Id AND IsDeleted = 0", new { Id = id });
    }

    public async Task<IEnumerable<HelpCode>> GetAllAsync()
    {
        using var connection = _context.CreateConnection();
        return await connection.QueryAsync<HelpCode>(
            "SELECT * FROM HelpCodes WHERE IsDeleted = 0");
    }

    public async Task<int> AddAsync(HelpCode entity)
    {
        using var connection = _context.CreateConnection();
        var sql = "INSERT INTO HelpCodes (EntityType, Code, Value) VALUES (@EntityType, @Code, @Value); SELECT last_insert_rowid();";
        var id = await connection.ExecuteScalarAsync<int>(sql, entity);
        entity.Id = id;
        return id;
    }

    public async Task<int> UpdateAsync(HelpCode entity)
    {
        using var connection = _context.CreateConnection();
        var sql = "UPDATE HelpCodes SET EntityType = @EntityType, Code = @Code, Value = @Value WHERE Id = @Id";
        return await connection.ExecuteAsync(sql, entity);
    }

    public async Task<int> DeleteAsync(int id)
    {
        using var connection = _context.CreateConnection();
        var sql = "UPDATE HelpCodes SET IsDeleted = 1 WHERE Id = @Id";
        return await connection.ExecuteAsync(sql, new { Id = id });
    }
}
