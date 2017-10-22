using Kralizek.AspNetCore.Metrics;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Tests
{
    [TestFixture]
    public class EC2MetricCollectorTests
    {
        [Test]
        [InlineAutoMoqData(true)]
        [InlineAutoMoqData(false)]
        public async Task Dimensions_are_added_if_in_AWS(bool isInAws, EC2MetricCollector sut, IAWSMetadataProvider provider, IDictionary<IMetricDimension, object> dimensions, IDictionary<IMetricValue, IValue> metrics)
        {
            Mock.Get(provider).Setup(p => p.IsInAWS()).Returns(isInAws);

            AWSMetadataProvider.SetInstance(provider);

            await sut.OnActionExecutingAsync(dimensions, metrics);

            Assert.That(dimensions.ContainsKey(EC2MetadataDimensions.InstanceId), Is.EqualTo(isInAws));
            Assert.That(dimensions.ContainsKey(EC2MetadataDimensions.AmiId), Is.EqualTo(isInAws));
            Assert.That(dimensions.ContainsKey(EC2MetadataDimensions.AvailabilityZone), Is.EqualTo(isInAws));
            Assert.That(dimensions.ContainsKey(EC2MetadataDimensions.InstanceType), Is.EqualTo(isInAws));
        }
    }
}
