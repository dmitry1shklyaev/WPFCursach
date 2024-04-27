using BaseHandler.DBase.Models;
using System.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Windows.Forms;
using Microsoft.IdentityModel.Abstractions;
using Microsoft.IdentityModel.Tokens;

namespace BaseHandler.DBase
{
    public class SchoolclassesController
    {
        private static List<Schoolclass> classes;
        public static List<Schoolclass> GetClasses()
        {
            SqlConnection con = new SqlConnection();
            con.ConnectionString = ConfigurationManager.ConnectionStrings["connteacher"].ConnectionString;
            con.Open();

            try
            {
                if(con.State != System.Data.ConnectionState.Open)
                {
                    throw new Exception("Unable to connect to the database");
                }
                SqlCommand cmd = con.CreateCommand();
                cmd.CommandText = $"SELECT * FROM [Schoolclasses]";
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    classes = new List<Schoolclass>();
                    while (reader.Read())
                    {
                        Schoolclass sc = new Schoolclass();
                        sc.class_id = reader.GetInt32(0);
                        sc.class_grade = reader.GetString(1);
                        classes.Add(sc);
                    }
                }
                con.Close();
                return classes;
            }
            catch (Exception ex) 
            {
                MessageBox.Show(ex.ToString());
            }
            return null;
        }
        public static Schoolclass GetSchoolclassByID(int id)
        {
            SqlConnection con = new SqlConnection();
            con.ConnectionString = ConfigurationManager.ConnectionStrings["connteacher"].ConnectionString;
            con.Open();
            Schoolclass sc = new Schoolclass();
            try
            {
                if (con.State != System.Data.ConnectionState.Open)
                {
                    throw new Exception("Unable to connect to the database");
                }
                SqlCommand cmd = con.CreateCommand();
                cmd.CommandText = $"SELECT * FROM Schoolclasses WHERE class_id={id}";
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        sc.class_id = reader.GetInt32(0);
                        sc.class_grade = reader.GetString(1);
                    }
                }
                reader.Close();
                return sc;
            }
            catch (Exception exc) 
            {
                MessageBox.Show(exc.ToString());
            }
            return null;
        }
        public static void AddClass(Schoolclass sc)
        {
            try
            {
                if(DBaseConnector.getBaseConnection().State != System.Data.ConnectionState.Open)
                {
                    throw new Exception("Unable to connect to the database");
                }
                string query = $"INSERT INTO [Schoolclasses] (class_grade) VALUES (@c_grade)";
                using (SqlCommand command = new SqlCommand(query, DBaseConnector.getBaseConnection()))
                {
                    command.Parameters.AddWithValue("@c_grade", sc.class_grade);

                    int result = command.ExecuteNonQuery();
                    if (result < 0)
                    {
                        throw new Exception("No data has been added");
                    }
                }
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.ToString());
            }
        }
        public static List<Schoolclass> SortClassesInAscendingOrder(List<Schoolclass> classes)
        {
            return classes.OrderBy(c =>
            {
                var parts = c.class_grade.Split(' ');
                int number = int.Parse(parts[0]);
                string letter = parts[1];

                // Обработка особых случаев для "10" и "11"
                if (number == 10)
                    number = 100;
                else if (number == 11)
                    number = 110;

                return number * 100 + (int)letter[0];
            }).ToList();
        }
        public static void DropSchoolclass(Schoolclass sc)
        {
            try
            {
                if (DBaseConnector.getBaseConnection().State != System.Data.ConnectionState.Open)
                {
                    throw new Exception("Unable to connect to the database");
                }

                string query = $"DELETE FROM [Schoolclasses] WHERE class_id = @id";

                List<Pupil> pupils = null;
                List<string> pupilsNames = null;

                foreach(var pupil in PupilsController.GetPupils())
                {
                    if(pupil.pupil_class.class_id == sc.class_id)
                    {
                        if(pupils.IsNullOrEmpty() && pupilsNames.IsNullOrEmpty())
                        {
                            pupils = new List<Pupil>();
                            pupilsNames = new List<string>();
                        }
                        pupils.Add(pupil);
                        pupilsNames.Add(pupil.pupil_name);
                    }
                }

                if(pupils != null && pupils.Any() && pupilsNames != null && pupilsNames.Any())
                {
                    string message = string.Join(Environment.NewLine, pupilsNames);
                    DialogResult dialogResult = MessageBox.Show($"На данный момент в {sc.class_grade} классе имеются данные ученики:\n\n{message}\n" +
                        $"\nВы действительно хотите удалить класс?", "Подтвердить удаление", MessageBoxButtons.YesNo);

                    if (dialogResult == DialogResult.Yes)
                    {
                        var pupilsToRemove = pupils.Where(p => p.pupil_class.class_id == sc.class_id);
                        foreach (var pupil in pupilsToRemove)
                        {
                            var marksToRemove = MarksController.GetMarks().Where(m => m.pupil.pupil_id == pupil.pupil_id);
                            foreach (var mark in marksToRemove)
                            {
                                MarksController.DropMark(mark);
                            }
                            PupilsController.DropPupil(pupil);
                        }
                    }
                    else return;
                }


                using (SqlCommand command = new SqlCommand(query, DBaseConnector.getBaseConnection()))
                {
                    command.Parameters.AddWithValue("@id", sc.class_id);

                    int result = command.ExecuteNonQuery();
                    if (result < 0)
                    {
                        throw new Exception("No data has been removed");
                    }
                    MessageBox.Show($"{sc.class_grade} класс успешно удалён!");
                }
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.ToString());
            }
        }
    }
}
