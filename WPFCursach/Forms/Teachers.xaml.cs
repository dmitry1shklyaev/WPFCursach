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
using System.Collections;
using System.Windows.Forms;
using System.Windows.Markup.Localizer;

namespace WPFCursach.Forms
{
    /// <summary>
    /// Логика взаимодействия для Teachers.xaml
    /// </summary>
    public partial class Teachers : Page
    {

        public Teachers()
        {
            InitializeComponent();
            g1.ItemsSource = BaseHandler.DBase.TeacherController.GetTeachers();
            //bindDataGrid.BindDataGrid("teachers", g1);
            LoadComboBox();
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
            var subjectsParse = SubjectsController.GetSubject();
            foreach (Subject subject in subjectsParse)
            {
                selectedSubjectComboBox.Items.Add(subject.name);
            }
        }

        private void addInfoButton_Click(object sender, RoutedEventArgs e)
        {
            if (selectedIdTextBox.Text != "" && (selectedSubjectComboBox.Text == "" || selectedTeacherTextBox.Text == "" || selectedAuditoryTextBox.Text == ""))
            {
                System.Windows.Forms.MessageBox.Show("Одно и/или несколько полей пусты!\nЗаполните все поля!");
                return;
            }
            try
            {
                Teacher teacher = new Teacher();
                teacher.teacher_fullname = selectedTeacherTextBox.Text;
                teacher.teacher_auditory = Convert.ToInt32(selectedAuditoryTextBox.Text);
                foreach (Subject sbj in SubjectsController.GetSubject())
                {
                    if (sbj.name == selectedSubjectComboBox.Text) 
                    {
                        teacher.teacher_spec = sbj.id;
                    }
                }
                TeacherController.AddTeacher(teacher);
                Classes.FrameSingleton.getFrame().Navigate(new Teachers());
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message);
            }
        }
        private void backToMenuButton_Click(object sender, RoutedEventArgs e)
        {
            Classes.FrameSingleton.getFrame().Navigate(new MainMenu());
        }

        private void g1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (g1.SelectedItem != null)
            {
                Teacher row = (Teacher)g1.SelectedItem;
                if (row.teacher_subject == null)
                {
                    System.Windows.Forms.MessageBox.Show("no sbj");
                }
                selectedTeacherTextBox.Text = row.teacher_fullname;
                selectedSubjectComboBox.SelectedItem = row.GetSubject().name;
                selectedAuditoryTextBox.Text = row.teacher_auditory.ToString();
                selectedIdTextBox.Text = row.teacher_id.ToString();
            }
        }
        private void zeroizeTextBoxes()
        {
            selectedAuditoryTextBox.Text = null;
            selectedIdTextBox.Text = null;
            selectedSubjectComboBox.Text = null;
            selectedTeacherTextBox.Text = null;
        }
        private void unselectCellsButton_Click(object sender, RoutedEventArgs e)
        {
            g1.UnselectAll();
            zeroizeTextBoxes();
        }

        private void updateInfoButton_Click(object sender, RoutedEventArgs e)
        {
            if (selectedIdTextBox.Text == "")
            {
                System.Windows.Forms.MessageBox.Show("Вы вписали данные учителя, но не выбрали его в таблице!");
                return;
            }
            else if(selectedIdTextBox.Text == "" || selectedSubjectComboBox.Text == "" || selectedTeacherTextBox.Text == "" || selectedAuditoryTextBox.Text == "")
            {
                System.Windows.Forms.MessageBox.Show("Одно и/или несколько полей пусты!\nЗаполните все поля!");
                return;
            }
            var editedTeacher = new Teacher();
            editedTeacher.teacher_id = int.Parse(selectedIdTextBox.Text);
            List<Subject> subjTemp = new List<Subject>();
            subjTemp = SubjectsController.GetSubject();
            int indexOfSubj = -1;
            for (int i = 0; i < subjTemp.Count(); i++)
            {
                if (subjTemp[i].name == selectedSubjectComboBox.Text)
                {
                    indexOfSubj = i;
                    break;
                }
            }
            editedTeacher.teacher_spec = indexOfSubj + 1;
            editedTeacher.teacher_fullname = selectedTeacherTextBox.Text;
            editedTeacher.teacher_auditory = int.Parse(selectedAuditoryTextBox.Text);
            TeacherController.EditTeacher(editedTeacher);
            zeroizeTextBoxes();
            System.Windows.Forms.MessageBox.Show("Изменение прошло успешно!");
            Classes.FrameSingleton.getFrame().Navigate(new Teachers());
        }

        private void deleteInfoButton_Click(object sender, RoutedEventArgs e)
        {
            if(selectedIdTextBox.Text == "" || selectedSubjectComboBox.Text == "" || selectedTeacherTextBox.Text == "" || selectedAuditoryTextBox.Text == "")
            {
                System.Windows.Forms.MessageBox.Show("Выберите учителя для удаления!");
                return;
            }
            Teacher teacher = new Teacher();
            teacher.teacher_id = int.Parse(selectedIdTextBox.Text);
            teacher.teacher_fullname = selectedTeacherTextBox.Text;
            teacher.teacher_auditory = int.Parse(selectedAuditoryTextBox.Text);
            var subjects = SubjectsController.GetSubject();
            int subjectID = -1;
            for(int i = 0; i < subjects.Count(); i++)
            {
                var subject = subjects[i];
                if (subject.name == selectedSubjectComboBox.Text)
                {
                    subjectID = i;
                    break;
                }
                else continue;
            }
            teacher.teacher_spec = subjectID;
            TeacherController.DropTeacher(teacher);
            System.Windows.Forms.MessageBox.Show("Удаление прошло успешно!");
            Classes.FrameSingleton.getFrame().Navigate(new Teachers());
        }
    }
}
