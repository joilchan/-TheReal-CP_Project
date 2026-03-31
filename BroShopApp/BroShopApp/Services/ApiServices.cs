using BroShopApp.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using static BroShopApp.Model.Product;

namespace BroShopApp.Services
{
    public class ApiService
    {

        private string GetBaseUrl()
        {
            if (DeviceInfo.Platform == DevicePlatform.Android)
            {
                // Если эмулятор - 10.0.2.2. Если реальный телефон - ваш IPv4!
                // Замените 192.168.1.XX на ваш IPv4 (узнать через cmd -> ipconfig)
                //return "http://10.0.2.2:5281/api/";
                return "http://192.168.0.3:5281/api/"; // ДЛЯ ТЕЛЕФОНА
            }
            return "http://localhost:5281/api/"; // ДЛЯ WINDOWS
        }

        private readonly HttpClient _httpClient;

        public ApiService()
        {
            _httpClient = new HttpClient { BaseAddress = new Uri(GetBaseUrl()) };
        }

        public async Task<List<Product>> GetProductsAsync()
        {
            try
            {
                // Добавляем настройки для десериализации
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                // Используем GetStringAsync + Deserialize вместо GetFromJsonAsync для гибкости
                var response = await _httpClient.GetStringAsync("products");
                return JsonSerializer.Deserialize<List<Product>>(response, options) ?? new List<Product>();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"ERROR {0}", ex.Message);
                return new List<Product>();
            }
        }

        public async Task<List<Brand>> GetBrandsAsync()
        {
            try
            {
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var response = await _httpClient.GetStringAsync("brands"); // Убедитесь, что в API есть такой эндпоинт
                return JsonSerializer.Deserialize<List<Brand>>(response, options) ?? new List<Brand>();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"Ошибка получения брендов: {0}", ex.Message);
                return new List<Brand>();
            }
        }

        public async Task<List<ProductType>> GetProductTypesAsync()
        {
            try
            {
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

                // "producttypes" — это имя контроллера (ProductTypesController)
                var response = await _httpClient.GetStringAsync("producttypes");

                return JsonSerializer.Deserialize<List<ProductType>>(response, options) ?? new List<ProductType>();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"Ошибка при получении типов: {0}", ex.Message);
                return new List<ProductType>();
            }
        }

        public async Task<User> LoginAsync(string identifier, string password)
        {
            try
            {
                var loginData = new LoginRequest { Identifier = identifier, Password = password };
                var response = await _httpClient.PostAsJsonAsync("users/login", loginData);

                if (response.IsSuccessStatusCode)
                {
                    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    return await response.Content.ReadFromJsonAsync<User>(options);
                }
                return null;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Критическая ошибка API: {ex.Message}");
                return null;
            }
        }

        public async Task<bool> AddToCartAsync(CartDTO cartDto)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("carts", cartDto);
                return response.IsSuccessStatusCode;
            }
            catch { return false; }
        }

        public async Task<List<CartItem>> GetCartAsync(int userId)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<List<CartItem>>($"carts/{userId}");
            }
            catch { return new List<CartItem>(); }
        }

        public async Task<bool> UpdateCartQuantityAsync(CartDTO cartDto)
        {
            try
            {
                // Используем PutAsJsonAsync, так как в контроллере мы прописали [HttpPut]
                var response = await _httpClient.PutAsJsonAsync("carts/update-quantity", cartDto);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Ошибка обновления количества: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> AddReviewAsync(Review review)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("reviews", review);

                if (!response.IsSuccessStatusCode)
                {
                    // Читаем, что именно ответил сервер (там будет текст ошибки)
                    string errorContent = await response.Content.ReadAsStringAsync();
                    Debug.WriteLine($"СЕРВЕР ВЕРНУЛ ОШИБКУ: {response.StatusCode} - {errorContent}");
                }

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"КРИТИЧЕСКАЯ ОШИБКА: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> CreateOrderAsync(CreateOrderRequest request)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("orders", request);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Ошибка при оформлении заказа: {ex.Message}");
                return false;
            }
        }

        public async Task<List<Order>> GetUserOrdersAsync(int userId)
        {
            try
            {
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var response = await _httpClient.GetStringAsync($"orders/user/{userId}");
                return JsonSerializer.Deserialize<List<Order>>(response, options) ?? new List<Order>();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Ошибка получения заказов: {ex.Message}");
                return new List<Order>();
            }
        }

        public async Task<List<OrderDetail>> GetOrderDetailsAsync(int orderId)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<List<OrderDetail>>($"orders/details/{orderId}");
            }
            catch { return new List<OrderDetail>(); }
        }

        public async Task<List<Order>> GetAllOrdersAsync()
        {
            try
            {
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var response = await _httpClient.GetStringAsync("orders/all");
                return JsonSerializer.Deserialize<List<Order>>(response, options) ?? new List<Order>();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Ошибка получения всех заказов: {ex.Message}");
                return new List<Order>();
            }
        }

        public async Task<bool> UpdateOrderStatusAsync(int orderId, string newStatus)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync($"orders/status/{orderId}", new { status = newStatus });
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Ошибка обновления статуса: {ex.Message}");
                return false;
            }
        }

        public async Task<StoreStatistics> GetStatisticsAsync()
        {
            try
            {
                // Убираем "api/", так как оно уже есть в BaseAddress
                var response = await _httpClient.GetAsync("statistics");

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    return JsonSerializer.Deserialize<StoreStatistics>(content, options);
                }
                else
                {
                    // Это поможет увидеть ошибку в консоли вывода (Output), если она будет
                    Debug.WriteLine($"Ошибка статистики: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Ошибка при получении статистики: {ex.Message}");
            }

            return new StoreStatistics();
        }
    }
}
