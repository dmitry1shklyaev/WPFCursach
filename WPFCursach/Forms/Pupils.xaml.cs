using System;
using System.Collections.Generic;
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
            var classesParse = SchoolclassesController.SortClassesInAscendingOrder(SchoolclassesController.GetClasses());
            foreach (Schoolclass sc in classesParse)
            {
                selectedClassComboBox.Items.Add(sc.class_grade);
            }
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
                Pupil pupil = new Pupil();
                pupil.pupil_name = selectedPupilTextBox.Text;
                foreach(Schoolclass sc in SchoolclassesController.GetClasses())
                {
                    if (sc.class_grade == selectedClassComboBox.Text) pupil.pupil_class.class_id = sc.class_id;
                }
                PupilsController.AddPupil(pupil);
                Classes.FrameSingleton.getFrame().Navigate(new Pupils());
            }
            catch(Exception ex)
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
            if (selectedIdTextBox.Text == "")
            {
                System.Windows.Forms.MessageBox.Show("Вы вписали данные ученика, но не выбрали его в таблице!");
                return;
            }
            else if (selectedIdTextBox.Text == "" || selectedClassComboBox.Text == "" || selectedPupilTextBox.Text.Trim() == "")
            {
                System.Windows.Forms.MessageBox.Show("Одно и/или несколько полей пусты!\nЗаполните все поля!");
                return;
            }
            Pupil editedPupil = new Pupil();
            editedPupil.pupil_id = int.Parse(selectedIdTextBox.Text);
            List<Schoolclass> classesTemp = new List<Schoolclass>();
            classesTemp = SchoolclassesController.GetClasses();
            int indexOfClass = -1;
            for(int i = 0; i < classesTemp.Count(); i++)
            {
                if (classesTemp[i].class_grade ==  selectedClassComboBox.Text)
                {
                    indexOfClass = classesTemp[i].class_id;
                    break;
                }
            }
            if (indexOfClass == -1)
            {
                System.Windows.Forms.MessageBox.Show("Класс не был найден");
                return;
            }

            editedPupil.pupil_name = selectedPupilTextBox.Text;
            editedPupil.pupil_class.class_id = indexOfClass;
            editedPupil.pupil_class.class_grade = SchoolclassesController.GetSchoolclassByID(editedPupil.pupil_class.class_id).class_grade;
            PupilsController.EditPupil(editedPupil);
            zeroizeTextBoxes();
            System.Windows.Forms.MessageBox.Show("Изменение прошло успешно!");
            Classes.FrameSingleton.getFrame().Navigate(new Pupils());
        }

        private void deleteInfoButton_Click(object sender, RoutedEventArgs e)
        {
            if (selectedIdTextBox.Text == "" || selectedClassComboBox.Text == "" || selectedPupilTextBox.Text == "")
            {
                System.Windows.Forms.MessageBox.Show("Выберите ученика для удаления!");
                return;
            }
            Pupil pupil = new Pupil();
            pupil.pupil_name = selectedPupilTextBox.Text;
            pupil.pupil_id = int.Parse(selectedIdTextBox.Text);
            var classes = SchoolclassesController.GetClasses();
            int classID = -1;
            for(int i = 0; i < classes.Count(); i++)
            {
                Schoolclass tempClass = classes[i];
                if(tempClass.class_grade == selectedClassComboBox.Text)
                {
                    classID = i;
                    break;
                }
            }
            pupil.pupil_class.class_id = classID;
            PupilsController.DropPupil(pupil);
            System.Windows.Forms.MessageBox.Show("Удаление прошло успешно!");
            Classes.FrameSingleton.getFrame().Navigate(new Pupils());
        }
    }
}
