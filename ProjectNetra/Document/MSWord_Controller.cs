using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using Word = Microsoft.Office.Interop.Word;

namespace ProjectNetra.Document
{
    class MSWord_Controller
    {
        private Word.Application wordApp = null;
        private Word.Document doc = null;
        private Word.Documents docs = null;
        private bool isOpenFirstTime = true;
        private List<string> wordFiles = new List<string>();

        public MSWord_Controller()                             
        {
            wordApp = new Word.Application();
            wordApp.Visible = true;
            docs = wordApp.Documents;
            doc = docs.Add();
        }
        public MSWord_Controller(string filepath)
        {
            wordApp = new Word.Application();
            wordApp.Visible = true;
            docs = wordApp.Documents;
            doc = docs.Open(filepath);
        }
        public void Open()
        {
            /*************   Get the list of all .doc/.docx files in the system    ************/
            if (isOpenFirstTime)                    // To be executed only first time this function is called
            {
                string[] drives = Environment.GetLogicalDrives();
                foreach (string dr in drives)
                {
                    wordFiles.AddRange(Directory.GetFiles(dr,"*.doc"));
                    wordFiles.AddRange(Directory.GetFiles(dr,"*.docx"));
                }
                isOpenFirstTime = false;
            }
            if(wordFiles.Count == 0)
            {
                Speak_Listen.Speak("Sorry, your system has no .doc or .docx file");
            }
            /********   End of getting the list of all .doc/.docx files in the system  ********/

        }
        public void New()
        {
            doc = docs.Add();
        }
        public void CloseAll()
        {
        }
        public void Close()
        {
        }
        /*
         *  Handles Save/Save As
         *  saveAs :
         *          true --> SaveAs to be applied
         *          false --> Normal Save to be applied
         */
        public void Save(bool saveAs)
        {
            wordApp.ActiveDocument.Save();
        }              
        public void Instruct(string cmd)
        {

        }
    }
}
