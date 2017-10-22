using System;
using System.Collections.Generic;
using System.Text;

namespace Kralizek.AspNetCore.Metrics
{
    public static class AspNetCoreMvcMetricDimensions
    {
        public static readonly IMetricDimension ControllerName = new MetricDimension("ControllerName");
        public static readonly IMetricDimension ActionName = new MetricDimension("ActionName");
    }
}
