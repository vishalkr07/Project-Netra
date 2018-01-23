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

namespace ProjectNetra
{
    /// <summary>
    /// Interaction logic for File_Manager.xaml
    /// </summary>
    public partial class File_Manager : Window
    {
        private static List<Folders> items = null;
        private static List<string> dirs = null;
        public File_Manager()
        {
            InitializeComponent();

            items = new List<Folders>();
            dirs = new List<string>();

            string[] drives = Environment.GetLogicalDrives();

            foreach (string dr in drives)
            {
                dirs.Add(dr);
                items.Add(new Folders() { Folder = dr });
            }

            items.Add(new Folders() { Folder = "Documents" });
            items.Add(new Folders() { Folder = "Desktop" });
            items.Add(new Folders() { Folder = "Downloads" });
            items.Add(new Folders() { Folder = "Music" });
            items.Add(new Folders() { Folder = "Videos" });
            items.Add(new Folders() { Folder = "Repeat" });

            LB.ItemsSource = items;
        }

        public void OnContentLoaded(object src, EventArgs e)
        {

            Test.Speak("Here is the list of directories");
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