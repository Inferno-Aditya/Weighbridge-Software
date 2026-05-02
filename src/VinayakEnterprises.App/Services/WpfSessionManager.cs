using System;
using System.Windows.Interop;
using System.Windows.Threading;
using Microsoft.Extensions.Configuration;
using VinayakEnterprises.Core.Models;
using VinayakEnterprises.Core.Services.Interfaces;

namespace VinayakEnterprises.App.Services;

public class WpfSessionManager : ISessionManager, IDisposable
{
    private readonly DispatcherTimer _timer;
    private readonly INavigationService _navigationService;
    private readonly int _timeoutMinutes;
    private bool _isHooked;

    public User? CurrentUser { get; private set; }
    public Role? CurrentRole { get; private set; }
    public bool IsAuthenticated => CurrentUser != null;

    public event EventHandler? SessionExpired;
    public event EventHandler? UserChanged;

    public WpfSessionManager(IConfiguration configuration, INavigationService navigationService)
    {
        _navigationService = navigationService;
        _timeoutMinutes = configuration.GetValue<int>("SessionTimeout", 30);
        
        _timer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMinutes(_timeoutMinutes)
        };
        _timer.Tick += Timer_Tick;
    }

    public void StartSession(User user, Role role)
    {
        CurrentUser = user;
        CurrentRole = role;
        
        if (!_isHooked)
        {
            ComponentDispatcher.ThreadPreprocessMessage += ComponentDispatcher_ThreadPreprocessMessage;
            _isHooked = true;
        }

        _timer.Start();
        UserChanged?.Invoke(this, EventArgs.Empty);
    }

    public void EndSession()
    {
        CurrentUser = null;
        CurrentRole = null;
        
        if (_isHooked)
        {
            ComponentDispatcher.ThreadPreprocessMessage -= ComponentDispatcher_ThreadPreprocessMessage;
            _isHooked = false;
        }

        _timer.Stop();
        UserChanged?.Invoke(this, EventArgs.Empty);
    }

    public void ResetTimer()
    {
        if (_timer.IsEnabled)
        {
            _timer.Stop();
            _timer.Start();
        }
    }

    private void Timer_Tick(object? sender, EventArgs e)
    {
        SessionExpired?.Invoke(this, EventArgs.Empty);
    }

    private void ComponentDispatcher_ThreadPreprocessMessage(ref MSG msg, ref bool handled)
    {
        // WM_MOUSEMOVE = 0x0200, WM_KEYDOWN = 0x0100
        const int WM_MOUSEMOVE = 0x0200;
        const int WM_LBUTTONDOWN = 0x0201;
        const int WM_RBUTTONDOWN = 0x0204;
        const int WM_KEYDOWN = 0x0100;

        if (msg.message == WM_MOUSEMOVE || msg.message == WM_KEYDOWN || 
            msg.message == WM_LBUTTONDOWN || msg.message == WM_RBUTTONDOWN)
        {
            ResetTimer();
        }
    }

    public void Dispose()
    {
        if (_isHooked)
        {
            ComponentDispatcher.ThreadPreprocessMessage -= ComponentDispatcher_ThreadPreprocessMessage;
        }
        _timer.Tick -= Timer_Tick;
    }
}
