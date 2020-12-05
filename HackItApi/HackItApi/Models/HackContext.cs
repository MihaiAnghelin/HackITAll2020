using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HackItApi.Models
{
    public class HackContext
    {
        public string ConnectionString { get; set; }

        public HackContext(string connectionString)
        {
            ConnectionString = connectionString;
        }

        private MySqlConnection GetConnection()
        {
            return new MySqlConnection(ConnectionString);
        }
    }
}
