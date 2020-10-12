using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using App.Telemetry.Sampler;
using Grpc.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace App.Telemetry
{
    public static class TracerConfigurer
    {
        public static OpenTelemetryConfig GetTracingConfiguration(IConfiguration config)
        {
            var otlpConfig = new OpenTelemetryConfig();
            config.Bind("OpenTelemetryTracing", otlpConfig);
            otlpConfig.ServiceVersion = EnvironmentConfigGetter.GetServiceVersion();
            otlpConfig.OsType = EnvironmentConfigGetter.GetOsName();
            otlpConfig.OsDescription = EnvironmentConfigGetter.GetOsDescription();
            otlpConfig.DeploymentEnvironment = EnvironmentConfigGetter.GetDeploymentEnvironment();
            return otlpConfig;
        }

        public static void ConfigureTracer(this IServiceCollection services, OpenTelemetryConfig conf)
        {
            TracerSingleton.Tracer = new ActivitySource(conf.ServiceName);
            var resource = Resources.CreateServiceResource(conf.ServiceName, serviceVersion: conf.ServiceVersion)
                .Merge(new Resource(new Dictionary<string, object>
                {
                    {"deployment.environment", conf.DeploymentEnvironment},
                    {"os.type", conf.OsType},
                    {"os.description", conf.OsDescription},
                    // TODO Add resource information
                    // {"cloud.zone", conf.CloudZone},
                    // {"host.id", conf.HostID},
                    // {"host.name", conf.HostName},
                    // {"host.type", conf.HostType},
                    // {"process.runtime.name", GetDotnetRuntimeName()},
                    // {"process.runtime.version", GetDotnetRuntimeVersion()},
                }));

            services.AddOpenTelemetryTracing((builder) =>
            {
                builder
                    .SetResource(resource)
                    .AddSource(conf.ServiceName)
                    .AddAspNetCoreInstrumentation(opt =>
                        opt.Filter = (httpContext) => !httpContext.Request.Path.StartsWithSegments("/healthcheck"))
                    .AddHttpClientInstrumentation((opt) =>
                        opt.Filter = _ => Activity.Current?.IsAllDataRequested ?? false)
                    .SetSampler(new ParentBasedSampler(new TraceIdDynamicRatioSampler(conf.InitialSamplingRatio)))
                    .AddOtlpExporter(opt =>
                    {
                        opt.Endpoint = conf.LightstepIngesterEndpoint;
                        opt.Headers = new Metadata {{"lightstep-access-token", conf.LightstepProjectToken}};
                        opt.Credentials = new SslCredentials();
                    });
            });
        }

        static string GetDotnetRuntimeVersion()
        {
            return "";
        }

        static string GetDotnetRuntimeName()
        {
            return "";
        }
    }
}