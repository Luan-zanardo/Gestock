using Microsoft.EntityFrameworkCore;
using MovementService.Data;
using MovementService.Services;

var builder = WebApplication.CreateBuilder(args);

// Conexão com PostgreSQL
builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddHttpClient<ProductsClient>();
builder.Services.AddHttpClient<SuppliersClient>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();
app.Run();
