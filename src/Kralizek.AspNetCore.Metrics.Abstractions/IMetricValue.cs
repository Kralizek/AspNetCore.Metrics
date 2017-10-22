using System;
using System.Collections.Generic;
using System.Text;

namespace Kralizek.AspNetCore.Metrics
{
    public interface IMetricValue
    {
        string Name { get; }
    }

    public class MetricValue : IMetricValue
    {
        public MetricValue(string name)
        {
            Name = name ?? throw new System.ArgumentNullException(nameof(name));
        }

        public string Name { get; }

        public override string ToString() => Name;
    }
}
