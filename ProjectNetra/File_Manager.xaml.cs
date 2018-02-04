using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Diagnostics;
using System.IO;

namespace ProjectNetra
{
    /// <summary>
    /// Interaction logic for File_Manager.xaml
    /// </summary>

    public partial class File_Manager : Window
    {
        private List<char> notAllowedCharacters = new List<char>(new char[] { '/','\\','"','|',':','?','<','>','*',' ' });
        private Dictionary<string, int> cbItem = new Dictionary<string, int>();             // For retireving comboboxitem no. for a specific comboboxitem
        private File_Manager_Page fmp = null; 
        private LinkedList<File_Manager_Page> ll = new LinkedList<File_Manager_Page>();
        private LinkedListNode<File_Manager_Page> llnode = null,temp = null, nxt = null;
        private short isFolder = 0;                                                         // Will have 3 values: 0 -> none, 1->folder, 2->file
        private int firstItemNo = 1,lastItemNo = 0;                                         // Tracks the first and last item no. of the set of items currently being displayed in GUI 
        private int noOfFiles = 0, noOfFolders = 0;                                         // Tracks the no. of files/folders in the current directory 
        private int noOfItems = 0;                                                          // Tracks no. of items displayed in the GUI
        private DirectoryInfo pD = null;
        private bool isFilterActive = false,isNewFolder = false;
        private string filter = "*";

        public File_Manager()
        {
            InitializeComponent();

            cbItem["None"] = 1;
            cbItem[".pdf"]= 2;
            cbItem[".txt"]= 3;
            cbItem[".doc"] = 4;
            cbItem[".docx"] = 5;
            cbItem[".mp3"] = 6;
            cbItem[".mp4"] = 7;
            cbItem[".wav"] = 8;
            cbItem[".wmv"] = 9;
            cbItem[".mpeg"] = 10;
            cbItem[".avi"] = 11;

            fmp = new File_Manager_Page();
            ll.AddFirst(fmp);
            llnode = ll.First;
            TB1.Text = "";
            B.IsEnabled = N.IsEnabled = TB1.IsEnabled = false;
            UpdateMembers(true);
            MainFrame.Navigate(fmp);
        }

        public Tuple<int,int> GetItemRange()
        {
            return new Tuple<int, int>(firstItemNo,lastItemNo);
        }
        private bool Validate(string s)
        {
            s = s.Trim(new char[] { ' ' });
            if (s.Length == 0)
                return false;
            foreach(var c in notAllowedCharacters)
            {
                if (s.Contains(c))
                    return false;
            }
            return true;
        }

        // bool ack ---> determines whether to give acknowledgement on opening a folder; if(ack == True) ---> Back() | Next() | Open() has been called or Home page is opened.
        private void UpdateMembers(bool ack)
        {
            isFolder = fmp.GetFolderStatus();
            firstItemNo = fmp.GetFirstItemNo();
            lastItemNo = fmp.GetLastItemNo();
            noOfItems = (lastItemNo - firstItemNo + 1);
            noOfFiles = fmp.GetNoOfFiles();
            noOfFolders = fmp.GetNoOfFolders();
            pD = fmp.GetParentDirectory();
            filter = fmp.GetFilterStatus();
            filter = (filter == "*" ? "None" : filter.Substring(1));

            DropDown.IsEnabled = (pD != null);
            F.Content = (isFolder == 2 ? "Folders" : "Files");
            F.IsEnabled = (noOfFolders != 0) && (noOfFiles != 0);
            U.IsEnabled = (firstItemNo != 1) || ((isFolder==2) && (firstItemNo == 1) && (noOfFolders != 0));
            D.IsEnabled = ((isFolder == 1) && ((lastItemNo < noOfFolders) || (noOfFiles != 0))) || ((isFolder == 2) && (lastItemNo < noOfFiles));
            O.IsEnabled = (isFolder != 0);
            FilterBtn.IsEnabled = (pD!=null);
            NF1.IsEnabled = NF2.IsEnabled = DF1.IsEnabled = DF2.IsEnabled = RNF1.IsEnabled = RNF2.IsEnabled = (pD!=null);
            ((ComboBoxItem)DropDown.Items[cbItem[filter]-1]).IsSelected = true;     // ComboBox_SelectionChanged Event is Triggered

            UpdateFilter(false);

            if(ack)
                fmp.AcknowlwdgeOpen();
        }
        private void UpdateFilter(bool b)
        {
            isFilterActive = b;
            if (b)
                FilterBtn.Background = Brushes.GreenYellow;
            else
                FilterBtn.Background = Brushes.Blue;
            DropDown.IsEnabled = b;
        }


