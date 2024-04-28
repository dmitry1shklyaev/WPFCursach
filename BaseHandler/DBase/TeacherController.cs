using BaseHandler.DBase.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BaseHandler.DBase
{
    public class TeacherController
    {
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
                cmd.CommandText = "SELECT * FROM [Teachers]";
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.HasRows) 
                {
                    while(reader.Read()) 
                    {
                        Teacher teacher = new Teacher();
                        teacher.teacher_id = reader.GetInt32(0);
                        teacher.teacher_fullname = reader.GetString(1);
                        teacher.teacher_spec = reader.GetInt32(2);
                        teacher.teacher_subject_name = SubjectsController.GetSubjectByID(teacher.teacher_spec).name;
                        teacher.teacher_auditory= reader.GetInt32(3);
                        teachers.Add(teacher);
                    }
                }
                reader.Close();
                return teachers;
            }
            catch(Exception exc) 
            {
                MessageBox.Show(exc.ToString());
            }
            return null;
        }
        public static void AddTeacher(Teacher teacher)
        {
            try
            {
                if (DBaseConnector.getBaseConnection().State != System.Data.ConnectionState.Open)
                {
                    throw new Exception("Unable to connect to the database");
                }
                string query = $"INSERT INTO [Teachers] (teacher_fullname, teacher_specialization, teacher_auditory) VALUES (@name, @spec, @aud)";
                using (SqlCommand command = new SqlCommand(query, DBaseConnector.getBaseConnection()))
                {
                    command.Parameters.AddWithValue("@name", teacher.teacher_fullname);
                    command.Parameters.AddWithValue("@spec", teacher.teacher_spec);
                    command.Parameters.AddWithValue("@aud", teacher.teacher_auditory);

                    int result = command.ExecuteNonQuery();
                    if (result < 0)
                    {
                        throw new Exception("No data has been added");
                    }
                }
                System.Windows.Forms.MessageBox.Show("Учитель успешно добавлен!");
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.ToString());
            }
        }

        public static void DropTeacher(Teacher teacher)
        {
            try
            {
                if (DBaseConnector.getBaseConnection().State != System.Data.ConnectionState.Open)
                {
                    throw new Exception("Unable to connect to the database");
                }
                string query = $"DELETE FROM [Teachers] WHERE teacher_id = @id";
                using (SqlCommand command = new SqlCommand(query, DBaseConnector.getBaseConnection()))
                {
                    command.Parameters.AddWithValue("@id", teacher.teacher_id);

                    int result = command.ExecuteNonQuery();
                    if (result < 0)
                    {
                        throw new Exception("No data has been removed");
                    }
                }
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.ToString());
            }
        }
        public static void EditTeacher(Teacher teacher)
        {
            try
            {
                if (DBaseConnector.getBaseConnection().State != System.Data.ConnectionState.Open)
                {
                    throw new Exception("Unable to connect to the database");
                }
                string queryToEdit = "UPDATE [Teachers] " +
                        "SET teacher_fullname = @name, " +
                        "teacher_specialization = @spec, " +
                        "teacher_auditory = @aud " +
                        "WHERE teacher_id = @t_id";
                int editedTeacherID = -1;
                using (SqlCommand command = new SqlCommand(queryToEdit, DBaseConnector.getBaseConnection()))
                {
                    var teachers = GetTeachers();
                    for(int i = 0; i < teachers.Count(); i++)
                    {
                        if (teachers[i].teacher_id == teacher.teacher_id)
                        {
                            editedTeacherID = i;
                            break;
                        }
                        else continue;
                    }
                    if (teachers != null)
                    {
                        if (editedTeacherID != -1)
                        {
                            teachers[editedTeacherID] = teacher;
                        }
                        else MessageBox.Show("Учителя с данным ID не существует."); // убрать потом
                    }
                    command.Parameters.AddWithValue("@t_id", teacher.teacher_id);
                    command.Parameters.AddWithValue("@name", teacher.teacher_fullname);
                    command.Parameters.AddWithValue("@spec", teacher.teacher_spec);
                    command.Parameters.AddWithValue("@aud", teacher.teacher_auditory);

                    int result = command.ExecuteNonQuery();
                    if (result < 0)
                    {
                        throw new Exception("Ничего не было изменено.");
                    }
                }
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.ToString());
            }
        }
    }
}
