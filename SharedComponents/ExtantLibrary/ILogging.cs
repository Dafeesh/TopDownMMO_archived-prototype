using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Extant
{
    public interface ILogging
    {
        DebugLogger Log
        {
            get;
        }
    }
}
