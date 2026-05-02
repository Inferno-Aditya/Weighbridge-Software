using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using VinayakEnterprises.Core.Models;
using VinayakEnterprises.Data.Interfaces;

namespace VinayakEnterprises.Data.Repositories;

public class CompanyMasterRepository : ICompanyMasterRepository
{
    private readonly AppDbContext _context;

    public CompanyMasterRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<CompanyMaster?> GetByIdAsync(int id)
    {
        using var connection = _context.CreateConnection();
        return await connection.QueryFirstOrDefaultAsync<CompanyMaster>(
            "SELECT * FROM CompanyMaster WHERE Id = @Id", new { Id = id });
    }

    public async Task<IEnumerable<CompanyMaster>> GetAllAsync()
    {
        using var connection = _context.CreateConnection();
        return await connection.QueryAsync<CompanyMaster>(
            "SELECT * FROM CompanyMaster");
    }

    public async Task<int> AddAsync(CompanyMaster entity)
    {
        using var connection = _context.CreateConnection();
        var sql = "INSERT INTO CompanyMaster (Name, Address, LogoPath, GSTNo, Phone) VALUES (@Name, @Address, @LogoPath, @GSTNo, @Phone); SELECT last_insert_rowid();";
        var id = await connection.ExecuteScalarAsync<int>(sql, entity);
        entity.Id = id;
        return id;
    }

    public async Task<int> UpdateAsync(CompanyMaster entity)
    {
        using var connection = _context.CreateConnection();
        var sql = "UPDATE CompanyMaster SET Name = @Name, Address = @Address, LogoPath = @LogoPath, GSTNo = @GSTNo, Phone = @Phone WHERE Id = @Id";
        return await connection.ExecuteAsync(sql, entity);
    }

    public async Task<int> DeleteAsync(int id)
    {
        using var connection = _context.CreateConnection();
        var sql = "DELETE FROM CompanyMaster WHERE Id = @Id";
        return await connection.ExecuteAsync(sql, new { Id = id });
    }
}
