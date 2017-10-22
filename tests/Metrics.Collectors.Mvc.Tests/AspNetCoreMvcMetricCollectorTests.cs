using Kralizek.AspNetCore.Metrics;
using Microsoft.AspNetCore.Http;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Tests
{
    [TestFixture]
    public class AspNetCoreMvcMetricCollectorTests
    {
        [Test, AutoMoqData]
        public void ControllerName_is_added_from_HttpContext(IHttpContextAccessor accessor, IDictionary<IMetricDimension, object> dimensions, IDictionary<IMetricValue, IValue> metrics, string controllerName)
        {
            var sut = new AspNetCoreMvcMetricCollector(accessor);

            var contextDimensions = new Dictionary<IMetricDimension, object>
            {
                [AspNetCoreMvcMetricDimensions.ControllerName] = controllerName
            };
            var contextMetrics = new Dictionary<IMetricValue, IValue>();

            Mock.Get(accessor.HttpContext.Items).Setup(p => p[AspNetCoreMvcMetricCollector.HttpContextDimensionsKey]).Returns(contextDimensions);
            Mock.Get(accessor.HttpContext.Items).Setup(p => p[AspNetCoreMvcMetricCollector.HttpContextMetricsKey]).Returns(contextMetrics);

            sut.OnActionExecutedAsync(dimensions, metrics);

            Assert.That(dimensions.ContainsKey(AspNetCoreMvcMetricDimensions.ControllerName));
            Assert.That(dimensions[AspNetCoreMvcMetricDimensions.ControllerName], Is.EqualTo(controllerName));
        }


        [Test, AutoMoqData]
        public void ActionName_is_added_from_HttpContext(IHttpContextAccessor accessor, IDictionary<IMetricDimension, object> dimensions, IDictionary<IMetricValue, IValue> metrics, string actionName)
        {
            var sut = new AspNetCoreMvcMetricCollector(accessor);

            var contextDimensions = new Dictionary<IMetricDimension, object>
            {
                [AspNetCoreMvcMetricDimensions.ActionName] = actionName
            };
            var contextMetrics = new Dictionary<IMetricValue, IValue>();

            Mock.Get(accessor.HttpContext.Items).Setup(p => p[AspNetCoreMvcMetricCollector.HttpContextDimensionsKey]).Returns(contextDimensions);
            Mock.Get(accessor.HttpContext.Items).Setup(p => p[AspNetCoreMvcMetricCollector.HttpContextMetricsKey]).Returns(contextMetrics);

            sut.OnActionExecutedAsync(dimensions, metrics);

            Assert.That(dimensions.ContainsKey(AspNetCoreMvcMetricDimensions.ActionName));
            Assert.That(dimensions[AspNetCoreMvcMetricDimensions.ActionName], Is.EqualTo(actionName));
        }
    }
}
