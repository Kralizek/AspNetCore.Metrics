using Amazon.CloudWatch;
using Amazon.CloudWatch.Model;
using AutoFixture;
using AutoFixture.AutoMoq;
using Kralizek.AspNetCore.Metrics;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Tests
{
    public class CloudWatchMetricPersisterTests
    {
        private IFixture fixture;

        private Mock<IAmazonCloudWatch> mockClient;
        private Mock<IOptions<CloudWatchMetricPersisterConfiguration>> mockOptions;

        public CloudWatchMetricPersisterTests()
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

        [Theory, AutoMoqData]
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
            Assert.Equal(configuration.Namespace, request.Namespace);

            Assert.Equal(1, request.MetricData.OfType<MetricDatum>().Count());

            return true;
        }

        [Theory, AutoMoqData]
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

            Assert.Equal(datum.Value, data.Metrics[metric.Metric].ReadAsDouble());

            Assert.Equal(datum.Unit, metric.Unit);

            Assert.Equal(datum.StorageResolution, (int)metric.StorageResolution);

            return true;
        }

        [Theory, AutoMoqData]
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

            Assert.Equal(datum.Dimensions.OfType<Dimension>().Count(),metric.Dimensions.Count);

            return true;
        }
    }
}
