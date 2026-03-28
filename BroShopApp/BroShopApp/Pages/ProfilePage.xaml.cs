using BroShopApp.Services;
using System.Xml;

namespace BroShopApp.Pages;

public partial class ProfilePage : ContentPage
{
    public ProfilePage()
    {
        InitializeComponent();

        // Заполняем данные из глобального хранилища
        var user = UserService.CurrentUser;
        if (user != null)
        {
            NameLabel.Text = user.FullName;
            LoginLabel.Text = user.Login;
            EmailLabel.Text = user.Email;
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
}