using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MongoDB.Driver;

namespace FelicaData
{
    public class DatabaseManager
    {
        private MongoClient client;
        private MongoServer server;
        private MongoDatabase database;

        /// <summary>
        /// データベースを初期化します。
        /// </summary>
        /// <param name="connectionString"></param>
        /// <exception cref="FelicaData.DatabaseException">データベースの初期化に失敗した場合</exception>
        public DatabaseManager(
            string databaseName,
            string connectionString = null
            )
        {
            if (connectionString == null)
            {
                this.client = new MongoClient();
            }
            else
            {
                this.client = new MongoClient(connectionString);
            }

            this.server = this.client.GetServer();
            this.database = this.server.GetDatabase(databaseName);
        }

        internal MongoCollection<T> GetCollection<T>(string collectionName){
            return this.database.GetCollection<T>(collectionName);
        }
    }
}
