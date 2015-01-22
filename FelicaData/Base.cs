using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace FelicaData
{
    abstract public class Base
    {
        [BsonIgnore]
        public string Id
        {
            get {
                if (this._Id == ObjectId.Empty)
                {
                    return null;
                }

                return this._Id.ToString();
            }
        }

        [BsonId]
        public ObjectId _Id
        {
            get;
            set;
        }
    }
}
