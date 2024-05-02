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
            int selectedSubjectID = SubjectsController.GetSubject().FirstOrDefault(s => s.name == selectedSubject)?.id ?? 0;

            if (selectedPupil != null)
            {
                var mark = MarksController.GetMarks()
                    .FirstOrDefault(m => m.pupil.pupil_name == selectedPupil.pupil_name &&
                                            (m.subject.id == selectedSubjectID || m.subject.name == selectedSubject));

                ifMarkIsSetTextBox.Text = mark != null ? "Да" : "Нет";
                markTextBox.Text = mark?.grade.ToString() ?? "";
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
            if (markComboBox.Text == "")
            {
                System.Windows.Forms.MessageBox.Show("Вы не выбрали оценку!");
                return;
            }
            else if ((((g4.SelectedItem == null || g5.SelectedItem == null) && markComboBox.Text != "") && ifMarkIsSetTextBox.Text == "") 
                || ((g4.SelectedItem != null && g5.SelectedItem == null) && markComboBox.Text != ""))
            {
                System.Windows.Forms.MessageBox.Show("Сначала выберите ученика в первой таблице, " +
                    "затем выберите во второй колонке предмет, по которому вы хотите поставить оценку ученику");
                return;
            }

            int settedGrade = int.Parse(markComboBox.Text);
            Pupil selectedPupil = g4.SelectedItem as Pupil;
            var selectedSubject = getSelectedSubjectName();
            var subjectID = SubjectsController.GetSubject().FirstOrDefault(s => s.name == selectedSubject)?.id;

            var mark = new Mark
            {
                grade = settedGrade,
                subject = subjectID != null ? SubjectsController.GetSubjectByID(subjectID.Value) : null,
                pupil = selectedPupil
            };

            var existingMark = MarksController.GetMarks().FirstOrDefault(m => m.pupil.pupil_id == mark.pupil.pupil_id && m.subject.id == mark.subject?.id);
            if (existingMark != null)
            {
                mark.id = existingMark.id;
            }

            if (MarksController.CheckDuplicatesMarks(mark) == 0)
            {
                if (MarksController.AddMark(mark) == true)
                {
                    System.Windows.Forms.MessageBox.Show($"Оценка \"{mark.grade}\" ученику \"{mark.pupil.pupil_name}\" по предмету \"{mark.subject.name}\" успешно проставлена!");
                }
            }
            else if (MarksController.CheckDuplicatesMarks(mark) == 1)
            {
                DialogResult dialogResult = System.Windows.Forms.MessageBox.Show($"Оценка у ученика \"{selectedPupil.pupil_name}\" по предмету \"{selectedSubject}\" уже имеется!\nИзменить оценку?", "Изменить оценку?", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    if (MarksController.EditMark(mark) == true)
                    {
                        System.Windows.Forms.MessageBox.Show($"Оценка успешно изменена на \"{mark.grade}\"!");
                    }
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
