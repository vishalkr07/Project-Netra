using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Diagnostics;

namespace ProjectNetra
{
    public class MyThread                                                               // Class for management of threads
    {
        /************  All Threads for the system  ***********/

        public static Thread InputThread = new Thread(new ThreadStart(VoiceInput));                        // Thread for Voice Input
        public static Thread OutputThread = new Thread(new ParameterizedThreadStart(VoiceOutput));         // Thread for Voice Output


        /***************  Threads end here  ******************/

        /********  Functions to be run in threads  ***********/

        public static void VoiceInput()                                                 
        {
            Speak_Listen.Listen(); 
        }
        public static void VoiceOutput(object obj)                                               
        {
            string speakout = (string)obj;
            Speak_Listen.Speak(speakout);
        }
        public static void OpenApp(object obj) {                                             
            Process p = Process.Start(obj.ToString());
            //p.WaitForInputIdle();
            //p.WaitForExit();
        }

        /*****  End of functions to be run in threads  ********/
    }
}
