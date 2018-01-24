using System;
using System.IO;
using org.apache.pdfbox.pdmodel;
using org.apache.pdfbox.util;

namespace ProjectNetra
{
    class Pdf_Reader
    {
        public static void Pdf2Speech(string filename)
        {
            Pdf2Text(filename);
            using (StreamReader sr = new StreamReader(filename+".txt"))
            {
                string readout = sr.ReadToEnd();
                Speak_Listen.Speak(readout);
            }
        }
        private static void Pdf2Text(string filename)
        {

            string[] args = { filename+".pdf", filename+".txt" };
            DateTime start = DateTime.Now;
            if (args.Length < 2)
            {
                Console.WriteLine("Usage: PDF2TEXT <input filename (PDF)> <output filename (text)>");
                return;
            }

            using (StreamWriter sw = new StreamWriter(args[1]))
            {
                sw.WriteLine(parseUsingPDFBox(args[0]));
            }

            Console.WriteLine("Done. Took " + (DateTime.Now - start));
        }
        private static string parseUsingPDFBox(string input)
        {
            PDDocument doc = null;

            try
            {
                doc = PDDocument.load(input);
                PDFTextStripper stripper = new PDFTextStripper();
                return stripper.getText(doc);
            }
            catch (Exception e) {
                Speak_Listen.Speak("Sorry, no such file found!");
                return "";
            }
            finally
            {
                if (doc != null)
                {
                    doc.close();
                }
            }
        }
         
    }
}

