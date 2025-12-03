using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MovementService.Models;

namespace MovementService.Services
{
    public class SuppliersClient
    {
        private readonly HttpClient _http;
        private readonly ILogger<SuppliersClient> _logger;

        public SuppliersClient(HttpClient http, ILogger<SuppliersClient> logger)
        {
            _http = http;
            _logger = logger;
        }

        public async Task<Supplier?> GetSupplier(int id)
        {
            try
            {
                var url = $"suppliers/{id}";
                _logger.LogDebug("Chamando Suppliers GET {Url}", new { Url = new Uri(_http.BaseAddress!, url) });

                var response = await _http.GetAsync(url);

                if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    _logger.LogInformation("Fornecedor {Id} não encontrado (404).", id);
                    return null;
                }

                response.EnsureSuccessStatusCode();

                var supplier = await response.Content.ReadFromJsonAsync<Supplier>();
                return supplier;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Erro de rede ao tentar obter fornecedor {Id}.", id);
                throw;
            }
        }
    }
}
