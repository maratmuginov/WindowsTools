using System;
using System.Windows;
using FileCleanup.Services;
using FileCleanup.ViewModels;
using FileCleanup.Views;
using Microsoft.Extensions.DependencyInjection;

namespace FileCleanup
{
    public partial class App
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            var services = ConfigureServices();
            var mainWindow = new MainWindow
            {
                DataContext = services.GetRequiredService<MainWindowViewModel>()
            };
            mainWindow.Show();
        }

        private static IServiceProvider ConfigureServices() => new ServiceCollection()
            .AddSingleton<MainWindowViewModel>()
            .AddSingleton<IDialogService, DialogService>()
                .BuildServiceProvider();
    }
}
