namespace BroShopApp.Model
{
    public partial class Product
    {
        public int ProductId { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string ImageUrl { get; set; }
        public string Description { get; set; }
        public ProductType ProductType { get; set; }
        public Brand Brand { get; set; }

        public List<ProductVariant> ProductVariants { get; set; } = new();
        public List<Review> Reviews { get; set; } = new();
    }
}
