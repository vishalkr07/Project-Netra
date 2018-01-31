using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImapX;
using ImapX.Collections;
using System.Windows;
namespace ProjectNetra.Mail
{
    class ImapService
    {
        private static ImapClient client { get; set; }

        // Initialize function
        public static void Initialize()
        {
            client = new ImapClient("imap.gmail.com", true);

            if (!client.Connect())
            {
                throw new Exception("Unable to connect to the Imap Server");
            }
        }

        // Closing connections
        private static void CloseConnection()
        {
            client.Disconnect();
        }

        // Login function
        public static bool Login(string user, string passwd)
        {
            return client.Login(user, passwd);
        }

        // Logout function

        public static void Logout()
        {
            if (client.IsAuthenticated) { client.Logout(); }
            Email.LoggedIn = false;
            CloseConnection();
        }

        public static List<EmailFolder> GetFolders()
        {
            var folders = new List<EmailFolder>();
            foreach (var folder in client.Folders)
            {
                folders.Add(new EmailFolder { Title = folder.Name });
            }
            
            folders.Add(new EmailFolder { Title = client.Folders.Important.Name });
            folders.Add(new EmailFolder { Title = client.Folders.Sent.Name });
            folders.Add(new EmailFolder { Title = client.Folders.All.Name });
            folders.Add(new EmailFolder { Title = client.Folders.Drafts.Name });
            folders.Add(new EmailFolder { Title = client.Folders.Flagged.Name });                       // Starred
            folders.Add(new EmailFolder { Title = client.Folders.Junk.Name });                          // Spam 
            folders.Add(new EmailFolder { Title = client.Folders.Trash.Name });
            // Before returning start the idling
            client.Folders.Inbox.StartIdling(); // And continue to listen for more.

            client.Folders.Inbox.OnNewMessagesArrived += Inbox_OnNewMessagesArrived;
            return folders;
        }

        private static void Inbox_OnNewMessagesArrived(object sender, IdleEventArgs e)
        {
            // Show a dialog
            MessageBox.Show($"A new message was downloaded in {e.Folder.Name} folder.");
        }

        public static MessageCollection GetMessagesForFolder(string name)
        {
            switch (name)
            {
                case "Important" :
                    client.Folders.Important.Messages.Download();
                    return client.Folders.Important.Messages;
                case "Sent Mail":
                    client.Folders.Sent.Messages.Download();
                    return client.Folders.Sent.Messages;
                case "All Mail":
                    client.Folders.All.Messages.Download();
                    return client.Folders.All.Messages;
                case "Drafts":
                    client.Folders.Drafts.Messages.Download();
                    return client.Folders.Drafts.Messages;
                case "Starred":
                    client.Folders.Flagged.Messages.Download();
                    return client.Folders.Flagged.Messages;
                case "Spam":
                    client.Folders.Junk.Messages.Download();
                    return client.Folders.Junk.Messages;
                case "Trash":
                    client.Folders.Trash.Messages.Download();
                    return client.Folders.Trash.Messages;
                default:
                    client.Folders[name].Messages.Download();                    // Start the download process;
                    return client.Folders[name].Messages;
            }
            
        }
    }
}
