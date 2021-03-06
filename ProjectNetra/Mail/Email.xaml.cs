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
using System.Windows.Shapes;
using ImapX.Collections;
namespace ProjectNetra.Mail
{
    /// <summary>
    /// Interaction logic for Email.xaml
    /// </summary>
    public partial class Email : Window
    {
        public static Frame MainFrame { get; set; }
        public static bool LoggedIn { get; set; }
        public static LoginPage loginpage = new LoginPage();
        public Email()
        {
            InitializeComponent();
            MainFrame = mainFrame;

            LoadLoginPage(ref loginpage);
        }
        public static void LoadLoginPage(ref LoginPage lp)
        {
            MainFrame.Content = lp;
            ImapService.Initialize();
        }
        private void mainFrame_Navigated(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {

        }
    }
}
