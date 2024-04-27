using BaseHandler.DBase;
using BaseHandler.DBase.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WPFCursach.Forms
{
    /// <summary>
    /// Логика взаимодействия для Journal.xaml
    /// </summary>
    public partial class Journal : Page
    {
        public Journal()
        {
            InitializeComponent();
            marksComboBoxFill();
            g4.ItemsSource = PupilsController.GetPupils();
        }

        private void g4_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            g5.ItemsSource = SubjectsController.GetSubject();
        }
        private void marksComboBoxFill()
        {
            List<int> marks = new List<int>() { 2, 3, 4, 5 };
            foreach(int mark in marks)
            {
                markComboBox.Items.Add(mark);
            }
        }
        private void UpdateMarkDisplay()
        {
            Pupil selectedPupil = g4.SelectedItem as Pupil;
            string selectedSubject = getSelectedSubjectName();
            int selectedSubjectID = -1;
            foreach (Subject sbj in SubjectsController.GetSubject())
            {
                if (selectedSubject == sbj.name)
                {
                    selectedSubjectID = sbj.id;
                    break;
                }
            }

            if (selectedPupil != null)
            {
                ifMarkIsSetTextBox.Text = "Нет";
                markTextBox.Text = "";

                foreach (var mark in MarksController.GetMarks())
                {
                    if (mark.pupil.pupil_name == selectedPupil.pupil_name && (mark.subject.id == selectedSubjectID || mark.subject.name == selectedSubject))
                    {
                        ifMarkIsSetTextBox.Text = "Да";
                        markTextBox.Text = mark.grade.ToString();
                        break;
                    }
                }
            }
        }
        private void g5_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateMarkDisplay();
        }

        private void backToMenuButton_Click(object sender, RoutedEventArgs e)
        {
            Classes.FrameSingleton.getFrame().Navigate(new MainMenu());
        }
        private string getSelectedSubjectName()
        {
            var selectedCell = g5.SelectedCells[0];
            var cellContent = selectedCell.Column.GetCellContent(selectedCell.Item);
            return (cellContent as TextBlock)?.Text;
        }

        private void setMarkButton_Click(object sender, RoutedEventArgs e)
        {
            if(markComboBox.Text == "")
            {
                System.Windows.Forms.MessageBox.Show("Вы не выбрали оценку!");
                return;
            }
            else if (((g4.SelectedItem == null || g5.SelectedItem == null) && markComboBox.Text != "") && ifMarkIsSetTextBox.Text != "")
            {
                System.Windows.Forms.MessageBox.Show("Сначала выберите ученика в первой таблице, " +
                    "затем выберите во второй колонке предмет, по которому вы хотите поставить оценку ученику");
                return;
            }
            int settedGrade = int.Parse(markComboBox.Text);
            Pupil selectedPupil = g4.SelectedItem as Pupil;

            var selectedSubject = getSelectedSubjectName();

            int subjectID = -1;
            foreach(Subject sbj in SubjectsController.GetSubject())
            {
                if(selectedSubject == sbj.name)
                {
                    subjectID = sbj.id;
                    break;
                }
            }
            var subjectByID = SubjectsController.GetSubjectByID(subjectID);
            Mark mark = new Mark();
            mark.grade = settedGrade;
            mark.subject = subjectByID;
            mark.pupil = selectedPupil;
            foreach(var m in MarksController.GetMarks())
            {
                if(m.pupil.pupil_id == mark.pupil.pupil_id && mark.subject.id == m.subject.id)
                {
                    mark.id = m.id;
                    break;
                }
            }
            if(MarksController.CheckDuplicatesMarks(mark) == 0) MarksController.AddMark(mark);
            else if(MarksController.CheckDuplicatesMarks(mark) == 1)
            {
                DialogResult dialogResult = System.Windows.Forms.MessageBox.Show($"Оценка у ученика \"{selectedPupil.pupil_name}\" по предмету \"{selectedSubject}\" уже имеется!\nИзменить оценку?", "Изменить оценку?", MessageBoxButtons.YesNo);
                if(dialogResult == DialogResult.Yes)
                {
                    MarksController.EditMark(mark);
                    System.Windows.Forms.MessageBox.Show($"Оценка успешно изменена на \"{mark.grade}\"!");
                }
                else return;
            }
            else
            {
                System.Windows.Forms.MessageBox.Show("Произошла непредвиденная ошибка");
            }
            UpdateMarkDisplay();
        }
    }
}
