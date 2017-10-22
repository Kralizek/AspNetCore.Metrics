using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kralizek.AspNetCore.Metrics
{
    public static class SetupExtensions
    {
        public static IServiceCollection CollectDimensionsFromEC2InstanceMetadata(this IServiceCollection services)
        {
            services.AddSingleton<IMetricCollector, EC2MetricCollector>();

            return services;
        }
    }
}
