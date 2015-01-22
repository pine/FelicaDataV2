using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MongoDB.Bson;

namespace FelicaData
{
    public class MoneyHistory : Base
    {
        public string UserId { get; set; }
        public string PerformerUserId { get; set; }
        public bool IsCancel { get; set; }
        public bool IsBuy { get; set; }
        public int Money { get; set; }
        public DateTime CreatedAt { get; set; }

        public MoneyHistory Clone()
        {
            return (MoneyHistory)this.MemberwiseClone();
        }
    }
}
