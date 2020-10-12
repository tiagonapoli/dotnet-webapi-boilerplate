using System;
using OpenTelemetry.Trace;

namespace App.Telemetry.Sampler
{
    public class TraceIdDynamicRatioSampler : OpenTelemetry.Trace.Sampler
    {
        private static long CalculateIdUpperBound(double probability)
        {
            if (probability < 0.0 || probability > 1.0)
            {
                throw new ArgumentOutOfRangeException(nameof(probability), "Probability must be in range [0.0, 1.0]");
            }

            if (probability == 0.0)
            {
                return long.MinValue;
            }

            if (probability == 1.0)
            {
                return long.MaxValue;
            }

            return (long) (probability * long.MaxValue);
        }


        private readonly long[] idUpperBoundArray = new long[2];
        private readonly double[] probabilityArray = new double[2];
        private int arrayPosition = 0;

        public TraceIdDynamicRatioSampler(double initialProbability)
        {
            this.probabilityArray[0] = this.probabilityArray[1] = initialProbability;
            this.idUpperBoundArray[0] = this.idUpperBoundArray[1] = CalculateIdUpperBound(initialProbability);
            this.Description = "TraceIdDynamicRatioSampler";
        }

        public override SamplingResult ShouldSample(in SamplingParameters samplingParameters)
        {
            // Always sample if we are within probability range. This is true even for child activities (that
            // may have had a different sampling decision made) to allow for different sampling policies,
            // and dynamic increases to sampling probabilities for debugging purposes.
            // Note use of '<' for comparison. This ensures that we never sample for probability == 0.0,
            // while allowing for a (very) small chance of *not* sampling if the id == Long.MAX_VALUE.
            // This is considered a reasonable trade-off for the simplicity/performance requirements (this
            // code is executed in-line for every Activity creation).
            Span<byte> traceIdBytes = stackalloc byte[16];
            samplingParameters.TraceId.CopyTo(traceIdBytes);
            return new SamplingResult(Math.Abs(GetLowerLong(traceIdBytes)) <
                                      this.idUpperBoundArray[this.arrayPosition]);
        }

        public void UpdateSamplingProbability(double probability)
        {
            this.probabilityArray[this.arrayPosition ^ 1] = probability;
            this.idUpperBoundArray[this.arrayPosition ^ 1] = CalculateIdUpperBound(probability);
            this.arrayPosition ^= 1;
        }

        public double GetCurrentSamplingProbability()
        {
            return this.probabilityArray[this.arrayPosition];
        }

        public long GetCurrentTraceIdUpperBound()
        {
            return this.idUpperBoundArray[this.arrayPosition];
        }

        private static long GetLowerLong(ReadOnlySpan<byte> bytes)
        {
            long result = 0;
            for (var i = 0; i < 8; i++)
            {
                result <<= 8;
#pragma warning disable CS0675 // Bitwise-or operator used on a sign-extended operand
                result |= bytes[i] & 0xff;
#pragma warning restore CS0675 // Bitwise-or operator used on a sign-extended operand
            }

            return result;
        }
    }
}