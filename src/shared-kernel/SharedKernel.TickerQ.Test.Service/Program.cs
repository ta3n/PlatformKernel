using SharedKernel.TickerQ;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure TickerQ - SharedKernel abstractions
builder.Services.AddTickerQCustom(builder.Configuration);
builder.Services.ValidateTickerQOptions();

// Note: Consumers should also add native TickerQ services here:
// builder.Services.AddTickerQ(/* configuration */);

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

// Note: Consumers should also use native TickerQ middleware here:
// app.UseTickerQ();

app.Run();

// Make the implicit Program class public for WebApplicationFactory
public partial class Program
{
}
