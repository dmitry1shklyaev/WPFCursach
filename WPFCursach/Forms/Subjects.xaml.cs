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
            else if(writtenSubjectTextBox.Text.Trim() == "")
            {
                System.Windows.Forms.MessageBox.Show("Пожалуйста, введите название предмета, который вы хотите добавить!");
                return;
            }
            foreach(var sbj in SubjectsController.GetSubject())
            {
                if(sbj.name == writtenSubjectTextBox.Text)
                {
                    System.Windows.Forms.MessageBox.Show("Такой предмет уже существует!");
                    return;
                }
            }
            Subject subject = new Subject();
            subject.name = writtenSubjectTextBox.Text.Substring(0, 1).ToUpper() + writtenSubjectTextBox.Text.Substring(1);
            SubjectsController.AddSubject(subject);
            Classes.FrameSingleton.getFrame().Navigate(new Subjects());
        }

        private void deleteSubjectButton_Click(object sender, RoutedEventArgs e)
        {
            Subject subject = new Subject();
            subject.name = subjectsComboBox.Text;
            foreach(var subj in SubjectsController.GetSubject())
            {
                if(subj.name == subject.name)
                {
                    subject.id = subj.id;
                    break;
                }
            }
            
            foreach(var mark in MarksController.GetMarks())
            {
                if(mark.subject.name == subject.name)
                {
                    DialogResult dialogResult = System.Windows.Forms.MessageBox.Show($"По данному предмету проставлены четвертные оценки.\n" +
                        $"Вы действительно хотите удалить предмет?", "Удалить предмет?", MessageBoxButtons.YesNo);
                    if (dialogResult == DialogResult.Yes)
                    {
                        MarksController.DropMark(mark);
                    }
                    else return;
                }
            }
            SubjectsController.DropSubject(subject);
            Classes.FrameSingleton.getFrame().Navigate(new Subjects());
        }
    }
}
