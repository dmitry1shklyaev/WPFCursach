using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseHandler.DBase
{
    public class DBaseConnector
    {
        private static SqlConnection _conn = null;


        public static SqlConnection getBaseConnection()
        {
            if (_conn == null)
            {
                try
                {
                    string connString = "Data Source=ANDREW-PC\\SQLEXPRESS01;Initial Catalog=Teacher_Manager;Integrated Security=True";
                    SqlConnection conn = new SqlConnection(connString);
                    conn.Open();
                    _conn = conn;
                }
                catch (Exception exc)
                {
                    Console.WriteLine(exc.Message);
                    return null;
                }
            }
            return _conn;
        }

        public static void closeConnection() 
        {
            try
            {
                _conn.Close();
            }
            catch 
            {
                _conn = null;
            }
        }
    }
}
