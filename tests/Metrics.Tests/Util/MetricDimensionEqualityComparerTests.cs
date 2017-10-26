using AutoFixture;
using AutoFixture.AutoMoq;
using Kralizek.AspNetCore.Metrics;
using Kralizek.AspNetCore.Metrics.Util;
using Moq;
using Xunit;
using System;
using System.Collections.Generic;
using System.Text;

namespace Tests.Util
{
    public class MetricDimensionEqualityComparerTests
    {
        IFixture fixture;

        public MetricDimensionEqualityComparerTests()
        {
            fixture = new Fixture().Customize(new AutoMoqCustomization());
        }

        [Theory, AutoMoqData]
        public void Equals_returns_true_if_same_name(string name)
        {
            var mockX = new Mock<IMetricDimension>();
            mockX.SetupGet(p => p.Name).Returns(name);

            var mockY = new Mock<IMetricDimension>();
            mockY.SetupGet(p => p.Name).Returns(name);

            Assert.True(MetricDimensionEqualityComparer.Default.Equals(mockX.Object, mockY.Object));
        }

        [Theory, AutoMoqData]
        public void Equals_returns_false_if_not_same_name(string nameX, string nameY)
        {
            var mockX = new Mock<IMetricDimension>();
            mockX.SetupGet(p => p.Name).Returns(nameX);

            var mockY = new Mock<IMetricDimension>();
            mockY.SetupGet(p => p.Name).Returns(nameY);

            Assert.False(MetricDimensionEqualityComparer.Default.Equals(mockX.Object, mockY.Object));
        }


    }
}
