using VinayakEnterprises.Core.Models;

namespace VinayakEnterprises.Data.Interfaces;

public interface IUserRepository : IRepository<User>
{
    Task<User?> GetByUsernameAsync(string username);
}
