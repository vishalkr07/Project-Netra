using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using System.IO;
using System.Diagnostics;
using System.Windows.Threading;
using System.Threading;
using System.Data;

namespace ProjectNetra
{
    /// <summary>
    /// Interaction logic for Media_Player.xaml
    /// </summary>
    public partial class Media_Player : Window
    {
        DispatcherTimer timer;

        public delegate void timerTick();
        timerTick tick;

        bool isDragging = false;
        bool fileIsPlaying = false;
        public string sec, min, hours;
        public static string input;
        List<string> l1 = new List<string>(); // contains address strings of all songs 
        List<string> l2 = new List<string>(); // contains song names 
        public int SongNo;// Current Song No
        public int TotSong;// Total count of Songs
        public Media_Player() 
        {
            InitializeComponent();

            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += new EventHandler(timer_Tick);
            tick = new timerTick(changeStatus);

            getfilenames();
            print();
            select();

        }

        void timer_Tick(object sender, EventArgs e)
        {
            Dispatcher.Invoke(tick);
        }


        //visualize progressBar 
        void changeStatus()
        {
            if (fileIsPlaying)
            {

                #region customizeTime
                if (mediaElement.Position.Seconds < 10)
                    sec = "0" + mediaElement.Position.Seconds.ToString();
                else
                    sec = mediaElement.Position.Seconds.ToString();


                if (mediaElement.Position.Minutes < 10)
                    min = "0" + mediaElement.Position.Minutes.ToString();
                else
                    min = mediaElement.Position.Minutes.ToString();

                if (mediaElement.Position.Hours < 10)
                    hours = "0" + mediaElement.Position.Hours.ToString();
                else
                    hours = mediaElement.Position.Hours.ToString();

                #endregion customizeTime

                seekSlider.Value = mediaElement.Position.TotalMilliseconds;
                progressBar.Value = mediaElement.Position.TotalMilliseconds;

                if (mediaElement.Position.Hours == 0)
                {

                    currentTimeTextBlock.Text = min + ":" + sec;
                }
                else
                {
                    currentTimeTextBlock.Text = hours + ":" + min + ":" + sec;
                }
            }
        }


        //open the file
        private void openFileButton_Click(object sender, RoutedEventArgs e)
        {
            getfilenames();
            print();
            select();
            //openFile();


        }
        //select song to be played 
        public void select()
        {
            Debug.WriteLine("Select the song NO. You want to play;");

            String choice = Console.ReadLine();
            SongNo = 5;
            startplay();

        }

        // start playing song after the choice is made 
        public void startplay()
        {
            mediaElement.Source = new Uri(l1[SongNo]);
            TextBox1.Text = l2[SongNo];
            Thread.Sleep(50);
            mediaElement.Close();
            mediaElement.Play();
        }

        //next song
        public void next()
        {
            if (SongNo == TotSong - 1)
                SongNo = 0;
            else
                SongNo += 1;
            startplay();
        }
        //previous song 
        public void prev()
        {
            if (SongNo == 0)
                SongNo = TotSong - 1;
            else
                SongNo -= 1;
            startplay();
        }
        public void getfilenames()
        {
            string[] ar = { "*.mp3", "*.mp4", "*.wav", "*.mpeg", "*.wmv", "*.avi" };



            foreach (string pat in ar)
                ExtractMusicFiles(pat);
        }
        // getting file names from the music folder
        public void ExtractMusicFiles(string pat)
        {
            string path = @"C:\Users\User\Music";
            string[] ar = Directory.GetFiles(path, pat);

            //Debug.WriteLine("@@@@@@@@  " + ar.Length.ToString());
            foreach (string name in ar)
            {
                //Debug.WriteLine(name);
                l1.Add(name);
                l2.Add(System.IO.Path.GetFileName(name));
                // Debug.WriteLine(System.IO.Path.GetFileName(name));
            }
        }
        public void print()
        {
            TotSong = l1.Count;
            for (int i = 0; i < TotSong; i++)
            {
                Debug.WriteLine((i + 1) + " " + l2[i]);
            }
        }
        public void openFile()
        {
            Stream checkStream = null;
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.InitialDirectory = "C:\\Users\\User\\Music";
            dlg.Filter = "All Supported File Types(*.mp3,*.wav,*.mpeg,*.wmv,*.avi,*.mp4)|*.mp3;*.wav;*.mpeg;*.wmv;*.avi;*.mp4";

            // Show open file dialog box 
            if ((bool)dlg.ShowDialog())
            {

                try
                {
                    if ((checkStream = dlg.OpenFile()) != null)
                    {
                        mediaElement.Source = new Uri(dlg.FileName);
                        TextBox1.Text = System.IO.Path.GetFileName(dlg.FileName);
                    }
                    Thread.Sleep(50);
                    mediaElement.Close();
                    mediaElement.Play();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message);
                }
            }
            Debug.WriteLine("$$$$$$$$$$$$  Open finished.");
        }


