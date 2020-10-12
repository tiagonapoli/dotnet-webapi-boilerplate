using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace App.Telemetry
{
    public static class EnvironmentConfigGetter
    {
        public static string GetOsName()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                return "LINUX";
            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return "WINDOWS";
            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                return "OSX";
            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.FreeBSD))
            {
                return "FREEBSD";
            }

            return "UNKNOWN";
        }

        public static string GetOsDescription()
        {
            return Environment.OSVersion.ToString();
        }

        public static string GetServiceVersion()
        {
            return Assembly.GetExecutingAssembly()
                .GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion ?? "";
        }

        public static string GetDeploymentEnvironment()
        {
            return Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "unknown";
        }
    }
}