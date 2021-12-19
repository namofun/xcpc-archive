using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;
using System.Threading.Tasks;

namespace XcpcArchive
{
    public class Startup
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            string cosmosDbConnectionString = builder.Configuration.GetConnectionString("CosmosDb");
            string storageConnectionString = builder.Configuration.GetConnectionString("StorageBlobs");

            // Add services to the container.
            builder.Services
                .AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
                .AddMicrosoftIdentityWebApp(builder.Configuration.GetSection("AzureAd"));

            builder.Services
                .AddRazorPages()
                .AddRazorRuntimeCompilation()
                .AddMicrosoftIdentityUI();

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
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapRazorPages();
            app.MapControllers();

            await app.Services.GetRequiredService<StartupInitializer>().DoWorkAsync();

            await app.RunAsync();
        }
    }
}
