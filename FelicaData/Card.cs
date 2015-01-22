using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MongoDB.Bson;

namespace FelicaData
{
    public class Card : Base
    {
        public string Uid { get; set; }
        public string UserId { get; set; }
        public string Name { get; set; }
    }
}
