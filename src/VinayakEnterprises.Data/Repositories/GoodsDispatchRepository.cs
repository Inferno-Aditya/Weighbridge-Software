using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using VinayakEnterprises.Core.Models;
using VinayakEnterprises.Data.Interfaces;

namespace VinayakEnterprises.Data.Repositories;

public class GoodsDispatchRepository : IGoodsDispatchRepository
{
    private readonly AppDbContext _context;

    public GoodsDispatchRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<GoodsDispatch?> GetByIdAsync(int id)
    {
        using var connection = _context.CreateConnection();
        return await connection.QueryFirstOrDefaultAsync<GoodsDispatch>(
            "SELECT * FROM GoodsDispatch WHERE Id = @Id AND IsDeleted = 0", new { Id = id });
    }

    public async Task<IEnumerable<GoodsDispatch>> GetAllAsync()
    {
        using var connection = _context.CreateConnection();
        return await connection.QueryAsync<GoodsDispatch>(
            "SELECT * FROM GoodsDispatch WHERE IsDeleted = 0");
    }

    public async Task<int> AddAsync(GoodsDispatch entity)
    {
        using var connection = _context.CreateConnection();
        var sql = "INSERT INTO GoodsDispatch (PartyName, VehicleNo, TicketNo, Gross, Tare, Net, Item) VALUES (@PartyName, @VehicleNo, @TicketNo, @Gross, @Tare, @Net, @Item); SELECT last_insert_rowid();";
        var id = await connection.ExecuteScalarAsync<int>(sql, entity);
        entity.Id = id;
        return id;
    }

    public async Task<int> UpdateAsync(GoodsDispatch entity)
    {
        using var connection = _context.CreateConnection();
        var sql = "UPDATE GoodsDispatch SET PartyName = @PartyName, VehicleNo = @VehicleNo, TicketNo = @TicketNo, Gross = @Gross, Tare = @Tare, Net = @Net, Item = @Item WHERE Id = @Id";
        return await connection.ExecuteAsync(sql, entity);
    }

    public async Task<int> DeleteAsync(int id)
    {
        using var connection = _context.CreateConnection();
        var sql = "UPDATE GoodsDispatch SET IsDeleted = 1 WHERE Id = @Id";
        return await connection.ExecuteAsync(sql, new { Id = id });
    }
}
