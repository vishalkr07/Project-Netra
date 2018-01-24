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


namespace ProjectNetra
{
    /// <summary>
    /// Interaction logic for File_Manager_Page.xaml
    /// </summary>
    public partial class File_Manager_Page : Page
    {
        private List<Folders> items = new List<Folders>();       // Used to display in list for GUI 
        private List<string> dirs = new List<string>();          // Used to dictate the list to the users
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
            items.Add(new Folders() { Folder = "Repeat" });

            dirs.Add("Documents");
            dirs.Add("Desktop");
            dirs.Add("Downloads");
            dirs.Add("Music");
            dirs.Add("Videos");
            dirs.Add("Repeat");

            dict["Documents"] = new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments));
            dict["Desktop"] = new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.Desktop));
            dict["Downloads"] = new DirectoryInfo(Path.Combine(Environment.GetEnvironmentVariable("USERPROFILE"), "Downloads"));
            dict["Music"] = new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.MyMusic));
            dict["Videos"] = new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.MyVideos));
            dict["Repeat"] = null;

            LB.ItemsSource = items;

        }
        public File_Manager_Page(DirectoryInfo dir)                        // To be invoked for getting the contents within a Folder/Drive
        {
            InitializeComponent();
            
            FileInfo[] files = null;
            DirectoryInfo[] subDirs = null;

            /************************  Retrieve Subfolders of Foder "dir"  **************************/
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
                    if (HasPermission(subDir.FullName))
                    {
                        items.Add(new Folders() { Folder = subDirName });
                        dirs.Add(subDirName);
                        dict[subDirName] = subDir;
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
                Debug.WriteLine("Some Error occured in retrieving subfolders of "+ dir.FullName);
            }
            

            /***************************  Finished Retrieving Subfolders  ***************************/

            /***************************  Retrieve Files in Folder "dir"  ***************************/

            try
            {
                files = dir.GetFiles("*.*");
                foreach (FileInfo file in files)
                {
                    string fileName;
                    try
                    {
                        fileName = file.Name;
                    }
                    catch (FileNotFoundException e)             // If file was deleted by a separate application or thread till we reach here.
                    {
                        Debug.WriteLine("Some file has been deleted : " + e.Message);
                        continue;
                    }
                    if (HasPermission(file.FullName))
                    {
                        items.Add(new Folders() { Folder = fileName });
                        dirs.Add(fileName);
                        dict[fileName] = null;
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
            

            /************************  Finished Retrieving Files  ********************************/

            LB.ItemsSource = items;

        }

        public DirectoryInfo GetSelectedItem()                      // Gives information about the selected item in the current page.
        {
            if (LB.SelectedItem == null)
                return null;
            return dict[(LB.SelectedItem as Folders).Folder];
        }


        private bool HasPermission(string path)
        {
            var permission = new FileIOPermission(FileIOPermissionAccess.Read, path);
            var permissionSet = new PermissionSet(PermissionState.None);
            permissionSet.AddPermission(permission);
            if (permissionSet.IsSubsetOf(AppDomain.CurrentDomain.PermissionSet))        // Checking if the folder has access permissions
                return true;
            return false;
        }

        public void ReadOutListItems()
        {
            //Read The Current List.
        }

    }
    public class Folders
    {
        public string Folder { get; set; }
    }
}
