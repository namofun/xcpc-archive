using Azure.Storage.Blobs;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Reflection;
using System.Threading.Tasks;

namespace XcpcArchive
{
    public class Startup
    {
        public static string Version { get; private set; } = "Unknown";

        public static void Main(string[] args)
        {
            string? versionName = typeof(Startup).Assembly
                .GetCustomAttribute<AssemblyInformationalVersionAttribute>()?
                .InformationalVersion;

            if (!string.IsNullOrEmpty(versionName))
            {
                string[] segments = versionName.Split('+');
                if (segments.Length == 2)
                {
                    Version = segments[0] + " (" + segments[1][..7] + ")";
                }
                else
                {
                    Version = versionName;
                }
            }

            var builder = WebApplication.CreateBuilder(args);

            string cosmosDbConnectionString = builder.Configuration.GetConnectionString("CosmosDb");
            string storageConnectionString = builder.Configuration.GetConnectionString("StorageBlobs");

            // Add services to the container.
            builder.Services.AddEasyAuthAuthentication(
                options => options.DeveloperMode = builder.Environment.IsDevelopment());

            builder.Services
                .AddControllersWithViews()
#if DEBUG
                .AddRazorRuntimeCompilation()
#endif
                .AddNewtonsoftJson();

            builder.Services.AddHttpClient();

            builder.Services.AddSingleton(new CosmosClient(cosmosDbConnectionString));
            builder.Services.AddSingleton(new BlobServiceClient(storageConnectionString));

            builder.Services.AddSingleton<CcsApi.CcsApiClient>();
            builder.Services.AddSingleton<CcsApi.CcsApiImportService>();
            builder.Services.AddHostedService(sp => sp.GetRequiredService<CcsApi.CcsApiImportService>());

            builder.Services.AddApplicationInsightsTelemetry();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/error");
            }

            app.UseStaticFiles();

            app.UseRouting();

            app.Use(TelemetryCorrelationMiddleware);
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }

        private static Task TelemetryCorrelationMiddleware(HttpContext context, RequestDelegate next)
        {
            Endpoint? endpoint = context.GetEndpoint();
            RequestTelemetry? telemetry = context.Features.Get<RequestTelemetry>();

            if (telemetry != null && string.IsNullOrEmpty(telemetry.Name))
            {
                if (endpoint == null)
                {
                    telemetry.Name = context.Request.Method + " /" + (context.Request.Path.Value ?? "<null>").TrimStart('/');
                }
                else if (endpoint is RouteEndpoint routeEndpoint && routeEndpoint.RoutePattern.RawText != null)
                {
                    telemetry.Name = context.Request.Method + " /" + routeEndpoint.RoutePattern.RawText.TrimStart('/');
                }
                else
                {
                    // It's another type of endpoint but not route endpoint. So strange.
                }
            }

            return next(context);
        }
    }
}
