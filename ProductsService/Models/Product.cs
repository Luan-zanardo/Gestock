namespace ProductsService.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Quantity { get; set; }
        public string SupplierId { get; set; }  // utilizado nas integrações
    }
}
