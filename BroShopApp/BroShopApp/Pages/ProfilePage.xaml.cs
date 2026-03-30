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
}