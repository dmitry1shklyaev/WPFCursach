﻿using BaseHandler.DBase.Models;
using Microsoft.IdentityModel.Tokens;
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
                cmd.CommandText = $"SELECT * FROM [Subjects]";
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
                MessageBox.Show(exc.ToString());
            }
            return null;
        }
        public static Subject GetSubjectByID(int id)
        {
            SqlConnection con = DBaseConnector.getBaseConnection();
            Subject subject = new Subject();
            try
            {
                if (con.State != System.Data.ConnectionState.Open)
                {
                    throw new Exception("Unable to connect to the database");
                }
                SqlCommand cmd = con.CreateCommand();
                cmd.CommandText = $"SELECT * FROM [Subjects] WHERE subject_id = @id";
                cmd.Parameters.AddWithValue("@id", id);
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        subject.id = reader.GetInt32(0);
                        subject.name = reader.GetString(1);
                    }
                }
                reader.Close();
                return subject;
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.ToString());
            }
            return null;
        }

        public static bool AddSubject(Subject subject)
        {
            try
            {
                if (DBaseConnector.getBaseConnection().State != System.Data.ConnectionState.Open)
                {
                    throw new Exception("Unable to connect to the database");
                }
                string query = "INSERT INTO [Subjects] (subject_name) VALUES (@name)";
                using(SqlCommand command = new SqlCommand(query, DBaseConnector.getBaseConnection()))
                {
                    string tmp = subject.name;
                    command.Parameters.AddWithValue("@name", subject.name);

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
        public static bool DropSubject(Subject subject)
        {
            try
            {
                if (DBaseConnector.getBaseConnection().State != System.Data.ConnectionState.Open)
                {
                    throw new Exception("Unable to connect to the database");
                }

                string query = $"DELETE FROM [Subjects] WHERE subject_id = @id";

                var teachersWithSubject = TeacherController.GetTeachers()
                    .Where(teacher => teacher.teacher_spec == subject.id)
                    .ToList();

                if (teachersWithSubject.Any())
                {
                    string message = string.Join(Environment.NewLine, teachersWithSubject.Select(teacher => teacher.teacher_fullname));
                    DialogResult dialogResult = MessageBox.Show($"На данный момент предмет преподаётся следующими учителями:\n\n{message}\n" +
                        $"\nВы действительно хотите удалить предмет?", "Подтвердить удаление", MessageBoxButtons.YesNo);
                    if (dialogResult != DialogResult.Yes)
                    {
                        return false;
                    }

                    teachersWithSubject.ForEach(TeacherController.DropTeacher);
                }

                var marksWithSubject = MarksController.GetMarks()
                    .Where(mark => mark.subject.name == subject.name)
                    .ToList();

                if (marksWithSubject.Any())
                {
                    DialogResult dialogResult = MessageBox.Show($"По данному предмету проставлены четвертные оценки. " +
                        $"Вы действительно хотите удалить эти оценки?", "Удалить оценки?", MessageBoxButtons.YesNo);
                    if (dialogResult != DialogResult.Yes)
                    {
                        return false;
                    }

                    marksWithSubject.ForEach(m => MarksController.DropMarkBySubjectID(m.subject.id));
                }

                using (SqlCommand command = new SqlCommand(query, DBaseConnector.getBaseConnection()))
                {
                    command.Parameters.AddWithValue("@id", subject.id);

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
