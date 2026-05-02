using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using VinayakEnterprises.Core.Models;
using VinayakEnterprises.Data.Interfaces;

namespace VinayakEnterprises.Data.Repositories;

public class Field02Repository : IField02Repository
{
    private readonly AppDbContext _context;

    public Field02Repository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Field02?> GetByIdAsync(int id)
    {
        using var connection = _context.CreateConnection();
        return await connection.QueryFirstOrDefaultAsync<Field02>(
            "SELECT * FROM Field02s WHERE Id = @Id AND IsDeleted = 0", new { Id = id });
    }

    public async Task<IEnumerable<Field02>> GetAllAsync()
    {
        using var connection = _context.CreateConnection();
        return await connection.QueryAsync<Field02>(
            "SELECT * FROM Field02s WHERE IsDeleted = 0");
    }

    public async Task<int> AddAsync(Field02 entity)
    {
        using var connection = _context.CreateConnection();
        var sql = "INSERT INTO Field02s (CodeNo, Name, Remarks) VALUES (@CodeNo, @Name, @Remarks); SELECT last_insert_rowid();";
        var id = await connection.ExecuteScalarAsync<int>(sql, entity);
        entity.Id = id;
        return id;
    }

    public async Task<int> UpdateAsync(Field02 entity)
    {
        using var connection = _context.CreateConnection();
        var sql = "UPDATE Field02s SET CodeNo = @CodeNo, Name = @Name, Remarks = @Remarks WHERE Id = @Id";
        return await connection.ExecuteAsync(sql, entity);
    }

    public async Task<int> DeleteAsync(int id)
    {
        using var connection = _context.CreateConnection();
        var sql = "UPDATE Field02s SET IsDeleted = 1 WHERE Id = @Id";
        return await connection.ExecuteAsync(sql, new { Id = id });
    }
}
