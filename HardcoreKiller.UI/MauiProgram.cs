using Microsoft.Extensions.Logging;
using HardcoreKiller.Domain.Repositories;
using HardcoreKiller.Infrastructure.Persistence;
using HardcoreKiller.Infrastructure.Configuration;

namespace HardcoreKiller.UI
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                });

            builder.Services.AddMauiBlazorWebView();

            // Configurazione database
            var connectionString = DatabaseConfiguration.GetConnectionString();

            // Registrazione servizi database
            builder.Services.AddSingleton<IDatabaseManager>(serviceProvider =>
            {
                var logger = serviceProvider.GetRequiredService<ILogger<SqliteDatabaseManager>>();
                return new SqliteDatabaseManager(connectionString, logger);
            });

#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
            builder.Logging.AddDebug();
#endif

            var app = builder.Build();

            // *** INIZIALIZZAZIONE GARANTITA DEL DATABASE ***
            // Questo avviene PRIMA che qualsiasi componente UI sia caricato
            try
            {
                using var scope = app.Services.CreateScope();
                var databaseManager = scope.ServiceProvider.GetRequiredService<IDatabaseManager>();
                var loggerFactory = scope.ServiceProvider.GetRequiredService<ILoggerFactory>();
                var logger = loggerFactory.CreateLogger("MauiProgram");

                logger.LogInformation("Initializing database at application startup...");

                // Inizializzazione sincrona del database
                var initTask = databaseManager.InitializeAsync();
                initTask.Wait(); // Aspetta che finisca prima di continuare

                logger.LogInformation("Database initialization completed successfully");
            }
            catch (Exception ex)
            {
                // Log critico - l'app non può continuare senza database
                var loggerFactory = app.Services.GetRequiredService<ILoggerFactory>();
                var logger = loggerFactory.CreateLogger("MauiProgram");
                logger.LogCritical(ex, "CRITICAL: Failed to initialize database at startup");

                // Qui potresti decidere di:
                // 1. Terminare l'app: throw;
                // 2. Mostrare messaggio di errore e continuare (meno sicuro)
                throw new InvalidOperationException("Database initialization failed", ex);
            }

            return app;
        }
    }
}