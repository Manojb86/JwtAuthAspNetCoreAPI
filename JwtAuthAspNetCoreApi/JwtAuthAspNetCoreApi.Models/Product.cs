namespace JwtAuthAspNetCoreApi.Models
{
    public class Product : BaseModel
    {
        public int ProductId { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public string Color { get; set; }
        public int AvailableQuantity { get; set; }
        public double UnitPrice { get; set; }
    }
}
