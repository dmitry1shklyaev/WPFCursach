using System;
using System.Collections.Generic;
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
using BaseHandler;
using BaseHandler.DBase;
using BaseHandler.DBase.Models;
using WPFCursach.Classes;

namespace WPFCursach.Forms
{
    /// <summary>
    /// Логика взаимодействия для Pupils.xaml
    /// </summary>
    public partial class Pupils : Page
    {
        public Pupils()
        {
            InitializeComponent();
            g2.ItemsSource = BaseHandler.DBase.PupilsController.GetPupils();
            LoadComboBox();
        }
        private void LoadComboBox()
        {
            var classesParse = ClassesView.SortClassesInAscendingOrder(SchoolclassesController.GetClasses());
            selectedClassComboBox.ItemsSource = classesParse.Select(sc => sc.class_grade);
        }

        private void g2_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (g2.SelectedItem != null)
            {
                Pupil row = (Pupil)g2.SelectedItem;

                selectedPupilTextBox.Text = row.pupil_name;
                selectedClassComboBox.SelectedItem = row.pupil_class.class_grade;
                selectedIdTextBox.Text = row.pupil_id.ToString();
            }
        }

        private void backToMenuButton_Click(object sender, RoutedEventArgs e)
        {
            Classes.FrameSingleton.getFrame().Navigate(new MainMenu());
        }

        private void addInfoButton_Click(object sender, RoutedEventArgs e)
        {
            if (selectedIdTextBox.Text != "" || selectedClassComboBox.Text == "" || selectedPupilTextBox.Text.Trim() == "")
                {
                System.Windows.Forms.MessageBox.Show("Одно и/или несколько полей пусты!\nПожалуйста, заполните все поля!");
                return;
            }

            try
            {
                Pupil pupil = new Pupil
                {
                    pupil_name = selectedPupilTextBox.Text,
                    pupil_class = SchoolclassesController.GetClasses().FirstOrDefault(sc => sc.class_grade == selectedClassComboBox.Text)
                };

                if (PupilsController.AddPupil(pupil))
                {
                    System.Windows.Forms.MessageBox.Show("Ученик успешно добавлен!");
                    Classes.FrameSingleton.getFrame().Navigate(new Pupils());
                }
                else return;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
        private void zeroizeTextBoxes()
        {
            selectedPupilTextBox.Text = null;
            selectedIdTextBox.Text = null;
            selectedClassComboBox.Text = null;
        }
        private void unselectCellsButton_Click(object sender, RoutedEventArgs e)
        {
            g2.UnselectAll();
            zeroizeTextBoxes();
        }

        private void updateInfoButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(selectedIdTextBox.Text))
            {
                System.Windows.Forms.MessageBox.Show("Вы вписали данные ученика, но не выбрали его в таблице!");
                return;
            }
            else if (string.IsNullOrEmpty(selectedIdTextBox.Text) || string.IsNullOrEmpty(selectedClassComboBox.Text) || string.IsNullOrEmpty(selectedPupilTextBox.Text.Trim()))
            {
                System.Windows.Forms.MessageBox.Show("Одно и/или несколько полей пусты!\nЗаполните все поля!");
                return;
            }

            int pupilId = int.Parse(selectedIdTextBox.Text);
            var selectedClass = SchoolclassesController.GetClasses().FirstOrDefault(c => c.class_grade == selectedClassComboBox.Text);
            if (selectedClass == null)
            {
                System.Windows.Forms.MessageBox.Show("Класс не был найден");
                return;
            }

            Pupil editedPupil = new Pupil
            {
                pupil_id = pupilId,
                pupil_name = selectedPupilTextBox.Text,
                pupil_class = selectedClass
            };

            if (PupilsController.EditPupil(editedPupil) == true)
            {
                zeroizeTextBoxes();
                System.Windows.Forms.MessageBox.Show("Изменение прошло успешно!");
            }
            else return;
            Classes.FrameSingleton.getFrame().Navigate(new Pupils());

        }

        private void deleteInfoButton_Click(object sender, RoutedEventArgs e)
        {
            if (selectedIdTextBox.Text == "" || selectedClassComboBox.Text == "" || selectedPupilTextBox.Text == "")
            {
                System.Windows.Forms.MessageBox.Show("Выберите ученика для удаления!");
                return;
            }

            int classID = SchoolclassesController.GetClasses()
                .FirstOrDefault(c => c.class_grade == selectedClassComboBox.Text)
                ?.class_id ?? -1;

            if (classID == -1)
            {
                System.Windows.Forms.MessageBox.Show("Класс не найден!");
                return;
            }

            Pupil pupil = new Pupil
            {
                pupil_name = selectedPupilTextBox.Text,
                pupil_id = int.Parse(selectedIdTextBox.Text),
                pupil_class = new Schoolclass { class_id = classID }
            };

            if (PupilsController.DropPupil(pupil) == true)
            {
                System.Windows.Forms.MessageBox.Show("Удаление прошло успешно!");
            }
            else return;

            Classes.FrameSingleton.getFrame().Navigate(new Pupils());
        }
    }
}
