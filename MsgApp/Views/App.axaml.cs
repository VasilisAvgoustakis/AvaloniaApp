using System;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;


namespace MsgApp;

public partial class App : Application
{
    // Property für DI-Container
    public IServiceProvider? Services { get; set; }
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop && Services is not null)
        {
            // ViewModel aus dem Container holen
            desktop.MainWindow = Services.GetRequiredService<MainWindow>();

            // MainWindow erstellen
            var mainWindown = Services.GetRequiredService<MainWindow>();

            desktop.MainWindow = mainWindown;
        }

        base.OnFrameworkInitializationCompleted();
    }
}