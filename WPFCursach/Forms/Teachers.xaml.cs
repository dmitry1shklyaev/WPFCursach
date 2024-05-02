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
            g1.ItemsSource = TeacherController.GetTeachers();
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
            foreach (Subject subject in SubjectsController.GetSubject())
            {
                selectedSubjectComboBox.Items.Add(subject.name);
            }
        }

        private void addInfoButton_Click(object sender, RoutedEventArgs e)
        {
            if ((selectedIdTextBox.Text != "" || selectedSubjectComboBox.Text == "" || selectedTeacherTextBox.Text == "" 
                || selectedAuditoryTextBox.Text == "") || (selectedIdTextBox.Text != "" || selectedSubjectComboBox.Text == "" || 
                selectedTeacherTextBox.Text.Trim() == "" || selectedAuditoryTextBox.Text.Trim() == ""))
            {
                System.Windows.Forms.MessageBox.Show("Одно и/или несколько полей пусты!\nЗаполните все поля!");
                return;
            }
            try
            {
                Teacher teacher = new Teacher
                {
                    teacher_fullname = selectedTeacherTextBox.Text,
                    teacher_auditory = Convert.ToInt32(selectedAuditoryTextBox.Text),
                    teacher_spec = SubjectsController.GetSubject()
                        .FirstOrDefault(s => s.name == selectedSubjectComboBox.Text)?.id ?? -1
                };

                if (TeacherController.AddTeacher(teacher) == true)
                {
                    System.Windows.Forms.MessageBox.Show($"Учитель \"{teacher.teacher_fullname}\" успешно добавлен!");
                }
                else return;

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
            if (selectedIdTextBox.Text.Trim() == "")
            {
                System.Windows.Forms.MessageBox.Show("Вы вписали данные учителя, но не выбрали его в таблице!");
                return;
            }
            else if(selectedIdTextBox.Text.Trim() == "" || selectedSubjectComboBox.Text == "" ||
                selectedTeacherTextBox.Text.Trim() == "" || selectedAuditoryTextBox.Text.Trim() == "")
            {
                System.Windows.Forms.MessageBox.Show("Одно и/или несколько полей пусты!\nЗаполните все поля!");
                return;
            }
            Teacher editedTeacher = new Teacher
            {
                teacher_id = int.Parse(selectedIdTextBox.Text),
                teacher_subject = SubjectsController.GetSubject()
           .FirstOrDefault(s => s.name == selectedSubjectComboBox.Text),
                teacher_fullname = selectedTeacherTextBox.Text.Trim(),
                teacher_auditory = int.Parse(selectedAuditoryTextBox.Text)
            };

            editedTeacher.teacher_spec = editedTeacher.teacher_subject?.id ?? -1;

            if (TeacherController.EditTeacher(editedTeacher) == true)
            {
                zeroizeTextBoxes();
                System.Windows.Forms.MessageBox.Show("Изменение прошло успешно!");
            }
            else return;

            Classes.FrameSingleton.getFrame().Navigate(new Teachers());
        }

        private void deleteInfoButton_Click(object sender, RoutedEventArgs e)
        {
            if(selectedIdTextBox.Text == "" || selectedSubjectComboBox.Text == "" ||
                selectedTeacherTextBox.Text == "" || selectedAuditoryTextBox.Text == "")
            {
                System.Windows.Forms.MessageBox.Show("Выберите учителя для удаления или допишите информацию об учителе полностью!");
                return;
            }
            int subjectID = SubjectsController.GetSubject()
        .FirstOrDefault(s => s.name == selectedSubjectComboBox.Text)?.id ?? -1;

            if (subjectID == -1)
            {
                System.Windows.Forms.MessageBox.Show("Предмет не найден!");
                return;
            }

            Teacher teacher = new Teacher
            {
                teacher_id = int.Parse(selectedIdTextBox.Text),
                teacher_fullname = selectedTeacherTextBox.Text,
                teacher_spec = subjectID,
                teacher_auditory = int.Parse(selectedAuditoryTextBox.Text)
            };

            TeacherController.DropTeacher(teacher);
            System.Windows.Forms.MessageBox.Show($"Удаление учителя \"{teacher.teacher_fullname}\" прошло успешно!");
            Classes.FrameSingleton.getFrame().Navigate(new Teachers());
        }
    }
}