        private void ButtonOpen(object sender, RoutedEventArgs e)
        {
            Instruct((fmp.GetSelectedItemNo() - 1 + firstItemNo).ToString());
        }
        private void ButtonBack(object sender, RoutedEventArgs e)
        {
            Instruct("Back");
        }
        private void ButtonNext(object sender, RoutedEventArgs e)
        {
            Instruct("Next");
        }
        private void ButtonUp(object sender, RoutedEventArgs e)
        {
            Instruct("Up");
        }
        private void ButtonDown(object sender, RoutedEventArgs e)
        {
            Instruct("Down");
        }
        private void ButtonRefresh(object sender, RoutedEventArgs e)
        {
            Instruct("Refresh");
        }
        private void ButtonReadOut(object sender, RoutedEventArgs e)
        {
            Instruct("Read Out");
        }
        private void ButtonControls(object sender, RoutedEventArgs e)
        {
            Instruct("Controls");
        }
        private void ButtonFileFolder(object sender, RoutedEventArgs e)
        {
            Instruct(F.Content.ToString());
        }
        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Debug.WriteLine("Combobox Event Triggered");
            if(isFilterActive)                                          // This is actually true when an ComboBoxItem is selected from GUI by clicking
                Instruct((DropDown.SelectedIndex + 1).ToString());
        }
        private void KeyDownHandler(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
            {
                if (isFilterActive)
                    Instruct((DropDown.SelectedIndex + 1).ToString());
                else if (TB1.IsEnabled)
                {
                    if (isNewFolder)
                        CreateDirectory(TB1.Text);
                    else
                        CreateFile(TB1.Text);
                }
                else 
                    Instruct(fmp.GetSelectedItemNo().ToString());

                R.Focus();
            }
                
        }
        private void ButtonFilterActivate(object sender, RoutedEventArgs e)
        {
            Instruct("Filter");
        }
        private void ButtonNewFolderBtn(object sender, RoutedEventArgs e)
        {
            Instruct("New Folder");
        }
        private void ButtonNewFileBtn(object sender, RoutedEventArgs e)
        {
            Instruct("New File");
        }

        private void ButtonDeleteFolderBtn(object sender, RoutedEventArgs e)
        {
            Instruct("Delete Folder");
        }
        private void ButtonDeleteFileBtn(object sender, RoutedEventArgs e)
        {
            Instruct("Delete File");
        }
        private void ButtonRenameFolderBtn(object sender, RoutedEventArgs e)
        {
            Instruct("Rename Folder");
        }
        private void ButtonRenameFileBtn(object sender, RoutedEventArgs e)
        {
            Instruct("Rename File");
        }


