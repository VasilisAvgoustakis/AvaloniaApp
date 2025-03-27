using Avalonia;
using System;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using MsgApp.Models;
using MsgApp.ViewModels;
using MsgApp.Services;
using System.Net.Http;


namespace MsgApp;

class Program
{
    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static void Main(string[] args)
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
        services.AddTransient<Message>();
        services.AddSingleton<HttpClient>();
        services.AddSingleton<JsonMessageLoader>();
        services.AddSingleton<MessageStateService>();
        services.AddSingleton<MainWindow>();
        services.AddSingleton<GravatarService>();
        services.AddSingleton<MainWindowViewModel>();
        services.AddTransient<MessageViewModel>();

        // DI-Container bauen (das objekt was tatsächlich die Objekten erzeugen kann)
        var serviceProvider = services.BuildServiceProvider();

        
        var logger = serviceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogInformation("=== Anwendung startet ===");

        // App erstellen, DI-Container setzen und starten
        BuildAvaloniaApp()
            .AfterSetup(builder =>
            {
                var app = (App)builder.Instance!;
                app.Services = serviceProvider; // ServiceProvider setzen!
            })
            .StartWithClassicDesktopLifetime(args);
    } 
    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace();
}
