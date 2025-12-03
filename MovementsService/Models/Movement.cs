using System;

namespace MovementService.Models
{
    public class Movement
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int SupplierId { get; set; }
        public int Quantity { get; set; }
        public DateTime Date { get; set; } = DateTime.UtcNow;

        // Agora é string para evitar erros de conversão
        public string Type { get; set; } = null!;
    }
}
