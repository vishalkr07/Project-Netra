using System;
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
        public MSWord_Controller()                              // Use it when user aks open MS Word
        {
            wordApp = new Word.Application();
            wordApp.Visible = true;
            doc = wordApp.Documents.Add();
            
        }
        public void Open(string filepath)
        {
            /*
             *  TODO 1: Check for changes in the current document
             *      TODO 1.1: YES --> Prompt the user for "save/save as/close without saving"
             *      TODO 1.2: NO  --> Close the currently opened file
             */
            doc.Close();
            doc = wordApp.Documents.Open(filepath);
        }
        public void New()
        {
            doc = wordApp.Documents.Add();
        }
        public void CloseAll()
        {
        }
        public void Close()
        {
        }

        public void Save()
        {

        }
        public void SaveAs()
        {
        }

    }
}
