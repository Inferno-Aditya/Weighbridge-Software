using System;
using System.IO;
using System.Windows;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VinayakEnterprises.App.Services;
using VinayakEnterprises.Core.Models;
using VinayakEnterprises.Core.Services.Implementations;
using VinayakEnterprises.Core.Services.Interfaces;
using VinayakEnterprises.Core.ViewModels;
using VinayakEnterprises.Core.ViewModels.Auth;
using VinayakEnterprises.Data;
using VinayakEnterprises.Data.Interfaces;
using VinayakEnterprises.Data.Migrations;
using VinayakEnterprises.Data.Repositories;

namespace VinayakEnterprises.App;

public partial class App : Application
{
    private readonly IServiceProvider _serviceProvider;

    public App()
    {
        var services = new ServiceCollection();
        ConfigureServices(services);
        _serviceProvider = services.BuildServiceProvider();
    }

    private void ConfigureServices(IServiceCollection services)
    {
        // Configuration
        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
        var configuration = builder.Build();
        services.AddSingleton<IConfiguration>(configuration);

        // Data Layer
        services.AddSingleton<AppDbContext>();
        services.AddTransient<SchemaInitializer>();
        services.AddTransient<IUserRepository, UserRepository>();
        services.AddTransient<IAuditLogRepository, AuditLogRepository>();
        services.AddTransient<IRepository<Role>, RoleRepository>();

        // Core Layer - Services
        services.AddSingleton<ISessionManager, WpfSessionManager>();
        services.AddSingleton<INavigationService, NavigationService>();
        services.AddTransient<IAuthService, AuthService>();

        // ViewModels
        services.AddSingleton<MainViewModel>();
        services.AddTransient<LoginViewModel>();
        services.AddTransient<ChangePasswordViewModel>();
        services.AddTransient<DashboardViewModel>();

        // Views
        services.AddTransient<MainWindow>();
    }

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        // Initialize the database on startup
        var initializer = _serviceProvider.GetRequiredService<SchemaInitializer>();
        initializer.Initialize();

        // Show MainWindow
        var mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
        
        // Navigate to Login initially
        var navService = _serviceProvider.GetRequiredService<INavigationService>();
        navService.NavigateTo<LoginViewModel>();

        mainWindow.Show();
    }
}
