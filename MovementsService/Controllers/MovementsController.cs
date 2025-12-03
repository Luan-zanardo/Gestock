using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MovementService.Data;
using MovementService.Models;
using MovementService.Services;

namespace MovementService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MovementsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ProductsClient _productsClient;
        private readonly SuppliersClient _suppliersClient;
        private readonly ILogger<MovementsController> _logger;

        public MovementsController(
            AppDbContext context,
            ProductsClient productsClient,
            SuppliersClient suppliersClient,
            ILogger<MovementsController> logger)
        {
            _context = context;
            _productsClient = productsClient;
            _suppliersClient = suppliersClient;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var movements = await _context.Movements.ToListAsync();
            return Ok(movements);
        }

        // GET by id para usar no CreatedAtAction
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var movement = await _context.Movements.FindAsync(id);
            if (movement == null) return NotFound();
            return Ok(movement);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Movement movement)
        {
            if (movement == null)
                return BadRequest("Movimentação inválida.");

            // Valida fields básicos
            if (movement.ProductId <= 0)
                return BadRequest("ProductId inválido.");

            if (movement.SupplierId <= 0)
                return BadRequest("SupplierId inválido.");

            if (movement.Quantity <= 0)
                return BadRequest("Quantity deve ser maior que zero.");

            if (string.IsNullOrWhiteSpace(movement.Type))
                return BadRequest("Type é obrigatório. Use 'Entrada' ou 'Saida'.");

            // Normaliza type e valida valores permitidos
            var typeNormalized = movement.Type.Trim();
            var isEntrada = typeNormalized.Equals("Entrada", StringComparison.OrdinalIgnoreCase);
            var isSaida  = typeNormalized.Equals("Saida", StringComparison.OrdinalIgnoreCase);

            if (!isEntrada && !isSaida)
                return BadRequest("Type inválido. Use 'Entrada' ou 'Saida'.");

            // Busca produto remoto
            Product? product;
            try
            {
                product = await _productsClient.GetProduct(movement.ProductId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar produto {ProductId}.", movement.ProductId);
                return StatusCode(502, "Erro ao contactar serviço de produtos.");
            }

            if (product == null)
                return NotFound("Produto não encontrado");

            // Busca fornecedor remoto
            Supplier? supplier;
            try
            {
                supplier = await _suppliersClient.GetSupplier(movement.SupplierId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar fornecedor {SupplierId}.", movement.SupplierId);
                return StatusCode(502, "Erro ao contactar serviço de fornecedores.");
            }

            if (supplier == null)
                return NotFound("Fornecedor não encontrado");

            // Validação de quantidade para saida
            if (isSaida && product.Quantity < movement.Quantity)
                return BadRequest("Quantidade insuficiente no estoque");

            // Ajusta o estoque no produto remoto
            if (isEntrada)
                product.Quantity += movement.Quantity;
            else
                product.Quantity -= movement.Quantity;

            // Atualiza produto remoto
            try
            {
                await _productsClient.UpdateProduct(product);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Falha ao atualizar estoque do produto {ProductId}.", product.Id);
                return StatusCode(502, "Falha ao atualizar produto no serviço remoto.");
            }

            // Persistir movimentação localmente
            // Salva o Type normalizado (por exemplo, "Entrada" ou "Saida")
            movement.Type = isEntrada ? "Entrada" : "Saida";

            _context.Movements.Add(movement);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = movement.Id }, movement);
        }
    }
}
