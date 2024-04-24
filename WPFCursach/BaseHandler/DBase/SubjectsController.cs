using BaseHandler.DBase.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseHandler.DBase
{
    public class SubjectsController
    {
        private static List<Subject> subjects;
        public static List<Subject> GetSubject()
        {
            SqlConnection con = new SqlConnection();
            con.ConnectionString = ConfigurationManager.ConnectionStrings["connteacher"].ConnectionString;
            con.Open();
            
            try
            {
                if (con.State != System.Data.ConnectionState.Open)
                {
                    throw new Exception("Unable to connect to the database");
                }
                SqlCommand cmd = con.CreateCommand();
                cmd.CommandText = $"SELECT * FROM Subjects";
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    subjects = new List<Subject>();
                    while (reader.Read())
                    {
                        Subject subject = new Subject();
                        subject.id = reader.GetInt32(0);
                        subject.name = reader.GetString(1);
                        subjects.Add(subject);
                    }
                }
                con.Close();
                return subjects;
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.Message);
            }
            return null;
        }

        //todo: edit
        //todo: delete
        //todo: add

    }
}
