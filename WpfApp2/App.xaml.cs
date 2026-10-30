using System.Windows;
using Marketplace.App.Services;
using Marketplace.App.Views;

namespace WpfApp2;

public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        var api = new ApiService();
        new LoginWindow(api).Show();
    }
}