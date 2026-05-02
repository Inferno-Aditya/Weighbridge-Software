using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using VinayakEnterprises.Core.Models;
using VinayakEnterprises.Data.Interfaces;

namespace VinayakEnterprises.Data.Repositories;

public class Field03Repository : IField03Repository
{
    private readonly AppDbContext _context;

    public Field03Repository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Field03?> GetByIdAsync(int id)
    {
        using var connection = _context.CreateConnection();
        return await connection.QueryFirstOrDefaultAsync<Field03>(
            "SELECT * FROM Field03s WHERE Id = @Id AND IsDeleted = 0", new { Id = id });
    }

    public async Task<IEnumerable<Field03>> GetAllAsync()
    {
        using var connection = _context.CreateConnection();
        return await connection.QueryAsync<Field03>(
            "SELECT * FROM Field03s WHERE IsDeleted = 0");
    }

    public async Task<int> AddAsync(Field03 entity)
    {
        using var connection = _context.CreateConnection();
        var sql = "INSERT INTO Field03s (CodeNo, Name, Remarks) VALUES (@CodeNo, @Name, @Remarks); SELECT last_insert_rowid();";
        var id = await connection.ExecuteScalarAsync<int>(sql, entity);
        entity.Id = id;
        return id;
    }

    public async Task<int> UpdateAsync(Field03 entity)
    {
        using var connection = _context.CreateConnection();
        var sql = "UPDATE Field03s SET CodeNo = @CodeNo, Name = @Name, Remarks = @Remarks WHERE Id = @Id";
        return await connection.ExecuteAsync(sql, entity);
    }

    public async Task<int> DeleteAsync(int id)
    {
        using var connection = _context.CreateConnection();
        var sql = "UPDATE Field03s SET IsDeleted = 1 WHERE Id = @Id";
        return await connection.ExecuteAsync(sql, new { Id = id });
    }
}
