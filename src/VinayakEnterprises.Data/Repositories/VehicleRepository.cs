using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using VinayakEnterprises.Core.Models;
using VinayakEnterprises.Data.Interfaces;

namespace VinayakEnterprises.Data.Repositories;

public class VehicleRepository : IVehicleRepository
{
    private readonly AppDbContext _context;

    public VehicleRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Vehicle?> GetByIdAsync(int id)
    {
        using var connection = _context.CreateConnection();
        return await connection.QueryFirstOrDefaultAsync<Vehicle>(
            "SELECT * FROM Vehicles WHERE Id = @Id AND IsDeleted = 0", new { Id = id });
    }

    public async Task<IEnumerable<Vehicle>> GetAllAsync()
    {
        using var connection = _context.CreateConnection();
        return await connection.QueryAsync<Vehicle>(
            "SELECT * FROM Vehicles WHERE IsDeleted = 0");
    }

    public async Task<int> AddAsync(Vehicle entity)
    {
        using var connection = _context.CreateConnection();
        var sql = "INSERT INTO Vehicles (VehicleNo, TareWtKg, RTOWtKg, TareDate, MaxTareAllow, MinTareAllow, IsBlacklist) VALUES (@VehicleNo, @TareWtKg, @RTOWtKg, @TareDate, @MaxTareAllow, @MinTareAllow, @IsBlacklist); SELECT last_insert_rowid();";
        var id = await connection.ExecuteScalarAsync<int>(sql, entity);
        entity.Id = id;
        return id;
    }

    public async Task<int> UpdateAsync(Vehicle entity)
    {
        using var connection = _context.CreateConnection();
        var sql = "UPDATE Vehicles SET VehicleNo = @VehicleNo, TareWtKg = @TareWtKg, RTOWtKg = @RTOWtKg, TareDate = @TareDate, MaxTareAllow = @MaxTareAllow, MinTareAllow = @MinTareAllow, IsBlacklist = @IsBlacklist WHERE Id = @Id";
        return await connection.ExecuteAsync(sql, entity);
    }

    public async Task<int> DeleteAsync(int id)
    {
        using var connection = _context.CreateConnection();
        var sql = "UPDATE Vehicles SET IsDeleted = 1 WHERE Id = @Id";
        return await connection.ExecuteAsync(sql, new { Id = id });
    }
}
