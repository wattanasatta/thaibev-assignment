using Microsoft.EntityFrameworkCore;
using ThaiBev.PersonApi.Data;
using ThaiBev.PersonApi.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default")));

builder.Services.AddScoped<IPersonService, PersonService>();
builder.Services.AddSingleton(TimeProvider.System); // ผ่านเข้า service เพื่อให้ mock เวลาในเทสต์ได้

// CORS: อนุญาตเฉพาะ origin ของ Vue dev server
builder.Services.AddCors(options => options.AddPolicy("frontend", policy =>
    policy.WithOrigins("http://localhost:5173").AllowAnyHeader().AllowAnyMethod()));

builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

// Swagger UI เฉพาะ Development (ทดสอบ 3 endpoints)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseExceptionHandler();
app.UseCors("frontend");
app.MapControllers();

await using (var scope = app.Services.CreateAsyncScope())
{
    await DbInitializer.ApplyAsync(scope.ServiceProvider); // migration (หรือข้ามถ้าสร้างจาก SQL script) + seed
}

app.Run();

// ให้ WebApplicationFactory / integration test มองเห็น Program
public partial class Program;
