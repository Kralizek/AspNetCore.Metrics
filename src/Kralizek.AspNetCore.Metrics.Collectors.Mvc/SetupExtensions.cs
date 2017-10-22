using Kralizek.AspNetCore.Metrics.Filters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kralizek.AspNetCore.Metrics
{
    public static class SetupExtensions
    {
        public static IMvcBuilder AddMvcDimensions(this IMvcBuilder builder)
        {
            builder.Services.Configure<MvcOptions>(options =>
            {
                options.Filters.Add<AspNetCoreMvcMetricCollectorActionFilter>();
            });

            builder.Services.AddScoped<IMetricCollector, AspNetCoreMvcMetricCollector>();

            builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            return builder;
        }
    }
}
