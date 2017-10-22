using System;
using System.Collections.Generic;
using System.Text;

namespace Kralizek.AspNetCore.Metrics
{
    public static class MetricDimensions
    {
        public static readonly IMetricDimension MachineName = new MetricDimension("MachineName");

        public static readonly IMetricDimension RequestHost = new MetricDimension("RequestHost");
        public static readonly IMetricDimension RequestPath = new MetricDimension("RequestPath");
        public static readonly IMetricDimension HttpMethod = new MetricDimension("HttpMethod");
        public static readonly IMetricDimension IsHttps = new MetricDimension("IsHttps");
        public static readonly IMetricDimension HttpResponseStatus = new MetricDimension("HttpResponseStatus");
    }

}
