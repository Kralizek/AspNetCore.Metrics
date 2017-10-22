using AutoFixture;
using AutoFixture.AutoMoq;
using Kralizek.AspNetCore.Metrics;
using Kralizek.AspNetCore.Metrics.Util;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Tests.Util
{
    [TestFixture]
    public class MetricDimensionEqualityComparerTests
    {
        IFixture fixture;

        [SetUp]
        public void Initialize()
        {
            fixture = new Fixture().Customize(new AutoMoqCustomization());
        }

        [Test, AutoMoqData]
        public void Equals_returns_true_if_same_name(string name)
        {
            var mockX = new Mock<IMetricDimension>();
            mockX.SetupGet(p => p.Name).Returns(name);

            var mockY = new Mock<IMetricDimension>();
            mockY.SetupGet(p => p.Name).Returns(name);

            Assert.That(MetricDimensionEqualityComparer.Default.Equals(mockX.Object, mockY.Object), Is.True);
        }

        [Test, AutoMoqData]
        public void Equals_returns_false_if_not_same_name(string nameX, string nameY)
        {
            var mockX = new Mock<IMetricDimension>();
            mockX.SetupGet(p => p.Name).Returns(nameX);

            var mockY = new Mock<IMetricDimension>();
            mockY.SetupGet(p => p.Name).Returns(nameY);

            Assert.That(MetricDimensionEqualityComparer.Default.Equals(mockX.Object, mockY.Object), Is.False);
        }


    }
}
