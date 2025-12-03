using Microsoft.EntityFrameworkCore;
using MovementService.Data;
using MovementService.Services;

var builder = WebApplication.CreateBuilder(args);

// Adiciona logging padrão
builder.Services.AddLogging();

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// DbContext (verifique connection string em appsettings.json)
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// HttpClients com BaseAddress - CONFIRA as URIs e ajuste se necessário.
builder.Services.AddHttpClient<ProductsClient>(c =>
{
    c.BaseAddress = new Uri(builder.Configuration["ProductsApi:BaseUrl"] ?? "http://localhost:5184/");
});

builder.Services.AddHttpClient<SuppliersClient>(c =>
{
    c.BaseAddress = new Uri(builder.Configuration["SuppliersApi:BaseUrl"] ?? "http://localhost:5122/");
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();
app.MapControllers();
app.Run();
