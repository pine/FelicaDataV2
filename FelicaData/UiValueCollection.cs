using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MongoDB.Driver.Builders;

namespace FelicaData
{
    abstract public class UiValueCollection<TUiValue, TEnum, TValue>
        : CollectionBase<TUiValue>

        where TEnum: struct
        where TUiValue: UiValue<TEnum, TValue>, new()
    {
        public UiValueCollection(
            DatabaseManager dbMgr,
            CollectionManager colMgr,
            string collectionName)
            : base(dbMgr, colMgr, collectionName)
        {
        }

        protected TValue GetValue(TEnum type, TValue defaultValue)
        {
            var val = this.FindOne(Query<TUiValue>.EQ(e => e.Type, type));

            if (val != null)
            {
                return val.Value;
            }

            this.Insert(new TUiValue { Type = type, Value = defaultValue });
            return defaultValue;
        }

        protected void UpdateValue(TEnum type, TValue value)
        {
            var val = this.FindOne(Query<TUiValue>.EQ(e => e.Type, type));

            if (val != null)
            {
                val.Value = value;
                this.Update(val);
            }
            else
            {
                this.Insert(new TUiValue { Type = type, Value = value });
            }
        }
    }
}
