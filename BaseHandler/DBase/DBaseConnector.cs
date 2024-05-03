using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

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
                    string connString = "Data Source=(local);Initial Catalog=Teacher_Manager;Integrated Security=True;MultipleActiveResultSets=True;";
                    SqlConnection conn = new SqlConnection(connString);
                    conn.Open();
                    _conn = conn;
                }
                catch (Exception exc)
                {
                    MessageBox.Show(exc.ToString());
                    return null;
                }
            }
            return _conn;
        }
    }
}
