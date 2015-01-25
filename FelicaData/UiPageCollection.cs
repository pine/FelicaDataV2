using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MongoDB.Driver.Builders;

namespace FelicaData
{
    public class UiPageCollection : CollectionBase<UiPage>
    {
        public UiPageCollection(
            DatabaseManager dbMgr,
            CollectionManager colMgr)
            : base(dbMgr, colMgr, "UiPages")
        {
        }

        public UiPage GetUiPage(UiPageType type)
        {
            var setting = this.FindOne(Query<UiPage>.EQ(e => e.Type, type));

            if (setting != null)
            {
                return setting;
            }

            return this.Insert(new UiPage { Type = type });
        }

        public UiPage UpdateUiPage(UiPage setting)
        {
            return this.Update(setting);
        }
    }
}
