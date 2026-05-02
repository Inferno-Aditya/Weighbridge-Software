using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using VinayakEnterprises.Core.Models;
using VinayakEnterprises.Data.Interfaces;

namespace VinayakEnterprises.Data.Repositories;

public class LicenseInfoRepository : ILicenseInfoRepository
{
    private readonly AppDbContext _context;

    public LicenseInfoRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<LicenseInfo?> GetByIdAsync(int id)
    {
        using var connection = _context.CreateConnection();
        return await connection.QueryFirstOrDefaultAsync<LicenseInfo>(
            "SELECT * FROM LicenseInfo WHERE Id = @Id", new { Id = id });
    }

    public async Task<IEnumerable<LicenseInfo>> GetAllAsync()
    {
        using var connection = _context.CreateConnection();
        return await connection.QueryAsync<LicenseInfo>(
            "SELECT * FROM LicenseInfo");
    }

    public async Task<int> AddAsync(LicenseInfo entity)
    {
        using var connection = _context.CreateConnection();
        var sql = "INSERT INTO LicenseInfo (MachineId, LicenseType, IssueDate, ExpiryDate, ActivatedBy, ActivationDate) VALUES (@MachineId, @LicenseType, @IssueDate, @ExpiryDate, @ActivatedBy, @ActivationDate); SELECT last_insert_rowid();";
        var id = await connection.ExecuteScalarAsync<int>(sql, entity);
        entity.Id = id;
        return id;
    }

    public async Task<int> UpdateAsync(LicenseInfo entity)
    {
        using var connection = _context.CreateConnection();
        var sql = "UPDATE LicenseInfo SET MachineId = @MachineId, LicenseType = @LicenseType, IssueDate = @IssueDate, ExpiryDate = @ExpiryDate, ActivatedBy = @ActivatedBy, ActivationDate = @ActivationDate WHERE Id = @Id";
        return await connection.ExecuteAsync(sql, entity);
    }

    public async Task<int> DeleteAsync(int id)
    {
        using var connection = _context.CreateConnection();
        var sql = "DELETE FROM LicenseInfo WHERE Id = @Id";
        return await connection.ExecuteAsync(sql, new { Id = id });
    }
}
