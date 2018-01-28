using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.IO;
using System.Diagnostics;
using System.Security;
using System.Security.Permissions;
using System.Security.AccessControl;
using System.Security.Principal;


namespace ProjectNetra
{
    /// <summary>
    /// Interaction logic for File_Manager_Page.xaml
    /// </summary>
    public partial class File_Manager_Page : Page, IDisposable
    {
        /* Constraint:  |items| <=10 ; We split the list of folders/files within a directory into set of maximum 10 elements  */
        private ObservableCollection<Folders> items = new ObservableCollection<Folders>();                  // Used to display Files/Folders in list for GUI 
        private List<FoderDetails> details1 = new List<FoderDetails>();                                     // Used to display details in the GUI StackPanel1
        private List<FoderDetails> details2 = new List<FoderDetails>();                                     // Used to display details in the GUI StackPanel2
        private List<string> dirs = new List<string>();                                                     // Used to dictate the list of directories to the users
        private List<string> fileList = new List<string>();                                                 // Used to dictate the list of files to the users
        private Dictionary<string, DirectoryInfo> dictFolder = new Dictionary<string, DirectoryInfo>();     // Used to reference to the directory that the user selects 
        private Dictionary<string, FileInfo> dictFiles = new Dictionary<string, FileInfo>();                // Used to reference to the files that the user selects 
        private short isFolder = 1;                                                                         // Will have 3 values: 0 -> none, 1->folder, 2->file
        private int firstItemNo = 1,lastItemNo = 0;                                                         // Tracks the first and last item no. of the set of items currently being displayed in GUI 

