using Microsoft.EntityFrameworkCore;
using Patient.Application.Interfaces;
using Patient.Application.Services;
using Patient.Infrastructure.Data;
using Patient.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddControllers();
builder.Services.AddScoped<IPatientRepository, PatientRepository>();
builder.Services.AddScoped<IPatientService, PatientService>();

var app = builder.Build();

app.MapControllers();
app.Run();
