using System;

namespace App.Telemetry
{
    public class OpenTelemetryConfig
    {
        public string ServiceName { get; set; } = String.Empty;
        public string ServiceVersion { get; set; } = String.Empty;
        public double InitialSamplingRatio { get; set; } = 0;

        /**
        * LightstepExporter config
        */
        public string LightstepIngesterEndpoint { get; set; } = String.Empty;
        public string LightstepProjectToken { get; set; } = String.Empty;

        /**
        * Host information
        */
        public string HostID { get; set; } = String.Empty;
        public string HostName { get; set; } = String.Empty;
        public string HostType { get; set; } = String.Empty;

        /**
         * OS Information
         */
        public string OsType { get; set; } = String.Empty;
        public string OsDescription { get; set; } = String.Empty;

        /**
         * Environment information
         */
        public string DeploymentEnvironment { get; set; } = String.Empty;

        /**
         * Cloud information
         */
        public string CloudZone { get; set; } = String.Empty;
    }
}