using BroShopApp.Services;
using Microsoft.Extensions.Logging.Abstractions;

namespace BroShopApp.Pages;

public partial class LoginPage : ContentPage
{
    private readonly Services.ApiService _apiService;
    public LoginPage()
	{
		InitializeComponent();
        _apiService = new Services.ApiService();
    }

    private async void OnLoginSubmitClicked(object sender, EventArgs e)
    {
        string email = LoginEntry.Text; // Убедись, что дал имена x:Name полям в XAML
        string password = PasswordEntry.Text;

        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            await DisplayAlert("Ошибка", "Введите email и пароль", "OK");
            return;
        }

        // Блокируем кнопку, пока идет загрузка (чтобы не кликали дважды)
        var button = (Button)sender;
        button.IsEnabled = false;
        button.Text = "ЗАГРУЗКА...";

        // Обращаемся к нашему API
        var user = await _apiService.LoginAsync(email, password);

        button.IsEnabled = true;
        button.Text = "ВОЙТИ";

        if (user != null)
        {
            // СОХРАНЯЕМ ДАННЫЕ В СЕРВИС
            BroShopApp.Services.UserService.CurrentUser = user;

            await Navigation.PopAsync();
        }
        else
        {
            await DisplayAlert("Ошибка", "Неверный логин или пароль", "OK");
        }
    }

    private void OnRegisterTapped(object sender, TappedEventArgs e)
    {

    }

    private async void OnBackClicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
}