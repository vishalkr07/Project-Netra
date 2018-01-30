using ImapX;
using ImapX.Collections;
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
        private List<EmailMessage> msgList = new List<EmailMessage>();
        private MessageCollection msgs = null;
        public FolderMessagePage()
        {
            InitializeComponent();
        }

        public FolderMessagePage(string name)
        {
            InitializeComponent();

            msgs = ImapService.GetMessagesForFolder(name);
            foreach(Message msg in msgs)
            {
                msgList.Add(new EmailMessage() { Subject = msg.Subject, From = msg.From.DisplayName, Time = msg.Date.Value.ToString()});
            }
            messagesList.ItemsSource = msgList;
        }
   
        private void messagesList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Get the index of the selected item
            int selectedId = messagesList.SelectedIndex;
               
            if(selectedId != -1)
            {
                HomePage.ContentFrame.Content = new MessagePage(msgs[selectedId]);
            }
        }
    }
   
    public class EmailMessage
    {
        public long Uid { get; set; }
        public string Subject { get; set; }
        public string From { get; set; }
        public string Time { get; set; }
    }
}

