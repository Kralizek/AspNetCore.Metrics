using Kralizek.AspNetCore.Metrics.Util;
using Kralizek.AspNetCore.Metrics.Values;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Threading.Tasks;

namespace Kralizek.AspNetCore.Metrics.Middlewares
{
    public class AspNetCoreMetricCollectorMiddleware
    {
        private readonly RequestDelegate next;
        private readonly IMetricPersister persister;
        private readonly ILogger<AspNetCoreMetricCollectorMiddleware> logger;

        public AspNetCoreMetricCollectorMiddleware(RequestDelegate next, IMetricPersister persister, ILogger<AspNetCoreMetricCollectorMiddleware> logger)
        {
            this.next = next ?? throw new ArgumentNullException(nameof(next));
            this.persister = persister ?? throw new ArgumentNullException(nameof(persister));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Invoke(HttpContext context, IEnumerable<IMetricCollector> collectors)
        {
            var metrics = new Dictionary<IMetricValue, IValue>(MetricValueEqualityComparer.Default);
            var dimensions = new Dictionary<IMetricDimension, object>(MetricDimensionEqualityComparer.Default)
            {
                [MetricDimensions.MachineName] = Environment.MachineName,
                [MetricDimensions.RequestHost] = context.Request.Host,
                [MetricDimensions.RequestPath] = context.Request.Path,
                [MetricDimensions.HttpMethod] = context.Request.Method,
                [MetricDimensions.IsHttps] = context.Request.IsHttps
            };

            Stopwatch sw = Stopwatch.StartNew();

            foreach (var collector in collectors)
                await collector.OnActionExecutingAsync(dimensions, metrics);

            await next(context);

            foreach (var collector in collectors)
                await collector.OnActionExecutedAsync(dimensions, metrics);

            sw.Stop();

            metrics.Add(MetricValues.ElapsedTime, new ElapsedTimeValue(sw.Elapsed));
            dimensions.Add(MetricDimensions.HttpResponseStatus, (HttpStatusCode)context.Response.StatusCode);

            var metricData = new MetricData
            {
                Dimensions = dimensions,
                Metrics = metrics
            };


            try
            {
                await persister.PushAsync(metricData);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unable to push metrics");
                throw;

            }
        }
    }
}