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

namespace ProjectNetra.Mail
{
    /// <summary>
    /// Interaction logic for LoginPage.xaml
    /// </summary>
    public partial class LoginPage : Page
    {
        public LoginPage()
        {
            InitializeComponent();
        }

        private void loginBtn_Click(object sender, RoutedEventArgs e)
        {
            Email.LoggedIn = ImapService.Login(username.Text, password.Password);

            // Also navigate the user
            if (Email.LoggedIn)
            {
                // Logged in
                Email.MainFrame.Content = new HomePage();
            }
            else
            {
                // Problem
                error.Text = "There was a problem logging you in to Google Mail.";
            }
        }
    }
}
