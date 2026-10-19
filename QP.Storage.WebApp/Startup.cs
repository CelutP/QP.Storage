using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using QP.Storage.WebApp.Middleware;
using QP.Storage.WebApp.Settings;

namespace QP.Storage.WebApp
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddLogging();
            services.Configure<FileSizeEndpointSettings>(Configuration.GetSection("FileSizeEndpointSettings"));
            services.Configure<ImageResizeSettings>(Configuration.GetSection("ImageResizeSettings"));
            services.AddTransient<ImageProcessor>();

            CorsSettings corsSettings = GetCorsSettings();
            if (corsSettings.Enabled)
            {
                services.AddCors(options => options.AddDefaultPolicy(policy =>
                {
                    policy.WithOrigins(corsSettings.AllowedOrigins)
                        .WithMethods("GET", "HEAD", "OPTIONS")
                        .AllowAnyHeader()
                        .SetPreflightMaxAge(TimeSpan.FromSeconds(corsSettings.PreflightMaxAgeSeconds));

                    if (corsSettings.AllowCredentials)
                    {
                        policy.AllowCredentials();
                    }
                }));
            }
        }

        private CorsSettings GetCorsSettings()
        {
            CorsSettings settings = Configuration.GetSection("Cors").Get<CorsSettings>() ?? new CorsSettings();
            settings.AllowedOrigins = (settings.AllowedOrigins ?? [])
                .Select(origin => origin?.Trim().TrimEnd('/'))
                .Where(origin => !string.IsNullOrEmpty(origin))
                .ToArray();

            if (settings.Enabled && settings.AllowedOrigins.Length == 0)
            {
                throw new InvalidOperationException("Cors:Enabled is true, but Cors:AllowedOrigins is empty");
            }

            return settings;
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // CORS должен стоять первым, чтобы заголовки попадали и в ответы middleware (размер файла, resize)
            if (GetCorsSettings().Enabled)
            {
                app.UseCors();
            }

            app.UseMiddleware<FileSizeMiddleware>();
            app.UseMiddleware<ReduceSettingsMiddleware>();
            app.UseMiddleware<ImageResizeMiddleware>();

            // Remove all code after this line and return default app.UseStaticFiles() call
            // after MS closes this issue https://github.com/dotnet/aspnetcore/issues/39984
            Dictionary<string, string> customExtensions = Configuration.GetSection("CustomExtensions")
                .Get<Dictionary<string, string>>();

            FileExtensionContentTypeProvider extensionProvider = new();

            foreach (KeyValuePair<string,string> customExtension in customExtensions)
            {
                extensionProvider.Mappings.Add(customExtension.Key, customExtension.Value);
            }

            app.UseStaticFiles(new StaticFileOptions {
                ContentTypeProvider = extensionProvider,
            });
        }
    }
}
