using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MongoDB.Bson;
using MongoDB.Driver.Builders;
using System.Diagnostics;

namespace FelicaData
{
    public class UserCollection : CollectionBase<User>
    {
        public UserCollection(
            DatabaseManager dbMgr,
            CollectionManager colMgr)
            : base(dbMgr, colMgr, "Users")
        {
        }

        protected CardCollection Cards
        {
            get { return this.Collections.Cards; }
        }

        public List<User> GetUsers()
        {
            var users = this.Find();
            users.Sort((lhs, rhs) =>
            {
                if (lhs.Id == rhs.Id) { return 0; }
                return lhs.Name.CompareTo(rhs.Name);
            });

            return users;
        }

        /// <summary>
        /// 管理者ユーザーの一覧を取得します。
        /// </summary>
        /// <returns>管理者ユーザーのリスト</returns>
        public List<User> GetAdminUsers()
        {
            var users = this.Find(Query<User>.EQ(e => e.IsAdmin, true));
            return users;
        }

        public User GetUser(string id)
        {
            if (id == null)
            {
                throw new ArgumentNullException("id");
            }

            return this.FindOne(id);
        }

        public User GetUserByName(string name)
        {
            return this.FindOne(Query<User>.EQ(e => e.Name, name));
        }

        public User GetUserByEmail(string email)
        {
            return this.FindOne(Query<User>.EQ(e => e.Email, email));
        }

        /// <summary>
        /// ユーザーを新規作成します。
        /// </summary>
        /// <param name="user">新規作成するユーザーの情報</param>
        /// <returns>新規作成に成功した場合、作成したユーザー</returns>
        /// <exception cref="DatabaseException">ユーザーの作成でエラーが発生した場合</exception>
        public User CreateUser(User user, Card card = null)
        {
            if (user == null) { throw new ArgumentNullException("user"); }

            if (user.Name == null)
            {
                throw new DatabaseException("ユーザー名が無効です。");
            }
            else
            {
                // 同じ名前のユーザーが居るか確認
                var sameName = this.GetUserByName(user.Name);

                if (sameName != null)
                {
                    throw new DatabaseException("既に同じ名前のユーザーが存在します。");
                }
                else
                {
                    if (this.Insert(user) == null)
                    {
                        return null;
                    }

                    if (card != null)
                    {
                        var sameCard = this.Cards.GetCardByUid(card.Uid);

                        if (sameCard != null)
                        {
                            throw new DatabaseException("既に同じカードが登録されています。");
                        }

                        card.UserId = user.Id;

                        if (this.Cards.Insert(card) != null)
                        {
                            return user;
                        }
                        else // カード登録失敗
                        {
                            this.DeleteUser(user.Id);
                        }
                    }

                    else
                    {
                        return user;
                    }
                }
                
            }

            return null;
        }

        public void UpdateUser(User user)
        {
            if (user == null) { throw new ArgumentNullException("user"); }

            var sameUser = this.GetUserByName(user.Name);
            if (sameUser != null && user.Id != sameUser.Id) { throw new DatabaseException("既に同じ名前のユーザーが存在します。"); }

            this.Update(user);
        }

        public void DeleteUser(string id)
        {
            this.Remove(id);
        }

        	/// <summary>
		/// 購入処理
		/// </summary>
		/// <param name="userId"></param>
		/// <param name="money"></param>
		/// <returns></returns>
        public bool Buy(
            string userId, 
            int money,
            string performerUserId = null
            )
        {
            if (money < 0) { return false; }
            return this.MoneyExecute(userId, -money, performerUserId, true);
        }

        /// <summary>
        /// チャージ処理
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="money"></param>
        /// <returns></returns>
        public bool Charge(
            string userId,
            int money,
            string performerUserId = null
            )
        {
            if (money < 0) { return false; }
            return this.MoneyExecute(userId, money, performerUserId, false);
        }

        /// <summary>
        /// 出金処理
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="money"></param>
        /// <returns></returns>
        public bool Withdraw(
            string userId,
            int money,
            string performerUserId = null
            )
        {
            if (money < 0) { return false; }
            return this.MoneyExecute(userId, -money, performerUserId, false);
        }

        /// <summary>
        /// 金銭処理
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="money"></param>
        /// <returns></returns>
        private bool MoneyExecute(
            string userId,
            int money,
            string performerUserId = null,
            bool isBuy = false
            )
        {
            try
            {
                User performerUser = null;
                var user = this.GetUser(userId);

                // 実行者が未指定な場合、本人の購入とみなす
                // 他者の購入
                if (performerUserId != null)
                {
                    performerUser = this.GetUser(performerUserId);
                }
                else
                {
                    // 本人の購入
                    performerUserId = userId;
                    performerUser = user;
                }

                if (user != null && performerUser != null)
                {
                    // 履歴の追加
                    var history = new FelicaData.MoneyHistory
                    {
                        UserId = userId,
                        PerformerUserId = performerUserId,
                        Money = money,
                        IsBuy = isBuy
                    };

                    checked
                    {
                        user.Money += money;
                    }

                    this.UpdateUser(user);
                    this.Collections.MoneyHistories.CreateMoneyHistory(history);

                    return true;
                }

                return false; // 失敗
            }
            catch (DatabaseException e)
            {
                Debug.WriteLine(e.Message);
            }
            catch (OverflowException e)
            {
                Debug.WriteLine(e.Message);
            }

            return false; // 例外発生
        }

        public void Cancel(MoneyHistory history)
        {
            var sameIdHistory = this.Collections.MoneyHistories.GetMoneyHistory(history.Id);
            var sameUser = this.GetUser(history.UserId);

            if (sameIdHistory == null)
            {
                throw new DatabaseException("無効な履歴です。");
            }

            if (sameIdHistory.IsCancel)
            {
                throw new DatabaseException("既にキャンセルされています。");
            }

            if (sameUser == null)
            {
                throw new DatabaseException("ユーザーが存在しません。");
            }

            if (sameIdHistory.PerformerUserId != sameIdHistory.UserId)
            {
                throw new DatabaseException("管理者による操作は取り消せません。");
            }

            // バリデーション通過
            try
            {
                checked
                {
                    sameUser.Money -= history.Money; // 取り消し (逆の処理)
                }

                sameIdHistory.IsCancel = true;

                // 更新
                this.UpdateUser(sameUser);
                this.Collections.MoneyHistories.UpdateMoneyHistory(sameIdHistory);
            }

            catch (OverflowException e)
            {
                Debug.WriteLine(e);
                throw new DatabaseException("オーバーフローが発生しました。");
            }
        }
    }
}