        private void Back()
        {
            llnode = llnode.Previous;
            fmp = llnode.Value;
            N.IsEnabled = true;
            B.IsEnabled = (llnode.Previous != null);
            UpdateMembers(true);
            Debug.WriteLine("Inside Back ---------> IsFilterActive  "+isFilterActive);
            MainFrame.Navigate(fmp);
        }
        private void Next()
        {
            llnode = llnode.Next;
            fmp = llnode.Value;
            B.IsEnabled = true;
            N.IsEnabled = (llnode.Next != null);
            UpdateMembers(true);
            MainFrame.Navigate(fmp);
        }
        private void Up()
        {
            fmp.MoveWithinList(true);
            UpdateMembers(false);
        }
        private void Down()
        {
            fmp.MoveWithinList(false);
            UpdateMembers(false);
        }
        private void FileOrFolder()
        {
            fmp.SetFolderStatus(F.Content.ToString() == "Folders");
            UpdateMembers(false);
        }
        private void Refresh()
        {
            if (pD != null)
                llnode.Value = new File_Manager_Page(pD);
            else
                llnode.Value = new File_Manager_Page();
            fmp = llnode.Value;
            B.IsEnabled = (llnode.Previous != null);
            N.IsEnabled = (llnode.Next != null);
            UpdateMembers(true);
            MainFrame.Navigate(fmp);
        }
        private void Filter(int index)
        {
            if (fmp == null || pD == null)                            // Occurs when the File_Manager() constructor is called
                return;
            fmp.Filter(((ComboBoxItem)(DropDown.Items[index - 1])).Content.ToString());
            UpdateMembers(false);
        }
        private void Open(int selectedItemNo)
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
                UpdateMembers(true);
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
        private void CreateDirectory(string dirName)
        {
            if (!Validate(dirName))
            {
                /*
                 *  TODO 1: Most of the chars in"notAllowedCharacters" are not recognised by SpeechSynthesis. Try to hardcode is.
                 */
                Speak_Listen.StartPromptBuilder();
                Speak_Listen.AddPrompt("A folder name can't contain any of these characters:");
                foreach (var c in notAllowedCharacters)
                    Speak_Listen.AddPrompt(c+"");
                Speak_Listen.AddPrompt("Please type again");
                Speak_Listen.SpeakPrompt();
                return;
            }
            string pathString = Path.Combine(pD.FullName, dirName);
            if (Directory.Exists(pathString))
            {
                Debug.WriteLine("Directory Already Present");
                Speak_Listen.Speak("Sorry, Folder Already Exists.");
                return;
            }
            Directory.CreateDirectory(pathString);
            Refresh();
            Speak_Listen.Speak("A new folder named "+dirName+" is created.");
        }
        private void CreateFile(string fileName)
        {
            // Regular Expression (for File Format): Any name with atleast one letter in [a-z,A-Z,0-9] before "." and atleast one letter in [a-z,A-Z,0-9] after "." 
            Regex regex = new Regex(@"^[a-zA-Z0-9]+\.[a-zA-Z0-9]+$");      
            if (!Validate(fileName))
            {
                /*
                 *  TODO 1: Most of the chars in"notAllowedCharacters" are not recognised by SpeechSynthesis. Try to hardcode is.
                 */
                Speak_Listen.StartPromptBuilder();
                Speak_Listen.AddPrompt("A file name can't contain any of these characters:");
                foreach (var c in notAllowedCharacters)
                    Speak_Listen.AddPrompt(c + "");
                Speak_Listen.AddPrompt("Please type again");
                Speak_Listen.SpeakPrompt();
                return;
            }
            if (!regex.IsMatch(fileName))
            {
                Speak_Listen.Speak("File name can't be without an extension.");
                return;
            }
            string pathString = Path.Combine(pD.FullName, fileName);
            if (File.Exists(pathString))
            {
                Debug.WriteLine("File Already Present");
                Speak_Listen.Speak("Sorry, File Already Exists.");
                return;
            }
            File.Create(pathString);
            Refresh();
            Speak_Listen.Speak("A new file named " + fileName + " is created.");
        }
        private void DeleteFile(string pathString)
        {
            if (File.Exists(pathString))
            {
                File.Delete(pathString);
                return;
            }
        }
        private void DeleteDirectory(string pathString)
        {
            if (Directory.Exists(pathString))
            {
                Directory.Delete(pathString);
                return;
            }
        }
        private void RenameDirectory(string dirName, string pathString)
        {
            string newPathString = Path.Combine(pD.FullName, dirName);
            if (Directory.Exists(newPathString))
            {
                Debug.WriteLine("Directory Already present");
                return;
            }
            Directory.Move(pathString, newPathString);
        }
        private void RenameFile(string fileName, string pathString)
        {
            string newPathString = Path.Combine(pD.FullName, fileName);
            if (File.Exists(newPathString))
            {
                Debug.WriteLine("File Name Already Present");
                return;
            }
            File.Move(pathString, newPathString);
        }


        private void ReadOutControls()
        {
            fmp.ReadOutControls(B.IsEnabled, N.IsEnabled, U.IsEnabled, D.IsEnabled, F.IsEnabled, F.Content.ToString());
        }
        private void ReadOutFilters()
        {
            Speak_Listen.StartPromptBuilder();
            for (int i = 1; i < 12;i++)
            {
                string s = ((ComboBoxItem)(DropDown.Items[i - 1])).Content.ToString();
                Speak_Listen.AddPrompt("Say "+ (i.ToString()) + " to filter " + s);
            }
            Speak_Listen.AddPrompt("Say Repeat to repeat the list");
            Speak_Listen.SpeakPrompt();
        }


