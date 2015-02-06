using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FelicaData
{
    abstract public class UiValue<TEnum, TValue> : Base
        where TEnum: struct
    {
        public TEnum Type { get; set; }
        public TValue Value { get; set; }
    }
}
