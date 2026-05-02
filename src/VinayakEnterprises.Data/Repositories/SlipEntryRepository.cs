using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using VinayakEnterprises.Core.Models;
using VinayakEnterprises.Data.Interfaces;

namespace VinayakEnterprises.Data.Repositories;

public class SlipEntryRepository : ISlipEntryRepository
{
    private readonly AppDbContext _context;

    public SlipEntryRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<SlipEntry?> GetByIdAsync(int id)
    {
        using var connection = _context.CreateConnection();
        return await connection.QueryFirstOrDefaultAsync<SlipEntry>(
            "SELECT * FROM SlipEntries WHERE Id = @Id AND IsDeleted = 0", new { Id = id });
    }

    public async Task<IEnumerable<SlipEntry>> GetAllAsync()
    {
        using var connection = _context.CreateConnection();
        return await connection.QueryAsync<SlipEntry>(
            "SELECT * FROM SlipEntries WHERE IsDeleted = 0");
    }

    public async Task<int> AddAsync(SlipEntry entity)
    {
        using var connection = _context.CreateConnection();
        var sql = "INSERT INTO SlipEntries (TicketNo, CustomerId, SupplierId, VehicleNo, ItemId, OperatorId, WBLocation, GrossWt, TareWt, NetWt, GrossTime, TareTime, CameraImagePath, Field01Id, Field02Id, Field03Id, TicketStatus, WeighmentNo, ManualData, CreatedAt, UpdatedAt) VALUES (@TicketNo, @CustomerId, @SupplierId, @VehicleNo, @ItemId, @OperatorId, @WBLocation, @GrossWt, @TareWt, @NetWt, @GrossTime, @TareTime, @CameraImagePath, @Field01Id, @Field02Id, @Field03Id, @TicketStatus, @WeighmentNo, @ManualData, @CreatedAt, @UpdatedAt); SELECT last_insert_rowid();";
        var id = await connection.ExecuteScalarAsync<int>(sql, entity);
        entity.Id = id;
        return id;
    }

    public async Task<int> UpdateAsync(SlipEntry entity)
    {
        using var connection = _context.CreateConnection();
        var sql = "UPDATE SlipEntries SET TicketNo = @TicketNo, CustomerId = @CustomerId, SupplierId = @SupplierId, VehicleNo = @VehicleNo, ItemId = @ItemId, OperatorId = @OperatorId, WBLocation = @WBLocation, GrossWt = @GrossWt, TareWt = @TareWt, NetWt = @NetWt, GrossTime = @GrossTime, TareTime = @TareTime, CameraImagePath = @CameraImagePath, Field01Id = @Field01Id, Field02Id = @Field02Id, Field03Id = @Field03Id, TicketStatus = @TicketStatus, WeighmentNo = @WeighmentNo, ManualData = @ManualData, CreatedAt = @CreatedAt, UpdatedAt = @UpdatedAt WHERE Id = @Id";
        return await connection.ExecuteAsync(sql, entity);
    }

    public async Task<int> DeleteAsync(int id)
    {
        using var connection = _context.CreateConnection();
        var sql = "UPDATE SlipEntries SET IsDeleted = 1 WHERE Id = @Id";
        return await connection.ExecuteAsync(sql, new { Id = id });
    }
}
