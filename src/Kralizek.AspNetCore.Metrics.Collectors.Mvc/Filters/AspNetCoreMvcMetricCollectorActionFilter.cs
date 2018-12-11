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
            var controllerName = context.RouteData.Values["controller"] as string;
            var actionName = context.RouteData.Values["action"] as string;
            
            var controllerNamespace = context.Controller?.GetType().Namespace;

            var dimensions = new Dictionary<IMetricDimension, object>(MetricDimensionEqualityComparer.Default);

            if (!string.IsNullOrEmpty(controllerName))
            {
                dimensions.Add(AspNetCoreMvcMetricDimensions.ControllerName, controllerName);
            }

            if (!string.IsNullOrEmpty(actionName))
            {
                dimensions.Add(AspNetCoreMvcMetricDimensions.ActionName, actionName);
            }

            if (!string.IsNullOrEmpty(controllerName) && !string.IsNullOrEmpty(actionName) &&
                !string.IsNullOrEmpty(controllerNamespace))
            {
                dimensions.Add(AspNetCoreMvcMetricDimensions.Action, $"{controllerName}.{actionName} ({controllerNamespace})");
            }

            context.HttpContext.Items.Add(AspNetCoreMvcMetricCollector.HttpContextDimensionsKey, dimensions);

        }
    }
}
