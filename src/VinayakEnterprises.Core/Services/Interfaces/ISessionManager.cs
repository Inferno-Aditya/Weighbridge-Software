using System;
using VinayakEnterprises.Core.Models;

namespace VinayakEnterprises.Core.Services.Interfaces;

public interface ISessionManager
{
    User? CurrentUser { get; }
    Role? CurrentRole { get; }
    bool IsAuthenticated { get; }
    
    event EventHandler? SessionExpired;
    event EventHandler? UserChanged;

    void StartSession(User user, Role role);
    void EndSession();
    void ResetTimer();
}
