using System;
using System.Collections.Generic;
using System.Text;

namespace Kralizek.AspNetCore.Metrics
{
    public interface IValue
    {
        double ReadAsDouble();
    }
}
