using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Perfect
{
    public interface IFormatter
    {
        object Format(object value);
    }
}
