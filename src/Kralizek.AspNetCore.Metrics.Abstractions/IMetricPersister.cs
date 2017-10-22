using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Kralizek.AspNetCore.Metrics
{
    public interface IMetricPersister
    {
        Task PushAsync(MetricData data);
    }

    public class MetricData
    {
        public IReadOnlyDictionary<IMetricDimension, object> Dimensions { get; set; }

        public IReadOnlyDictionary<IMetricValue, IValue> Metrics { get; set; }
    }
}
