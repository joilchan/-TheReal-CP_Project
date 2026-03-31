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

    private async void OnCancelClicked(object sender, EventArgs e)
    {
        // Объявляем кнопку один раз для всего метода
        var cancelButton = sender as Button;

        bool confirm = await DisplayAlert("Отмена заказа", "Вы уверены, что хотите отменить этот заказ?", "Да", "Нет");

        if (confirm)
        {
            // Выключаем кнопку, чтобы избежать спама нажатиями
            if (cancelButton != null) cancelButton.IsEnabled = false;

            var success = await _apiService.UpdateOrderStatusAsync(_currentOrder.OrderId, "Отменен");

            if (success)
            {
                _currentOrder.Status = "Отменен";
                await DisplayAlert("Готово", "Заказ успешно отменен", "OK");
                await Navigation.PopModalAsync();
            }
            else
            {
                await DisplayAlert("Ошибка", "Не удалось отменить заказ. Попробуйте позже.", "OK");

                // Если произошла ошибка, возвращаем кнопке активность
                if (cancelButton != null) cancelButton.IsEnabled = true;
            }
        }
    }
}