        public File_Manager_Page()                                      // To be invoked only for getting the Drives and Special Folders
        {
            InitializeComponent();
            
            string[] drives = Environment.GetLogicalDrives();

            foreach (string dr in drives)
            {
                
                DriveInfo di = new DriveInfo(dr);
                if (!di.IsReady)
                {
                    Debug.WriteLine("The drive {0} could not be read", di.Name);
                    continue;
                }
                DirectoryInfo rootDir = di.RootDirectory;
                                
                items.Add(new Folders() { Folder = dr, IconPath = "local_disk.ico" });
                dirs.Add(dr);
                dictFolder[dr] = rootDir;
            }

            items.Add(new Folders() { Folder = "Documents", IconPath = "folder_icon.png"});
            items.Add(new Folders() { Folder = "Desktop", IconPath = "folder_icon.png" });
            items.Add(new Folders() { Folder = "Downloads", IconPath = "folder_icon.png" });
            items.Add(new Folders() { Folder = "Music", IconPath = "folder_icon.png" });
            items.Add(new Folders() { Folder = "Videos", IconPath = "folder_icon.png" });

            dirs.Add("Documents");
            dirs.Add("Desktop");
            dirs.Add("Downloads");
            dirs.Add("Music");
            dirs.Add("Videos");

            dictFolder["Documents"] = new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments));
            dictFolder["Desktop"] = new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.Desktop));
            dictFolder["Downloads"] = new DirectoryInfo(Path.Combine(Environment.GetEnvironmentVariable("USERPROFILE"), "Downloads"));
            dictFolder["Music"] = new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.MyMusic));
            dictFolder["Videos"] = new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.MyVideos));

            details1.Add(new FoderDetails() { FFNo = "Drives : " + (items.Count - 5).ToString(), IconPath = "local_disk.ico" });
            details2.Add(new FoderDetails() { FFNo = "Folders : 5", IconPath = "folder_icon.png" });

            LB.ItemsSource = items;
            SP1.DataContext = details1;
            SP2.DataContext = details2;

            lastItemNo = dirs.Count();
            firstItemNo = Math.Min(firstItemNo,lastItemNo);
        }

        public File_Manager_Page(DirectoryInfo dir)                        // To be invoked for getting the contents within a Folder/Drive
        {
            InitializeComponent();

            RetrieveSubfolders(dir);
            RetrieveFiles(dir);

            details1.Add(new FoderDetails() { FFNo = "Folders : " + dirs.Count.ToString(), IconPath = "folder_icon.png" });
            details2.Add(new FoderDetails() { FFNo = "Files : " + fileList.Count.ToString(), IconPath = "file_icon.png" });
            GetInitialItems();
       
            LB.ItemsSource = items;
            SP1.DataContext = details1;
            SP2.DataContext = details2;
        }

        public void Dispose()                                    // For memory management
        {
            items.Clear();
            details1.Clear();
            details2.Clear();
            dirs.Clear();            
            fileList.Clear();
            dictFolder.Clear();
            dictFiles.Clear();
        }
        public short GetFolderStatus()
        {
            return isFolder;
        }
        public void SetFolderStatus(bool isfolder)
        {
            isFolder = (isfolder ? (short)1 : (short)2);
            firstItemNo = 11;
            MoveWithinList(true);
        }
        public int GetNoOfFolders()
        {
            return dirs.Count;
        }
        public int GetNoOfFiles()
        {
            return fileList.Count;
        }
        public int GetFirstItemNo()
        {
            return firstItemNo;
        }
        public void GetInitialItems()                               // Gets the first set of items in the folder
        {
            if (dirs.Count != 0)
            {
                lastItemNo = Math.Min(firstItemNo + 9, dirs.Count);
                for (int i = firstItemNo; i <= lastItemNo; i++)
                {
                    items.Add(new Folders() { Folder = dirs[i - 1], IconPath = "folder_icon.png" });
                }
                isFolder = 1;
            }
            else if (fileList.Count != 0)
            {
                lastItemNo = Math.Min(firstItemNo + 9, fileList.Count);
                for (int i = firstItemNo; i <= lastItemNo; i++)
                {
                    items.Add(new Folders() { Folder = fileList[i - 1], IconPath = "file_icon.png" });
                }
                isFolder = 2;
            }
            else
            {
                isFolder = 0;
            }
        }

        /* The following function MoveWithinList(bool) is used to move up or down a list of (files/folders) within a given directory.
         * bool Up ---------> true, if we are to move up the list(i.e show previous 10 items) and false otherwise.
         */
        public void MoveWithinList(bool up)          
        {
            items.Clear();
            if (up)                                                 // Move Up
            {
                if ((isFolder==2) && (firstItemNo == 1))                // Check if this is the first set of files
                {
                    isFolder = 1;
                    firstItemNo = ((dirs.Count-1)/10) *10 + 1;
                }
                else
                    firstItemNo -= 10;
            }
            else                                                    // Move Down
            {
                if((isFolder==1) && (firstItemNo + 10 > dirs.Count))   // Check if this is the last set of folders
                {
                    isFolder = 2;
                    firstItemNo = 1;
                }
                else
                    firstItemNo += 10;
            }
            if(isFolder == 1)                                       // Check if the current set of items to be displayed is a set of folders
            {
                lastItemNo = Math.Min(firstItemNo + 9, dirs.Count);
                for (int i = firstItemNo; i <= lastItemNo; i++)
                {
                    items.Add(new Folders() { Folder = dirs[i - 1], IconPath = "folder_icon.png" });
                }
            }
            else                                                    // Check if the current set of items to be displayed is a set of files
            {
                lastItemNo = Math.Min(firstItemNo + 9, fileList.Count);
                for (int i = firstItemNo; i <= lastItemNo; i++)
                {
                    items.Add(new Folders() { Folder = fileList[i - 1], IconPath = "file_icon.png" });
                }
            }
            
        }
        
        private void RetrieveSubfolders(DirectoryInfo dir)
        {
            DirectoryInfo[] subDirs = null;
            try
            {
                subDirs = dir.GetDirectories();
                foreach (DirectoryInfo subDir in subDirs)
                {
                    string subDirName;
                    try
                    {
                        subDirName = subDir.Name;
                    }
                    catch (DirectoryNotFoundException e)       // If folder was deleted by a separate application or thread till we reach here.
                    {
                        Debug.WriteLine("Some folder has been deleted : " + e.Message);
                        continue;
                    }
                    try
                    {
                        DirectoryInfo[] subSubDirs = subDir.GetDirectories();
                        dirs.Add(subDirName);
                        dictFolder[subDirName] = subDir;
                    }
                    catch (Exception e)                         // "subDir" having no access permission is ignored in the list
                    {
                        Debug.WriteLine(e.Message);
                    }
                }

            }
            catch (UnauthorizedAccessException e)          // Exception thrown if we do not have discovery permission on a folder or file.
            {
                Debug.WriteLine("Access Denied : " + e.Message);
            }
            catch (DirectoryNotFoundException e)          // This will happen if "dir" has been deleted by another application or thread.
            {
                Debug.WriteLine("Folder not found : " + e.Message);
            }
            catch (Exception e)                           // Any other errors
            {
                Debug.WriteLine("Some Error occured in retrieving subfolders of " + dir.FullName);
            }

        }
        private void RetrieveFiles(DirectoryInfo dir)
        {
            FileInfo[] files = null;
            try
            {
                files = dir.GetFiles("*.*");
                foreach (FileInfo file in files)
                {
                    try
                    {
                        string fileName = file.Name;
                        if (fileName == "desktop.ini")        // Ignore all "desktop.ini" files
                            continue;
                        fileList.Add(fileName);
                        dictFolder[fileName] = null;
                        dictFiles[fileName] = file;
                    }
                    catch (FileNotFoundException e)             // If file was deleted by a separate application or thread till we reach here.
                    {
                        Debug.WriteLine("Some file has been deleted : " + e.Message);
                    }

                }
            }
            catch (UnauthorizedAccessException e)          // Exception thrown if we do not have discovery permission on a folder or file.
            {
                Debug.WriteLine("Access Denied : " + e.Message);
            }
            catch (DirectoryNotFoundException e)          // This will happen if "dir" has been deleted by another application or thread.
            {
                Debug.WriteLine("Folder not found : " + e.Message);
            }
            catch (Exception e)                           // Any other errors
            {
                Debug.WriteLine("Some Error occured in retrieving files of " + dir.FullName);
            }

        }

        // GetSelectedFolder() gives information about the selected folder in the current page or returns null if the selected item is not a folder
        public DirectoryInfo GetSelectedFolder()                
        {
            if (LB.SelectedItem == null || isFolder==2)
                return null;
            return dictFolder[(LB.SelectedItem as Folders).Folder];
        }

        // GetSelectedFile() gives information about the selected file in the current page or returns null if the selected item is not a file
        public FileInfo GetSelectedFile()                      
        {
            if (LB.SelectedItem == null)
                return null;
            return dictFiles[(LB.SelectedItem as Folders).Folder];
        }
        
        public void ReadOutListItems(bool isBack, bool isNext, bool isUp, bool isDown, bool isFF , string ff)
        {
            Speak_Listen.StartPromptBuilder();
            if (isFolder==0)
                Speak_Listen.AddPrompt("Sorry, this folder is empty");
            else
            {
                if(firstItemNo == 1)
                {
                    if(isBack)                             
                        Speak_Listen.AddPrompt("There is a total of " + dirs.Count.ToString() + " folders and " + fileList.Count.ToString() + " files in this directory.");
                    else      // Indicates "This PC" part
                        Speak_Listen.AddPrompt("There is a total of " + (dirs.Count - 5).ToString() + " drives and 5 special folders in your Computer.");
                }
                if (isFolder == 1)
                {
                    if(isBack)      
                        Speak_Listen.AddPrompt("Here is the list of folders from " + firstItemNo.ToString() + " to " + lastItemNo.ToString());
                }
                else
                    Speak_Listen.AddPrompt("Here is the list of files from " + firstItemNo.ToString() + " to " + lastItemNo.ToString());

                int len = items.Count;
                for(int i = 1; i<=len; i++)
                {
                    Speak_Listen.AddPrompt("Number " + i.ToString() + ", " + items[i-1].Folder);
                }
            }
            if (isBack)
                Speak_Listen.AddPrompt("Say Back to go back");
            if (isNext)
                Speak_Listen.AddPrompt("Say Next to go forward");
            if (isUp)
                Speak_Listen.AddPrompt("Say Up to move up the list");
            if (isDown)
                Speak_Listen.AddPrompt("Say Down to move down the list");
            if (isFF)
                Speak_Listen.AddPrompt("Say " + ff + " to dictate the " + ff);
            
            Speak_Listen.AddPrompt("Say Repeat to repeat the list");
            Speak_Listen.SpeakPrompt();
        }
        
    }


    public class Folders
    {
        public string Folder { get; set; }
        public string IconPath { get; set; }
    }

    public class FoderDetails
    {
        public string FFNo { get; set; }
        public string IconPath { get; set; }
    }
}
