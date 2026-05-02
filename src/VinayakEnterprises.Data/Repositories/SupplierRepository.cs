using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using VinayakEnterprises.Core.Models;
using VinayakEnterprises.Data.Interfaces;

namespace VinayakEnterprises.Data.Repositories;

public class SupplierRepository : ISupplierRepository
{
    private readonly AppDbContext _context;

    public SupplierRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Supplier?> GetByIdAsync(int id)
    {
        using var connection = _context.CreateConnection();
        return await connection.QueryFirstOrDefaultAsync<Supplier>(
            "SELECT * FROM Suppliers WHERE Id = @Id AND IsDeleted = 0", new { Id = id });
    }

    public async Task<IEnumerable<Supplier>> GetAllAsync()
    {
        using var connection = _context.CreateConnection();
        return await connection.QueryAsync<Supplier>(
            "SELECT * FROM Suppliers WHERE IsDeleted = 0");
    }

    public async Task<int> AddAsync(Supplier entity)
    {
        using var connection = _context.CreateConnection();
        var sql = "INSERT INTO Suppliers (CodeNo, Name, Address, City, Phone, Mobile, VATNo, Email, W_Charges, Website, IsBlacklist) VALUES (@CodeNo, @Name, @Address, @City, @Phone, @Mobile, @VATNo, @Email, @W_Charges, @Website, @IsBlacklist); SELECT last_insert_rowid();";
        var id = await connection.ExecuteScalarAsync<int>(sql, entity);
        entity.Id = id;
        return id;
    }

    public async Task<int> UpdateAsync(Supplier entity)
    {
        using var connection = _context.CreateConnection();
        var sql = "UPDATE Suppliers SET CodeNo = @CodeNo, Name = @Name, Address = @Address, City = @City, Phone = @Phone, Mobile = @Mobile, VATNo = @VATNo, Email = @Email, W_Charges = @W_Charges, Website = @Website, IsBlacklist = @IsBlacklist WHERE Id = @Id";
        return await connection.ExecuteAsync(sql, entity);
    }

    public async Task<int> DeleteAsync(int id)
    {
        using var connection = _context.CreateConnection();
        var sql = "UPDATE Suppliers SET IsDeleted = 1 WHERE Id = @Id";
        return await connection.ExecuteAsync(sql, new { Id = id });
    }
}
