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
}