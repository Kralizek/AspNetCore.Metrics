using Amazon.Util;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using static Kralizek.AspNetCore.Metrics.EC2MetadataDimensions;

namespace Kralizek.AspNetCore.Metrics
{
    public class EC2MetricCollector : IMetricCollector
    {
        public Task OnActionExecutedAsync(IDictionary<IMetricDimension, object> dimensions, IDictionary<IMetricValue, IValue> metrics) => Task.CompletedTask;

        public Task OnActionExecutingAsync(IDictionary<IMetricDimension, object> dimensions, IDictionary<IMetricValue, IValue> metrics)
        {
            if (AWSMetadataProvider.IsInAWS())
            {
                dimensions.Add(InstanceId, AWSMetadataProvider.GetInstanceId());
                dimensions.Add(InstanceType, AWSMetadataProvider.GetInstanceType());
                dimensions.Add(AvailabilityZone, AWSMetadataProvider.GetAvailabilityZone());
                dimensions.Add(AmiId, AWSMetadataProvider.GetAmiId());
            }

            return Task.CompletedTask;
        }
    }

    public static class EC2MetadataDimensions
    {
        public static IMetricDimension InstanceId = new MetricDimension("EC2:InstanceID");
        public static IMetricDimension InstanceType = new MetricDimension("EC2:InstanceType");
        public static IMetricDimension AvailabilityZone = new MetricDimension("EC2:AvailabilityZone");
        public static IMetricDimension AmiId = new MetricDimension("EC2:AmiId");
    }
}
