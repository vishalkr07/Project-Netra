using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Interop;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Microsoft.Win32;

namespace ProjectNetra
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        
        public MainWindow()                                                     // Defines what to be done with assisstant start up          
        {
            InitializeComponent();
            Test.Initialize();
            Speak_Listen.Initialize();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Speak_Listen.Speak("Hi Jerwin Mistry, how are you!");
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            //Speak_Listen.Speak("Fuck off I am working!");
            //Speak_Listen.Speak(" Souvik you are going in the right way. Have confidence and proceed!");
            /*Process[] pr = Process.GetProcesses();
            foreach (var p in pr)
            {
                Debug.WriteLine("@@@  "+ p.ProcessName);
            }*/
            Pdf_Reader.Pdf2Speech("numbertheory");
        }

        private void OnContentLoaded(object sender,EventArgs e) {
            Speak_Listen.Speak("Hi I am your blind assistant!");
            Speak_Listen.Listen();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            Volume_Control.VolumeUp();
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            Volume_Control.VolumeDown();
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            Volume_Control.Mute(); 
        }

        private void Button_Click_5(object sender, RoutedEventArgs e)
        {
            Media_Player mp = new Media_Player();
            mp.Show();
            Speak_Listen.MPInit(mp);
        }

        private void Button_Click_6(object sender, RoutedEventArgs e)
        {
            File_Manager fm = new File_Manager();
            fm.Show();
            Speak_Listen.FMInit(fm);
        }

        private void Button_Click_7(object sender, RoutedEventArgs e)
        {
            Calculator cal = new Calculator();
            cal.Show();
            Speak_Listen.CalcInit(cal);
        }
    }

        
}
