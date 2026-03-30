using BroShopApp.Services;

namespace BroShopApp.Pages;

public partial class MenuPage : ContentPage
{
    public MenuPage()
    {
        InitializeComponent();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        CheckAccess();
    }

    private void CheckAccess()
    {
        if (UserService.IsLoggedIn)
        {
            // Показываем админ-секцию только если RoleId == 1 (админ)
            AdminSection.IsVisible = UserService.CurrentUser.RoleId == 1;
        }
        else
        {
            AdminSection.IsVisible = false;
        }
    }

    private async void OnProfileClicked(object sender, EventArgs e) =>
        await Navigation.PushAsync(new ProfilePage());

    private async void OnCartClicked(object sender, EventArgs e) =>
        await Navigation.PushAsync(new CartPage());

    private async void OnSecurityClicked(object sender, EventArgs e) =>
        await DisplayAlert("Безопасность", "Раздел смены пароля в разработке", "OK");

    private async void OnSettingsClicked(object sender, EventArgs e) =>
        await DisplayAlert("Настройки", "Настройки приложения будут здесь", "OK");

    // Методы админа
    private async void OnManageOrdersClicked(object sender, EventArgs e) =>
        await DisplayAlert("Админ", "Переход к управлению заказами", "OK");

    private async void OnAccountingClicked(object sender, EventArgs e) =>
        await DisplayAlert("Админ", "Переход к учету и статистике", "OK");

    private async void OnLogoutClicked(object sender, EventArgs e)
    {
        bool confirm = await DisplayAlert("Выход", "Вы уверены, что хотите выйти?", "Да", "Нет");
        if (confirm)
        {
            UserService.Logout(); // Метод в твоем UserService для очистки данных
            await Navigation.PopToRootAsync();
        }
    }

    private async void OnBackClicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
}