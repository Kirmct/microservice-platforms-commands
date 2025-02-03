using System.Windows.Input;
using Microsoft.EntityFrameworkCore;
using PlatformService.Data;
using PlatformService.SyncDataServices.Http;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Add database
builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseInMemoryDatabase("InMen"));

//Add repository
builder.Services.AddScoped<IPlatformRepository, PlatformRepository>();

// Add httpClient
builder.Services.AddHttpClient<ICommandDataClient, HttpCommandDataClient>();


builder.Services.AddControllers();

// Add Automapper
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

//swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

Console.WriteLine($"--> CommandService Endpoint {builder.Configuration["CommandService"]}");

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

//populating with static class
PrepDb.PrepPopulation(app);

Console.WriteLine("--> Applications is running");
app.Run();