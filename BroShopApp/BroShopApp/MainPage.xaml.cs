using BroShopApp.Model;
using BroShopApp.Pages;
using BroShopApp.Services;

namespace BroShopApp
{
    public partial class MainPage : ContentPage
    {
        private readonly ApiService _apiService = new ApiService();

        private List<Product> _allProducts = new();

        public string CurrentUser { get; private set; } = "Гость";

        public MainPage()
        {
            InitializeComponent();
            UserStatusLabel.Text = "Гость";
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            if (BroShopApp.Services.UserService.IsLoggedIn)
            {
                UserStatusLabel.Text = BroShopApp.Services.UserService.CurrentUser.Login;
            }
            else
            {
                UserStatusLabel.Text = "Гость";
            }

            try
            {
                var products = await _apiService.GetProductsAsync();

                if (products != null)
                {
                    _allProducts = products; // ОБЯЗАТЕЛЬНО сохраняем копию здесь
                    ProductsCollection.ItemsSource = _allProducts;
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Ошибка", $"Не удалось загрузить данные: {ex.Message}", "OK");
            }
        }

        private async void OnProfileClicked(object sender, EventArgs e)
        {
            if (BroShopApp.Services.UserService.IsLoggedIn)
            {
                // Если залогинен — переходим в профиль
                await Navigation.PushAsync(new ProfilePage());
            }
            else
            {
                // Если гость — на страницу логина
                await Navigation.PushAsync(new LoginPage());
            }
        }

        // Поиск по названию
        private void OnSearchTextChanged(object sender, TextChangedEventArgs e)
        {
            var searchTerm = e.NewTextValue.ToLower();
            ProductsCollection.ItemsSource = _allProducts
                .Where(p => p.Name.ToLower().Contains(searchTerm))
                .ToList();
        }

        // Сортировка по цене (по возрастанию)
        private void OnSortPriceAscClicked(object sender, EventArgs e)
        {
            ProductsCollection.ItemsSource = _allProducts
                .OrderBy(p => p.Price)
                .ToList();
        }

        // Сортировка по цене (по убыванию)
        private void OnSortPriceDescClicked(object sender, EventArgs e)
        {
            ProductsCollection.ItemsSource = _allProducts
                .OrderByDescending(p => p.Price)
                .ToList();
        }

        private async void OnBrandFilterClicked(object sender, EventArgs e)
        {
            var brands = await _apiService.GetBrandsAsync();
            if (brands == null || !brands.Any()) return;

            var brandNames = brands.Select(b => b.Name).ToArray();
            string action = await DisplayActionSheet("Выберите бренд", "Отмена", null,
                brandNames.Append("Все").ToArray());

            if (string.IsNullOrEmpty(action) || action == "Отмена") return;

            if (action == "Все")
            {
                ProductsCollection.ItemsSource = _allProducts;
            }
            else
            {
                // Используем StringComparison для надежности
                var filtered = _allProducts
                    .Where(p => p.Brand != null &&
                                p.Brand.Name.Equals(action, StringComparison.OrdinalIgnoreCase))
                    .ToList();

                ProductsCollection.ItemsSource = filtered;
            }
        }

        private async void OnTypeFilterClicked(object sender, EventArgs e)
        {
            // Предполагается, что в ApiService есть метод GetTypesAsync
            var types = await _apiService.GetProductTypesAsync();
            if (types == null || !types.Any()) return;

            var typeNames = types.Select(t => t.Name).ToArray();
            string action = await DisplayActionSheet("Выберите тип", "Отмена", null,
                typeNames.Append("Все").ToArray());

            if (string.IsNullOrEmpty(action) || action == "Отмена") return;

            if (action == "Все")
            {
                ProductsCollection.ItemsSource = _allProducts;
            }
            else
            {
                ProductsCollection.ItemsSource = _allProducts
                    .Where(p => p.ProductType != null &&
                                p.ProductType.Name.Equals(action, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }
        }

        private async void OnProductSelected(object sender, SelectionChangedEventArgs e)
        {
            // 1. Проверяем, что объект вообще выбран
            var selectedProduct = e.CurrentSelection.FirstOrDefault() as Product;

            if (selectedProduct == null)
                return;

            try
            {
                // 2. Снимаем выделение (чтобы можно было нажать снова)
                ((CollectionView)sender).SelectedItem = null;

                // 3. Формируем параметры
                var navigationParameter = new Dictionary<string, object>
                {
                    { "Product", selectedProduct }
                };

                // 4. Пробуем перейти
                await Shell.Current.GoToAsync(nameof(ProductDetailsPage), navigationParameter);
            }
            catch (Exception ex)
            {
                // Если маршрут не зарегистрирован в AppShell, выскочит ошибка здесь
                await DisplayAlert("Ошибка навигации", ex.Message, "OK");
            }
        }

        private async void OnProductTapped(object sender, TappedEventArgs e)
        {
            // Получаем товар из параметра команды
            var selectedProduct = e.Parameter as Product;

            if (selectedProduct == null)
            {
                await DisplayAlert("Ошибка", "Товар не найден в параметрах", "OK");
                return;
            }

            try
            {
                var navigationParameter = new Dictionary<string, object>
        {
            { "Product", selectedProduct }
        };

                // Пробуем перейти
                await Shell.Current.GoToAsync(nameof(ProductDetailsPage), navigationParameter);
            }
            catch (Exception ex)
            {
                await DisplayAlert("Ошибка навигации", ex.Message, "OK");
            }
        }

        private void OnHomeClicked(object sender, EventArgs e)
        {

        }

        private void OnMenuClicked(object sender, EventArgs e)
        {

        }

        private void OnProfileClicked(object sender, TappedEventArgs e)
        {

        }

        private void OnCartClicked(object sender, EventArgs e)
        {

        }
        public void UpdateUserStatus(string login)
        {
            // Важно: MainThread гарантирует, что UI обновится мгновенно
            MainThread.BeginInvokeOnMainThread(() =>
            {
                if (string.IsNullOrEmpty(login))
                {
                    UserStatusLabel.Text = "Гость";
                }
                else
                {
                    UserStatusLabel.Text = login;
                }
            });
        }
    }
}
