using BroShopApp.Services;

namespace BroShopApp.Pages;

public partial class AdminStatisticsPage : ContentPage
{
    private readonly ApiService _apiService = new ApiService();

    public AdminStatisticsPage()
    {
        InitializeComponent();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadStatistics();
    }

    private async Task LoadStatistics()
    {
        var stats = await _apiService.GetStatisticsAsync();

        // Передаем полученные данные в XAML
        BindingContext = stats;
    }

    private async void OnBackClicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
}