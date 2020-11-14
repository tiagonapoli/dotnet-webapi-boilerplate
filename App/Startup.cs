using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace App
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            var resource = Resources.CreateServiceResource("dotnet-playground", serviceVersion: "0.0.1");
            services.AddOpenTelemetryTracing((builder) =>
            {
                builder
                    .SetResource(resource)
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .SetSampler(new AlwaysOnSampler())
                    // .AddOtlpExporter(opt =>
                    // {
                    //     opt.Endpoint = conf.LightstepIngesterEndpoint;
                    //     opt.Headers = new Metadata {{"lightstep-access-token", conf.LightstepProjectToken}};
                    //     opt.Credentials = new SslCredentials();
                    // })
                    .AddConsoleExporter();
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMiddleware<RequestMiddleware>();
            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}

