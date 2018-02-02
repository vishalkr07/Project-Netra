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
        private Dictionary<string, int> number = new Dictionary<string, int>();
        private File_Manager_Page fmp = null; 
        private LinkedList<File_Manager_Page> ll = new LinkedList<File_Manager_Page>();
        private LinkedListNode<File_Manager_Page> llnode = null,temp = null, nxt = null;
        private short isFolder = 0;                                                         // Will have 3 values: 0 -> none, 1->folder, 2->file
        private int firstItemNo = 1,lastItemNo = 0;                                         // Tracks the first and last item no. of the set of items currently being displayed in GUI 
        private int noOfFiles = 0, noOfFolders = 0;                                         // Tracks the no. of files/folders in the current directory 
        private int noOfItems = 0;                                                          // Tracks no. of items displayed in the GUI

        public File_Manager()
        {
            InitializeComponent();

            number["one"] = 1;
            number["two"] = 2;
            number["three"] = 3;
            number["four"] = 4;
            number["five"] = 5;
            number["six"] = 6;
            number["seven"] = 7;
            number["eight"] = 8;
            number["nine"] = 9;
            number["ten"] = 10;

            fmp = new File_Manager_Page();
            B.IsEnabled = N.IsEnabled = false;
            UpdateMembers("");
            ll.AddFirst(fmp);
            llnode = ll.First;
            MainFrame.Navigate(fmp);
        }
        private void UpdateMembers(string parentDir)                      // parentDir != "", if Open() is called; parentDir indicates Parent Directory name
        {
            isFolder = fmp.GetFolderStatus();
            firstItemNo = fmp.GetFirstItemNo();
            lastItemNo = fmp.GetLastItemNo();
            noOfItems = (lastItemNo - firstItemNo + 1);
            noOfFiles = fmp.GetNoOfFiles();
            noOfFolders = fmp.GetNoOfFolders();
            F.Content = (isFolder == 2 ? "Folders" : "Files");
            F.IsEnabled = (noOfFolders != 0) && (noOfFiles != 0);
            U.IsEnabled = (firstItemNo != 1) || ((isFolder==2) && (firstItemNo == 1) && (noOfFolders != 0));
            D.IsEnabled = ((isFolder == 1) && ((lastItemNo < noOfFolders) || (noOfFiles != 0))) || ((isFolder == 2) && (lastItemNo < noOfFiles));
            O.IsEnabled = (isFolder != 0);
            fmp.ReadOutListItems(parentDir,B.IsEnabled, N.IsEnabled, U.IsEnabled, D.IsEnabled, F.IsEnabled, F.Content.ToString());
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
            Open(fmp.GetSelectedItemNo());            
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
            UpdateMembers("");
            MainFrame.Navigate(fmp);
        }

        public void Next()
        {
            llnode = llnode.Next;
            fmp = llnode.Value;
            B.IsEnabled = true;
            N.IsEnabled = (llnode.Next != null);
            UpdateMembers("");
            MainFrame.Navigate(fmp);
        }
        public void Repeat()
        {
            B.IsEnabled = (llnode.Previous != null);
            N.IsEnabled = (llnode.Next != null);
            UpdateMembers("");
            MainFrame.Navigate(fmp);
        }
        public void Open(int selectedItemNo)
        {
            if(selectedItemNo == 0)
            {
                Speak_Listen.Speak("You haven't selected any item.");
            }
            else if(isFolder == 1)
            {
                DirectoryInfo dI = fmp.GetSelectedFolder(selectedItemNo);
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

                ll.AddAfter(llnode, new File_Manager_Page(dI));
                llnode = llnode.Next;
                fmp = llnode.Value;
                B.IsEnabled = true;
                N.IsEnabled = false;
                UpdateMembers(dI.Name);
                MainFrame.Navigate(fmp);
            }
            else
            {
                FileInfo fI = fmp.GetSelectedFile(selectedItemNo);
                Debug.WriteLine("QQQQQQQQQQQQQ    " + fI.Name);
                if (fI.Name == "firefox.exe" || fI.Name == "chrome.exe" || fI.Name == "iexplore")
                {
                    // TODO 1: Call the Web Browser Controller
                }
                else
                {
                    switch (fI.Extension)
                    {
                        case ".pdf":
                            // TODO 1: Call the PDF Reader
                            break;
                        case ".txt":
                            // TODO 1: Call the Respective Document Controller
                            break;
                        case ".doc":
                        case ".docx":
                            Document.MSWord_Controller mSWord = new Document.MSWord_Controller();
                            mSWord.Open(fI.FullName);
                            break;
                        case ".mp3":
                        case ".mp4":
                        case ".wav":
                        case ".mpeg":
                        case ".wmv":
                        case ".avi":
                            // TODO 1: Call the Media Player
                            break;
                        default:
                            Speak_Listen.Speak("Sorry, the file format is not supported.");
                            break;
                    }
                }
            }
                        
        }
        public void Up()
        {
            fmp.MoveWithinList(true);
            UpdateMembers("");
        }
        public void Down()
        {
            fmp.MoveWithinList(false);
            UpdateMembers("");
        }
        public void FileOrFolder()
        {
            fmp.SetFolderStatus(F.Content.ToString()=="Folders");
            UpdateMembers("");
        }
        public void Instruct(string cmd)
        {
            Debug.WriteLine("File Manager Input:  " + cmd);
            switch (cmd)
            {
                case "Back":
                    if (B.IsEnabled)
                        Back();
                    else
                        Speak_Listen.Speak("Sorry, No such control is present");
                    break;
                case "Next":
                    if(N.IsEnabled)
                        Next();
                    else
                        Speak_Listen.Speak("Sorry, No such control is present");
                    break;
                case "Repeat":
                    Repeat();
                    break;
                case "Up":
                    if(U.IsEnabled)
                        Up();
                    else
                        Speak_Listen.Speak("Sorry, No such control is present");
                    break;
                case "Down":
                    if(D.IsEnabled)
                        Down();
                    else
                        Speak_Listen.Speak("Sorry, No such control is present");
                    break;
                case "Files":                    
                case "Folders":
                    if(F.IsEnabled)
                        FileOrFolder();
                    else
                        Speak_Listen.Speak("Sorry, No such control is present");
                    break;
                default:
                    if (isFolder == 0)
                    {
                        Speak_Listen.Speak("Sorry, this folder is empty");
                    }
                    else
                    {
                        if(number[cmd] > noOfItems)    // Item no. selected is not actually present in the list
                        {
                            Speak_Listen.Speak("Sorry, wrong choice. Say a number from 1 to " + noOfItems.ToString());
                        }
                        else
                        {
                            Open(number[cmd]);
                        }
                    }
                    break;
            }
        }
    }

    
}