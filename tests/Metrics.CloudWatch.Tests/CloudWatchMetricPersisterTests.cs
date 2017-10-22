using Amazon.CloudWatch;
using Amazon.CloudWatch.Model;
using AutoFixture;
using AutoFixture.AutoMoq;
using Kralizek.AspNetCore.Metrics;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Tests
{
    [TestFixture]
    public class CloudWatchMetricPersisterTests
    {
        private IFixture fixture;

        private Mock<IAmazonCloudWatch> mockClient;
        private Mock<IOptions<CloudWatchMetricPersisterConfiguration>> mockOptions;

        //private CloudWatchMetricPersisterConfiguration configuration = new CloudWatchMetricPersisterConfiguration();

        [SetUp]
        public void Initialize()
        {
            fixture = new Fixture().Customize(new AutoMoqCustomization());

            fixture.Register(() =>
            {
                var name = fixture.Create<string>();

                Mock<IMetricDimension> mock = new Mock<IMetricDimension>();
                mock.SetupGet(p => p.Name).Returns(name);

                return mock.Object;
            });

            fixture.Register(() =>
            {
                var name = fixture.Create<string>();

                Mock<IMetricValue> mock = new Mock<IMetricValue>();
                mock.SetupGet(p => p.Name).Returns(name);

                return mock.Object;
            });

            mockClient = new Mock<IAmazonCloudWatch>();

            mockOptions = new Mock<IOptions<CloudWatchMetricPersisterConfiguration>>();
        }

        private CloudWatchMetricPersister CreateSystemUnderTest()
        {
            return new CloudWatchMetricPersister(mockClient.Object, mockOptions.Object, Mock.Of<ILogger<CloudWatchMetricPersister>>());
        }

        [Test, AutoMoqData]
        public async Task PutRequest_body_is_correctly_created(IValue value)
        {
            var metric = fixture.Create<CloudWatchMetric>();

            var configuration = fixture.Build<CloudWatchMetricPersisterConfiguration>()
                                       .With(p => p.Metrics, new List<CloudWatchMetric> { metric })
                                       .With(p => p.SkipDataValidation, false)
                                       .Create();

            mockOptions.SetupGet(p => p.Value).Returns(configuration);

            var sut = CreateSystemUnderTest();

            MetricData data = new MetricData
            {
                Metrics = new Dictionary<IMetricValue, IValue>
                {
                    [metric.Metric] = value
                },
                Dimensions = (from dimension in metric.Dimensions
                              select new
                              {
                                  Dimension = dimension,
                                  Value = fixture.Create<string>()
                              }).ToDictionary(k => k.Dimension, v => (object)v.Value)
            };

            await sut.PushAsync(data);

            mockClient.Verify(p => p.PutMetricDataAsync(It.Is<PutMetricDataRequest>(re => ValidatePutRequestBody(re, data, configuration)), default(CancellationToken)));

        }

        bool ValidatePutRequestBody(PutMetricDataRequest request, MetricData data, CloudWatchMetricPersisterConfiguration configuration)
        {
            Assert.That(request.Namespace, Is.EqualTo(configuration.Namespace));

            Assert.That(request.MetricData, Has.Exactly(1).InstanceOf<MetricDatum>());

            return true;
        }

        [Test, AutoMoqData]
        public async Task PutRequest_datum_is_correctly_created(IValue value)
        {
            var metric = fixture.Create<CloudWatchMetric>();

            var configuration = fixture.Build<CloudWatchMetricPersisterConfiguration>()
                                       .With(p => p.Metrics, new List<CloudWatchMetric> { metric })
                                       .With(p => p.SkipDataValidation, false)
                                       .Create();

            mockOptions.SetupGet(p => p.Value).Returns(configuration);

            var sut = CreateSystemUnderTest();

            MetricData data = new MetricData
            {
                Metrics = new Dictionary<IMetricValue, IValue>
                {
                    [metric.Metric] = value
                },
                Dimensions = (from dimension in metric.Dimensions
                              select new
                              {
                                  Dimension = dimension,
                                  Value = fixture.Create<string>()
                              }).ToDictionary(k => k.Dimension, v => (object)v.Value)
            };

            await sut.PushAsync(data);

            mockClient.Verify(p => p.PutMetricDataAsync(It.Is<PutMetricDataRequest>(re => ValidateMetricDatum(re, data, configuration)), default(CancellationToken)));
        }

        bool ValidateMetricDatum(PutMetricDataRequest request, MetricData data, CloudWatchMetricPersisterConfiguration configuration)
        {
            var metric = configuration.Metrics[0];

            var datum = request.MetricData[0];

            Assert.That(datum.Value, Is.EqualTo(data.Metrics[metric.Metric].ReadAsDouble()));

            Assert.That(datum.Unit, Is.EqualTo(metric.Unit));

            Assert.That(datum.StorageResolution, Is.EqualTo((int)metric.StorageResolution));

            return true;
        }

        [Test, AutoMoqData]
        public async Task PutRequest_datum_dimensions_are_correctly_created(IValue value)
        {
            var metric = fixture.Create<CloudWatchMetric>();

            var configuration = fixture.Build<CloudWatchMetricPersisterConfiguration>()
                                       .With(p => p.Metrics, new List<CloudWatchMetric> { metric })
                                       .With(p => p.SkipDataValidation, false)
                                       .Create();

            mockOptions.SetupGet(p => p.Value).Returns(configuration);

            var sut = CreateSystemUnderTest();

            MetricData data = new MetricData
            {
                Metrics = new Dictionary<IMetricValue, IValue>
                {
                    [metric.Metric] = value
                },
                Dimensions = (from dimension in metric.Dimensions
                              select new
                              {
                                  Dimension = dimension,
                                  Value = fixture.Create<string>()
                              }).ToDictionary(k => k.Dimension, v => (object)v.Value)
            };

            await sut.PushAsync(data);

            mockClient.Verify(p => p.PutMetricDataAsync(It.Is<PutMetricDataRequest>(re => ValidateMetricDatumDimensions(re, data, configuration)), default(CancellationToken)));
        }

        bool ValidateMetricDatumDimensions(PutMetricDataRequest request, MetricData data, CloudWatchMetricPersisterConfiguration configuration)
        {
            var metric = configuration.Metrics[0];

            var datum = request.MetricData[0];

            Assert.That(datum.Dimensions, Has.Exactly(metric.Dimensions.Count).InstanceOf<Dimension>());

            return true;
        }
    }
}
