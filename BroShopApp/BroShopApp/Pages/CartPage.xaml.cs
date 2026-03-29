using BroShopApp.Services;
using BroShopApp.Model;

namespace BroShopApp.Pages;

public partial class CartPage : ContentPage
{
    private readonly ApiService _apiService;

    public CartPage()
    {
        InitializeComponent();
        _apiService = new ApiService();
    }

    // OnAppearing срабатывает каждый раз, когда страница появляется на экране
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadCartItems();
    }

    private async Task LoadCartItems()
    {
        try
        {
            // 1. Берем сохраненный ID пользователя
            int userId = Preferences.Get("UserId", 0);

            if (userId == 0)
            {
                // Если пользователь не вошел, можно обнулить список или отправить на логин
                CartCollection.ItemsSource = null;
                return;
            }

            // 2. Загружаем данные из API
            var items = await _apiService.GetCartAsync(userId);

            // 3. Привязываем к списку
            CartCollection.ItemsSource = items;
        }
        catch (Exception ex)
        {
            await DisplayAlert("Ошибка", "Не удалось загрузить корзину", "ОК");
        }
    }

    private async void OnIncreaseQuantityClicked(object sender, EventArgs e)
    {
        var button = (Button)sender;
        var item = (CartItem)button.CommandParameter;
        
        button.IsEnabled = false;
        item.Quantity++;
        await UpdateQuantityOnServer(item);
        button.IsEnabled = true;
    }

    private async void OnDecreaseQuantityClicked(object sender, EventArgs e)
    {
        var button = (Button)sender;
        var item = (CartItem)button.CommandParameter;

        if (item.Quantity > 1)
        {
            button.IsEnabled = false;
            item.Quantity--;
            await UpdateQuantityOnServer(item);
            button.IsEnabled = true;
        }
        else
        {
            // Если уменьшаем с 1 до 0 — спрашиваем об удалении
            bool answer = await DisplayAlert("Удаление", "Удалить товар из корзины?", "Да", "Нет");
            if (answer)
            {
                item.Quantity = 0;
                await UpdateQuantityOnServer(item);
            }
        }
    }

    private async Task UpdateQuantityOnServer(CartItem item)
    {
        int userId = Preferences.Get("UserId", 0);

        var dto = new CartDTO
        {
            UserId = userId,
            ProductVariantId = item.ProductVariantId,
            Quantity = item.Quantity
        };

        bool success = await _apiService.UpdateCartQuantityAsync(dto);

        if (success)
        {
            // Перезагружаем список, чтобы обновить UI
            await LoadCartItems();
        }
        else
        {
            await DisplayAlert("Ошибка", "не удалось обновить корзину", "ОК");
        }
    }
}