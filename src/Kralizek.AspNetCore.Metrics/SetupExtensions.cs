using Kralizek.AspNetCore.Metrics.Middlewares;
using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kralizek.AspNetCore.Metrics
{
    public static class SetupExtensions
    {
        public static IApplicationBuilder UseMetricsCollector(this IApplicationBuilder builder)
        {
            builder.UseMiddleware<AspNetCoreMetricCollectorMiddleware>();

            return builder;
        }
    }
}
