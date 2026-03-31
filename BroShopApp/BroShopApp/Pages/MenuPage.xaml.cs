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
        var user = UserService.CurrentUser;
        if (user != null)
        {
            // 1. Если админ (Role 1) - видит ВСЁ в админ-секции
            if (user.RoleId == 1)
            {
                AdminSection.IsVisible = true;
                ManageOrdersRow.IsVisible = true;
                OrderSeparator.IsVisible = true;
            }
            // 2. Если менеджер (Role 3) - видит только статистику
            else if (user.RoleId == 3)
            {
                AdminSection.IsVisible = true;
                ManageOrdersRow.IsVisible = false; // Скрываем заказы
                OrderSeparator.IsVisible = false; // Скрываем полоску
            }
            // 3. Обычный пользователь
            else
            {
                AdminSection.IsVisible = false;
            }
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
        await Navigation.PushAsync(new AdminOrdersPage());

    private async void OnAccountingClicked(object sender, EventArgs e) =>
        await Navigation.PushAsync(new AdminStatisticsPage());

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