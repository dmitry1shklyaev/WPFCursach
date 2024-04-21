﻿using System;
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

        public Teachers()
        {
            InitializeComponent();
            g1.ItemsSource = BaseHandler.DBase.TeacherController.GetTeachers();
            //bindDataGrid.BindDataGrid("teachers", g1);
            LoadComboBox();
        }
        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (g1.SelectedItem != null)
            {
                Teacher row = (Teacher)g1.SelectedItem;
                if (row.teacher_subject == null) 
                {
                    MessageBox.Show("no sbj");
                } 
                selectedTeacherTextBox.Text = row.teacher_fullname;

                //selectedSubjectComboBox.SelectedItem = row.GetSubject().name; //?
                selectedAuditoryTextBox.Text = row.teacher_auditory.ToString();
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

        private void addInfoButton_Click(object sender, RoutedEventArgs e)
        {
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
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }

}