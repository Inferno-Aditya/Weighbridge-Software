using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using VinayakEnterprises.Core.Models;
using VinayakEnterprises.Data.Interfaces;

namespace VinayakEnterprises.Data.Repositories;

public class CustomerRepository : ICustomerRepository
{
    private readonly AppDbContext _context;

    public CustomerRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Customer?> GetByIdAsync(int id)
    {
        using var connection = _context.CreateConnection();
        return await connection.QueryFirstOrDefaultAsync<Customer>(
            "SELECT * FROM Customers WHERE Id = @Id AND IsDeleted = 0", new { Id = id });
    }

    public async Task<IEnumerable<Customer>> GetAllAsync()
    {
        using var connection = _context.CreateConnection();
        return await connection.QueryAsync<Customer>(
            "SELECT * FROM Customers WHERE IsDeleted = 0");
    }

    public async Task<int> AddAsync(Customer entity)
    {
        using var connection = _context.CreateConnection();
        var sql = "INSERT INTO Customers (CodeNo, Name, Address, City, Phone, Mobile, VATNo, Email, W_Charges, RateType, GSTNo, IsBlacklist) VALUES (@CodeNo, @Name, @Address, @City, @Phone, @Mobile, @VATNo, @Email, @W_Charges, @RateType, @GSTNo, @IsBlacklist); SELECT last_insert_rowid();";
        var id = await connection.ExecuteScalarAsync<int>(sql, entity);
        entity.Id = id;
        return id;
    }

    public async Task<int> UpdateAsync(Customer entity)
    {
        using var connection = _context.CreateConnection();
        var sql = "UPDATE Customers SET CodeNo = @CodeNo, Name = @Name, Address = @Address, City = @City, Phone = @Phone, Mobile = @Mobile, VATNo = @VATNo, Email = @Email, W_Charges = @W_Charges, RateType = @RateType, GSTNo = @GSTNo, IsBlacklist = @IsBlacklist WHERE Id = @Id";
        return await connection.ExecuteAsync(sql, entity);
    }

    public async Task<int> DeleteAsync(int id)
    {
        using var connection = _context.CreateConnection();
        var sql = "UPDATE Customers SET IsDeleted = 1 WHERE Id = @Id";
        return await connection.ExecuteAsync(sql, new { Id = id });
    }
}
