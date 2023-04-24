var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;
var configuration = builder.Configuration;
var environment = builder.Environment;

var inDevelopment = environment.IsDevelopment();

// Add services to the container.

services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
services.AddEndpointsApiExplorer();
services.AddSwaggerGen();

services.AddInfrastructure(configuration);
services.AddApplication(configuration);

services.AddSentry();
builder.WebHost.UseSentry(opt =>
{
    opt.Dsn = configuration["SentryAPI:Dsn"];
    opt.TracesSampleRate = 1.0;
});

var settingsSection = configuration.GetSection(LoggingSettings.SectionName);
services.Configure<LoggingSettings>(settingsSection);

builder.Host.UseSerilog((context, services, configuration) =>
{
    configuration
    .ReadFrom.Configuration(context.Configuration, settingsSection.Key)
    .ReadFrom.Services(services);
});

services.AddOutputCache(opt =>
{
    opt.AddPolicy(RatesController.CachePolicyName, policy => policy
        .Expire(TimeSpan.FromHours(1))
    );
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (inDevelopment)
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseOutputCache();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.UseSerilogRequestLogging();

if (!inDevelopment)
{
    app.UseExceptionHandler(builder => builder.Run(async ctx =>
    {
        var exception = ctx.Features.Get<IExceptionHandlerPathFeature>()?.Error;
        SentrySdk.CaptureException(exception);
        await ctx.Response.WriteAsJsonAsync(TypedResults.StatusCode(500));
    }));
}

app.Run();
