using BingService.Application;
using BingService.Infrastructure;
using BingService.WebUI;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.Extensions.Logging.EventLog;
using Serilog;

internal static class Program
{
    private const int EXIT_FAILURE = 1;
    private const int EXIT_SUCCESS = 0;

    public static async Task<int> Main(string[] args)
    {
        var webApplicationOptions = new WebApplicationOptions
        {
            ContentRootPath = AppContext.BaseDirectory,
            Args = args,
        };

        var builder = WebApplication.CreateBuilder(webApplicationOptions);

        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(builder.Configuration)
            .Enrich.WithProperty(nameof(ServiceConstants.ServiceName), ServiceConstants.ServiceName)
            .Enrich.WithProperty(nameof(ServiceConstants.ServiceVersion), ServiceConstants.ServiceVersion)
            .CreateLogger();

        try
        {
            builder.Host.UseWindowsService();
            builder.Services.Configure<EventLogSettings>(config =>
            {
                config.LogName = string.Empty;
                config.SourceName = ServiceConstants.ServiceName;
            });

            builder.Host.UseSerilog((_, lc) => lc
                .ReadFrom.Configuration(builder.Configuration)
                .Enrich.WithProperty(nameof(ServiceConstants.ServiceName), ServiceConstants.ServiceName)
                .Enrich.WithProperty(nameof(ServiceConstants.ServiceVersion), ServiceConstants.ServiceVersion));

            builder.Services.AddHttpLogging(options => options.LoggingFields = HttpLoggingFields.All);
            builder.Services.AddHealthChecks();

            // Add services to the container.
            builder.Services.AddApplication();
            builder.Services.AddInfrastructure(builder.Configuration);
            builder.Services.AddControllersWithViews(options => options.Filters.Add<SerilogLoggingActionFilter>());

            var app = builder.Build();

            app.UseHttpLogging();
            app.UseHealthChecks("/health");

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseStaticFiles();

            app.UseSerilogRequestLogging(options =>
            {
                options.EnrichDiagnosticContext = EnrichDiagnosticContext;
            });

            app.UseRouting();

            app.UseRequestResponseLogging();

            app.UseAuthorization();

            app.UseInfrastructure(builder.Configuration);

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            await app.RunAsync();

            return EXIT_SUCCESS;
        }
        catch (Exception exception)
        {
            Log.Fatal(exception, "Host terminated unexpectedly");
            return EXIT_FAILURE;
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }

    static void EnrichDiagnosticContext(IDiagnosticContext diagnosticContext, HttpContext httpContext)
    {
        var request = httpContext.Request;

        diagnosticContext.Set("Host", request.Host);
        diagnosticContext.Set("Protocol", request.Protocol);
        diagnosticContext.Set("Scheme", request.Scheme);

        foreach (var (name, value) in request.Headers /*.Where(kv => kv.Key.Equals("X-Real-IP"))*/)
        {
            diagnosticContext.Set(name, value);
        }

        if (request.QueryString.HasValue)
        {
            diagnosticContext.Set("QueryString", request.QueryString.Value);
        }

        diagnosticContext.Set("ContentType", httpContext.Response.ContentType);

        var endpoint = httpContext.GetEndpoint();

        if (endpoint is { })
        {
            diagnosticContext.Set("EndpointName", endpoint.DisplayName);
        }
    }
}
