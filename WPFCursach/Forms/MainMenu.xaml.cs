﻿using System;
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
    /// Логика взаимодействия для Page1.xaml
    /// </summary>
    public partial class MainMenu : Page
    {
        public MainMenu()
        {
            InitializeComponent();
        }
        private static void OpenTeacherWindow()
        {
            //NavigationWindow navWIN = new NavigationWindow();
            //navWIN.Content = new Teachers();
            //navWIN.Show();

            Classes.FrameSingleton.getFrame().Navigate(new Teachers());
        }

        private void TeachersButton_Click(object sender, RoutedEventArgs e)
        {
            OpenTeacherWindow();
        }
    }
}