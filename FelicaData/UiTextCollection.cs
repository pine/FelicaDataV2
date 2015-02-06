using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MongoDB.Driver.Builders;
using TValue = FelicaData.UiText;

namespace FelicaData
{
    public class UiTextCollection : UiValueCollection<UiText, UiTextType, string>
    {
        public UiTextCollection(
            DatabaseManager dbMgr,
            CollectionManager colMgr)
            : base(dbMgr, colMgr, "UiTexts")
        {
        }

        public string GetText(UiTextType type)
        {
            return this.GetValue(type, "");
        }

        public void UpdateText(UiTextType type, string value)
        {
            this.UpdateValue(type, value);
        }
    }
}
