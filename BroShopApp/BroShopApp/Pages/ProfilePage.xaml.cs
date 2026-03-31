using BroShopApp.Model;
using BroShopApp.Services;
using System.Xml;

namespace BroShopApp.Pages;

public partial class ProfilePage : ContentPage
{
    private readonly ApiService _apiService;
    public ProfilePage()
    {
        InitializeComponent();
        _apiService = new ApiService();

        // Заполняем данные из глобального хранилища
        var user = UserService.CurrentUser;
        if (user != null)
        {
            NameLabel.Text = user.FullName;
            LoginLabel.Text = user.Login;
            EmailLabel.Text = user.Email;
        }
    }

    // Подгружаем заказы при каждом открытии страницы профиля
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadOrders();
    }

    private async Task LoadOrders()
    {
        int userId = Preferences.Get("UserId", 0);
        if (userId > 0)
        {
            var orders = await _apiService.GetUserOrdersAsync(userId);
            OrdersCollection.ItemsSource = orders;
            OrdersCountLabel.Text = $"Всего: {orders.Count}";
        }
    }

    private async void OnLogoutClicked(object sender, EventArgs e)
    {
        UserService.Logout();
        await Navigation.PopAsync();
    }

    private async void OnBackClicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }

    private async void OnOrderSelected(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is Order selectedOrder)
        {
            ((CollectionView)sender).SelectedItem = null;

            // Передаем весь объект заказа
            await Navigation.PushModalAsync(new OrderDetailsPage(selectedOrder));
        }
    }

    private async void OnCancelOrderClicked(object sender, EventArgs e)
    {
        var button = sender as ImageButton;
        var order = button?.CommandParameter as Order;

        if (order == null) return;

        // Подтверждение важно, так как крестик нажать легче, чем большую кнопку
        bool confirm = await DisplayAlert("Отмена", $"Отменить заказ #{order.OrderId}?", "Да", "Нет");

        if (confirm)
        {
            var success = await _apiService.UpdateOrderStatusAsync(order.OrderId, "Отменен");

            if (success)
            {
                // Обновляем статус в локальном объекте (если есть INotifyPropertyChanged)
                // или просто обновляем список
                await LoadOrders();
            }
            else
            {
                await DisplayAlert("Ошибка", "Не удалось отменить заказ", "OK");
            }
        }
    }
}