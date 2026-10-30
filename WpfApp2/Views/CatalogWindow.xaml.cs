using System.Windows;
using System.Windows.Controls;
using Marketplace.App.Services;

namespace Marketplace.App.Views;

public partial class CatalogWindow : Window
{
    private readonly ApiService _api;
    public CatalogWindow(ApiService api)
    {
        InitializeComponent();
        _api = api;
        Loaded += async (_, _) => await LoadDataAsync();
    }

    private async Task LoadDataAsync()
    {
        CategoryCombo.ItemsSource = await _api.GetCategoriesAsync();
        ProductsList.ItemsSource = await _api.GetProductsAsync();

        UserInfo.Text = _api.IsAuthenticated
            ? $"Вы: {_api.CurrentLogin} ({_api.CurrentRole})"
            : "Гость";
    }

    private async void Search_Click(object sender, RoutedEventArgs e)
    {
        var catId = CategoryCombo.SelectedValue as int?;
        ProductsList.ItemsSource = await _api.GetProductsAsync(SearchBox.Text, catId);
    }

    private async void AddToCart_Click(object sender, RoutedEventArgs e)
    {
        if (!_api.IsAuthenticated) { MessageBox.Show("Войдите в аккаунт!"); return; }
        var id = (int)((Button)sender).Tag;
        var ok = await _api.AddToCartAsync(id, 1);
        MessageBox.Show(ok ? "Добавлено!" : "Ошибка");
    }

    private void Details_Click(object sender, RoutedEventArgs e)
    {
        var product = ((FrameworkElement)sender).DataContext as dynamic;
        MessageBox.Show($"{product.Name}\n\n{product.Description}\n\nЦена: {product.Price:C}");
    }

    private void OpenCart_Click(object sender, RoutedEventArgs e)
    {
        if (!_api.IsAuthenticated) { MessageBox.Show("Войдите в аккаунт!"); return; }
        new CartWindow(_api).Show();
    }

    private void Logout_Click(object sender, RoutedEventArgs e)
    {
        _api.Logout();
        new LoginWindow(_api).Show();
        Close();
    }
}