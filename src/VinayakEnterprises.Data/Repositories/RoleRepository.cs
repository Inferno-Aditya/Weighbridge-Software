using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using VinayakEnterprises.Core.Models;
using VinayakEnterprises.Data.Interfaces;

namespace VinayakEnterprises.Data.Repositories;

public class RoleRepository : IRoleRepository
{
    private readonly AppDbContext _context;

    public RoleRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Role?> GetByIdAsync(int id)
    {
        using var connection = _context.CreateConnection();
        return await connection.QueryFirstOrDefaultAsync<Role>(
            "SELECT * FROM Roles WHERE Id = @Id AND IsDeleted = 0", new { Id = id });
    }

    public async Task<IEnumerable<Role>> GetAllAsync()
    {
        using var connection = _context.CreateConnection();
        return await connection.QueryAsync<Role>(
            "SELECT * FROM Roles WHERE IsDeleted = 0");
    }

    public async Task<int> AddAsync(Role entity)
    {
        using var connection = _context.CreateConnection();
        var sql = "INSERT INTO Roles (Name) VALUES (@Name); SELECT last_insert_rowid();";
        var id = await connection.ExecuteScalarAsync<int>(sql, entity);
        entity.Id = id;
        return id;
    }

    public async Task<int> UpdateAsync(Role entity)
    {
        using var connection = _context.CreateConnection();
        var sql = "UPDATE Roles SET Name = @Name WHERE Id = @Id";
        return await connection.ExecuteAsync(sql, entity);
    }

    public async Task<int> DeleteAsync(int id)
    {
        using var connection = _context.CreateConnection();
        var sql = "UPDATE Roles SET IsDeleted = 1 WHERE Id = @Id";
        return await connection.ExecuteAsync(sql, new { Id = id });
    }
}
