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
        private File_Manager_Page fmp = null,temp = null,nxt = null;
        private Dictionary<File_Manager_Page,File_Manager_Page> next = new Dictionary<File_Manager_Page, File_Manager_Page>();
        private Dictionary<File_Manager_Page,File_Manager_Page> back = new Dictionary<File_Manager_Page, File_Manager_Page>();

        public File_Manager()
        {
            InitializeComponent();
            fmp = new File_Manager_Page();
            back[fmp] = null;
            next[fmp] = null;
            MainFrame.Navigate(fmp);
            B.IsEnabled = false;
            N.IsEnabled = false;
            fmp.ReadOutListItems(false, false);
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

        public void Back()
        {
            fmp = back[fmp];
            MainFrame.Navigate(fmp);
            N.IsEnabled = true;
            B.IsEnabled = (back[fmp] != null);
            fmp.ReadOutListItems(B.IsEnabled, true);
        }

        public void Next()
        {
            fmp = next[fmp];
            MainFrame.Navigate(fmp);
            B.IsEnabled = true;
            N.IsEnabled = (next[fmp] != null);
            fmp.ReadOutListItems(true,N.IsEnabled);
        }
        public void Repeat()
        {
            MainFrame.Navigate(fmp);
            B.IsEnabled = (back[fmp] != null);
            N.IsEnabled = (next[fmp] != null);
            fmp.ReadOutListItems(B.IsEnabled,N.IsEnabled);
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
                temp = next[fmp];
                while (temp != null)
                {
                    nxt = next[temp];
                    back.Remove(temp);
                    next.Remove(temp);
                    temp.Dispose();
                    temp = nxt;
                }
                /*******************  Memory Management Finish  ****************/
                next[fmp] = new File_Manager_Page(di);
                back[next[fmp]] = fmp;
                fmp = next[fmp];
                next[fmp] = null;
                MainFrame.Navigate(fmp);
                B.IsEnabled = true;
                N.IsEnabled = false;
                fmp.ReadOutListItems(true, false);
            }
        }
    }

    
}