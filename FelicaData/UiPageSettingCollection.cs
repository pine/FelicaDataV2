using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MongoDB.Driver.Builders;

namespace FelicaData
{
    public class UiPageSettingCollection : CollectionBase<UiPageSetting>
    {
        public UiPageSettingCollection(
            DatabaseManager dbMgr,
            CollectionManager colMgr)
            : base(dbMgr, colMgr, "UiPageSettings")
        {
        }

        public UiPageSetting GetUiPageSetting(UiPageType type)
        {
            var setting = this.FindOne(Query<UiPageSetting>.EQ(e => e.Type, type));

            if (setting != null)
            {
                return setting;
            }

            return this.Insert(new UiPageSetting { Type = type });
        }

        public UiPageSetting UpdateUiPageSetting(UiPageSetting setting)
        {
            return this.Update(setting);
        }
    }
}
