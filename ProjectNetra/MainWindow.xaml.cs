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

namespace ProjectNetra
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Speak_Listen.Initialize();
            Thread thr1 = new Thread(new ThreadStart(MyThread.T1));
            thr1.Start();
        }
        

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //Thread thr2 = new Thread(new ParameterizedThreadStart(MyThread.T2));
            //thr2.Start("Hi I am Cortan, husband of Cortana.");
        }
    }

        
}
