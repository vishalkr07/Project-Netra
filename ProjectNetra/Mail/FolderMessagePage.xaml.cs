using ImapX;
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
    /// Interaction logic for FolderMessagePage.xaml
    /// </summary>
    public partial class FolderMessagePage : Page
    {
        public FolderMessagePage()
        {
            InitializeComponent();
        }

        public FolderMessagePage(string name)
           {
               InitializeComponent();
               
               messagesList.ItemsSource = ImapService.GetMessagesForFolder(name);
           }
   
           private void messagesList_SelectionChanged(object sender, SelectionChangedEventArgs e)
           {
               // Get the message
               var message = (sender as ListView).SelectedItem as Message;
               
               if(message != null)
               {
                   HomePage.ContentFrame.Content = new MessagePage(message);
               }
           }
       }
   
       public class EmailMessage
       {
           public long Uid { get; set; }
           public string Subject { get; set; }
       }
    }

