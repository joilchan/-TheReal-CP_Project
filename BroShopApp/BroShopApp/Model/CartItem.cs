using System;
using System.Collections.Generic;
using System.Text;

namespace BroShopApp.Model
{
    public partial class CartItem
    {
        public int ProductVariantId { get; set; }
        public int Quantity { get; set; }
        public string ProductName { get; set; }
        public string Size { get; set; }
        public decimal Price { get; set; }
        public string Image { get; set; }
    }

    // Для отправки на сервер
    public partial class CartDTO
    {
        public int UserId { get; set; }
        public int ProductVariantId { get; set; }
        public int Quantity { get; set; }
    }
}
