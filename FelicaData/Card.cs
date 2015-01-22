using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace FelicaData
{
    public class Card : Base
    {
        [BsonIgnore]
        public const int SHORT_UID_LENGTH = 32;

        [BsonIgnore]
        public string PlainUid
        {
            set
            {
                this.Uid = Card.HashUid(value);
            }
        }

        [BsonIgnore]
        public string ShortUid
        {
            get
            {
                if(string.IsNullOrEmpty(this.Uid)){
                    return null;
                }

                return this.Uid.Substring(0, SHORT_UID_LENGTH);
            }
        }

        public string Uid { get; set; }
        public string UserId { get; set; }
        public string Name { get; set; }

        /// <summary>
        /// UID をハッシュ化する
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        public static string HashUid(string password)
        {
            using (var sha512 = SHA512.Create())
            {
                var passBytes = Encoding.UTF8.GetBytes(password);
                var hashBytes = sha512.ComputeHash(passBytes);
                var hashPassword = BitConverter.ToString(hashBytes).ToLower().Replace("-", "");

                return hashPassword;
            }
        }
    }
}
