namespace BroShopApp.Model
{
    public class StoreStatistics
    {
        public int TotalOrders { get; set; }
        public decimal TotalRevenue { get; set; }
        public int TotalUsers { get; set; }
        public int PendingOrders { get; set; }

        // Красивое отображение для UI
        public string DisplayRevenue => $"{TotalRevenue:N0} ₽";
    }
}
