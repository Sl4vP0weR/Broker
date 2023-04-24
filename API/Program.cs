var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;
var configuration = builder.Configuration;

// Add services to the container.

services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
services.AddEndpointsApiExplorer();
services.AddSwaggerGen();

services.AddInfrastructure(configuration);
services.AddApplication(configuration);

services.AddOutputCache(opt =>
{
    opt.AddPolicy(RatesController.CachePolicyName, policy => policy
        .Expire(TimeSpan.FromHours(1))
    );
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseOutputCache();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
