using BroShopApp.Model;
using BroShopApp.Services;

namespace BroShopApp.Pages;

public partial class AdminOrdersPage : ContentPage
{
    private readonly ApiService _apiService;

    // Храним полный список заказов для локального поиска
    private List<Order> _allOrders = new List<Order>();

    public List<string> AvailableStatuses { get; set; } = new List<string>
    {
        "В обработке",
        "Отправлен",
        "Доставлен",
        "Отменен"
    };

    public AdminOrdersPage()
    {
        InitializeComponent();
        _apiService = new ApiService();
        BindingContext = this;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadAllOrders();
    }

    private async Task LoadAllOrders()
    {
        OrdersRefreshView.IsRefreshing = true;

        // Загружаем данные из API
        _allOrders = await _apiService.GetAllOrdersAsync();

        // Применяем фильтр (если в строке поиска уже что-то есть при обновлении)
        FilterOrders(OrderSearchBar.Text);

        OrdersRefreshView.IsRefreshing = false;
    }

    private async void OnRefreshing(object sender, EventArgs e)
    {
        await LoadAllOrders();
    }

    // Обработчик изменения текста в SearchBar
    private void OnSearchTextChanged(object sender, TextChangedEventArgs e)
    {
        FilterOrders(e.NewTextValue);
    }

    // Метод локальной фильтрации
    private void FilterOrders(string searchText)
    {
        if (string.IsNullOrWhiteSpace(searchText))
        {
            // Если поиск пуст, показываем всё
            OrdersCollectionView.ItemsSource = _allOrders;
        }
        else
        {
            searchText = searchText.ToLower(); // Приводим к нижнему регистру для поиска

            // Ищем совпадения по номеру заказа ИЛИ имени клиента
            var filteredList = _allOrders.Where(o =>
                o.OrderId.ToString().Contains(searchText) ||
                (!string.IsNullOrEmpty(o.UserName) && o.UserName.ToLower().Contains(searchText))
            ).ToList();

            OrdersCollectionView.ItemsSource = filteredList;
        }
    }

    private async void OnSaveStatusClicked(object sender, EventArgs e)
    {
        var button = sender as Button;
        if (button?.CommandParameter is Order order)
        {
            button.IsEnabled = false;

            bool success = await _apiService.UpdateOrderStatusAsync(order.OrderId, order.Status);

            if (success)
            {
                await DisplayAlert("Успех", $"Статус заказа #{order.OrderId} обновлен", "OK");
            }
            else
            {
                await DisplayAlert("Ошибка", "Не удалось обновить статус. Проверьте подключение.", "OK");
            }

            button.IsEnabled = true;
        }
    }

    private async void OnBackClicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
}