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
        string email = LoginEntry.Text;
        string password = PasswordEntry.Text;

        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            await DisplayAlert("Ошибка", "Введите email и пароль", "OK");
            return;
        }

        var button = (Button)sender;
        button.IsEnabled = false;
        button.Text = "ЗАГРУЗКА...";

        var user = await _apiService.LoginAsync(email, password);

        button.IsEnabled = true;
        button.Text = "ВОЙТИ";

        if (user != null)
        {
            // 1. Сохраняем в оперативную память (как ты и делал)
            BroShopApp.Services.UserService.CurrentUser = user;

            // 2. СОХРАНЯЕМ В ПОСТОЯННУЮ ПАМЯТЬ ТЕЛЕФОНА
            // Теперь даже после перезагрузки мы будем знать, кто это
            Preferences.Set("UserId", user.UserId);
            Preferences.Set("UserName", user.FullName); // Можно еще имя сохранить для приветствия

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

    private void OnForgotPasswordTapped(object sender, TappedEventArgs e)
    {

    }
}