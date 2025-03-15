
// Configure services and register dependencies
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
    // Configure the app to use Autofac as the DI container (service provider factory) instead of the default Microsoft DI
    builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());

    // Register Autofac module
    builder.Host.ConfigureContainer<ContainerBuilder>(containerBuilder =>
    {
        //containerBuilder.RegisterModule(new MeetingsAutofacModule());
        //containerBuilder.RegisterModule(new AdministrationAutofacModule());
        containerBuilder.RegisterModule(new UserAccessAutofacModule());
        //containerBuilder.RegisterModule(new PaymentsAutofacModule());	
    });

    //var _configuration = new ConfigurationBuilder()
    //			.SetBasePath(Directory.GetCurrentDirectory())
    //			.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)				
    //			.AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)				
    //			.AddEnvironmentVariables("Order_")
    //			.Build();

    builder.Services.AddScoped<DbContext>(provider =>
    {
        var dbContexts = provider.GetServices<IDomainDbContext>(); // Lấy tất cả DbContext đã đăng ký
        if (dbContexts.Count() > 1)
        {
            throw new InvalidOperationException("Multiple DbContexts found. Specify which one to use.");
        }
        return (DbContext)dbContexts.FirstOrDefault()!;
    });

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

    // Register ExecutionContextAccessor to retrieve user, tenant, or request-related information 
    // if it depends on IHttpContextAccessor.
    // In modular monolith, it helps extract UserId, CorrelationId, request context from the request.
    builder.Services.AddScoped<IExecutionContextAccessor, ExecutionContextAccessor>();

    // Configure authorization policy for permission-based access control
    builder.Services.AddAuthorization(options =>
    {
        options.AddPolicy(HasPermissionAttribute.HasPermissionPolicyName, policyBuilder =>
        {
            // Require a custom permission-based authorization requirement
            policyBuilder.Requirements.Add(new HasPermissionAuthorizationRequirement());

            // Specify that this policy applies only to Bearer authentication scheme (JWT)
            policyBuilder.AddAuthenticationSchemes("Bearer");
        });
    });

    // Add a custom authorization handler for evaluating permission-based policies (to handle custom permission-based authorization).
    // This is used to determine whether a user has the required permissions for a request.
    builder.Services.AddScoped<IAuthorizationHandler, HasPermissionAuthorizationHandler>();

    builder.Services.AddControllers();

    // Configure the HTTP request pipeline.
    var app = builder.Build();

    // Registers the ExceptionHandlerMiddleware in the request processing pipeline. 
    // This middleware catches unhandled exceptions, logs them, and returns a standardized error response.
    app.UseMiddleware<ExceptionHandlerMiddleware>();

    // Get the Serilog logger from DI (service container)
    var logger = app.Services.GetRequiredService<Serilog.ILogger>();

    // Get the Autofac root container from the built service provider
    var container = app.Services.GetAutofacRoot();
    //using (var scope = container.BeginLifetimeScope())
    //{
    //	Console.WriteLine("📌 Danh sách các services đã đăng ký trong Autofac:");
    //	foreach (var registration in scope.ComponentRegistry.Registrations)
    //	{
    //		Console.WriteLine($"🔍 {registration.Activator.LimitType}");
    //	}
    //}

    app.UseCors(builder =>
        builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
   
    // Initialize modules
    //string connectionName = builder.Configuration.GetSection("ConnectionStrings").GetChildren().FirstOrDefault()?.Key;
    var connectionString = builder.Configuration.GetConnectionString("OrderConnectionString");
    string emailConfig = builder.Configuration.GetSection("EmailsConfiguration:FromEmail")?.Value;
    string textEncryptionKey = builder.Configuration.GetSection("Security:TextEncryptionKey")?.Value;
    InitializeModules(container, logger, connectionString, emailConfig, textEncryptionKey);
    //using (var scope = container.BeginLifetimeScope())
    //{
    //	Console.WriteLine("📌 Danh sách các services đã đăng ký trong Autofac:");
    //	foreach (var registration in scope.ComponentRegistry.Registrations)
    //	{
    //		Console.WriteLine($"🔍 {registration.Activator.LimitType}");
    //	}
    //}

    app.UseMiddleware<CorrelationMiddleware>();

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

void InitializeModules(ILifetimeScope container, Serilog.ILogger logger, string connectionString, string fromEmail, string textEncryption)
{
    var httpContextAccessor = container.Resolve<IHttpContextAccessor>();
    var executionContextAccessor = new ExecutionContextAccessor(httpContextAccessor);

    var emailsConfiguration = new EmailsConfiguration(fromEmail);

    //MeetingsStartup.Initialize(
    //	_configuration[MeetingsConnectionString],
    //	executionContextAccessor,
    //	logger,
    //	emailsConfiguration,
    //	null);

    //AdministrationStartup.Initialize(
    //	_configuration[MeetingsConnectionString],
    //	executionContextAccessor,
    //	logger,
    //	null);

    UserAccessStartup.Initialize(
        connectionString,
        executionContextAccessor,
        logger,
        emailsConfiguration,
        textEncryption,
        null,
        null);

    //PaymentsStartup.Initialize(
    //	_configuration[MeetingsConnectionString],
    //	executionContextAccessor,
    //	logger,
    //	emailsConfiguration,
    //	null);

    RegistrationsStartup.Initialize(
        connectionString,
        executionContextAccessor,
        logger,
        emailsConfiguration,
        textEncryption,
        null,
        null);

    //using (var scope = container.BeginLifetimeScope())
    //{
    //	Console.WriteLine("📌 Danh sách các services đã đăng ký trong Autofac:");
    //	foreach (var registration in scope.ComponentRegistry.Registrations)
    //	{
    //		Console.WriteLine($"🔍 {registration.Activator.LimitType}");
    //	}	
    //}
}