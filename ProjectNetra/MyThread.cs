using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectNetra
{
    public class MyThread
    {
        public static void T1()
        {
            Speak_Listen.Listen(); 
        }
        public static void T2(object msg)
        {
            string speakout = (string)msg;
            Speak_Listen.Speak(speakout);
        }
    }
}
