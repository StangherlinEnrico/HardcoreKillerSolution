using HardcoreKiller.Application.Services;
using HardcoreKiller.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace HardcoreKiller.Infrastructure.Services;

public class StartupService : IStartupService
{
    private readonly IDatabaseManager _databaseManager;
    private readonly ILogger<StartupService> _logger;

    public StartupService(IDatabaseManager databaseManager, ILogger<StartupService> logger)
    {
        _databaseManager = databaseManager ?? throw new ArgumentNullException(nameof(databaseManager));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task InitializeApplicationAsync()
    {
        try
        {
            _logger.LogInformation("Starting application initialization...");

            // Inizializza il database
            await _databaseManager.InitializeAsync();

            _logger.LogInformation("Application initialization completed successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Critical error during application initialization");
            throw;
        }
    }
}