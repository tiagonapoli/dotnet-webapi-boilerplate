using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using App.Telemetry.Sampler;
using Grpc.Core;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace App.Telemetry
{
    public static class TracerSingleton
    {
        public static ActivitySource Tracer = null;
    }
}