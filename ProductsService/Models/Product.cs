namespace ProductsService.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public int SupplierId { get; set; } // ou string se você preferir
    }
}
