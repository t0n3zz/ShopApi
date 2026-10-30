using System.Windows;
using Marketplace.App.Models;
using Marketplace.App.Services;

namespace Marketplace.App.Views;

public partial class CartWindow : Window
{
    private readonly ApiService _api;
    public CartWindow(ApiService api) { InitializeComponent(); _api = api; Loaded += async (_, _) => await LoadCartAsync(); }

    private async Task LoadCartAsync()
    {
        var cart = await _api.GetCartAsync();
        if (cart == null) return;
        CartGrid.ItemsSource = cart.Items;
        TotalText.Text = $"Итого: {cart.TotalSum:C}";
    }

    private async void Remove_Click(object sender, RoutedEventArgs e)
    {
        if (CartGrid.SelectedItem is CartItemModel item)
        {
            await _api.RemoveCartItemAsync(item.Id);
            await LoadCartAsync();
        }
    }

    private void Checkout_Click(object sender, RoutedEventArgs e)
    {
        MessageBox.Show("todo: Выбор доставки и создание заказа.");
    }
}