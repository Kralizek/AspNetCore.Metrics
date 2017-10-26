using Kralizek.AspNetCore.Metrics;
using Moq;
using Xunit;
using System;
using System.Collections.Generic;
using System.Text;

namespace Tests
{
    public class AWSMetadataProviderTests
    {
        [Fact]
        public void SetInstance_throws_if_provider_is_null()
        {
            Assert.Throws<ArgumentNullException>(() => AWSMetadataProvider.SetInstance(null));
        }

        [Theory, AutoMoqData]
        public void SetInstance_replaces_current_provicer(IAWSMetadataProvider provider)
        {
            AWSMetadataProvider.SetInstance(provider);

            AWSMetadataProvider.IsInAWS();

            Mock.Get(provider).Verify(p => p.IsInAWS(), Times.Once);
        }

        [Theory, AutoMoqData]
        public void Reset_sets_provider_to_default(IAWSMetadataProvider provider)
        {
            AWSMetadataProvider.SetInstance(provider);

            AWSMetadataProvider.IsInAWS();

            Mock.Get(provider).Verify(p => p.IsInAWS(), Times.Once);
            Mock.Get(provider).ResetCalls();

            AWSMetadataProvider.ResetInstance();

            AWSMetadataProvider.GetInstanceId();

            Mock.Get(provider).Verify(p => p.IsInAWS(), Times.Never);

        }

        [Theory, AutoMoqData]
        public void IsInAWS_forwards_to_provider_True(IAWSMetadataProvider provider)
        {
            Mock.Get(provider).Setup(p => p.IsInAWS()).Returns(true);

            AWSMetadataProvider.SetInstance(provider);

            Assert.True(AWSMetadataProvider.IsInAWS());

        }

        [Theory, AutoMoqData]
        public void IsInAWS_forwards_to_provider_False(IAWSMetadataProvider provider)
        {
            Mock.Get(provider).Setup(p => p.IsInAWS()).Returns(false);

            AWSMetadataProvider.SetInstance(provider);

            Assert.False(AWSMetadataProvider.IsInAWS());

        }

        [Theory, AutoMoqData]
        public void InstanceId_forwards_to_provider(IAWSMetadataProvider provider, string instanceId)
        {
            Mock.Get(provider).Setup(p => p.GetInstanceId()).Returns(instanceId);

            AWSMetadataProvider.SetInstance(provider);

            Assert.Equal(AWSMetadataProvider.GetInstanceId(), instanceId);
        }

        [Theory, AutoMoqData]
        public void InstanceType_forwards_to_provider(IAWSMetadataProvider provider, string instanceType)
        {
            Mock.Get(provider).Setup(p => p.GetInstanceType()).Returns(instanceType).Verifiable();

            AWSMetadataProvider.SetInstance(provider);

            Assert.Equal(AWSMetadataProvider.GetInstanceType(), instanceType);
        }

        [Theory, AutoMoqData]
        public void AvailabilityZone_forwards_to_provider(IAWSMetadataProvider provider, string availabilityZone)
        {
            Mock.Get(provider).Setup(p => p.GetAvailabilityZone()).Returns(availabilityZone);

            AWSMetadataProvider.SetInstance(provider);

            Assert.Equal(AWSMetadataProvider.GetAvailabilityZone(), availabilityZone);
        }

        [Theory, AutoMoqData]
        public void AmiId_forwards_to_provider(IAWSMetadataProvider provider, string amiId)
        {
            Mock.Get(provider).Setup(p => p.GetAmiId()).Returns(amiId);

            AWSMetadataProvider.SetInstance(provider);

            Assert.Equal(AWSMetadataProvider.GetAmiId(), amiId);
        }
    }
}
