using System.Net.Http.Json;

namespace MovementService.Services
{
    public class ProductsClient
    {
        private readonly HttpClient _http;

        public ProductsClient(HttpClient http)
        {
            _http = http;
        }

        public async Task<dynamic?> GetProductById(int id)
        {
            return await _http.GetFromJsonAsync<dynamic>($"http://localhost:5001/api/products/{id}");
        }

        public async Task UpdateProductQuantity(int id, int newQuantity)
        {
            var body = new { quantity = newQuantity };

            await _http.PutAsJsonAsync($"http://localhost:5001/api/products/updateQuantity/{id}", body);
        }
    }
}
