using System;
using System.Collections.Generic;
using System.Text;

namespace Kralizek.AspNetCore.Metrics.Values
{
    public class NumericValue : IValue
    {
        private readonly double value;

        public NumericValue(double value)
        {
            this.value = value;
        }

        public double ReadAsDouble() => value;
    }
}
