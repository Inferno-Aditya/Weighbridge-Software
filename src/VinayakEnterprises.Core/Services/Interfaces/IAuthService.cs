using System.Threading.Tasks;
using VinayakEnterprises.Core.Models;

namespace VinayakEnterprises.Core.Services.Interfaces;

public interface IAuthService
{
    Task<(bool Success, string Message)> LoginAsync(string username, string password);
    Task<bool> ChangePasswordAsync(int userId, string oldPassword, string newPassword);
    Task LogoutAsync();
}
