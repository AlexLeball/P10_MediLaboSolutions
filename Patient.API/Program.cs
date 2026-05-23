using Microsoft.EntityFrameworkCore;
using Patient.Application.Interfaces;
using Patient.Application.Services;
using Patient.Infrastructure.Data;
using Patient.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// ========================
// DATABASE
// ========================
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")
    ));

// ========================
// DEPENDENCY INJECTION
// ========================
builder.Services.AddScoped<IPatientRepository, PatientRepository>();
builder.Services.AddScoped<IPatientService, PatientService>();

// ========================
// CONTROLLERS
// ========================
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// ========================
// PIPELINE
// ========================
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization(); // optional (can even remove)

app.MapControllers();

// ========================
// DB MIGRATION
// ========================
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

app.Run();