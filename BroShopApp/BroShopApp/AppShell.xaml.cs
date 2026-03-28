using BroShopApp.Pages;

namespace BroShopApp
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            // Регистрация пути к деталям
            Routing.RegisterRoute(nameof(ProductDetailsPage), typeof(ProductDetailsPage));
        }
    }
}
