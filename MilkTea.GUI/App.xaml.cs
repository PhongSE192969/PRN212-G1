using System.Windows;
using Microsoft.Extensions.Configuration;
using MilkTea.GUI.Utils;

namespace MilkTea.GUI
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            
            // Load configuration
            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();
            
            // Load app settings
            AppConfig.VATRate = decimal.Parse(configuration["AppSettings:VAT"] ?? "0.10");
            AppConfig.CompanyName = configuration["AppSettings:CompanyName"] ?? "TeaPOS";
            AppConfig.CompanyAddress = configuration["AppSettings:CompanyAddress"] ?? "";
            AppConfig.CompanyPhone = configuration["AppSettings:CompanyPhone"] ?? "";
        }
    }
}
