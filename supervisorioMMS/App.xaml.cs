using Microsoft.Extensions.DependencyInjection;
using supervisorioMMS.Services;
using supervisorioMMS.ViewModels;
using supervisorioMMS.Views;
using System;
using System.Windows;

namespace supervisorioMMS
{
    public partial class App : Application
    {
        public IServiceProvider? ServiceProvider { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);

            ServiceProvider = serviceCollection.BuildServiceProvider();

            var mainWindow = ServiceProvider.GetRequiredService<MainWindow>();
            mainWindow.Show();
        }

        private void ConfigureServices(IServiceCollection services)
        {
            // Services
            services.AddSingleton<ModbusService>();
            services.AddSingleton<TagService>();

            // ViewModels
            services.AddTransient<ConfiguracoesViewModel>();
            services.AddTransient<TagConfigViewModel>();
            services.AddTransient<UsuariosViewModel>();

            // Views - Registering views that are navigated to.
            services.AddTransient<AlarmesView>();
            services.AddTransient<ConfiguracoesView>();
            services.AddTransient<GraficosView>();
            services.AddTransient<HistoricoView>();
            services.AddTransient<PrincipalView>();
            services.AddTransient<TagConfigView>();
            services.AddTransient<UsuariosView>();

            // Main Window
            services.AddTransient<MainWindow>();
        }
    }
}
