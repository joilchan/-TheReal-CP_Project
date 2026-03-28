using BroShopApp.Model;
using BroShopApp.Services;

namespace BroShopApp.Pages;

//// Передача объекта Product через навигацию
[QueryProperty(nameof(Product), "Product")]
public partial class ProductDetailsPage : ContentPage
{
    private Product _product;
    private readonly ApiService _apiService;
    private ProductVariant _selectedVariant;

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

    private void OnSizeSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        // Получаем выбранный элемент и приводим его к ProductVariant
        _selectedVariant = e.CurrentSelection.FirstOrDefault() as ProductVariant;
    }

    private async void OnAddToCartClicked(object sender, EventArgs e)
    {
        // 1. Проверяем, выбрал ли пользователь размер
        if (_selectedVariant == null)
        {
            await DisplayAlert("Внимание", "Пожалуйста, выберите размер одежды", "ОК");
            return;
        }

        // 2. Получаем ID пользователя (Предполагается, что он сохранен при входе)
        int userId = Preferences.Get("UserId", 0);

        if (userId == 0)
        {
            await DisplayAlert("Ошибка", "Для добавления в корзину необходимо авторизоваться", "ОК");
            return;
        }

        // 3. Формируем DTO для отправки
        var cartDto = new CartDTO
        {
            UserId = userId,
            ProductVariantId = _selectedVariant.ProductVariantId,
            Quantity = 1 // Добавляем по 1 штуке за клик
        };

        // 4. Отправляем запрос
        bool isSuccess = await _apiService.AddToCartAsync(cartDto);

        if (isSuccess)
        {
            await DisplayAlert("Успех", "Товар добавлен в корзину!", "Отлично");
        }
        else
        {
            await DisplayAlert("Ошибка", "Не удалось добавить товар. Проверьте подключение.", "ОК");
        }
    }
}

