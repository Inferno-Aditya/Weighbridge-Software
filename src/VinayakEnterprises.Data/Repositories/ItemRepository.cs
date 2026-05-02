using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using VinayakEnterprises.Core.Models;
using VinayakEnterprises.Data.Interfaces;

namespace VinayakEnterprises.Data.Repositories;

public class ItemRepository : IItemRepository
{
    private readonly AppDbContext _context;

    public ItemRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Item?> GetByIdAsync(int id)
    {
        using var connection = _context.CreateConnection();
        return await connection.QueryFirstOrDefaultAsync<Item>(
            "SELECT * FROM Items WHERE Id = @Id AND IsDeleted = 0", new { Id = id });
    }

    public async Task<IEnumerable<Item>> GetAllAsync()
    {
        using var connection = _context.CreateConnection();
        return await connection.QueryAsync<Item>(
            "SELECT * FROM Items WHERE IsDeleted = 0");
    }

    public async Task<int> AddAsync(Item entity)
    {
        using var connection = _context.CreateConnection();
        var sql = "INSERT INTO Items (CodeNo, Name, Price, Weight, Unit) VALUES (@CodeNo, @Name, @Price, @Weight, @Unit); SELECT last_insert_rowid();";
        var id = await connection.ExecuteScalarAsync<int>(sql, entity);
        entity.Id = id;
        return id;
    }

    public async Task<int> UpdateAsync(Item entity)
    {
        using var connection = _context.CreateConnection();
        var sql = "UPDATE Items SET CodeNo = @CodeNo, Name = @Name, Price = @Price, Weight = @Weight, Unit = @Unit WHERE Id = @Id";
        return await connection.ExecuteAsync(sql, entity);
    }

    public async Task<int> DeleteAsync(int id)
    {
        using var connection = _context.CreateConnection();
        var sql = "UPDATE Items SET IsDeleted = 1 WHERE Id = @Id";
        return await connection.ExecuteAsync(sql, new { Id = id });
    }
}
