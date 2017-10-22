using AutoFixture;
using AutoFixture.AutoMoq;
using Kralizek.AspNetCore.Metrics;
using Kralizek.AspNetCore.Metrics.Middlewares;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using DimensionDictionary = System.Collections.Generic.IDictionary<Kralizek.AspNetCore.Metrics.IMetricDimension, object>;
using MetricDictionary = System.Collections.Generic.IDictionary<Kralizek.AspNetCore.Metrics.IMetricValue, Kralizek.AspNetCore.Metrics.IValue>;


namespace Tests.Middlewares
{
    [TestFixture]
    public class AspNetCoreMetricCollectorMiddlewareTests
    {
        private IFixture fixture;
        private Mock<IMetricPersister> mockPersister;
        private Mock<RequestDelegate> mockDelegate;

        [SetUp]
        public void Initialize()
        {
            fixture = new Fixture().Customize(new AutoMoqCustomization());

            mockPersister = new Mock<IMetricPersister>();

            mockDelegate = new Mock<RequestDelegate>();
        }

        private AspNetCoreMetricCollectorMiddleware CreateSystemUnderTest()
        {
            return new AspNetCoreMetricCollectorMiddleware(mockDelegate.Object, mockPersister.Object, Mock.Of<ILogger<AspNetCoreMetricCollectorMiddleware>>());
        }

        [Test, AutoMoqData]
        public async Task Invoke_uses_RequestDelegate(HttpContext context)
        {
            var collectors = new[] { Mock.Of<IMetricCollector>() };

            var sut = CreateSystemUnderTest();

            await sut.Invoke(context, collectors);

            mockDelegate.Verify(p => p(context));
        }

        [Test, AutoMoqData]
        public async Task Invoke_uses_collectors(HttpContext context, IMetricCollector collector)
        {
            var collectors = new[] { collector };

            var sut = CreateSystemUnderTest();

            await sut.Invoke(context, collectors);

            Mock.Get(collector).Verify(p => p.OnActionExecutingAsync(It.IsAny<DimensionDictionary>(), It.IsAny<MetricDictionary>()));
            Mock.Get(collector).Verify(p => p.OnActionExecutedAsync(It.IsAny<DimensionDictionary>(), It.IsAny<MetricDictionary>()));
        }

        [Test, AutoMoqData]
        public async Task Invoke_uses_persister(HttpContext context)
        {
            var collectors = new[] { Mock.Of<IMetricCollector>() };

            var sut = CreateSystemUnderTest();

            await sut.Invoke(context, collectors);

            mockPersister.Verify(p => p.PushAsync(It.IsAny<MetricData>()), Times.Once);
        }
    }
}
