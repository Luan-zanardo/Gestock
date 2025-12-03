using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MovementService.Models;

namespace MovementService.Services
{
    public class ProductsClient
    {
        private readonly HttpClient _http;
        private readonly ILogger<ProductsClient> _logger;

        public ProductsClient(HttpClient http, ILogger<ProductsClient> logger)
        {
            _http = http;
            _logger = logger;
        }

        public async Task<Product?> GetProduct(int id)
        {
            try
            {
                var url = $"products/{id}";
                _logger.LogDebug("Chamando Products GET {Url}", new { Url = new Uri(_http.BaseAddress!, url) });

                var response = await _http.GetAsync(url);

                if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    _logger.LogInformation("Produto {Id} não encontrado (404).", id);
                    return null;
                }

                response.EnsureSuccessStatusCode();

                var product = await response.Content.ReadFromJsonAsync<Product>();
                return product;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Erro de rede ao tentar obter produto {Id}.", id);
                throw;
            }
        }

        public async Task UpdateProduct(Product product)
        {
            try
            {
                var url = $"products/{product.Id}";
                _logger.LogDebug("Chamando Products PUT {Url}", new { Url = new Uri(_http.BaseAddress!, url) });

                var response = await _http.PutAsJsonAsync(url, product);

                if (!response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    _logger.LogError("Falha ao atualizar produto {Id}. Status: {Status}. Conteúdo: {Content}",
                        product.Id, response.StatusCode, content);

                    throw new Exception($"Falha ao atualizar produto remoto (status: {response.StatusCode}).");
                }
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Erro de rede ao atualizar produto {Id}.", product.Id);
                throw;
            }
        }
    }
}
