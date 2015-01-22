using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MongoDB.Driver.Builders;
using TValue = FelicaData.UiText;

namespace FelicaData
{
    public class UiTextCollection : CollectionBase<TValue>
    {
        public UiTextCollection(
            DatabaseManager dbMgr,
            CollectionManager colMgr)
            : base(dbMgr, colMgr, "UiTexts")
        {
        }

        public string GetText(UiTextType type)
        {
            var text = this.FindOne(Query<TValue>.EQ(e => e.Type, type));

            if (text != null)
            {
                return text.Value;
            }

            this.Insert(new TValue { Type = type, Value = "" });
            return "";
        }

        public void UpdateText(UiTextType type, string value)
        {
            var text = this.FindOne(Query<TValue>.EQ(e => e.Type, type));

            if (text != null)
            {
                text.Value = value;
                this.Update(text);
            }
            else
            {
                this.Insert(new TValue { Type = type, Value = value });
            }
        }
    }
}
