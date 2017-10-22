using System;
using System.Collections.Generic;
using System.Text;

namespace Kralizek.AspNetCore.Metrics.Util
{
    public class MetricValueEqualityComparer : IEqualityComparer<IMetricValue>
    {
        private MetricValueEqualityComparer() { }

        public static IEqualityComparer<IMetricValue> Default = new MetricValueEqualityComparer();

        public bool Equals(IMetricValue x, IMetricValue y)
        {
            if (x == null && y == null) return true;

            if (x == null ^ y == null) return false;

            return string.Equals(x.Name, y.Name, System.StringComparison.InvariantCulture);
        }

        public int GetHashCode(IMetricValue obj)
        {
            if (obj == null) throw new System.ArgumentNullException(nameof(obj));

            return nameof(IMetricValue).GetHashCode() ^ obj.Name.GetHashCode();
        }
    }

}