        //occurs when the file is opened
        public void mediaElement_MediaOpened(object sender, RoutedEventArgs e)
        {
            timer.Start();
            fileIsPlaying = true;
            openMedia();
        }


        //opens media,adds file to playlist and gets file info
        public void openMedia()
        {
            InitializePropertyValues();
            try
            {
                #region customizeTime
                if (mediaElement.NaturalDuration.TimeSpan.Seconds < 10)
                    sec = "0" + mediaElement.Position.Seconds.ToString();
                else
                    sec = mediaElement.NaturalDuration.TimeSpan.Seconds.ToString();

                if (mediaElement.NaturalDuration.TimeSpan.Minutes < 10)
                    min = "0" + mediaElement.NaturalDuration.TimeSpan.Minutes.ToString();
                else
                    min = mediaElement.NaturalDuration.TimeSpan.Minutes.ToString();

                if (mediaElement.NaturalDuration.TimeSpan.Hours < 10)
                    hours = "0" + mediaElement.NaturalDuration.TimeSpan.Hours.ToString();
                else
                    hours = mediaElement.NaturalDuration.TimeSpan.Hours.ToString();

                if (mediaElement.NaturalDuration.TimeSpan.Hours == 0)
                {

                    endTimeTextBlock.Text = min + ":" + sec;
                }
                else
                {
                    endTimeTextBlock.Text = hours + ":" + min + ":" + sec;
                }

                #endregion customizeTime
            }
            catch { }
            string path = mediaElement.Source.LocalPath.ToString();

            double duration = mediaElement.NaturalDuration.TimeSpan.TotalMilliseconds;
            seekSlider.Maximum = duration;
            progressBar.Maximum = duration;

            mediaElement.Volume = volumeSlider.Value;

            mediaElement.ScrubbingEnabled = true;

            volumeSlider.ValueChanged += new RoutedPropertyChangedEventHandler<double>(volumeSlider_ValueChanged);
        }

        //occurs when the file is done playing and plays next song
        private void mediaElement_MediaEnded(object sender, RoutedEventArgs e)
        {
            mediaElement.Stop();
            volumeSlider.ValueChanged -= new RoutedPropertyChangedEventHandler<double>(volumeSlider_ValueChanged);
            next();

        }


        //initialize properties of file
        void InitializePropertyValues()
        {
            mediaElement.Volume = (double)volumeSlider.Value;

        }


        //seek to desirable position of the file
        private void seekSlider_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            TimeSpan ts = new TimeSpan(0, 0, 0, 0, (int)seekSlider.Value);

            changePostion(ts);
        }


        //mouse down on slide bar in order to seek
        private void seekSlider_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            isDragging = true;
            fileIsPlaying = false;
        }


        private void seekSlider_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (isDragging)
            {
                TimeSpan ts = new TimeSpan(0, 0, 0, 0, (int)seekSlider.Value);
                changePostion(ts);
                fileIsPlaying = true;
            }
            isDragging = false;
        }


        //change position of the file
        void changePostion(TimeSpan ts)
        {
            mediaElement.Position = ts;
        }






        //play the file
        private void playButton__Click(object sender, RoutedEventArgs e)
        {
            playsong();
        }
        //function for playing a song 
        public void playsong()
        {
            fileIsPlaying = true;
            mediaElement.Play();
            timer.Start();
        }

        //pause the file
        private void pauseButton_Click(object sender, RoutedEventArgs e)
        {
            pausesong();
        }
        //pause the song
        public void pausesong()
        {
            fileIsPlaying = false;
            mediaElement.Pause();
            timer.Stop();
        }

        //stop the file
        private void stopButton_Click(object sender, RoutedEventArgs e)
        {
            stopsong();
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            // TextBox1.Text = " ";
        }

        private void TextBox1_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void prevButton_Click(object sender, RoutedEventArgs e)
        {
            prev();
        }

        private void nextButton_Click(object sender, RoutedEventArgs e)
        {
            next();
        }

        //stop the song
        public void stopsong()
        {
            fileIsPlaying = false;
            timer.Stop();
            mediaElement.Stop();
            seekSlider.Value = 0;
            progressBar.Value = 0;
            currentTimeTextBlock.Text = "00:00";
        }

        //turn volume up-down
        private void volumeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            mediaElement.Volume = volumeSlider.Value;
        }
    }
}
