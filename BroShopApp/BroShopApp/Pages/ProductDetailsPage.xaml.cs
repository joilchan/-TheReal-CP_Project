using BroShopApp.Model;
using BroShopApp.Services;
using static BroShopApp.Model.Product;

namespace BroShopApp.Pages;

//// Передача объекта Product через навигацию
[QueryProperty(nameof(Product), "Product")]
public partial class ProductDetailsPage : ContentPage
{
    private Product _product;
    private readonly ApiService _apiService;
    private ProductVariant _selectedVariant;
    public bool IsSizeVisible => Product?.ProductVariants != null && Product.ProductVariants.Count > 1;

    public Product Product
    {
        get => _product;
        set
        {
            _product = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(IsSizeVisible));
            BindingContext = _product;
        }
    }

    public ProductDetailsPage()
    { 
        InitializeComponent();    
        CheckAuthStatus();
        _apiService = new ApiService();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        CheckAuthStatus();
    }

    private void CheckAuthStatus()
    {
        // Проверяем наличие сохраненного UserId
        int userId = Preferences.Get("UserId", 0);

        if (userId > 0)
        {
            ReviewSection.IsVisible = true;
            GuestSection.IsVisible = false;
        }
        else
        {
            ReviewSection.IsVisible = false;
            GuestSection.IsVisible = true;
        }
    }

    private async void OnLoginRedirectClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new LoginPage());
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
        int userId = Preferences.Get("UserId", 0);
        if (userId == 0)
        {
            await DisplayAlert("Ошибка", "Нужна авторизация", "ОК");
            return;
        }

        // 2. Получаем список вариантов (например, список размеров)
        var variants = Product?.ProductVariants?.ToList();
        int targetVariantId = 0;

        if (variants == null || !variants.Any())
        {
            await DisplayAlert("Ошибка", "Товара нет в наличии (нет вариантов в БД)", "ОК");
            return;
        }

        // 3. Условие автоматического выбора
        if (variants.Count == 1)
        {
            // Если вариант один (как у часов Casio), берем его молча
            targetVariantId = variants[0].ProductVariantId;
        }
        else
        {
            // Если вариантов много (как у зипки Balenciaga), проверяем, выбрал ли пользователь что-то
            if (_selectedVariant == null)
            {
                await DisplayAlert("Внимание", "Пожалуйста, выберите размер", "ОК");
                return;
            }
            targetVariantId = _selectedVariant.ProductVariantId;
        }

        // 4. Отправка DTO на сервер
        var cartDto = new CartDTO
        {
            UserId = userId,
            ProductVariantId = targetVariantId,
            Quantity = 1
        };

        bool isSuccess = await _apiService.AddToCartAsync(cartDto);

        if (isSuccess)
        {
            await DisplayAlert("Успех", "Товар в корзине!", "Отлично");
        }
        else
        {
            await DisplayAlert("Ошибка", "Не удалось добавить. Проверьте сервер.", "ОК");
        }
    }

    private async void OnSendReviewClicked(object sender, EventArgs e)
    {
        try
        {
            // 1. Проверяем авторизацию
            int userId = Preferences.Get("UserId", 0);
            if (userId == 0)
            {
                await DisplayAlert("Упс", "Нужно войти в аккаунт, чтобы оставить отзыв", "ОК");
                return;
            }

            // 2. Проверяем, не пустой ли текст
            if (string.IsNullOrWhiteSpace(ReviewEditor.Text))
            {
                await DisplayAlert("Внимание", "Напишите текст отзыва", "ОК");
                return;
            }

            var review = new Review
            {
                ProductId = Product.ProductId,
                UserId = userId,
                Text = ReviewEditor.Text,
                Rating = int.TryParse(RatingEntry.Text, out int r) ? r : 5,
            };

            // Блокируем кнопку на время отправки
            var btn = (Button)sender;
            btn.IsEnabled = false;

            bool success = await _apiService.AddReviewAsync(review);

            btn.IsEnabled = true;

            if (success)
            {
                await DisplayAlert("Спасибо!", "Отзыв опубликован", "ОК");
                ReviewEditor.Text = "";
            }
            else
            {
                await DisplayAlert("Ошибка", "Сервер не принял отзыв. Проверьте подключение.", "ОК");
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Ошибка при нажатии: {ex.Message}");
            await DisplayAlert("Критическая ошибка", ex.Message, "ОК");
        }
    }
}

