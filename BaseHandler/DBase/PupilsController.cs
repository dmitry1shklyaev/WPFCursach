using BaseHandler.DBase.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BaseHandler.DBase
{
    public class PupilsController
    {
        private static List<Pupil> pupils;
        public static List<Pupil> GetPupils()
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
                cmd.CommandText = $"SELECT * FROM [Pupils]";
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    pupils = new List<Pupil>();
                    while (reader.Read())
                    {
                        Pupil pupil = new Pupil();
                        pupil.pupil_id = reader.GetInt32(0);
                        pupil.pupil_name = reader.GetString(1);
                        pupil.pupil_class.class_id = reader.GetInt32(2);
                        pupil.pupil_class.class_grade = SchoolclassesController.GetSchoolclassByID(pupil.pupil_class.class_id).class_grade;
                        pupils.Add(pupil);
                    }
                }
                reader.Close();
                con.Close();
                return pupils;
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.ToString());
            }
            return null;
        }
        public static Pupil GetPupilByID(int id)
        {
            SqlConnection con = DBaseConnector.getBaseConnection();
            Pupil pupil = new Pupil();
            try
            {
                if (con.State != System.Data.ConnectionState.Open)
                {
                    throw new Exception("Unable to connect to the database");
                }
                SqlCommand cmd = con.CreateCommand();
                cmd.CommandText = $"SELECT * FROM [Pupils] WHERE pupil_id = @id";
                cmd.Parameters.AddWithValue("@id", id);
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        pupil.pupil_id = reader.GetInt32(0);
                        pupil.pupil_name = reader.GetString(1);
                    }
                }
                reader.Close();
                return pupil;
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.ToString());
            }
            return null;
        }

        public static bool AddPupil(Pupil pupil)
        {
            try
            {
                if (DBaseConnector.getBaseConnection().State != System.Data.ConnectionState.Open)
                {
                    throw new Exception("Unable to connect to the database");
                }
                string query = $"INSERT INTO [Pupils] (pupil_fullname, pupil_class) VALUES (@name, @class)";
                using (SqlCommand command = new SqlCommand(query, DBaseConnector.getBaseConnection()))
                {
                    command.Parameters.AddWithValue("@name", pupil.pupil_name);
                    command.Parameters.AddWithValue("@class", pupil.pupil_class.class_id);

                    int result = command.ExecuteNonQuery();
                    if (result < 0)
                    {
                        throw new Exception("No data has been added");
                    }
                    return true;
                }
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.ToString());
                return false;
            }
        }

        public static bool EditPupil(Pupil pupil)
        {
            try
            {
                if (DBaseConnector.getBaseConnection().State != System.Data.ConnectionState.Open)
                {
                    throw new Exception("Unable to connect to the database");
                }

                string queryToEdit = "UPDATE [Pupils] " +
                        "SET pupil_fullname = @name, " +
                        "pupil_class = @class " +
                        "WHERE pupil_id = @p_id";

                using (SqlCommand command = new SqlCommand(queryToEdit, DBaseConnector.getBaseConnection()))
                {
                    command.Parameters.AddWithValue("@name", pupil.pupil_name);
                    command.Parameters.AddWithValue("@class", pupil.pupil_class.class_id);
                    command.Parameters.AddWithValue("@p_id", pupil.pupil_id);

                    int result = command.ExecuteNonQuery();

                    if (result < 0)
                    {
                        throw new Exception("Ничего не было изменено.");
                    }

                    return true;
                }
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.ToString());
                return false;
            }
        }
        public static bool DropPupil(Pupil pupil)
        {
            try
            {
                if (DBaseConnector.getBaseConnection().State != System.Data.ConnectionState.Open)
                {
                    throw new Exception("Unable to connect to the database");
                }
                foreach (var mark in MarksController.GetMarks())
                {
                    if (mark.pupil.pupil_id == pupil.pupil_id)
                    {
                        DialogResult dialogResult = System.Windows.Forms.MessageBox.Show($"У ученика \"{pupil.pupil_name}\" проставлена " +
                            $"четвертная оценка по предмету " +
                            $"\"{mark.subject.name}\".\n" +
                            $"Вы действительно хотите удалить оценку по этому предмету?", "Удалить оценку?", MessageBoxButtons.YesNo);
                        if (dialogResult == DialogResult.Yes)
                        {
                            MarksController.DropMark(mark);
                        }
                        else return false;
                    }
                }
                string query = $"DELETE FROM [Pupils] WHERE pupil_id = @id";
                using (SqlCommand command = new SqlCommand(query, DBaseConnector.getBaseConnection()))
                {
                    command.Parameters.AddWithValue("@id", pupil.pupil_id);

                    int result = command.ExecuteNonQuery();
                    if (result < 0)
                    {
                        throw new Exception("No data has been removed");
                    }
                    return true;
                }
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.ToString());
                return false;
            }
        }
    }
}
