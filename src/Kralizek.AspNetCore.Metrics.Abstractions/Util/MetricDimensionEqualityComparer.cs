using System.Collections.Generic;

namespace Kralizek.AspNetCore.Metrics.Util
{
    public class MetricDimensionEqualityComparer : IEqualityComparer<IMetricDimension>
    {
        private MetricDimensionEqualityComparer() { }

        public static IEqualityComparer<IMetricDimension> Default = new MetricDimensionEqualityComparer();

        public bool Equals(IMetricDimension x, IMetricDimension y)
        {
            if (x == null && y == null) return true;

            if (x == null ^ y == null) return false;

            return string.Equals(x.Name, y.Name, System.StringComparison.InvariantCulture);
        }

        public int GetHashCode(IMetricDimension obj)
        {
            if (obj == null) throw new System.ArgumentNullException(nameof(obj));

            return nameof(IMetricDimension).GetHashCode() ^ obj.Name.GetHashCode();
        }
    }

}
