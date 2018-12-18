using Amazon.CloudWatch;
using Amazon.CloudWatch.Model;
using Kralizek.AspNetCore.Metrics.Abstractions.Util;
using Kralizek.AspNetCore.Metrics.Util;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kralizek.AspNetCore.Metrics
{
    public class CloudWatchMetricPersister : IMetricPersister
    {
        private readonly IAmazonCloudWatch cloudWatch;
        private readonly ILogger<CloudWatchMetricPersister> logger;
        private readonly CloudWatchMetricPersisterConfiguration configuration;

        public CloudWatchMetricPersister(IAmazonCloudWatch cloudWatch, IOptions<CloudWatchMetricPersisterConfiguration> configuration, ILogger<CloudWatchMetricPersister> logger)
        {
            this.cloudWatch = cloudWatch ?? throw new ArgumentNullException(nameof(cloudWatch));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.configuration = configuration?.Value ?? throw new ArgumentNullException(nameof(configuration));
        }

        public async Task PushAsync(MetricData data)
        {
            if (!configuration.SkipDataValidation && !IsDataSufficient(data))
            {
                logger.LogDebug("Collected data is not sufficient for configured metrics");
                return;
            }

            var now = DateTimeOffset.UtcNow.UtcDateTime;

            var metrics = from item in configuration.Metrics
                          where data.Metrics.ContainsKey(item.Metric)
                          let mitem = data.Metrics[item.Metric]
                          where mitem != null
                          select new MetricDatum
                          {
                              MetricName = item.Name,
                              Unit = item.Unit,
                              StorageResolution = (int)item.StorageResolution,
                              Timestamp = now,
                              Value = mitem.ReadAsDouble(),
                              Dimensions = new List<Dimension>(from dim in item.Dimensions
                                                               where data.Dimensions.ContainsKey(dim)
                                                               let ditem = data.Dimensions[dim]
                                                               where ditem != null
                                                               select new Dimension
                                                               {
                                                                   Name = dim.Name,
                                                                   Value = ditem.ToString()
                                                               })
                          };

            var chunks = metrics.Chunk(20);

            foreach (var chunk in chunks)
            {
                var request = new PutMetricDataRequest
                {
                    Namespace = configuration.Namespace,
                    MetricData = new List<MetricDatum>(chunk)
                };

                if (request.MetricData.Count > 0)
                {
                    try
                    {
                        var response = await cloudWatch.PutMetricDataAsync(request);

                        logger.LogDebug($"Pushed {request.MetricData.Count} metrics to CloudWatch");
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, "Unable to push metrics to CloudWatch");
                        throw;
                    }
                }
                else
                {
                    logger.LogDebug("No metric was pushed to CloudWatch. Check your configuration.");
                }
            }
        }

        private bool IsDataSufficient(MetricData data)
        {
            if (!configuration.Metrics.Select(m => m.Metric).Distinct(MetricValueEqualityComparer.Default).All(data.Metrics.ContainsKey))
                return false;

            if (!configuration.Metrics.SelectMany(m => m.Dimensions).Distinct(MetricDimensionEqualityComparer.Default).All(data.Dimensions.ContainsKey))
                return false;

            return true;
        }
    }

}
