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
        bool RepeatFlag = false;
        public Media_Player() 
        {
            InitializeComponent();

            timer = new DispatcherTimer()
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            timer.Tick += new EventHandler(TimerTick);
            tick = new timerTick(ChangeStatus);

            GetFileNames();
            Print();
            Select();

        }

        void TimerTick(object sender, EventArgs e)
        {
            Dispatcher.Invoke(tick);
        }


        //visualize progressBar 
        void ChangeStatus()
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

                    CurrentTimeTextBlock.Text = min + ":" + sec;
                }
                else
                {
                    CurrentTimeTextBlock.Text = hours + ":" + min + ":" + sec;
                }
            }
        }


        //open the file
        private void OpenButtonClick(object sender, RoutedEventArgs e)
        {
            OpenList();
            //openFile();


        }
        //select song to be played 
        public void Select()
        {
            Debug.WriteLine("Select the song NO. You want to play;");

            String choice = Console.ReadLine();
            SongNo = 5;
            StartPlay();

        }

        // start playing song after the choice is made 
        public void StartPlay()
        {
            try
            {
                mediaElement.Source = new Uri(l1[SongNo]);
                TextBox1.Text = l2[SongNo];
                Thread.Sleep(50);
                mediaElement.Close();
                mediaElement.Play();
            }
            catch (ArgumentOutOfRangeException e)
            {
                Speak_Listen.Speak("No Files in Your Computer.");
                Debug.WriteLine(e.Message);
            } catch(Exception e)
            {
                Debug.WriteLine(e.Message);
            }
        }

        //next song
        public void Next()
        {
            if (SongNo == TotSong - 1)
                SongNo = 0;
            else
                SongNo += 1;
            StartPlay();
        }
        //previous song 
        public void Previous()
        {
            if (SongNo == 0)
                SongNo = TotSong - 1;
            else
                SongNo -= 1;
            StartPlay();
        }
        public void GetFileNames()
        {
            string[] ar = { "*.mp3", "*.mp4", "*.wav", "*.mpeg", "*.wmv", "*.avi" };



            foreach (string pat in ar)
                ExtractMusicFiles(pat);
        }
        // getting file names from the music folder
        public void ExtractMusicFiles(string pat)
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);
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
        public void Print()
        {
            TotSong = l1.Count;
            for (int i = 0; i < TotSong; i++)
            {
                Debug.WriteLine((i + 1) + " " + l2[i]);
            }
        }
        public void OpenList()
        {
            Print();
            Select();

        }


        //occurs when the file is opened
        public void MediaElementMediaOpened(object sender, RoutedEventArgs e)
        {
            timer.Start();
            fileIsPlaying = true;
            OpenMedia();
        }


        //opens media,adds file to playlist and gets file info
        public void OpenMedia()
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

                    EndTimeTextBlock.Text = min + ":" + sec;
                }
                else
                {
                    EndTimeTextBlock.Text = hours + ":" + min + ":" + sec;
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

            volumeSlider.ValueChanged += new RoutedPropertyChangedEventHandler<double>(VolumeSliderValueChanged);
        }

        //occurs when the file is done playing and plays next song
        private void MediaElementMediaEnded(object sender, RoutedEventArgs e)
        {
            mediaElement.Stop();
            volumeSlider.ValueChanged -= new RoutedPropertyChangedEventHandler<double>(VolumeSliderValueChanged);
            if(RepeatFlag)
            {
                StopSong();
                playsong();
            }
            else
                Next();

        }


        //initialize properties of file
        void InitializePropertyValues()
        {
            mediaElement.Volume = (double)volumeSlider.Value;

        }


        //seek to desirable position of the file
        private void SeekSliderMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            TimeSpan ts = new TimeSpan(0, 0, 0, 0, (int)seekSlider.Value);

            ChangePostion(ts);
        }


        //mouse down on slide bar in order to seek
        private void SeekSliderPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            isDragging = true;
            fileIsPlaying = false;
        }


        private void SeekSliderPreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (isDragging)
            {
                TimeSpan ts = new TimeSpan(0, 0, 0, 0, (int)seekSlider.Value);
                ChangePostion(ts);
                fileIsPlaying = true;
            }
            isDragging = false;
        }


        //change position of the file
        void ChangePostion(TimeSpan ts)
        {
            mediaElement.Position = ts;
        }






        //play the file 
        private void PlayButtonClick(object sender, RoutedEventArgs e)
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
        private void PauseButtonClick(object sender, RoutedEventArgs e)
        {
            PauseSong();
        }
        //pause the song
        public void PauseSong()
        {
            fileIsPlaying = false;
            mediaElement.Pause();
            timer.Stop();
        }

        //stop the file
        private void StopButtonClick(object sender, RoutedEventArgs e)
        {
            StopSong();
        }

        private void TextBoxTextChanged(object sender, TextChangedEventArgs e)
        {
            // TextBox1.Text = " ";
        }

        private void TextBox1TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void PrevButtonClick(object sender, RoutedEventArgs e)
        {
            Previous();
        }

        private void NextButtonClick(object sender, RoutedEventArgs e)
        {
            Next();
        }
        
        public void RepeatOn()
        {
            RepeatFlag = true;
        }
        public void RepeatOff()
        {
            RepeatFlag = false;
        }
        private void RepeatButtonClick(object sender, RoutedEventArgs e)
        {
            if (RepeatFlag)
            {
                RepeatOff();
            }
            else
                RepeatOn();
        }

        //stop the song
        public void StopSong()
        {
            fileIsPlaying = false;
            timer.Stop();
            mediaElement.Stop();
            seekSlider.Value = 0;
            progressBar.Value = 0;
            CurrentTimeTextBlock.Text = "00:00";
        }

        //turn volume up-down
        private void VolumeSliderValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            mediaElement.Volume = volumeSlider.Value;
        }
    }
}
