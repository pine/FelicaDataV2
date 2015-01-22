using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Security.Cryptography;
using System.Diagnostics;

namespace FelicaData
{
    public class User : Base
    {
        [BsonIgnore]
        public int HASHED_PASSWORD_LENGTH = 64;

        public string Name { get; set; }
        public string Email { get; set; }
        public int Money { get; set; }
        public bool IsAdmin { get; set; }
        public byte[] Avatar { get; set; }

        [BsonIgnore]
        private string password;

        public string Password {
            get { return this.password; }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    this.password = null;
                }

                else if (value.Length == 64)
                {
                    this.password = value;
                }

                else
                {
                    this.password = this.HashPassword(value);
                }
            }
        }

        /// <summary>
        /// パスワードで認証を行います。
        /// </summary>
        /// <param name="plainPassword">パスワード</param>
        /// <returns>認証が通った場合、<value>true</value>を返します。</returns>
        public bool Auth(string plainPassword)
        {
            var hashed = this.HashPassword(plainPassword);
            return !string.IsNullOrWhiteSpace(plainPassword) &&
                this.Password == this.HashPassword(plainPassword);
        }

        /// <summary>
        /// 同じ情報を持つユーザーの別インスタンスを返します。
        /// </summary>
        /// <returns></returns>
        public User Clone()
        {
            var user = (User)this.MemberwiseClone();

            return user;
        }

        /// <summary>
        /// パスワードをハッシュ化する
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        public string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var passBytes = Encoding.UTF8.GetBytes(password);
                var hashBytes = sha256.ComputeHash(passBytes);
                var hashPassword = BitConverter.ToString(hashBytes).ToLower().Replace("-", "");

                return hashPassword;
            }
        }
    }
}
