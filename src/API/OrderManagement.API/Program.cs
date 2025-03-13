
// Configure services and register dependencies
using Common.Logging;
using OrderManagement.API.Configuration.Extensions;
using OrderManagement.API.Middlewares;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

try
{
    // Configure Serilog as the logging provider for the application, using Serilog to replace the default logging provider
    builder.Host.UseSerilog(Serilogger.Configure);
}
catch (Exception ex)
{
    Console.WriteLine($"Error configuring Serilog: {ex.Message}");
}

try
{   
    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    // Enable API explorer for endpoint metadata (required for Swagger)
    builder.Services.AddEndpointsApiExplorer();

    // Register custom Swagger documentation configuration
    builder.Services.AddSwaggerDocumentation();

    // Add support for Newtonsoft.Json in Swagger (e.g., handling polymorphic serialization)
    builder.Services.AddSwaggerGenNewtonsoftSupport();


    // Register IHttpContextAccessor to allow access to HttpContext in services, non-controller classes, background tasks,
    // or singleton services (where direct dependency injection of HttpContext is not possible)	
    builder.Services.AddHttpContextAccessor();

    builder.Services.AddControllers();


    var app = builder.Build();

    // Registers the ExceptionHandlerMiddleware in the request processing pipeline. 
    // This middleware catches unhandled exceptions, logs them, and returns a standardized error response.
    app.UseMiddleware<ExceptionHandlerMiddleware>();

    // Get the Serilog logger from DI (service container)
    var logger = app.Services.GetRequiredService<Serilog.ILogger>();

    app.UseCors(builder =>
        builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwaggerDocumentation();
    }
    else
    {
        app.UseExceptionHandler("/Error");
        app.UseHsts();
    }

    app.UseHttpsRedirection();

    app.UseAuthorization();

    app.MapControllers();
   
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Unhandled exception");
}
finally
{
    Log.CloseAndFlush();
}