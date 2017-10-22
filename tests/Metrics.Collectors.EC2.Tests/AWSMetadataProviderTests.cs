using AutoFixture.NUnit3;
using Kralizek.AspNetCore.Metrics;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Tests
{
    [TestFixture]
    public class AWSMetadataProviderTests
    {
        [Test]
        public void SetInstance_throws_if_provider_is_null()
        {
            Assert.Throws<ArgumentNullException>(() => AWSMetadataProvider.SetInstance(null));
        }

        [Test, AutoMoqData]
        public void SetInstance_replaces_current_provicer(IAWSMetadataProvider provider)
        {
            AWSMetadataProvider.SetInstance(provider);

            AWSMetadataProvider.IsInAWS();

            Mock.Get(provider).Verify(p => p.IsInAWS(), Times.Once);
        }

        [Test, AutoMoqData]
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

        [Test]
        [InlineAutoMoqData(true)]
        [InlineAutoMoqData(false)]
        public void IsInAWS_forwards_to_provider(bool isInAws, IAWSMetadataProvider provider)
        {
            Mock.Get(provider).Setup(p => p.IsInAWS()).Returns(isInAws);

            AWSMetadataProvider.SetInstance(provider);

            Assert.That(AWSMetadataProvider.IsInAWS(), Is.EqualTo(isInAws));

        }

        [Test, AutoMoqData]
        public void InstanceId_forwards_to_provider(IAWSMetadataProvider provider, string instanceId)
        {
            Mock.Get(provider).Setup(p => p.GetInstanceId()).Returns(instanceId);

            AWSMetadataProvider.SetInstance(provider);

            Assert.That(AWSMetadataProvider.GetInstanceId(), Is.EqualTo(instanceId));
        }

        [Test, AutoMoqData]
        public void InstanceType_forwards_to_provider(IAWSMetadataProvider provider, string instanceType)
        {
            Mock.Get(provider).Setup(p => p.GetInstanceType()).Returns(instanceType).Verifiable();

            AWSMetadataProvider.SetInstance(provider);

            Assert.That(AWSMetadataProvider.GetInstanceType(), Is.EqualTo(instanceType));
        }

        [Test, AutoMoqData]
        public void AvailabilityZone_forwards_to_provider(IAWSMetadataProvider provider, string availabilityZone)
        {
            Mock.Get(provider).Setup(p => p.GetAvailabilityZone()).Returns(availabilityZone);

            AWSMetadataProvider.SetInstance(provider);

            Assert.That(AWSMetadataProvider.GetAvailabilityZone(), Is.EqualTo(availabilityZone));
        }

        [Test, AutoMoqData]
        public void AmiId_forwards_to_provider(IAWSMetadataProvider provider, string amiId)
        {
            Mock.Get(provider).Setup(p => p.GetAmiId()).Returns(amiId);

            AWSMetadataProvider.SetInstance(provider);

            Assert.That(AWSMetadataProvider.GetAmiId(), Is.EqualTo(amiId));
        }
    }
}
