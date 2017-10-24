using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Kralizek.AspNetCore.Metrics
{
    public class AspNetCoreMvcMetricCollector : IMetricCollector
    {
        public const string HttpContextDimensionsKey = "AspNetMetricCollector:Dimensions";
        public const string HttpContextMetricsKey = "AspNetMetricCollector:Metrics";

        private readonly HttpContext httpContext;

        public AspNetCoreMvcMetricCollector(IHttpContextAccessor httpContext)
        {
            this.httpContext = httpContext?.HttpContext ?? throw new ArgumentNullException(nameof(httpContext));
        }

        public Task OnActionExecutingAsync(IDictionary<IMetricDimension, object> dimensions, IDictionary<IMetricValue, IValue> metrics) => Task.CompletedTask;

        public Task OnActionExecutedAsync(IDictionary<IMetricDimension, object> dimensions, IDictionary<IMetricValue, IValue> metrics)
        {
            var contextDimensions = httpContext.Items[HttpContextDimensionsKey] as IReadOnlyDictionary<IMetricDimension, object>;

            foreach (var pair in contextDimensions)
            {
                dimensions.Add(pair.Key, pair.Value);
            }

            return Task.CompletedTask;
        }
    }

}