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
    /// <summary>
    /// Felica Cashing System で扱うユーザーの情報を表します。
    /// </summary>
    public class User : Base
    {
        [BsonIgnore]
        public int PASSWORD_LENGTH = 64;

        public string Name { get; set; }
        public string Email { get; set; }
        public int Money { get; set; }
        public bool IsAdmin { get; set; }
        public byte[] Avatar { get; set; }

        /// <summary>
        /// 暗号化されているパスワードです。認証には、<see cref="User.Auth"/>メソッドを使います。
        /// </summary>
        public string Password { set; get; }

        /// <summary>
        /// 寮の部屋番号
        /// </summary>
        public string DormitoryRoomNumber { get; set; }

        /// <summary>
        /// 電話番号
        /// </summary>
        public string PhoneNumber { get; set; }

        /// <summary>
        /// 暗号化されていないパスワードです。設定のみ可能です。
        /// </summary>
        [BsonIgnore]
        public string PlainPassword
        {
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    this.Password = string.Empty;
                }
                else
                {
                    this.Password = User.HashPassword(value);
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
            return !string.IsNullOrWhiteSpace(this.Password) &&
                this.Password == User.HashPassword(plainPassword);
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
        /// パスワードをハッシュ化します。
        /// </summary>
        /// <param name="plainPassword">ハッシュ化されていないパスワード</param>
        /// <returns></returns>
        public static string HashPassword(string plainPassword)
        {
            using (var sha256 = SHA256.Create())
            {
                var passBytes = Encoding.UTF8.GetBytes(plainPassword);
                var hashBytes = sha256.ComputeHash(passBytes);
                var hashPassword = BitConverter.ToString(hashBytes).ToLower().Replace("-", "");

                return hashPassword;
            }
        }
    }
}
