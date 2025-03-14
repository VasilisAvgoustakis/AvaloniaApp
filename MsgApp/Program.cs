using Avalonia;
using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Extensions.Logging;

namespace MsgApp;

class Program
{
    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static void Main(string[] args)
    {
        BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);

        // Serilog-Logger anlegen
        Log.Logger = new LoggerConfiguration()
            // In eine Datei schreiben, z.B. "Logs/app.log"
            // RollingInterval.Day => erzeugt jeden Tag eine neue Logdatei "app20250314.log" usw.
            .WriteTo.File("Logs/MsgApp.log", rollingInterval: RollingInterval.Day)
            .CreateLogger();

        // DI-Container erstellen
        var services = new ServiceCollection();


        // Logging mit Serilog an Microsoft.Extensions.Logging anbinden
        services.AddLogging(loggingBuilder =>
        {
            // Alle bestehenden Provider entfernen (z.B. Konsole) 
            loggingBuilder.ClearProviders();
            // Serilog als Provider nutzen
            loggingBuilder.AddSerilog(Log.Logger);
        });

        // Services registrieren
        // (z.B. den JsonMessageLoader)
        services.AddTransient<JsonMessageLoader>();

        // DI-Container bauen
        var serviceProvider = services.BuildServiceProvider();

        // Test Loggen
        var logger = serviceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogInformation("=== Anwendung startet ===");


        // JsonMessageLoader ausprobieren
        var loader = serviceProvider.GetRequiredService<JsonMessageLoader>();
        var messages = loader.LoadMessagesFromJson("Data/sample-messages.json");

        logger.LogInformation("Anwendung fertig, geladene Nachrichten: {Count}", messages.Count);

    } 
    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace();
}
