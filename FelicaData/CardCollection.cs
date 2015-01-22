using MongoDB.Driver.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FelicaData
{
    public class CardCollection : CollectionBase<Card>
    {
        private readonly UserCollection users;

        public CardCollection(
            DatabaseManager dbMgr,
            CollectionManager colMgr
            )
            : base(dbMgr, colMgr, "Cards")
        {
            this.users = colMgr.Users;
        }

        public Card GetCard(string id)
        {
            return this.FindOne(id);
        }

        public Card GetCardByUid(string uid)
        {
            return this.FindOne(Query<Card>.EQ(e => e.Uid, uid));
        }

        public List<Card> GetCardsByUserId(string userId)
        {
            return this.Find(Query<Card>.EQ(e => e.UserId, userId));
        }

        public Card GetCardByName(string userId, string name)
        {
            return this.FindOne(Query.And(
                Query<Card>.EQ(e => e.UserId, userId),
                Query<Card>.EQ(e => e.Name, name)));
        }

        public Card CreateCard(Card card)
        {
            var user = this.users.GetUser(card.UserId);
            var sameUid = this.GetCard(card.Uid);
            var sameName = this.GetCardByName(card.UserId, card.Name);

            if (user == null)
            {
                throw new DatabaseException("ユーザーが存在しません。");
            }

            if (sameUid != null)
            {
                throw new DatabaseException("既にカードが登録されています。");
            }

            if (sameName != null)
            {
                throw new DatabaseException("既に同じ名前のカードが存在します。");
            }

            return this.Insert(card);
        }

        public void UpdateCard(Card card)
        {
            var oldCard = this.GetCard(card.Id);

            if (oldCard != null)
            {
                // 変更不可項目
                card.Uid = oldCard.Uid;
                card.UserId = oldCard.UserId;

                this.Update(card);
            }
        }

        /// <summary>
        /// カードを削除します
        /// </summary>
        public void DeleteCard(string id)
        {
            if (id == null){
                throw new ArgumentNullException("id");
            }

            var sameCard = this.GetCard(id);

            if (sameCard == null)
            {
                throw new DatabaseException("このカードは削除できません。");
            }

            this.Remove(sameCard.Id);
        }

        /// <summary>
        /// カードを既存ユーザーに関連付ける
        /// </summary>
        /// <param name="user"></param>
        /// <param name="card"></param>
        public Card Assoate(Card card)
        {
            var sameIdUser = this.Collections.Users.GetUser(card.UserId);

            if (sameIdUser == null)
            {
                throw new DatabaseException("ユーザーが存在しません");
            }

            var sameUidCard = this.GetCard(card.Uid);

            if (sameUidCard != null)
            {
                throw new DatabaseException("カードが既に登録されています。");
            }

            return this.CreateCard(card);
        }
    }
}
