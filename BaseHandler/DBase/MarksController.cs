﻿using BaseHandler.DBase.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Rebar;

namespace BaseHandler.DBase
{
    public class MarksController
    {
        public static bool AddMark(Mark mark)
        {
            try
            {
                if (DBaseConnector.getBaseConnection().State != System.Data.ConnectionState.Open)
                {
                    throw new Exception("Unable to connect to the database");
                }
                string query = "INSERT INTO [Marks] (mark_grade, mark_subj, mark_pupil) VALUES (@grade, @subj, @pupil)";
                using (SqlCommand command = new SqlCommand(query, DBaseConnector.getBaseConnection()))
                {
                    command.Parameters.AddWithValue("@grade", mark.grade);
                    command.Parameters.AddWithValue("@subj", mark.subject.id);
                    command.Parameters.AddWithValue("@pupil", mark.pupil.pupil_id);

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
        public static int CheckDuplicatesMarks(Mark mark)
        {
            SqlConnection con = DBaseConnector.getBaseConnection();
            try
            {
                if (con.State != System.Data.ConnectionState.Open)
                {
                    throw new Exception("Unable to connect to the database");
                }
                SqlCommand cmd = con.CreateCommand();
                cmd.CommandText = $"SELECT CASE WHEN EXISTS " +
                    $"(SELECT 1 FROM Marks WHERE mark_pupil = @mark_pupil AND mark_subj = @mark_subj)" +
                    $" THEN 1 ELSE 0 END AS HasDuplicates";

                cmd.Parameters.AddWithValue("@mark_pupil", mark.pupil.pupil_id);
                cmd.Parameters.AddWithValue("@mark_subj", mark.subject.id);
                
                int count = (int)cmd.ExecuteScalar();

                return count;

            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.ToString());
                return -1;
            }
        }

        private static List<Mark> marks;
        public static List<Mark> GetMarks()
        {
            SqlConnection con = DBaseConnector.getBaseConnection();
            try
            {
                if (con.State != System.Data.ConnectionState.Open)
                {
                    throw new Exception("Unable to connect to the database");
                }
                SqlCommand cmd = con.CreateCommand();
                cmd.CommandText = "SELECT * FROM [Marks]";
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    marks = new List<Mark>();
                    while (reader.Read())
                    {
                        Mark mark = new Mark();
                        mark.id = reader.GetInt32(0);
                        mark.grade = reader.GetInt32(1);
                        mark.subject = SubjectsController.GetSubjectByID(reader.GetInt32(2));
                        mark.pupil = PupilsController.GetPupilByID(reader.GetInt32(3));
                        marks.Add(mark);
                    }
                }
                reader.Close();
                return marks;
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.ToString());
            }
            return null;
        }
        public static bool EditMark(Mark mark)
        {
            try
            {
                if(DBaseConnector.getBaseConnection().State != System.Data.ConnectionState.Open)
                {
                    throw new Exception("Unable to connect to the database");
                }
                string queryToEdit = "UPDATE [Marks] SET mark_grade = @grade, mark_subj = @subj, mark_pupil = @pupil WHERE mark_id = @id";
                using (SqlCommand cmd = new SqlCommand(queryToEdit, DBaseConnector.getBaseConnection()))
                {
                    int id = mark.id;

                    cmd.Parameters.AddWithValue("@grade", mark.grade);
                    cmd.Parameters.AddWithValue("@subj", mark.subject.id);
                    cmd.Parameters.AddWithValue("@pupil", mark.pupil.pupil_id);
                    cmd.Parameters.AddWithValue("@id", mark.id);

                    int result = cmd.ExecuteNonQuery();
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
        public static bool DropMark(Mark mark)
        {
            try
            {
                if (DBaseConnector.getBaseConnection().State != System.Data.ConnectionState.Open)
                {
                    throw new Exception("Unable to connect to the database");
                }
                string query = $"DELETE FROM [Marks] WHERE mark_id = @id";
                using (SqlCommand command = new SqlCommand(query, DBaseConnector.getBaseConnection()))
                {
                    command.Parameters.AddWithValue("@id", mark.id);

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
        public static void DropMarkBySubjectID(int subject_id)
        {
            try
            {
                if (DBaseConnector.getBaseConnection().State != System.Data.ConnectionState.Open)
                {
                    throw new Exception("Unable to connect to the database");
                }
                string query = $"DELETE FROM [Marks] WHERE mark_subj = @id";
                using (SqlCommand command = new SqlCommand(query, DBaseConnector.getBaseConnection()))
                {
                    command.Parameters.AddWithValue("@id", subject_id);

                    int result = command.ExecuteNonQuery();
                    if (result < 0)
                    {
                        throw new Exception("No data has been removed");
                    }
                }
            }
            catch(Exception exc)
            {
                MessageBox.Show(exc.ToString());
            }
        }
    }
}
