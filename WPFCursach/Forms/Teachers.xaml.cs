using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WPFCursach.Classes;
using System.Configuration;
using BaseHandler.DBase;
using BaseHandler.DBase.Models;
using System.Drawing;

namespace WPFCursach.Forms
{
    /// <summary>
    /// Логика взаимодействия для Teachers.xaml
    /// </summary>
    public partial class Teachers : Page
    {
        SqlConnection connection = null;
        public Teachers()
        {
            InitializeComponent();
            bindDataGrid.BindDataGrid("teachers", g1);
            LoadComboBox();
        }
        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (g1.SelectedItem != null)
            {
                DataRowView row = (DataRowView)g1.SelectedItem;
                selectedTeacherTextBox.Text = row["teacher_fullname"].ToString();
                selectedSubjectComboBox.Text = row["teacher_specialization"].ToString();
                selectedAuditoryTextBox.Text = row["teacher_auditory"].ToString();
            }
        }
        private void textBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!char.IsDigit(e.Text, 0))
            {
                e.Handled = true;
            }
        }

        private void LoadComboBox()
        {
            var subjects = SubjectsController.GetSubject();
            foreach (Subject subject in subjects)
            {
                selectedSubjectComboBox.Items.Add(subject.name);
            }
        }
        private int GetSubjectIDbyName(string subjectName, SqlConnection connection)
        {
            int subjectID = -1;

            SqlCommand sql = new SqlCommand($"SELECT subject_id FROM Subjects WHERE subject_name = @subjectName", connection);
            sql.Parameters.AddWithValue("@subjectName", subjectName);

            using (SqlDataReader reader = sql.ExecuteReader())
            {
                if (reader.Read())
                {
                    subjectID = reader.GetInt32(0);
                }
            }
            return subjectID;
        }

        private void addInfoButton_Click(object sender, RoutedEventArgs e)
        {
            connection = new SqlConnection();
            connection.ConnectionString = ConfigurationManager.ConnectionStrings["connteacher"].ConnectionString;
            connection.Open();
            SqlCommand cmd = new SqlCommand($"INSERT INTO [Teachers] (teacher_fullname, teacher_specialization, teacher_auditory) VALUES (@teacher_fullname, @teacher_specialization, @teacher_auditory)", connection);
            cmd.Parameters.AddWithValue("@teacher_fullname", selectedTeacherTextBox.Text);
            cmd.Parameters.AddWithValue("@teacher_specialization", selectedSubjectComboBox.Text);
            cmd.Parameters.AddWithValue("@teacher_auditory", selectedAuditoryTextBox.Text);

            string subjectName = cmd.Parameters[1].Value.ToString();
            cmd.Parameters[1].Value = GetSubjectIDbyName(subjectName, connection);

            cmd.ExecuteNonQuery();
            bindDataGrid.BindDataGrid("teachers",g1);
            connection.Close();
        }
    }

}
