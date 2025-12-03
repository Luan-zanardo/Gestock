namespace MovementService.Models
{
    public class Movement
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int SupplierId { get; set; }

        public int Quantity { get; set; }
        public string Type { get; set; } = "Entrada"; // Entrada ou Saída

        public DateTime Date { get; set; } = DateTime.Now;
    }
}