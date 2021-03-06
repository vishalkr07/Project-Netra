﻿using System;
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
        private List<char> notAllowedCharacters = new List<char>(new char[] { '/','\\','"','|',':','?','<','>','*' });
        private Dictionary<string, int> cbItem = new Dictionary<string, int>();             // For retireving comboboxitem no. for a specific comboboxitem
        private File_Manager_Page fmp = null; 
        private LinkedList<File_Manager_Page> ll = new LinkedList<File_Manager_Page>();
        private LinkedListNode<File_Manager_Page> llnode = null,temp = null, nxt = null;
        private short isFolder = 0;                                                         // Will have 3 values: 0 -> none, 1->folder, 2->file
        private int firstItemNo = 1,lastItemNo = 0;                                         // Tracks the first and last item no. of the set of items currently being displayed in GUI 
        private int noOfFiles = 0, noOfFolders = 0;                                         // Tracks the no. of files/folders in the current directory 
        private int noOfItems = 0;                                                          // Tracks no. of items displayed in the GUI
        private DirectoryInfo pD = null;
        private bool isFilterActive = false;
        private short modifyListStatus = 0;                                                 // modifyListStatus = 0(None),1(New Folder),2(New File),3(Delete),4(Rename)
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
        private bool Valid(string s)
        {
            if (s.Trim(new char[] { ' ' }).Length == 0)
                return false;
            foreach(var c in notAllowedCharacters)
            {
                if (s.Contains(c))
                    return false;
            }
            return true;
        }
        private bool ValidateWithAcknowlwdgement(string s,bool fo)                             // s => name of file/folder; (f == true) => folder, else file
        {
            string item = (fo?"folder":"file");
            if (!Valid(s))
            {
                Speak_Listen.StartPromptBuilder();
                Speak_Listen.AddPrompt("A "+item+" name can't contain any of these characters:");
                Speak_Listen.AddPrompt("Forward slash, Backslash, Pipe, Quote, Colon, Question Mark, Less Than, Greater Than, Asterisk");
                Speak_Listen.AddPrompt("A "+ item + " can't contain only spaces. There has to be atleast one allowed character with space.");
                Speak_Listen.AddPrompt("Please type again");
                Speak_Listen.SpeakPrompt();
                TB1.Text = "";
                return false;
            }
            if (fo)
                return true;
            // Regular Expression (for File Format): Any name with atleast one letter in [a-z,A-Z,0-9] before "." and atleast one letter in [a-z,A-Z,0-9] after "." 
            Regex regex = new Regex(@"^[a-zA-Z0-9]+\.[a-zA-Z0-9]+$");
            if (!regex.IsMatch(s))
            {
                Speak_Listen.Speak("File name can't be without an extension.");
                TB1.Text = "";
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
            NF1.IsEnabled = NF2.IsEnabled = DF1.IsEnabled = RNF1.IsEnabled  = (pD!=null);
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
            Instruct("Open");
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
                    if (modifyListStatus == 1)
                        CreateDirectory(TB1.Text);
                    else if(modifyListStatus == 2)
                        CreateFile(TB1.Text);
                    else if(modifyListStatus == 4)
                    {
                        if (isFolder == 1)
                            RenameDirectory(TB1.Text,fmp.GetSelectedFolder(fmp.GetSelectedItemNo()).Name);
                        else                                // isFolder == 2 ; isFolder==0 not possible to reach here
                            RenameFile(TB1.Text, fmp.GetSelectedFile(fmp.GetSelectedItemNo()).Name);
                    }

                }
                else 
                    Instruct("Open");
                
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
        private void ButtonDeleteBtn(object sender, RoutedEventArgs e)
        {
            Instruct("Delete");
        }

        private void ButtonRenameBtn(object sender, RoutedEventArgs e)
        {
            Instruct("Rename");
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
            string s = ((ComboBoxItem)(DropDown.Items[index - 1])).Content.ToString();
            fmp.Filter(s);
            UpdateMembers(false);
            Speak_Listen.Speak("Filter is set to "+s);
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
            if (!ValidateWithAcknowlwdgement(dirName, true))
                return;
            string pathString = Path.Combine(pD.FullName, dirName);
            if (Directory.Exists(pathString))
            {
                Debug.WriteLine("Directory Already Present");
                Speak_Listen.Speak("Sorry,a folder named " + dirName + " already exists. Please type again.");
                TB1.Text = "";
                return;
            }
            Directory.CreateDirectory(pathString);
            Refresh();
            TB1.Text = "";
            BORDER.BorderBrush = Brushes.Gray;
            TB1.IsEnabled = false;
            Speak_Listen.Speak("A new folder named "+dirName+" is created successfully.");
            modifyListStatus = 0;
        }
        private void CreateFile(string fileName)
        {
            if (!ValidateWithAcknowlwdgement(fileName, false))
                return;
            string pathString = Path.Combine(pD.FullName, fileName);
            if (File.Exists(pathString))
            {
                Debug.WriteLine("File Already Present");
                Speak_Listen.Speak("Sorry, a file named "+fileName+" already exists. Please type again.");
                TB1.Text = "";
                return;
            }
            File.Create(pathString);
            Refresh();
            TB1.Text = "";
            BORDER.BorderBrush = Brushes.Gray;
            TB1.IsEnabled = false;
            Speak_Listen.Speak("A new file named " + fileName + " is created successfully.");
            modifyListStatus = 0;
        }
        private void DeleteFile(string fileName)
        {
            string pathString = Path.Combine(pD.FullName, fileName);
            if (File.Exists(pathString))
            {
                File.Delete(pathString);
                Refresh();
                Speak_Listen.Speak("The file named " + fileName + " is deleted successfully");
                return;
            }
            Speak_Listen.Speak("Sorry, the file has been removed by an unknown source.");
            modifyListStatus = 0;
        }
        private void DeleteDirectory(string dirName)
        {
            string pathString = Path.Combine(pD.FullName, dirName);
            if (Directory.Exists(pathString))
            {
                Directory.Delete(pathString);
                Refresh();
                Speak_Listen.Speak("The folder named "+dirName+" is deleted successfully");
                return;
            }
            Speak_Listen.Speak("Sorry, the folder has been removed by an unknown source.");
            modifyListStatus = 0;
        }
        private void RenameDirectory(string newDirName, string oldDirName)
        {
            if (!ValidateWithAcknowlwdgement(newDirName, true))
                return;
            string newPathString = Path.Combine(pD.FullName, newDirName);
            string oldPathString = Path.Combine(pD.FullName, oldDirName);
            if (Directory.Exists(newPathString))
            {
                Debug.WriteLine("Directory Already present");
                Speak_Listen.Speak("A Folder named "+newDirName+" already exists. Please type again.");
                TB1.Text = "";
                return;
            }
            Directory.Move(oldPathString, newPathString);
            Refresh();
            TB1.Text = "";
            BORDER.BorderBrush = Brushes.Gray;
            TB1.IsEnabled = false;
            Speak_Listen.Speak("The folder "+oldDirName+ " is renamed to "+newDirName+" successfully.");
            modifyListStatus = 0;
        }
        private void RenameFile(string newFileName, string oldFileName)
        {
            if (!ValidateWithAcknowlwdgement(newFileName, false))
                return;
            string newPathString = Path.Combine(pD.FullName, newFileName);
            string oldPathString = Path.Combine(pD.FullName, oldFileName);
            if (File.Exists(newPathString))
            {
                Debug.WriteLine("File Name Already Present");
                Speak_Listen.Speak("A Folder named " + newFileName + " already exists. Please type again.");
                TB1.Text = "";
                return;
            }
            File.Move(oldPathString, newPathString);
            Refresh();
            TB1.Text = "";
            BORDER.BorderBrush = Brushes.Gray;
            TB1.IsEnabled = false;
            Speak_Listen.Speak("The file " + oldFileName + " is renamed to " + newFileName + " successfully.");
            modifyListStatus = 0;
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
                    if (NF1.IsEnabled)
                    {
                        modifyListStatus = 1;
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
                    if (NF2.IsEnabled)
                    {
                        modifyListStatus = 2;
                        BORDER.BorderBrush = Brushes.Yellow;
                        Speak_Listen.Speak("Type the name of the new file.");
                        TB1.IsEnabled = true;
                        TB1.Focus();
                    }
                    else
                        Speak_Listen.Speak("Sorry, No such control is present");
                    break;
                case "Delete":
                    UpdateFilter(false);
                    if (DF1.IsEnabled)
                    {
                        int sel = fmp.GetSelectedItemNo();
                        if (sel == 0)
                            Speak_Listen.Speak("You haven't selected any item.");
                        else
                        {
                            modifyListStatus = 3;
                            if (isFolder == 1)
                                DeleteDirectory(fmp.GetSelectedFolder(sel).Name);
                            else                                // isFolder == 2; isFolder == 0 not possible to reach here
                                DeleteFile(fmp.GetSelectedFile(sel).Name);
                        }
                    }
                    else
                        Speak_Listen.Speak("Sorry, No such control is present");
                    
                    break;
                case "Rename":
                    UpdateFilter(false);
                    if (RNF1.IsEnabled)
                    {
                        int sel = fmp.GetSelectedItemNo();
                        if (sel == 0)
                            Speak_Listen.Speak("You haven't selected any item.");
                        else
                        {
                            modifyListStatus = 4;
                            BORDER.BorderBrush = Brushes.Yellow;
                            Speak_Listen.Speak("Type the new name of the item selected");
                            TB1.IsEnabled = true;
                            TB1.Focus();
                        }
                    }
                    else
                        Speak_Listen.Speak("Sorry, No such control is present");
                    break;
                case "Open":
                    UpdateFilter(false);
                    Open(fmp.GetSelectedItemNo());
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
                                fmp.SelectItem(n);
                            }
                        }
                    }
                    break;
            }
        }

    }

    
}