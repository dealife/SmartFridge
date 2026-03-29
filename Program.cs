using Microsoft.EntityFrameworkCore;
using SmartFridgeAPI.Data;

var builder = WebApplication.CreateBuilder(args);

// 1. Настройка сервисов
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=smartfridge.db"));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options => {
    options.AddPolicy("AllowAll", policy => {
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    });
});
var app = builder.Build();

// --- МАГИЯ: Автоматическое создание базы без миграций ---
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    // Эта строка создаст файл .db и все таблицы автоматически при старте
    db.Database.EnsureCreated();
}
// -------------------------------------------------------

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthorization();
app.MapControllers();

app.Run();