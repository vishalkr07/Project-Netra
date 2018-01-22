using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Globalization;
using System.Speech.Synthesis;
using System.Speech.Recognition;
using System.Diagnostics;

namespace ProjectNetra
{
    public class Speak_Listen_Emulated                                                    // Class for Listening to User and providing voice output.
    {
        private static SpeechSynthesizer synth = null;
        private static SpeechRecognitionEngine recog = null;
        private static bool completed;                                           // Indicate when an asynchronous operation is finished.


        private static Grammar CreateGrammarBuilderGrammarForAssistant()         // Build Grammars for speech recognition
        {
            GrammarBuilder builder = new GrammarBuilder();

            Choices cityChoice = new Choices(new string[]                        // Commands for interacting with the Assistant
            {"Open Browser", "Help", "Menu", "Search Computer","Send mail","Show Inbox","Close all","Status","Increase Volume","Shut down"});

            builder.Append(cityChoice);

            Grammar citiesGrammar = new Grammar(builder);
            citiesGrammar.Name = "GrammarBuilder Cities Grammar";

            return citiesGrammar;
        }

        public static void Initialize()                                          // Initialize components
        {
            synth = new SpeechSynthesizer();
            synth.SetOutputToDefaultAudioDevice();                               // Configure output to the speech synthesizer.

            recog = new SpeechRecognitionEngine(new CultureInfo("en-US"));       // Create an in-process speech recognizer for the en-US locale.
            recog.LoadGrammar(CreateGrammarBuilderGrammarForAssistant());        // Create and load a grammar.
            recog.SetInputToNull();                                              // Disable audio input to the recognizer.

            recog.SpeechRecognized += new EventHandler<SpeechRecognizedEventArgs>(SpeechRecognizedHandler);           // Attach event handlers for recognition events.            
            recog.SpeechDetected += new EventHandler<SpeechDetectedEventArgs>(SpeechDetectedHandler);
            recog.SpeechHypothesized += new EventHandler<SpeechHypothesizedEventArgs>(SpeechHypothesizedHandler);
            recog.SpeechRecognitionRejected += new EventHandler<SpeechRecognitionRejectedEventArgs>(SpeechRecognitionRejectedHandler);
            recog.EmulateRecognizeCompleted += new EventHandler<EmulateRecognizeCompletedEventArgs>(EmulateRecognizeCompletedHander);

        }

        private static void TestRecognizeAsync(SpeechRecognitionEngine recognizer, string input)                      // Send emulated input to the recognizer for asynchronous recognition.
        {
            completed = false;
            Debug.WriteLine("TestRecognizeAsync(\"{0}\")...", input);
            recognizer.EmulateRecognizeAsync(input);
            while (!completed)                                                     // Wait for the operation to complete.
            {
                Thread.Sleep(333);
            }
            Debug.WriteLine(" Done.");
        }

        private static void SpeechDetectedHandler(object sender, SpeechDetectedEventArgs e)
        {
            Debug.WriteLine(" SpeechDetected event raised.");
        }

        private static void SpeechHypothesizedHandler(object sender, SpeechHypothesizedEventArgs e)
        {
            Debug.WriteLine(" SpeechHypothesized event raised.");

            if (e.Result != null)
            {
                Debug.WriteLine("  Grammar = {0}; Text = {1}", e.Result.Grammar.Name ?? "<none>", e.Result.Text);
            }
            else
            {
                Debug.WriteLine("  No recognition result available.");
            }
        }
        private static void SpeechRecognitionRejectedHandler(object sender, SpeechRecognitionRejectedEventArgs e)
        {
            Debug.WriteLine(" SpeechRecognitionRejected event raised.");

            if (e.Result != null)
            {
                string grammarName;
                if (e.Result.Grammar != null)
                {
                    grammarName = e.Result.Grammar.Name ?? "<none>";
                }
                else
                {
                    grammarName = "<not available>";
                }
                Debug.WriteLine("  Grammar = {0}; Text = {1}", grammarName, e.Result.Text);
            }
            else
            {
                Debug.WriteLine("  No recognition result available.");
            }
        }

        private static void SpeechRecognizedHandler(object sender, SpeechRecognizedEventArgs e)
        {
            Debug.WriteLine(" SpeechRecognized event raised.");

            if (e.Result != null)
            {
                Debug.WriteLine("  Grammar = {0}; Text = {1}", e.Result.Grammar.Name ?? "<none>", e.Result.Text);
            }
            else
            {
                Debug.WriteLine("  No recognition result available.");
            }
        }

        private static void EmulateRecognizeCompletedHander(object sender, EmulateRecognizeCompletedEventArgs e)
        {
            Debug.WriteLine(" EmulateRecognizeCompleted event raised.");

            if (e.Error != null)
            {
                Debug.WriteLine("  {0} exception encountered: {1}:", e.Error.GetType().Name, e.Error.Message);
            }
            else if (e.Cancelled)
            {
                Debug.WriteLine("  Operation cancelled.");
            }
            else if (e.Result != null)
            {
                Debug.WriteLine("  Grammar = {0}; Text = {1}", e.Result.Grammar.Name ?? "<none>", e.Result.Text);
            }
            else
            {
                Debug.WriteLine("  No recognition result available.");
            }

            completed = true;
        }


        public static void Listen()                                              // Start Listening to emulated commands
        {
            Debug.WriteLine("Starting Recognition.........");
            TestRecognizeAsync(recog, "Smith");                                  // Start asynchronous emulated recognition operation.
            TestRecognizeAsync(recog, "Jones");
            TestRecognizeAsync(recog, "Mister");
            TestRecognizeAsync(recog, "Mister Smith");
        }

        public static void Speak(string msg)                                      // Use for Voice Output
        {
            synth.Speak(msg);
        }

        public static void Close()                                               // Custom function with the aim to release all references before shutdown
        {
            synth.Dispose();
            recog.Dispose();
            /*
             *  TODO 1: Dispose all references.
             *  TODO 2: Close all opened apps here before closing the assistant. 
             *  TODO 3: Shut down the Computer.
             */
            Debug.WriteLine("Assistant is closed!");
        }


    }
}
