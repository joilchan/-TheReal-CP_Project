using BroShopApp.Model;
using BroShopApp.Services;

namespace BroShopApp.Pages;

public partial class OrderDetailsPage : ContentPage
{
    private readonly ApiService _apiService = new ApiService();
    private readonly Order _currentOrder;
    public OrderDetailsPage(Order order)
	{
		InitializeComponent();
        _currentOrder = order;
        // Привязываем данные самого заказа к странице (для шапки)
        BindingContext = _currentOrder;

        LoadDetails(order.OrderId);
    }

    private async void LoadDetails(int orderId)
    {
        var details = await _apiService.GetOrderDetailsAsync(orderId);
        DetailsCollection.ItemsSource = details;
    }

    private async void OnCloseClicked(object sender, EventArgs e) => await Navigation.PopModalAsync();
}