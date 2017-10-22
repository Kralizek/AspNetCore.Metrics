using Kralizek.AspNetCore.Metrics.Util;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kralizek.AspNetCore.Metrics.Filters
{
    public class AspNetCoreMvcMetricCollectorActionFilter : IActionFilter
    {
        public void OnActionExecuted(ActionExecutedContext context) { }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            var dimensions = new Dictionary<IMetricDimension, object>(MetricDimensionEqualityComparer.Default)
            {
                [AspNetCoreMvcMetricDimensions.ControllerName] = context.RouteData.Values["controller"] as string,
                [AspNetCoreMvcMetricDimensions.ActionName] = context.RouteData.Values["action"] as string
            };

            context.HttpContext.Items.Add(AspNetCoreMvcMetricCollector.HttpContextDimensionsKey, dimensions);

        }
    }
}
