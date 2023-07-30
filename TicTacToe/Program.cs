using TicTacToe.Controllers;
using TicTacToe.Hubs;
using TicTacToe.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Services
builder.Services.AddSingleton<IGameService, GameService>();

builder.Services.AddSignalR();

// Cors policy for Angular
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", 
        builder =>
    {
        builder.AllowAnyMethod().AllowAnyHeader()
        .WithOrigins("http://localhost:4200")
        .AllowCredentials();
    });
});

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

app.UseRouting();

app.UseCors("CorsPolicy");

// SignalR
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    endpoints.MapHub<GameHub>("/game-live");
});

app.Run();
