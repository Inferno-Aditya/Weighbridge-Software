using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using BCrypt.Net;
using VinayakEnterprises.Core.Models;
using VinayakEnterprises.Core.Services.Interfaces;
using VinayakEnterprises.Data.Interfaces;

namespace VinayakEnterprises.Core.Services.Implementations;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IAuditLogRepository _auditLogRepository;
    private readonly ISessionManager _sessionManager;
    private readonly IRepository<Role> _roleRepository;
    private readonly INavigationService _navigationService;

    public AuthService(
        IUserRepository userRepository, 
        IAuditLogRepository auditLogRepository, 
        ISessionManager sessionManager, 
        IRepository<Role> roleRepository,
        INavigationService navigationService)
    {
        _userRepository = userRepository;
        _auditLogRepository = auditLogRepository;
        _sessionManager = sessionManager;
        _roleRepository = roleRepository;
        _navigationService = navigationService;
    }

    public async Task<(bool Success, string Message)> LoginAsync(string username, string password)
    {
        var user = await _userRepository.GetByUsernameAsync(username);

        if (user == null || !user.IsActive)
        {
            await LogAuditAsync(null, username, "LoginFailed", "Invalid username or inactive user");
            return (false, "Invalid username or password.");
        }

        if (user.IsLocked)
        {
            await LogAuditAsync(user.Id, username, "LoginFailed", "Account is locked");
            return (false, "Account locked. Contact administrator.");
        }

        bool passwordValid = false;
        try
        {
            passwordValid = BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
        }
        catch (Exception)
        {
            // Invalid hash format or other bcrypt error
        }

        if (!passwordValid)
        {
            user.FailedLoginAttempts++;
            if (user.FailedLoginAttempts >= 5)
            {
                user.IsLocked = true;
                await _userRepository.UpdateAsync(user);
                await LogAuditAsync(user.Id, username, "AccountLocked", $"Locked after {user.FailedLoginAttempts} failed attempts");
                return (false, "Account locked. Contact administrator.");
            }
            
            await _userRepository.UpdateAsync(user);
            await LogAuditAsync(user.Id, username, "LoginFailed", "Invalid password");
            return (false, "Invalid username or password.");
        }

        // Success
        if (user.FailedLoginAttempts > 0)
        {
            user.FailedLoginAttempts = 0;
            await _userRepository.UpdateAsync(user);
        }

        var role = await _roleRepository.GetByIdAsync(user.RoleId);
        if (role == null)
        {
            return (false, "User role not found. Contact administrator.");
        }

        _sessionManager.StartSession(user, role);
        await LogAuditAsync(user.Id, username, "LoginSuccess", null);

        return (true, string.Empty);
    }

    public async Task<bool> ChangePasswordAsync(int userId, string oldPassword, string newPassword)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null) return false;

        if (!BCrypt.Net.BCrypt.Verify(oldPassword, user.PasswordHash))
        {
            return false;
        }

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
        user.ForcePasswordChange = false;
        await _userRepository.UpdateAsync(user);

        await LogAuditAsync(user.Id, user.Username, "PasswordChanged", null);
        return true;
    }

    public async Task LogoutAsync()
    {
        var user = _sessionManager.CurrentUser;
        if (user != null)
        {
            await LogAuditAsync(user.Id, user.Username, "Logout", null);
        }

        _sessionManager.EndSession();
    }

    private async Task LogAuditAsync(int? userId, string username, string action, string? details)
    {
        string ipAddress = string.Empty;
        string pcName = Environment.MachineName;

        try
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            var ip = host.AddressList.FirstOrDefault(a => a.AddressFamily == AddressFamily.InterNetwork);
            if (ip != null) ipAddress = ip.ToString();
        }
        catch { }

        var log = new AuditLog
        {
            Timestamp = DateTime.Now,
            UserId = userId,
            UserName = username,
            Action = action,
            EntityType = "User",
            EntityId = userId?.ToString(),
            OldValue = null,
            NewValue = details,
            IPAddress = ipAddress,
            PCName = pcName
        };

        await _auditLogRepository.AddAsync(log);
    }
}
