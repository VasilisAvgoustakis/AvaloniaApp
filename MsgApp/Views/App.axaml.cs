using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Extensions.Logging;
using MsgApp.ViewModels;
using MsgApp.Services;
using System;


namespace MsgApp;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        // Globaler Serilog-Logger anlegen (Quelle fürs Logging für die ganz App)
        Log.Logger = new LoggerConfiguration()
            // In eine Datei schreiben
            // RollingInterval.Day => erzeugt jeden Tag eine neue Logdatei "app20250314.log" usw.
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

        // Services registrieren
        
        // HttpClient Service addieren als Singleton
        services.AddSingleton<HttpClientService>();
        // erstelle eine neuen JsonMessageLoader und versorge den konstruktor mit dessen Abhängigkeiten (ILogger<JsonMessageLoader> vorher mit services.AddLogging eingefügt und somit bekannt zu den DI Container)
        services.AddTransient<JsonMessageLoader>();
        // Services für MainWindowViewModel addieren
        services.AddTransient<ITimerService, ProductionTimer>();

        services.AddTransient<MessageStateService>();

        // erstelle eine neue MainWIndow mit Abhängigkeiten
        services.AddTransient<MainWindow>();
        // GravatarService im DI registrieren
        services.AddTransient<GravatarService>();
        // estelle eine neuen MainWindowViewModel und versorge ihn mit dessen Abhängigkeiten
        services.AddTransient<MainWindowViewModel>();

        // DI-Container bauen (das objekt was tatsächlich die Objekten erzeugen kann)
        var serviceProvider = services.BuildServiceProvider();

        // Test Loggen
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