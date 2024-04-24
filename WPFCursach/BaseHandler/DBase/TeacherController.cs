using BaseHandler.DBase.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace BaseHandler.DBase
{
    public class TeacherController
    {
        //public List<Models.Teacher> Teachers;\

        public static List<Teacher> GetTeachers() 
        {
            List<Teacher> teachers = new List<Teacher>();
            SqlConnection con = DBaseConnector.getBaseConnection();
            try 
            {
                if (con.State != System.Data.ConnectionState.Open) 
                {
                    throw new Exception("Unable to connect to the database");
                }
                SqlCommand cmd = con.CreateCommand();
                cmd.CommandText = "SELECT * FROM Teachers";
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.HasRows) 
                {
                    while(reader.Read()) 
                    {
                        Teacher teacher = new Teacher();
                        teacher.teacher_id = reader.GetInt32(0);
                        teacher.teacher_fullname = reader.GetString(1);
                        teacher.teacher_spec = reader.GetInt32(2);
                        teacher.teacher_auditory= reader.GetInt32(3);
                        teachers.Add(teacher);
                    }
                }

                return teachers;
            }
            catch(Exception exc) 
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
