using BaseHandler.DBase.Models;
using BaseHandler.DBase;
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

namespace WPFCursach.Forms
{
    /// <summary>
    /// Логика взаимодействия для ClassesView.xaml
    /// </summary>
    public partial class ClassesView : Page
    {
        private static List<Pupil> pupils;
        public ClassesView()
        {
            InitializeComponent();
            g3.ItemsSource = PupilsController.GetPupils();
            LoadComboBox();
            LoadComboBoxesToAddClass();
        }
        private void LoadComboBoxesToAddClass()
        {
            addClassDigit.ItemsSource = Enumerable.Range(1, 11).Select(i => i.ToString());
            addClassLetter.ItemsSource = new List<string>()
            {
                "А", "Б", "В", "Г", "Д", "Е", "Ё", "Ж", "З", "И", "Й", "К", "Л", "М",
                "Н", "О", "П", "Р", "С", "Т", "У", "Ф", "Х", "Ц", "Ч", "Ш", "Щ", "Ъ",
                "Ы", "Ь", "Э", "Ю", "Я"
            };
        }
        private void LoadComboBox()
        {
            foreach(var cl in SortClassesInAscendingOrder(SchoolclassesController.GetClasses()))
            {
                selectClassComboBox.Items.Add(cl.class_grade);
                deleteClassComboBox.Items.Add(cl.class_grade);
            }
        }

        private void displayPupilsFromSelectedClass_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(selectClassComboBox.Text))
            {
                MessageBox.Show("Класс не выбран!");
                return;
            }
            g3.ItemsSource = PupilsFromOneClass(selectClassComboBox.Text);
            MessageBox.Show($"Ученики из класса {selectClassComboBox.Text} успешно выведены!");
        }
        public static List<Pupil> PupilsFromOneClass(string cl)
        {
            List<Pupil> allPupils = PupilsController.GetPupils();
            if (!allPupils.Any()) return null;
            pupils = new List<Pupil>();
            foreach (var pupil in allPupils)
            {
                if (pupil.pupil_class.class_grade == cl)
                    pupils.Add(pupil);
            }
            return pupils;
        }

        private void addClass_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(addClassDigit.Text) || string.IsNullOrEmpty(addClassLetter.Text))
            {
                MessageBox.Show("Неправильно выбран класс для добавления!");
                return;
            }

            string classToAdd = $"{addClassDigit.Text} {addClassLetter.Text}";

            if (SchoolclassesController.GetClasses().Any(sclass => sclass.class_grade == classToAdd))
            {
                MessageBox.Show("Класс, который вы хотите добавить, уже существует.\nВыберите другой класс.");
                return;
            }

            Schoolclass sc = new Schoolclass { class_grade = classToAdd };
            if (SchoolclassesController.AddClass(sc))
            {
                MessageBox.Show($"{classToAdd} класс успешно добавлен!");
            }
            else return;

            selectClassComboBox.Items.Clear();
            selectClassComboBox.ItemsSource = SortClassesInAscendingOrder(SchoolclassesController.GetClasses()).Select(sclass => sclass.class_grade);
            Classes.FrameSingleton.getFrame().Navigate(new ClassesView());
        }
        public static List<Schoolclass> SortClassesInAscendingOrder(List<Schoolclass> classes)
        {
            return classes.OrderBy(c =>
            {
                string[] parts = c.class_grade.Split(' ');
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

        private void backToMenu_Click(object sender, RoutedEventArgs e)
        {
            Classes.FrameSingleton.getFrame().Navigate(new MainMenu());
        }

        private void deleteClassButton_Click(object sender, RoutedEventArgs e)
        {
            if (deleteClassComboBox.Text == "")
            {
                MessageBox.Show("Выберите класс для удаления!");
                return;
            }
            Schoolclass sc = SchoolclassesController.GetClasses()
                .FirstOrDefault(cl => cl.class_grade == deleteClassComboBox.Text);
            if (sc == null)
            {
                MessageBox.Show("Класс не найден!");
                return;
            }
            SchoolclassesController.DropSchoolclass(sc);
            MessageBox.Show($"{sc.class_grade} класс успешно удалён!");
            Classes.FrameSingleton.getFrame().Navigate(new ClassesView());
        }
    }
}
