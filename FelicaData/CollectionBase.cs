using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MongoDB.Driver;
using MongoDB.Bson;
using System.Diagnostics;
using MongoDB.Driver.Builders;

namespace FelicaData
{
    abstract public class CollectionBase<T>
        where T: Base
    {
        private readonly DatabaseManager dbManager;
        private readonly CollectionManager colManager;

        private readonly string collectionName;
        private readonly MongoCollection<T> collection;

        public string CollectionName
        {
            get { return this.collectionName; }
        }

        private MongoCollection<T> Collection
        {
            get { return this.collection; }
        }

        protected CollectionManager Collections
        {
            get { return this.colManager; }
        }

        /// <summary>
        /// データベースを初期化します。
        /// </summary>
        protected CollectionBase(
            DatabaseManager dbMgr,
            CollectionManager colMgr,
            string collectionName
            )
        {
            this.dbManager = dbMgr;
            this.colManager = colMgr;

            this.collectionName = collectionName;
            this.collection = dbMgr.GetCollection<T>(collectionName);
        }

        protected TReturn Transaction<TReturn>(Func<TReturn> exec)
        {
            try
            {
                return exec();
            }

            catch (MongoConnectionException e)
            {
                Debug.WriteLine(e);
                throw new DatabaseException("データベースとの接続でエラーが発生しました。");
            }
            catch (System.IO.IOException e)
            {
                Debug.WriteLine(e);
                throw new DatabaseException("データベースとの接続でエラーが発生しました。");
            }
            catch (System.Net.Sockets.SocketException e)
            {
                Debug.WriteLine(e);
                throw new DatabaseException("データベースとの接続でエラーが発生しました。");
            }
        }

        protected void Transaction(Action exec)
        {
            this.Transaction<object>(() =>
            {
                exec();
                return null;
            });
        }

        internal List<T> Find()
        {
            return this.Transaction(() =>
            {
                return this.Collection.FindAll().ToList();
            });
            
        }

        internal List<T> Find(IMongoQuery query)
        {
            return this.Transaction(() =>
            {
                return this.Collection.Find(query).ToList();
            });
        }

        internal T FindOne(string id)
        {
            return this.Transaction(() =>
            {
                return this.Collection.FindOneById(ObjectId.Parse(id));
            });
        }

        internal T FindOne(IMongoQuery query)
        {
            return this.Transaction(() =>
            {
                return this.Collection.FindOne(query);
            });
        }

        internal T Insert(T entity)
        {
             this.Transaction(() =>
             {
                 this.Collection.Insert(entity);
             });

            if (entity.Id != null)
            {
                this.OnChanged();
                return entity;
            }
            
            return null;
        }

        internal T Update(T entity)
        {
            this.Transaction(() =>
            {
                this.Collection.Save(entity);
            });

            this.OnChanged();
            return entity;
        }

        internal void Remove(string id)
        {
            this.Transaction(() =>
            {
                this.Collection.Remove(Query<T>.EQ(e => e._Id, ObjectId.Parse(id)));
            });

            this.OnChanged();
        }

        #region Changed Event

        public event EventHandler Changed;

        protected void OnChanged()
        {
            if (this.Changed != null)
            {
                this.Changed(this, null);
            }
        }

        #endregion

    }
}
