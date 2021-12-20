using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Builder;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace XcpcArchive
{
    public class Startup
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            string cosmosDbConnectionString = builder.Configuration.GetConnectionString("CosmosDb");
            string storageConnectionString = builder.Configuration.GetConnectionString("StorageBlobs");

            // Add services to the container.
            builder.Services.AddEasyAuthAuthentication(
                options => options.DeveloperMode = builder.Environment.IsDevelopment());

            builder.Services
                .AddRazorPages()
#if DEBUG
                .AddRazorRuntimeCompilation();
#else
                ;
#endif

            builder.Services
                .AddControllers()
                .AddNewtonsoftJson();

            builder.Services.AddHttpClient();

            builder.Services.AddSingleton(new CosmosClient(cosmosDbConnectionString));
            builder.Services.AddSingleton(new BlobServiceClient(storageConnectionString));

            builder.Services.AddSingleton<StartupInitializer>();
            builder.Services.AddTransient<IStartupInitializer, CcsApi.CcsApiInitializer>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
            }

            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapRazorPages();
            app.MapControllers();

            app.Run();
        }
    }
}
