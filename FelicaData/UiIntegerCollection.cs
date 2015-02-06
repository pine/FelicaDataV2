using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FelicaData
{
    public class UiIntegerCollection : UiValueCollection<UiInteger, UiIntegerType, int>
    {
        public UiIntegerCollection(
            DatabaseManager dbMgr,
            CollectionManager colMgr)
            : base(dbMgr, colMgr, "UiIntegers")
        {
        }

        public int GetInteger(UiIntegerType type)
        {
            return this.GetValue(type, 0);
        }

        public void UpdateInteger(UiIntegerType type, int value)
        {
            this.UpdateValue(type, value);
        }
    }
}
