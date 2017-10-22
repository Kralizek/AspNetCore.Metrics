using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kralizek.AspNetCore.Metrics
{
    public static class SetupExtensions
    {
        public static IServiceCollection PersistMetricsOnCloudWatch(this IServiceCollection services, params CloudWatchMetric[] metrics)
        {
            services.Configure<CloudWatchMetricPersisterConfiguration>(cfg =>
            {
                cfg.Metrics = metrics;
                cfg.Namespace = "AspNetCore";
                cfg.SkipDataValidation = false;
            });

            services.AddTransient<IMetricPersister, CloudWatchMetricPersister>();

            return services;
        }
    }
}
