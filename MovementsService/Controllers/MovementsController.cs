using Microsoft.AspNetCore.Mvc;
using MovementService.Data;
using MovementService.Models;
using MovementService.Services;
using Microsoft.EntityFrameworkCore;

namespace MovementService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MovementsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ProductsClient _products;
        private readonly SuppliersClient _suppliers;

        public MovementsController(AppDbContext context, ProductsClient products, SuppliersClient suppliers)
        {
            _context = context;
            _products = products;
            _suppliers = suppliers;
        }

        // GET ALL
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Movement>>> Get()
        {
            return await _context.Movements.ToListAsync();
        }

        // CREATE MOVEMENT
        [HttpPost]
        public async Task<ActionResult> CreateMovement(Movement movement)
        {
            // 1️⃣ Verificar se o produto existe
            var product = await _products.GetProductById(movement.ProductId);
            if (product == null) return BadRequest("Produto não encontrado.");

            // 2️⃣ Verificar se o fornecedor existe
            var supplier = await _suppliers.GetSupplierById(movement.SupplierId);
            if (supplier == null) return BadRequest("Fornecedor não encontrado.");

            // 3️⃣ Registrar movimentação
            _context.Movements.Add(movement);
            await _context.SaveChangesAsync();

            // 4️⃣ Atualizar quantidade do produto no ProductsService
            int currentQuantity = product.quantity;
            int finalQuantity = movement.Type == "Entrada" ? currentQuantity + movement.Quantity
                                                           : currentQuantity - movement.Quantity;

            await _products.UpdateProductQuantity(movement.ProductId, finalQuantity);

            return Ok(new
            {
                Message = "Movimentação registrada e estoque atualizado.",
                Movement = movement
            });
        }
    }
}
