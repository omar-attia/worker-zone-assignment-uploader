using Microsoft.EntityFrameworkCore;
using WakeCap.Service;
using WakeCap.DAL;
using WakeCap.DAL.Interfaces;
using WakeCap.Service.Interfaces;
using WakeCap.DAL.Repositories.Interfaces;
using WakeCap.DAL.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<IWakeCapDbContext, WakeCapDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("WakecapDb")));

// Add services to the container.
builder.Services.AddScoped<IWorkerZoneAssignmentImportService, WorkerZoneAssignmentImportService>();
builder.Services.AddScoped<IWorkerRepository, WorkerRepository>();
builder.Services.AddScoped<IZoneRepository, ZoneRepository>();
builder.Services.AddScoped<IWorkerZoneAssignmentRepository, WorkerZoneAssignmentRepository>();
builder.Services.AddScoped<IUploadLogRepository, UploadLogRepository>();


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
