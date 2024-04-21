using BaseHandler.DBase;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace WPFCursach.Classes
{
    internal class bindDataGrid
    {
        public static void BindDataGrid(string tableName, DataGrid dataGridName)
        {
            SqlConnection con = new SqlConnection();
            con.ConnectionString = ConfigurationManager.ConnectionStrings["connteacher"].ConnectionString;
            con.Open();
            SqlCommand cmd = new SqlCommand();
            string query = "";

            if (tableName == "teachers")
            {
                query = "SELECT t.teacher_id, t.teacher_fullname, s.subject_name AS 'teacher_specialization', t.teacher_auditory FROM teachers t LEFT JOIN subjects s ON t.teacher_specialization = s.subject_id;";
            }
            else if (tableName == "subjects")
            {
                query = "SELECT subject_id, subject_name FROM subjects;";
            }
            else if (tableName == "students")
            {
                query = "SELECT pupil_id, pupil_fullname, pupil_class FROM students;";
            }
            // типа цифры только дает написать в поле "Кабинет"
            // букавы нельзя)))

            cmd.CommandText = query;
            cmd.Connection = con;
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable(tableName);
            da.Fill(dt);
            dataGridName.ItemsSource = dt.DefaultView;
            con.Close();
        }
    }
}
