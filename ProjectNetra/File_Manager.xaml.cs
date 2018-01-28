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
using System.Windows.Shapes;
using System.Diagnostics;
using System.IO;

namespace ProjectNetra
{
    /// <summary>
    /// Interaction logic for File_Manager.xaml
    /// </summary>
    public partial class File_Manager : Window
    {
        private File_Manager_Page fmp = null; 
        private LinkedList<File_Manager_Page> ll = new LinkedList<File_Manager_Page>();
        private LinkedListNode<File_Manager_Page> llnode = null,temp = null, nxt = null;
        private short isFolder = 0;                                                         // Will have 3 values: 0 -> none, 1->folder, 2->file
        private int firstItemNo = 1;                                                        // Tracks the first item no. of the set of items currently being displayed in GUI 
        private int noOfFiles = 0, noOfFolders = 0;                                         // Tracks the no. of files/folders in the current directory 

        public File_Manager()
        {
            InitializeComponent();
            fmp = new File_Manager_Page();
            B.IsEnabled = N.IsEnabled = false;
            UpdateMembers();
            ll.AddFirst(fmp);
            llnode = ll.First;
            MainFrame.Navigate(fmp);
        }
        private void UpdateMembers()
        {
            isFolder = fmp.GetFolderStatus();
            firstItemNo = fmp.GetFirstItemNo();
            noOfFiles = fmp.GetNoOfFiles();
            noOfFolders = fmp.GetNoOfFolders();
            F.Content = (isFolder == 2 ? "Folders" : "Files");
            F.IsEnabled = (noOfFolders != 0) && (noOfFiles != 0);
            U.IsEnabled = (firstItemNo != 1) || ((isFolder==2) && (firstItemNo == 1) && (noOfFolders != 0));
            D.IsEnabled = ((isFolder == 1) && ((firstItemNo+10<noOfFolders) || (noOfFiles != 0))) || ((isFolder == 2 )&& (firstItemNo + 10 < noOfFiles));
            O.IsEnabled = (isFolder != 0);
            fmp.ReadOutListItems(B.IsEnabled, N.IsEnabled, U.IsEnabled, D.IsEnabled, F.IsEnabled, F.Content.ToString());
        }

        private void ButtonBack(object sender, RoutedEventArgs e)
        {
            Back();
        }
        private void ButtonNext(object sender, RoutedEventArgs e)
        {
            Next();
        }
        private void ButtonRefresh(object sender, RoutedEventArgs e)
        {
            Repeat();
        }
        private void ButtonOpen(object sender, RoutedEventArgs e)
        {
            Open();            
        }
        private void ButtonUp(object sender, RoutedEventArgs e)
        {
            Up();
        }
        private void ButtonDown(object sender, RoutedEventArgs e)
        {
            Down();
        }
        private void ButtonFileFolder(object sender, RoutedEventArgs e)
        {
            FileOrFolder();
        }

        public void Back()
        {
            llnode = llnode.Previous;
            fmp = llnode.Value;
            N.IsEnabled = true;
            B.IsEnabled = (llnode.Previous != null);
            UpdateMembers();
            MainFrame.Navigate(fmp);
        }

        public void Next()
        {
            llnode = llnode.Next;
            fmp = llnode.Value;
            B.IsEnabled = true;
            N.IsEnabled = (llnode.Next != null);
            UpdateMembers();
            MainFrame.Navigate(fmp);
        }
        public void Repeat()
        {
            B.IsEnabled = (llnode.Previous != null);
            N.IsEnabled = (llnode.Next != null);
            UpdateMembers();
            MainFrame.Navigate(fmp);
        }
        public void Open()
        {
            DirectoryInfo di = fmp.GetSelectedFolder();
            if (di == null)
            {
                FileInfo fi = fmp.GetSelectedFile();
                if(fi == null)
                {
                    Speak_Listen.Speak("You have not selected any item.");
                }
                else
                {
                    Debug.WriteLine("QQQQQQQQQQQQQ    " + fi.Name);
                    if (fi.Name == "firefox.exe" || fi.Name == "chrome.exe" || fi.Name == "iexplore")
                    {
                        // TODO 1: Call the Web Browser Controller
                    }
                    else if (fi.Extension == ".pdf" || fi.Extension == ".txt" || fi.Extension == ".docx")
                    {
                        // TODO 1: Call the Document Controller
                    }
                    else if (fi.Extension == ".mp3" || fi.Extension == ".mp4" || fi.Extension == ".wav" || fi.Extension == ".mpeg" || fi.Extension == ".wmv" || fi.Extension == ".avi")
                    { 
                        // TODO 1: Call the Media Player
                    }
                    else
                    {
                        Speak_Listen.Speak("Sorry, the file format is not supported.");
                    }
                }
            }
            else
            {
                /*******************  Memory Management Start  ***************/

                temp = llnode.Next;

                while (temp != null)
                {
                    nxt = temp.Next;
                    temp.Value.Dispose();
                    ll.Remove(temp);
                    temp = nxt;
                }
                /*******************  Memory Management Finish  ****************/

                ll.AddAfter(llnode, new File_Manager_Page(di));
                llnode = llnode.Next;
                fmp = llnode.Value;
                B.IsEnabled = true;
                N.IsEnabled = false;
                UpdateMembers();
                MainFrame.Navigate(fmp);
            }
        }
        public void Up()
        {
            fmp.MoveWithinList(true);
            UpdateMembers();
        }
        public void Down()
        {
            fmp.MoveWithinList(false);
            UpdateMembers();
        }
        public void FileOrFolder()
        {
            fmp.SetFolderStatus(F.Content.ToString()=="Folders");
            UpdateMembers();
        }
        public void TakeCommands(string cmd)
        {
            
        }
    }

    
}