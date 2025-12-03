using System.Net.Http.Json;

namespace MovementService.Services
{
    public class SuppliersClient
    {
        private readonly HttpClient _http;

        public SuppliersClient(HttpClient http)
        {
            _http = http;
        }

        public async Task<dynamic?> GetSupplierById(int id)
        {
            return await _http.GetFromJsonAsync<dynamic>($"http://localhost:5002/api/suppliers/{id}");
        }
    }
}
