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
    public class Speak_Listen                                                    // Class for Listening to User and providing voice output.
    {
        private static SpeechSynthesizer synth = null;         
        private static SpeechRecognitionEngine recog = null;
        private static bool completed;                                           // Indicate when an asynchronous operation is finished.


        private static Grammar CreateGrammarBuilderGrammarForAssistant()         // Build Grammars for speech recognition
        {
            GrammarBuilder builder = new GrammarBuilder();

            Choices cityChoice = new Choices(new string[]                        // Commands for interacting with the Assistant
            {"Google it", "Help", "Menu", "Search Computer","Send mail","Show Inbox","Close all","Status","Increase Volume","Shut down"});
            
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
            recog.SetInputToDefaultAudioDevice();                                // Configure input to the speech recognizer.

            recog.SpeechRecognized += new EventHandler<SpeechRecognizedEventArgs>(SpeechRecognizedHandler);         // Attach event handlers for recognition events.
            recog.SpeechDetected += new EventHandler<SpeechDetectedEventArgs>(SpeechDetectedHandler);
            recog.SpeechHypothesized += new EventHandler<SpeechHypothesizedEventArgs>(SpeechHypothesizedHandler);
            recog.SpeechRecognitionRejected += new EventHandler<SpeechRecognitionRejectedEventArgs>(SpeechRecognitionRejectedHandler);
            recog.RecognizeCompleted += new EventHandler<RecognizeCompletedEventArgs>(RecognizeCompletedHandler);
        }

            
        private static void SpeechRecognizedHandler(object sender, SpeechRecognizedEventArgs e)                     // Handle the SpeechRecognized event.
        {
            Debug.WriteLine(" In SpeechRecognizedHandler.");

            string grammarName = "<not available>";
            string resultText = "<not available>";
            if (e.Result != null)
            {
                if (e.Result.Grammar != null)
                {
                    grammarName = e.Result.Grammar.Name;
                }
                resultText = e.Result.Text;
            }

            Debug.WriteLine(" - Grammar Name = {0}; Result Text = {1}", grammarName, resultText);

        }
                
        private static void SpeechDetectedHandler(object sender, SpeechDetectedEventArgs e)                         // Handle the SpeechDetected event.
        {
            Debug.WriteLine(" In SpeechDetectedHandler:");
            Debug.WriteLine(" - AudioPosition = {0}", e.AudioPosition);
        }
        
        private static void SpeechHypothesizedHandler(object sender, SpeechHypothesizedEventArgs e)                 // Handle the SpeechHypothesized event.
        {
            Debug.WriteLine(" In SpeechHypothesizedHandler:");

            string grammarName = "<not available>";
            string resultText = "<not available>";
            if (e.Result != null)
            {
                if (e.Result.Grammar != null)
                {
                    grammarName = e.Result.Grammar.Name;
                }
                resultText = e.Result.Text;
            }

            Debug.WriteLine(" - Grammar Name = {0}; Result Text = {1}", grammarName, resultText);
        }
                
        private static void SpeechRecognitionRejectedHandler(object sender, SpeechRecognitionRejectedEventArgs e)   // Handle the SpeechRecognitionRejected event.
        {
            Debug.WriteLine(" In SpeechRecognitionRejectedHandler:");

            string grammarName = "<not available>";
            string resultText = "<not available>";
            if (e.Result != null)
            {
                if (e.Result.Grammar != null)
                {
                    grammarName = e.Result.Grammar.Name;
                }
                resultText = e.Result.Text;
            }

            Debug.WriteLine(" - Grammar Name = {0}; Result Text = {1}", grammarName, resultText);
        }
                
        private static void RecognizeCompletedHandler(object sender, RecognizeCompletedEventArgs e)                 // Handle the RecognizeCompleted event.
        {
            Debug.WriteLine(" In RecognizeCompletedHandler.");

            if (e.Error != null)
            {
                Debug.WriteLine(" - Error occurred during recognition: {0}", e.Error);
                return;
            }
            if (e.InitialSilenceTimeout || e.BabbleTimeout)
            {
                Debug.WriteLine(" - BabbleTimeout = {0}; InitialSilenceTimeout = {1}", e.BabbleTimeout, e.InitialSilenceTimeout);
                return;
            }
            if (e.InputStreamEnded)
            {
                Debug.WriteLine(" - AudioPosition = {0}; InputStreamEnded = {1}",e.AudioPosition, e.InputStreamEnded);
            }
            if (e.Result != null)
            {
                Debug.WriteLine(" - Grammar = {0}; Text = {1}; Confidence = {2}", e.Result.Grammar.Name, e.Result.Text, e.Result.Confidence);
                Debug.WriteLine(" - AudioPosition = {0}", e.AudioPosition);
            }
            else
            {
                Debug.WriteLine(" - No result.");
            }

            completed = true;
        }

        public static void Listen()                                              // Start Listening to user voice commands
        {
            completed = false;
            Debug.WriteLine("Starting Recognition.........");
            recog.RecognizeAsync(RecognizeMode.Multiple);
            while (!completed)                                                   // Wait for the operation to complete.
            {
                Thread.Sleep(333);                                               
            }
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
