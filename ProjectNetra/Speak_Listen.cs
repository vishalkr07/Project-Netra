using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Globalization;
using System.Speech.Synthesis;
using System.Speech.Recognition;

namespace ProjectNetra
{
    public class Speak_Listen                           // Class for Listening to User and providing voice output.
    {
        private static SpeechSynthesizer synth = null;         
        private static SpeechRecognitionEngine recog = null;

        private static bool completed;                          // Indicate whether the asynchronous emulate recognition operation has completed.


        public static void Initialize()                    
        {
            synth = new SpeechSynthesizer();
            synth.SetOutputToDefaultAudioDevice();                                  // Configure output to the speech synthesizer.

            recog = new SpeechRecognitionEngine(new CultureInfo("en-US"));          // Create an in-process speech recognizer for the en-US locale.
            recog.LoadGrammar(new DictationGrammar());                              // Create and load a dictation grammar.
            recog.SetInputToDefaultAudioDevice();                                   // Configure input to the speech recognizer.
            recog.SpeechRecognized += new EventHandler<SpeechRecognizedEventArgs>(SpeechRecognizedHandler);           // Attach event handlers for recognition events.
            completed = false;
        }

        public static void close()                            // Custom function with the aim to release all references before shutdown
        { 
            synth.Dispose();
            recog.Dispose();
            completed = true;
        }

        public static void Speak(string msg)
        {
            synth.Speak(msg);
        }

        public static void Listen()
        {
            recog.RecognizeAsync(RecognizeMode.Multiple);
            /*while (!completed)
            {
                Thread.Sleep(333);
            }*/
        }

        
        private static void SpeechRecognizedHandler(object sender, SpeechRecognizedEventArgs e)      // Handle the SpeechRecognized event.
        {
            MainWindow mwnd = new MainWindow();
            mwnd.showOutput("Recognized Text :   " + e.Result.Text);
            if (e.Result.Text == "Close Assisstant")
                close();
        }
    }
}
