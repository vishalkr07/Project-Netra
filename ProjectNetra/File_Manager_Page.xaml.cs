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
        private Dictionary<string, DirectoryInfo> dict = new Dictionary<string, DirectoryInfo>();  // Used to reference to the directory that the user selects 

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
                                
                items.Add(new Folders() { Folder = dr });
                dirs.Add(dr);
                dict[dr] = rootDir;
            }
            
            items.Add(new Folders() { Folder = "Documents" });
            items.Add(new Folders() { Folder = "Desktop" });
            items.Add(new Folders() { Folder = "Downloads" });
            items.Add(new Folders() { Folder = "Music" });
            items.Add(new Folders() { Folder = "Videos" });

            dirs.Add("Documents");
            dirs.Add("Desktop");
            dirs.Add("Downloads");
            dirs.Add("Music");
            dirs.Add("Videos");

            dict["Documents"] = new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments));
            dict["Desktop"] = new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.Desktop));
            dict["Downloads"] = new DirectoryInfo(Path.Combine(Environment.GetEnvironmentVariable("USERPROFILE"), "Downloads"));
            dict["Music"] = new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.MyMusic));
            dict["Videos"] = new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.MyVideos));

            LB.ItemsSource = items;

        }

        public File_Manager_Page(DirectoryInfo dir)                        // To be invoked for getting the contents within a Folder/Drive
        {
            InitializeComponent();

            RetrieveSubfolders(dir);
            RetrieveFiles(dir);

            LB.ItemsSource = items;
        }

        public void Dispose()
        {
            items.Clear();
            dirs.Clear();
            dict.Clear();
            fileList.Clear();
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
                        items.Add(new Folders() { Folder = subDirName });
                        dirs.Add(subDirName);
                        dict[subDirName] = subDir;
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
                        items.Add(new Folders() { Folder = fileName });
                        fileList.Add(fileName);
                        dict[fileName] = null;
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


        public DirectoryInfo GetSelectedItem()                      // Gives information about the selected item in the current page.
        {
            if (LB.SelectedItem == null)
                return null;
            return dict[(LB.SelectedItem as Folders).Folder];
        }

        
        public void ReadOutListItems(bool isBack, bool isNext)
        {
            int i = 1;
            Speak_Listen.Speak("Here is the List of Folders");
            foreach(string dir in dirs)
            {
                Speak_Listen.Speak("Number " +i.ToString() +","+ dir);
                i++;
            }
            Speak_Listen.Speak("Here is the List of Files");
            foreach (string f in fileList)
            {
                Speak_Listen.Speak("Number " + i.ToString() + "," + f);
                i++;
            }
            // Wait for sometime
            if (isBack)
                Speak_Listen.Speak("Say Back to go back");
            if (isNext)
                Speak_Listen.Speak("Say Next to go forward");
            Speak_Listen.Speak("Say Repeat to repeat the list");
        }

    }
    public class Folders
    {
        public string Folder { get; set; }
    }
}
