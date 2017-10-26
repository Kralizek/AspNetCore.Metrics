using Kralizek.AspNetCore.Metrics;
using Moq;
using Xunit;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Tests
{
    public class EC2MetricCollectorTests
    {
        [Theory, AutoMoqData]
        public async Task Dimensions_are_added_if_in_AWS(EC2MetricCollector sut, IAWSMetadataProvider provider, IDictionary<IMetricDimension, object> dimensions, IDictionary<IMetricValue, IValue> metrics)
        {
            Mock.Get(provider).Setup(p => p.IsInAWS()).Returns(true);

            AWSMetadataProvider.SetInstance(provider);

            await sut.OnActionExecutingAsync(dimensions, metrics);

            Assert.True(dimensions.ContainsKey(EC2MetadataDimensions.InstanceId));
            Assert.True(dimensions.ContainsKey(EC2MetadataDimensions.AmiId));
            Assert.True(dimensions.ContainsKey(EC2MetadataDimensions.AvailabilityZone));
            Assert.True(dimensions.ContainsKey(EC2MetadataDimensions.InstanceType));
        }

        [Theory, AutoMoqData]
        public async Task Dimensions_are_not_added_if_not_in_AWS(EC2MetricCollector sut, IAWSMetadataProvider provider, IDictionary<IMetricDimension, object> dimensions, IDictionary<IMetricValue, IValue> metrics)
        {
            Mock.Get(provider).Setup(p => p.IsInAWS()).Returns(false);

            AWSMetadataProvider.SetInstance(provider);

            await sut.OnActionExecutingAsync(dimensions, metrics);

            Assert.False(dimensions.ContainsKey(EC2MetadataDimensions.InstanceId));
            Assert.False(dimensions.ContainsKey(EC2MetadataDimensions.AmiId));
            Assert.False(dimensions.ContainsKey(EC2MetadataDimensions.AvailabilityZone));
            Assert.False(dimensions.ContainsKey(EC2MetadataDimensions.InstanceType));
        }
    }
}
