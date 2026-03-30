using System;
using System.Collections.Generic;
using System.Text;

namespace BroShopApp.Model
{
    public class OrderDetail
    {
        public string ProductName { get; set; }
        public string ImageUrl { get; set; }
        public string Size { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public string DisplayPrice => $"{Price:N0} ₽";
    }
}
