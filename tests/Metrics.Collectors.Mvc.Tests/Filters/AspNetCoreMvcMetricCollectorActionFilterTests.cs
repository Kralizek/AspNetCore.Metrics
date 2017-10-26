using Kralizek.AspNetCore.Metrics;
using Kralizek.AspNetCore.Metrics.Filters;
using Microsoft.AspNetCore.Mvc.Filters;
using Moq;
using Xunit;
using System.Collections.Generic;

namespace Tests.Filters
{
    public class AspNetCoreMvcMetricCollectorActionFilterTests
    {
        [Theory, AutoMoqData]
        public void OnActionExecuting_adds_Dimensions_to_HttpContext_items(AspNetCoreMvcMetricCollectorActionFilter sut, ActionExecutingContext actionExecutingContext)
        {
            Mock.Get(actionExecutingContext.HttpContext.Items)
                .Setup(p => p.Add(AspNetCoreMvcMetricCollector.HttpContextDimensionsKey, It.IsAny<IReadOnlyDictionary<IMetricDimension, object>>()))
                .Verifiable();

            sut.OnActionExecuting(actionExecutingContext);

            Mock.Get(actionExecutingContext.HttpContext.Items).Verify();
        }

        [Theory, AutoMoqData]
        public void OnActionExecuting_adds_current_Controller_name_to_HttpContext_items(AspNetCoreMvcMetricCollectorActionFilter sut, ActionExecutingContext actionExecutingContext, string controllerName)
        {
            actionExecutingContext.RouteData.Values["controller"] = controllerName;

            var httpContextItems = new Dictionary<object, object>();

            Mock.Get(actionExecutingContext.HttpContext).SetupGet(p => p.Items).Returns(httpContextItems);

            sut.OnActionExecuting(actionExecutingContext);

            var dimensions = httpContextItems[AspNetCoreMvcMetricCollector.HttpContextDimensionsKey] as IReadOnlyDictionary<IMetricDimension, object>;

            Assert.True(dimensions.ContainsKey(AspNetCoreMvcMetricDimensions.ControllerName));
            Assert.Equal(dimensions[AspNetCoreMvcMetricDimensions.ControllerName], controllerName);
        }

        [Theory, AutoMoqData]
        public void OnActionExecuting_adds_current_Action_name_to_HttpContext_items(AspNetCoreMvcMetricCollectorActionFilter sut, ActionExecutingContext actionExecutingContext, string actionName)
        {
            actionExecutingContext.RouteData.Values["action"] = actionName;

            var httpContextItems = new Dictionary<object, object>();

            Mock.Get(actionExecutingContext.HttpContext).SetupGet(p => p.Items).Returns(httpContextItems);

            sut.OnActionExecuting(actionExecutingContext);

            var dimensions = httpContextItems[AspNetCoreMvcMetricCollector.HttpContextDimensionsKey] as IReadOnlyDictionary<IMetricDimension, object>;

            Assert.True(dimensions.ContainsKey(AspNetCoreMvcMetricDimensions.ActionName));
            Assert.Equal(dimensions[AspNetCoreMvcMetricDimensions.ActionName], actionName);
        }

    }
}
