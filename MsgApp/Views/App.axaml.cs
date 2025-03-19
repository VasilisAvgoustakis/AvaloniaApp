using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using MsgApp.ViewModels;
using MsgApp.Services;


namespace MsgApp;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        // Globaler Serilog-Logger (Logging für die ganz App)
        Log.Logger = new LoggerConfiguration()
            // In eine Datei schreiben
            // RollingInterval.Day => erzeugt jeden Tag eine neue Logdatei
            .WriteTo.File("Logs/MsgApp.log", rollingInterval: RollingInterval.Day)
            .CreateLogger();

        // DI-Container erstellen (Eine Sammlung von Services), da hintelegen wie Objekte für DI erzeugt werden
        var services = new ServiceCollection();

        // Logging mit Serilog an Microsoft.Extensions.Logging anbinden
        // jetzt wenn eine Klasse ein ILogger<I> braucht wird von SeriLog geholt
        services.AddLogging(loggingBuilder =>
        {
            // Alle bestehenden Provider entfernen (z.B. Konsole) 
            loggingBuilder.ClearProviders();
            // Serilog als Provider nutzen indem mann ihn mit den .NET ILogger<T> System verknüpft
            loggingBuilder.AddSerilog(Log.Logger);
        });

        // Services im DI Container registrieren
        services.AddSingleton<HttpClientService>();
        services.AddSingleton<JsonMessageLoader>();
        services.AddTransient<ITimerService, ProductionTimer>();
        services.AddSingleton<MessageStateService>();
        services.AddSingleton<MainWindow>();
        services.AddSingleton<GravatarService>();
        services.AddSingleton<MainWindowViewModel>();

        // DI-Container bauen (das objekt was tatsächlich die Objekten erzeugen kann)
        var serviceProvider = services.BuildServiceProvider();

        var logger = serviceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogInformation("=== Anwendung startet ===");

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            // ViewModel aus dem Container holen
            var mainVm = serviceProvider.GetRequiredService<MainWindowViewModel>();

            // MainWindow erstellen
            var mainWindown = serviceProvider.GetRequiredService<MainWindow>();

            desktop.MainWindow = mainWindown;
        }

        base.OnFrameworkInitializationCompleted();
    }
}