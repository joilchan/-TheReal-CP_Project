namespace BroShopApp.Model
{
    public class Order
    {
        public int OrderId { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string Address { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal Amount { get; set; }
        public decimal DeliveryCost { get; set; }
        public string Status { get; set; }

        // Вспомогательное свойство для красивого отображения в XAML
        public string DisplayTotal => $"{(Amount + DeliveryCost):N0} ₽";
        public string DisplayDate => OrderDate.ToString("dd.MM.yyyy HH:mm");
    }

    public class CreateOrderRequest
    {
        public int UserId { get; set; }
        public string Address { get; set; }

        public List<int> SelectedVariantIds { get; set; }
    }
}