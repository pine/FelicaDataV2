using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MongoDB.Driver.Builders;

namespace FelicaData
{
    public class MoneyHistoryCollection : CollectionBase<MoneyHistory>
    {
        public MoneyHistoryCollection(
            DatabaseManager dbMgr,
            CollectionManager colMgr)
            : base(dbMgr, colMgr, "MoneyHistories")
        {
        }

        public MoneyHistory GetMoneyHistory(string id)
        {
            return this.FindOne(id);
        }

        public List<MoneyHistory> GetMoneyHistories(string userId)
        {
            return this.Find(Query<MoneyHistory>.EQ(e => e.UserId, userId));
        }

        public MoneyHistory CreateMoneyHistory(MoneyHistory history)
        {
            var user = this.Collections.Users.GetUser(history.UserId);
            var performerUser = this.Collections.Users.GetUser(history.PerformerUserId);

            if (user == null || performerUser == null)
            {
                throw new DatabaseException("ユーザーが存在しません。");
            }

            history.CreatedAt = DateTime.Now;

            return this.Insert(history);
        }

        public void UpdateMoneyHistory(MoneyHistory history)
        {
            this.Update(history);
        }
    }
}
