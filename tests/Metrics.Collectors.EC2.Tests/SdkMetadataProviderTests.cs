using Amazon.Util;
using Xunit;
using static Kralizek.AspNetCore.Metrics.SdkMetadataProvider;


namespace Tests
{
    public class SdkMetadataProviderTests
    {
        [Fact]
        public void InstanceId_forwards_to_EC2InstanceMetadata_service()
        {
            Assert.Equal(Instance.GetInstanceId(), EC2InstanceMetadata.InstanceId);
        }

        [Fact]
        public void InstanceType_forwards_to_EC2InstanceMetadata_service()
        {
            Assert.Equal(Instance.GetInstanceType(), EC2InstanceMetadata.InstanceType);
        }

        [Fact]
        public void AvailabilityZone_forwards_to_EC2InstanceMetadata_service()
        {
            Assert.Equal(Instance.GetAvailabilityZone(), EC2InstanceMetadata.AvailabilityZone);
        }

        [Fact]
        public void AmiId_forwards_to_EC2InstanceMetadata_service()
        {
            Assert.Equal(Instance.GetAmiId(), EC2InstanceMetadata.AmiId);
        }
    }
}
