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
        private int firstItemNo = 1,lastItemNo = 0;                                         // Tracks the first and last item no. of the set of items currently being displayed in GUI 
        private int noOfFiles = 0, noOfFolders = 0;                                         // Tracks the no. of files/folders in the current directory 
        private int noOfItems = 0;                                                          // Tracks no. of items displayed in the GUI
        private DirectoryInfo pD = null;
        private bool isFilterActive = false;

        public File_Manager()
        {
            InitializeComponent();
            
            fmp = new File_Manager_Page();
            B.IsEnabled = N.IsEnabled = false;
            UpdateMembers("");
            ll.AddFirst(fmp);
            llnode = ll.First;
            MainFrame.Navigate(fmp);
        }

        public Tuple<int,int> GetItemRange()
        {
            return new Tuple<int, int>(firstItemNo,lastItemNo);
        }

        private void UpdateMembers(string parentDir)                      // parentDir != "", if Open() is called; parentDir indicates Parent Directory name
        {
            isFolder = fmp.GetFolderStatus();
            firstItemNo = fmp.GetFirstItemNo();
            lastItemNo = fmp.GetLastItemNo();
            noOfItems = (lastItemNo - firstItemNo + 1);
            noOfFiles = fmp.GetNoOfFiles();
            noOfFolders = fmp.GetNoOfFolders();
            pD = fmp.GetParentDirectory();
            DropDown.IsEnabled = (pD != null);
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
        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Filter(DropDown.SelectedIndex + 1);
        }
        private void KeyDownHandler(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
                Open(fmp.GetSelectedItemNo());
        }
        private void ButtonFilterActivate(object sender, RoutedEventArgs e)
        {
            isFilterActive = true;
        }

        public void Back()
        {
            /*** revert the filter selection to none  **/
            CB1.IsSelected = true;                       // ComboBox_SelectionChanged event will be triggered
            /********************************/
            llnode = llnode.Previous;
            fmp = llnode.Value;
            N.IsEnabled = true;
            B.IsEnabled = (llnode.Previous != null);
            UpdateMembers("");
            MainFrame.Navigate(fmp);
        }

        public void Next()
        {
            /*** revert the filter selection to none  **/
            CB1.IsSelected = true;                      // ComboBox_SelectionChanged event will be triggered
            /********************************/
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
        private void Filter(int index)
        {
            if (fmp == null || pD == null)                            // Occurs when the File_Manager() constructor is called
                return;
            fmp.Filter(((ComboBoxItem)(DropDown.Items[index - 1])).Content.ToString());
            UpdateMembers("");
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
                            Document.MSWord_Controller mSWord = new Document.MSWord_Controller(fI.FullName);
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

        private void ReadOutFilters()
        {
            Speak_Listen.StartPromptBuilder();
            for (int i = 1; i < 12;i++)
            {
                //string s = ((ComboBoxItem)DropDown.ItemContainerGenerator.ContainerFromIndex(i)).Content.ToString();
                string s = ((ComboBoxItem)(DropDown.Items[i - 1])).Content.ToString();
                Speak_Listen.AddPrompt("Say "+ (i.ToString()) + " to filter " + s);
            }
            Speak_Listen.AddPrompt("Say Repeat to repeat the list");
            Speak_Listen.SpeakPrompt();
        }
        public void Instruct(string cmd)
        {
            Debug.WriteLine("File Manager Input:  " + cmd);
            firstItemNo = fmp.GetFirstItemNo();
            lastItemNo = fmp.GetLastItemNo();
            switch (cmd)
            {
                case "Back":
                    isFilterActive = false;
                    if (B.IsEnabled)
                        Back();         
                    else
                        Speak_Listen.Speak("Sorry, No such control is present");
                    break;
                case "Next":
                    isFilterActive = false;
                    if (N.IsEnabled)
                        Next(); 
                    else
                        Speak_Listen.Speak("Sorry, No such control is present");
                    break;
                case "Repeat":
                    if (isFilterActive)
                    {
                        ReadOutFilters();
                        firstItemNo = 1;
                        lastItemNo = 11;
                    }
                    else
                        Repeat();
                    break;
                case "Up":
                    isFilterActive = false;
                    if (U.IsEnabled)
                        Up();
                    else
                        Speak_Listen.Speak("Sorry, No such control is present");
                    break;
                case "Down":
                    isFilterActive = false;
                    if (D.IsEnabled)
                        Down();
                    else
                        Speak_Listen.Speak("Sorry, No such control is present");
                    break;
                case "Files":                    
                case "Folders":
                    isFilterActive = false;
                    if (F.IsEnabled)
                    {
                        if (cmd == F.Content.ToString())
                            FileOrFolder();
                        else
                            Repeat();
                    }
                    else
                        Speak_Listen.Speak("Sorry, No such control is present");
                    break;
                case "Filter":
                    if (pD == null)                          // If this is the home page of the file manager
                        Speak_Listen.Speak("Sorry, No such control is present");
                    else
                    {
                        isFilterActive = true;
                        ReadOutFilters();
                        firstItemNo = 1;
                        lastItemNo = 11;
                    }
                    break;
                default:                                     //  Number input
                    if (isFilterActive)                      //  Number input for filters
                    {
                        Filter(int.Parse(cmd));
                        isFilterActive = false;
                    }
                    else                                     //  Number input for file/folder selection
                    {
                        if (isFolder == 0)
                        {
                            Speak_Listen.Speak("Sorry, this folder is empty");
                        }
                        else
                        {
                            /*
                            if (number[cmd] > noOfItems)          // Item no. selected is not actually present in the list
                            {
                                Speak_Listen.Speak("Sorry, wrong choice. Say a number from 1 to " + noOfItems.ToString());
                            }
                            else
                            {
                                Open(number[cmd]);
                            }*/

                            Open((((int.Parse(cmd))-1)%10) + 1);
                        }
                    }
                    break;
            }
        }
    }

    
}