using BroShopApp.Model;
using BroShopApp.Services;

namespace BroShopApp.Pages;

//// Передача объекта Product через навигацию
[QueryProperty(nameof(Product), "Product")]
public partial class ProductDetailsPage : ContentPage
{
    private Product _product;
    private readonly ApiService _apiService;

    public Product Product
    {
        get => _product;
        set
        {
            _product = value;
            OnPropertyChanged();
            BindingContext = _product;
        }
    }

    public ProductDetailsPage()
    {
        // Теперь InitializeComponent() заработает, так как XAML корректен
        InitializeComponent();
        _apiService = new ApiService();
    }

    private async void OnBackClicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
}

