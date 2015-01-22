using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FelicaDataTest
{
    class Program
    {
        static void Main(string[] args)
        {
            var mgr = new FelicaData.DatabaseManager();
            var collections = new FelicaData.CollectionManager(mgr);
            var users = collections.Users;

            users.CreateUser(new FelicaData.User
            {
                Name = "pine"
            });
        }
    }
}
