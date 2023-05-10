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

services.AddApplication(configuration);
services.AddInfrastructure(configuration);

services.AddResponseCompression(opt =>
{
    opt.EnableForHttps = true;
});

AddSentry();

AddSerilog();

AddCaching();

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

app.UseResponseCompression();

UseExceptionHandler();

await app.RunAsync();

void AddSerilog()
{
    var settingsSection = configuration.GetSection(LoggingSettings.SectionName);
    services.Configure<LoggingSettings>(settingsSection);

    builder.Host.UseSerilog((context, services, configuration) =>
    {
        configuration
        .ReadFrom.Configuration(context.Configuration, settingsSection.Key)
        .ReadFrom.Services(services);
    });
}

void AddSentry()
{
    if (inDevelopment) return;

    services.AddSentry();
    builder.WebHost.UseSentry(opt =>
    {
        opt.Dsn = configuration["SentryAPI:Dsn"];
        opt.TracesSampleRate = 1.0;
    });
}

void UseExceptionHandler()
{
    app.UseExceptionHandler(builder => builder.Run(async ctx =>
    {
        var exception = ctx.Features.Get<IExceptionHandlerPathFeature>()?.Error;

        object response = exception switch
        {
            _ => inDevelopment ? exception : TypedResults.StatusCode(500)
        };

        await ctx.Response.WriteAsJsonAsync(response);
        
        if (inDevelopment) return;
        if (exception is ValidationException) return;

        SentrySdk.CaptureException(exception);
    }));
}

void AddCaching()
{
    services.AddOutputCache(opt =>
    {
        opt.AddPolicy(RatesController.CachePolicyName, policy => policy
            .Expire(TimeSpan.FromHours(1))
        );
    });
}