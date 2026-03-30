using BroShopApp.Services;
using BroShopApp.Model;
using System.Collections.ObjectModel;

namespace BroShopApp.Pages;

public partial class CartPage : ContentPage
{
    private readonly ApiService _apiService;

    public CartPage()
    {
        InitializeComponent();
        _apiService = new ApiService();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadCartItems();
    }

    private async Task LoadCartItems()
    {
        try
        {
            int userId = Preferences.Get("UserId", 0);

            if (userId == 0)
            {
                CartCollection.ItemsSource = null;
                return;
            }

            // Загружаем данные из API
            var items = await _apiService.GetCartAsync(userId);

            // Привязываем к списку
            CartCollection.ItemsSource = items;

            // Обновляем итоговую сумму (только для выбранных)
            UpdateTotalAmount();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Ошибка", "Не удалось загрузить корзину", "ОК");
        }
    }

    // Метод для пересчета суммы на основе состояния IsSelected каждого товара
    private void UpdateTotalAmount()
    {
        var items = CartCollection.ItemsSource as IEnumerable<CartItem>;
        if (items == null) return;

        decimal total = items
            .Where(i => i.IsSelected)
            .Sum(i => i.Quantity * i.Price);

        TotalAmountLabel.Text = $"{total:N0} ₽";
    }

    // Обработчик изменения состояния CheckBox в XAML
    private void OnIsSelectedChanged(object sender, CheckedChangedEventArgs e)
    {
        UpdateTotalAmount();
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
            await LoadCartItems();
        }
        else
        {
            await DisplayAlert("Ошибка", "Не удалось обновить корзину", "ОК");
        }
    }

    private async void OnCheckoutClicked(object sender, EventArgs e)
    {
        var allItems = CartCollection.ItemsSource as IEnumerable<CartItem>;

        // Фильтруем только те товары, у которых стоит галочка
        var selectedItems = allItems?.Where(i => i.IsSelected).ToList();

        if (selectedItems == null || !selectedItems.Any())
        {
            await DisplayAlert("Корзина", "Выберите хотя бы один товар для оформления заказа", "ОК");
            return;
        }

        string address = await DisplayPromptAsync("Оформление заказа", "Введите адрес доставки:", "ОК", "Отмена", "г. Москва, ул. Пушкина, д. 1");

        if (string.IsNullOrWhiteSpace(address))
            return;

        int userId = Preferences.Get("UserId", 0);

        // Формируем запрос, передавая список ID выбранных вариантов
        var request = new CreateOrderRequest
        {
            UserId = userId,
            Address = address,
            SelectedVariantIds = selectedItems.Select(i => i.ProductVariantId).ToList()
        };

        bool success = await _apiService.CreateOrderAsync(request);

        if (success)
        {
            await DisplayAlert("Успех", "Ваш заказ успешно оформлен!", "ОК");
            await LoadCartItems();
        }
        else
        {
            await DisplayAlert("Ошибка", "Не удалось оформить заказ. Попробуйте позже.", "ОК");
        }
    }

    private async void OnBackClicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
}