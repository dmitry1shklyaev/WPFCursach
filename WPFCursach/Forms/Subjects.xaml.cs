using BaseHandler.DBase;
using BaseHandler.DBase.Models;
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

namespace WPFCursach.Forms
{
    /// <summary>
    /// Логика взаимодействия для SubjectsAdd.xaml
    /// </summary>
    public partial class Subjects : Page
    {
        public Subjects()
        {
            InitializeComponent();
            LoadComboBox();
        }
        private void LoadComboBox()
        {
            var subjectsParse = SubjectsController.GetSubject();
            foreach (Subject subject in subjectsParse)
            {
                subjectsComboBox.Items.Add(subject.name);
            }
        }

        private void backToMenuButton_Click(object sender, RoutedEventArgs e)
        {
            Classes.FrameSingleton.getFrame().Navigate(new MainMenu());
        }

        private void addNewSubjectButton_Click(object sender, RoutedEventArgs e)
        {
            if (writtenSubjectTextBox.Text.Any(char.IsDigit))
            {
                System.Windows.Forms.MessageBox.Show("Предмет не должен содержать в себе цифру/ы!\nПредмет не добавлен!");
                return;
            }
            else if (writtenSubjectTextBox.Text.Trim() == "")
            {
                System.Windows.Forms.MessageBox.Show("Пожалуйста, введите название предмета, который вы хотите добавить!");
                return;
            }
            if (SubjectsController.GetSubject().Any(sbj => sbj.name == writtenSubjectTextBox.Text))
            {
                System.Windows.Forms.MessageBox.Show("Такой предмет уже существует!");
                return;
            }
            Subject subject = new Subject();
            subject.name = writtenSubjectTextBox.Text.Substring(0, 1).ToUpper() + writtenSubjectTextBox.Text.Substring(1);
            if (SubjectsController.AddSubject(subject) == true)
            {
                System.Windows.Forms.MessageBox.Show($"Предмет \"{subject.name}\" успешно добавлен!");
            }
            Classes.FrameSingleton.getFrame().Navigate(new Subjects());
        }

        private void deleteSubjectButton_Click(object sender, RoutedEventArgs e)
        {
            if(subjectsComboBox.Text == "")
            {
                System.Windows.Forms.MessageBox.Show("Выберите предмет для удаления!");
                return;
            }
            Subject subject = new Subject();
            subject.name = subjectsComboBox.Text;
            var matchingSubject = SubjectsController.GetSubject().FirstOrDefault(subj => subj.name == subject.name);
            if (matchingSubject != null)
            {
                subject.id = matchingSubject.id;
            }
            if(SubjectsController.DropSubject(subject) == true)
            {
                System.Windows.Forms.MessageBox.Show($"Предмет \"{subject.name}\" успешно удалён!");
            }

            Classes.FrameSingleton.getFrame().Navigate(new Subjects());
        }
    }
}
