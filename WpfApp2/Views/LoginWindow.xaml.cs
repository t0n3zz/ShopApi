using System.Windows;
using Marketplace.App.Services;

namespace Marketplace.App.Views;

public partial class LoginWindow : Window
{
    private readonly ApiService _api;
    public LoginWindow(ApiService api) { InitializeComponent(); _api = api; }

    private async void Login_Click(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(LoginBox.Text) || string.IsNullOrWhiteSpace(PasswordBox.Password))
        { MessageText.Text = "Заполните логин и пароль"; return; }

        if (await _api.LoginAsync(LoginBox.Text.Trim(), PasswordBox.Password))
        {
            new CatalogWindow(_api).Show();
            Close();
        }
        else MessageText.Text = "Неверный логин или пароль";
    }

    private async void Register_Click(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(LoginBox.Text) || string.IsNullOrWhiteSpace(PasswordBox.Password))
        { MessageText.Text = "Заполните все поля"; return; }
        if (PasswordBox.Password != RepeatPasswordBox.Password)
        { MessageText.Text = "Пароли не совпадают"; return; }

        var (success, message) = await _api.RegisterAsync(LoginBox.Text.Trim(), PasswordBox.Password);
        MessageText.Text = success ? "Успешно! Теперь войдите." : "Ошибка: " + message;
    }

    private void Guest_Click(object sender, RoutedEventArgs e)
    {
        new CatalogWindow(_api).Show();
        Close();
    }
}