using System;
using Xunit;
using App.Telemetry.Sampler;

namespace Telemetry.Test
{
    public class TraceIdDynamicRatioSamplerTests
    {
        [Theory]
        [InlineData(0.5, 4611686018427387904)]
        [InlineData(1, long.MaxValue)]
        [InlineData(0, long.MinValue)]
        public void InitialProbabilityIsSet(double prob, long expectedValue)
        {
            var sampler = new TraceIdDynamicRatioSampler(prob);
            Assert.Equal(prob, sampler.GetCurrentSamplingProbability());
            Assert.Equal(expectedValue, sampler.GetCurrentTraceIdUpperBound());
        }

        [Fact]
        public void UpdateProbabilityWorksProperly()
        {
            var sampler = new TraceIdDynamicRatioSampler(1);
            sampler.UpdateSamplingProbability(0);
            Assert.Equal(0, sampler.GetCurrentSamplingProbability());
            Assert.Equal(long.MinValue, sampler.GetCurrentTraceIdUpperBound());
            
            sampler.UpdateSamplingProbability(0.5);
            Assert.Equal(0.5, sampler.GetCurrentSamplingProbability());
            Assert.Equal(4611686018427387904, sampler.GetCurrentTraceIdUpperBound());
            
            sampler.UpdateSamplingProbability(0.25);
            Assert.Equal(0.25, sampler.GetCurrentSamplingProbability());
            Assert.Equal(2305843009213693952, sampler.GetCurrentTraceIdUpperBound());
            
            sampler.UpdateSamplingProbability(0.1);
            Assert.Equal(0.1, sampler.GetCurrentSamplingProbability());
            Assert.Equal(922337203685477632, sampler.GetCurrentTraceIdUpperBound());
        }
    }
}