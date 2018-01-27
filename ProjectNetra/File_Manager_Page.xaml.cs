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
        private List<Folders> items = new List<Folders>();       // Used to display in list for GUI 
        private List<string> dirs = new List<string>();          // Used to dictate the list of directories to the users
        private List<string> fileList = new List<string>();         // Used to dictate the list of files to the users
        private Dictionary<string, DirectoryInfo> dictFolder = new Dictionary<string, DirectoryInfo>();  // Used to reference to the directory that the user selects 
        private Dictionary<string, FileInfo> dictFiles = new Dictionary<string, FileInfo>(); // Used to reference to the files that the user selects 
        private int foldersCount = 0;

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

            LB.ItemsSource = items;

        }

        public File_Manager_Page(DirectoryInfo dir)                        // To be invoked for getting the contents within a Folder/Drive
        {
            InitializeComponent();

            RetrieveSubfolders(dir);
            RetrieveFiles(dir);

            LB.ItemsSource = items;
        }

        public void Dispose()                                    // For memory management
        {
            items.Clear();
            dirs.Clear();
            dictFolder.Clear();
            fileList.Clear();
            dictFiles.Clear();
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
                        items.Add(new Folders() { Folder = subDirName, IconPath = "folder_icon.png" });
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
                        items.Add(new Folders() { Folder = fileName, IconPath = "file_icon.png" });
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


        public DirectoryInfo GetSelectedFolder()                // Gives information about the selected folder in the current page.
        {
            if (LB.SelectedItem == null || LB.SelectedIndex >= foldersCount)
                return null;
            return dictFolder[(LB.SelectedItem as Folders).Folder];
        }
        public FileInfo GetSelectedFile()                      // Gives information about the selected file in the current page.
        {
            if (LB.SelectedItem == null)
                return null;
            return dictFiles[(LB.SelectedItem as Folders).Folder];
        }


        public void ReadOutListItems(bool isBack, bool isNext)
        {
            int i = 1;
            Speak_Listen.StartPromptBuilder();
            if(dirs.Count!=0)
                Speak_Listen.AddPrompt("Here is the List of Folders");
            foreach(string dir in dirs)
            {
                Speak_Listen.AddPrompt("Number " +i.ToString() +","+ dir);
                i++;
            }
            foldersCount = i;
            if(fileList.Count!=0)
                Speak_Listen.AddPrompt("Here is the List of Files");
            foreach (string f in fileList)
            {
                Speak_Listen.AddPrompt("Number " + i.ToString() + "," + f);
                i++;
            }
            if (dirs.Count == 0 && fileList.Count == 0)
                Speak_Listen.AddPrompt("Sorry, this folder is empty");
            if (isBack)
                Speak_Listen.AddPrompt("Say Back to go back");
            if (isNext)
                Speak_Listen.AddPrompt("Say Next to go forward");
            Speak_Listen.AddPrompt("Say Repeat to repeat the list");
            Speak_Listen.SpeakPrompt();
        }
        
    }


    public class Folders
    {
        public string Folder { get; set; }
        public string IconPath { get; set; }
    }
}
