using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using WebApiTest.Web.DataAccess;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<MyDbCOntext>(db => db.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddRateLimiter(x =>
{
    x.RejectionStatusCode = 429;
    x.AddSlidingWindowLimiter("sliding", options =>
    {
        options.Window = TimeSpan.FromSeconds(3);
        options.SegmentsPerWindow = 5;
        options.PermitLimit = 100;
        options.QueueLimit = 3;
    });
});


var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseRateLimiter();
app.MapControllers().RequireRateLimiting("sliding");

app.Run();
