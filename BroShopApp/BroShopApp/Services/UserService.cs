using BroShopApp.Model;

namespace BroShopApp.Services
{
    public static class UserService
    {
        // Здесь будет лежать наш пользователь после входа
        public static User CurrentUser { get; set; }

        // Проверка: вошел ли кто-то?
        public static bool IsLoggedIn => CurrentUser != null;

        // Метод для выхода
        public static void Logout()
        {
            Preferences.Remove("UserId");
            CurrentUser = null;
        }
    }
}
