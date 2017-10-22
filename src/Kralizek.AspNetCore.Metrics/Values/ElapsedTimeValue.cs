using System;
using System.Collections.Generic;
using System.Text;

namespace Kralizek.AspNetCore.Metrics.Values
{
    public class ElapsedTimeValue : IValue
    {
        private readonly TimeSpan value;

        public ElapsedTimeValue(TimeSpan value)
        {
            this.value = value;
        }

        public double ReadAsDouble() => value.TotalMilliseconds;
    }

}
