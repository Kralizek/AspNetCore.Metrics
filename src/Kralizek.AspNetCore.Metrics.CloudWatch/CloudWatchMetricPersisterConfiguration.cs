using Amazon.CloudWatch;
using System.Collections.Generic;

namespace Kralizek.AspNetCore.Metrics
{
    public class CloudWatchMetricPersisterConfiguration
    {
        public IReadOnlyList<CloudWatchMetric> Metrics { get; set; }

        public string Namespace { get; set; }

        public bool SkipDataValidation { get; set; }
    }

    public class CloudWatchMetric
    {
        public CloudWatchMetric(string name, IMetricValue metric, params IMetricDimension[] dimensions)
        {
            Dimensions = dimensions ?? throw new System.ArgumentNullException(nameof(dimensions));
            Name = name ?? throw new System.ArgumentNullException(nameof(name));
            Metric = metric ?? throw new System.ArgumentNullException(nameof(metric));
        }

        public string Name { get; }

        public IMetricValue Metric { get; }

        public StandardUnit Unit { get; set; } = StandardUnit.None;

        public StorageResolution StorageResolution { get; set; } = StorageResolution.Low;

        public IReadOnlyList<IMetricDimension> Dimensions { get; }
    }

    public enum StorageResolution
    {
        High = 1,
        Low = 60
    }
}
