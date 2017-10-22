using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Kralizek.AspNetCore.Metrics
{
    public interface IMetricCollector
    {
        Task OnActionExecutingAsync(IDictionary<IMetricDimension, object> dimensions, IDictionary<IMetricValue, IValue> metrics);

        Task OnActionExecutedAsync(IDictionary<IMetricDimension, object> dimensions, IDictionary<IMetricValue, IValue> metrics);
    }
}