        public void Instruct(string cmd)
        {
            Debug.WriteLine("File Manager Input:  " + cmd);
            if (fmp != null)
            {
                firstItemNo = fmp.GetFirstItemNo();
                lastItemNo = fmp.GetLastItemNo();
            }

            TB1.Text = "";
            TB1.IsEnabled = false;
            BORDER.BorderBrush = Brushes.Gray;
            Speak_Listen.Speak("");                                 // Tricky way to interrupt the file manager Voice Output

            /* 
             *  READ THIS CAREFULLY BEFORE YOU ADD NEW case in switch
             *  =====================================================
             *  
             *  NOTE 1:- Add this function at the start of your case(if it has nothing to do with variable "isFilterActive"):
             *  ------
             *              UpdateFilter(false);
             */

            switch (cmd)
            {
                case "Back":
                    UpdateFilter(false);
                    if (B.IsEnabled)
                        Back();         
                    else
                        Speak_Listen.Speak("Sorry, No such control is present");
                    break;
                case "Next":
                    UpdateFilter(false);
                    if (N.IsEnabled)
                        Next(); 
                    else
                        Speak_Listen.Speak("Sorry, No such control is present");
                    break;
                case "Up":
                    UpdateFilter(false);
                    if (U.IsEnabled)
                        Up();
                    else
                        Speak_Listen.Speak("Sorry, No such control is present");
                    break;
                case "Down":
                    UpdateFilter(false);
                    if (D.IsEnabled)
                        Down();
                    else
                        Speak_Listen.Speak("Sorry, No such control is present");
                    break;
                case "Files":
                case "Folders":
                    UpdateFilter(false);
                    if (F.IsEnabled)
                    {
                        if (cmd == F.Content.ToString())                    // If this is the same content as wrtten on the Button F.
                            FileOrFolder();
                        else
                            fmp.ReadOutListItems();
                    }
                    else
                        Speak_Listen.Speak("Sorry, No such control is present");
                    break;
                case "Refresh":
                    UpdateFilter(false);
                    Refresh();
                    break;
                case "Read Out":
                    if (isFilterActive)
                    {
                        ReadOutFilters();
                        firstItemNo = 1;            // For Dynamic Loading of Grammar in "Speak_Listen" class
                        lastItemNo = 11;            // For Dynamic Loading of Grammar in "Speak_Listen" class
                    }
                    else
                        fmp.ReadOutListItems();
                    break;
                case "Controls":
                    UpdateFilter(false);
                    ReadOutControls();
                    break;
                case "Filter":
                    if (pD == null)                          // If this is the home page of the file manager
                        Speak_Listen.Speak("Sorry, No such control is present");
                    else
                    {
                        UpdateFilter(true);
                        ReadOutFilters();
                        firstItemNo = 1;                    // Use to dynamically load grammar in Speak_Listen class
                        lastItemNo = 11;                    // Use to dynamically load grammar in Speak_Listen class
                    }
                    break;
                case "New Folder":
                    UpdateFilter(false);
                    isNewFolder = true;
                    if (NF1.IsEnabled)
                    {
                        BORDER.BorderBrush = Brushes.Yellow;
                        Speak_Listen.Speak("Type the name of the new folder.");
                        TB1.IsEnabled = true;
                        TB1.Focus();
                    }
                    else
                        Speak_Listen.Speak("Sorry, No such control is present");
                    break;
                case "New File":
                    UpdateFilter(false);
                    isNewFolder = false;
                    if (NF2.IsEnabled)
                    {
                        BORDER.BorderBrush = Brushes.Yellow;
                        Speak_Listen.Speak("Type the name of the new file.");
                        TB1.IsEnabled = true;
                        TB1.Focus();
                    }
                    else
                        Speak_Listen.Speak("Sorry, No such control is present");
                    break;
                case "Delete Folder":
                    UpdateFilter(false);
                    int sel = fmp.GetSelectedItemNo() - 1 + firstItemNo;
                    if (sel >= firstItemNo && sel <= lastItemNo)
                    {

                    }
                    else
                    {
                        Speak_Listen.Speak("You haven't selected any item.");
                    }
                    break;
                case "Delete File":
                    UpdateFilter(false);
                    sel = fmp.GetSelectedItemNo() - 1 + firstItemNo;
                    if (sel >= firstItemNo && sel <= lastItemNo)
                    {

                    }
                    else
                    {
                        Speak_Listen.Speak("You haven't selected any item.");
                    }
                    break;
                case "Rename Folder":
                    UpdateFilter(false);
                    sel = fmp.GetSelectedItemNo() - 1 + firstItemNo;
                    if (sel >= firstItemNo && sel <= lastItemNo)
                    {

                    }
                    else
                    {
                        Speak_Listen.Speak("You haven't selected any item.");
                    }
                    break;
                case "Rename File":
                    UpdateFilter(false);
                    sel = fmp.GetSelectedItemNo() - 1 + firstItemNo;
                    if (sel >= firstItemNo && sel <= lastItemNo)
                    {

                    }
                    else
                    {
                        Speak_Listen.Speak("You haven't selected any item.");
                    }
                    break;
                default:                                     //  Number input
                    int n = int.Parse(cmd);
                    if (isFilterActive)                      //  Number input for filters
                    {
                        if (n>=1 && n <= 11)                 // In range
                        {
                            UpdateFilter(false);
                            Filter(n);
                        }
                        else                                 // Out of range
                        {
                            Speak_Listen.Speak("Wrong option. Say a number from 1 to 11.");
                        }
                    }
                    else                                     //  Number input for file/folder selection
                    {
                        if (isFolder == 0)
                        {
                            Speak_Listen.Speak("Sorry, this folder is empty");
                        }
                        else
                        {
                            if ((n<firstItemNo) || (n>lastItemNo))          // Item no. selected is not actually present in the list
                            {
                                Speak_Listen.Speak("Wrong option. Say a number from "+ firstItemNo.ToString() + " to " + lastItemNo.ToString());
                            }
                            else                                            // In Range
                            {
                                Open(((n - 1) % 10) + 1);
                            }
                        }
                    }
                    break;
            }
        }

    }

    
}