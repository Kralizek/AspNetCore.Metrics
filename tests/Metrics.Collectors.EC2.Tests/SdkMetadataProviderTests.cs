using Amazon.Util;
using NUnit.Framework;
using static Kralizek.AspNetCore.Metrics.SdkMetadataProvider;


namespace Tests
{
    [TestFixture]
    public class SdkMetadataProviderTests
    {
        [Test]
        public void InstanceId_forwards_to_EC2InstanceMetadata_service()
        {
            Assert.That(Instance.GetInstanceId(), Is.EqualTo(EC2InstanceMetadata.InstanceId));
        }

        [Test]
        public void InstanceType_forwards_to_EC2InstanceMetadata_service()
        {
            Assert.That(Instance.GetInstanceType(), Is.EqualTo(EC2InstanceMetadata.InstanceType));
        }

        [Test]
        public void AvailabilityZone_forwards_to_EC2InstanceMetadata_service()
        {
            Assert.That(Instance.GetAvailabilityZone(), Is.EqualTo(EC2InstanceMetadata.AvailabilityZone));
        }

        [Test]
        public void AmiId_forwards_to_EC2InstanceMetadata_service()
        {
            Assert.That(Instance.GetAmiId(), Is.EqualTo(EC2InstanceMetadata.AmiId));
        }
    }
}
